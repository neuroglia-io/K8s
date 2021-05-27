using k8s;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IResourceWatcher{TResource}"/> interface.
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to listen the Kubernetes events for.</typeparam>
    public class ResourceWatcher<TResource>
        : IResourceWatcher<TResource>
        where TResource : class, ICustomResource, new()
    {

        private CancellationTokenSource _CancellationTokenSource;
        private Task _ExecutingTask;
        private readonly object _SubscriptionLock = new();

        /// <summary>
        /// Initializes a new <see cref="IResourceWatcher{TResource}"/>.
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="kubernetes">The service used to communicate with Kubernetes.</param>
        /// <param name="options">The current <see cref="ResourceWatcherOptions{TResource}"/></param>
        public ResourceWatcher(ILogger<ResourceWatcher<TResource>> logger, IKubernetes kubernetes, ResourceWatcherOptions<TResource> options)
        {
            this.Logger = logger;
            this.Kubernetes = kubernetes;
            this.ResourceDefinition = new TResource().Definition;
            this.Options = options;
        }

        Type IResourceWatcher.ResourceType
        {
            get
            {
                return typeof(TResource);
            }
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to communicate with Kubernetes
        /// </summary>
        protected IKubernetes Kubernetes { get; }

        /// <summary>
        /// Gets the <see cref="ICustomResourceDefinition"/> of the <see cref="ICustomResource"/> type to to listen the Kubernetes events for.
        /// </summary>
        protected ICustomResourceDefinition ResourceDefinition { get; }

        /// <summary>
        /// Gets the current <see cref="ResourceWatcherOptions{TResource}"/>
        /// </summary>
        protected ResourceWatcherOptions<TResource> Options { get; }

        /// <summary>
        /// Gets the <see cref="IResourceEvent{TResource}"/> <see cref="Subject{T}"/>
        /// </summary>
        protected Subject<IResourceEvent<TResource>> Subject { get; } = new Subject<IResourceEvent<TResource>>();

        /// <inheritdoc/>
        public virtual Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (this._CancellationTokenSource != null
                && this._CancellationTokenSource.IsCancellationRequested)
            {
                this.Logger.LogDebug($"Failed to start the {nameof(IResourceWatcher)} because it is being stopped.");
                this._CancellationTokenSource = null;
                this._ExecutingTask = null;
                return Task.CompletedTask;
            }
            if (this._ExecutingTask != null
                && this._CancellationTokenSource != null)
                this._CancellationTokenSource.Cancel();
            this._CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            this._ExecutingTask = Task.Run(async () => await this.ListenAsync(), this._CancellationTokenSource.Token);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Starts listening for Kubernetes events concerning the specified <see cref="ICustomResource"/> type
        /// </summary>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task ListenAsync()
        {
            while (!this._CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    using CancellationTokenSource operationCancellationTokenSource = new();
                    using CancellationTokenSource linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._CancellationTokenSource.Token, operationCancellationTokenSource.Token);
                    HttpOperationResponse<object> operationResponse;
                    if (string.IsNullOrWhiteSpace(this.Options.Namespace))
                    {
                        this.Logger.LogDebug("Creating a new watcher for events on CRD of kind '{crdKind}' with API version '{apiVersion}' in cluster.", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Options.Namespace);
                        operationResponse = await this.Kubernetes.ListClusterCustomObjectWithHttpMessagesAsync(this.ResourceDefinition.Group, this.ResourceDefinition.Version, this.ResourceDefinition.Plural, fieldSelector: this.Options.FieldSelector, watch: true, cancellationToken: linkedCancellationTokenSource.Token).ConfigureAwait(false);
                    }
                    else
                    {
                        this.Logger.LogDebug("Creating a new watcher for events on CRD of kind '{crdKind}' with API version '{apiVersion}' in namespace '{namespace}'.", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Options.Namespace);
                        operationResponse = await this.Kubernetes.ListNamespacedCustomObjectWithHttpMessagesAsync(this.ResourceDefinition.Group, this.ResourceDefinition.Version, this.Options.Namespace, this.ResourceDefinition.Plural, fieldSelector: this.Options.FieldSelector, watch: true, cancellationToken: linkedCancellationTokenSource.Token).ConfigureAwait(false);
                    }
                    using (operationResponse)
                    {
                        using (Watcher<TResource> watcher = operationResponse.Watch<TResource, object>(this.OnNext,
                        (ex) =>
                        {
                            this.Logger.LogError($"An exception occured while watching over the CRD of kind '{{crdKind}}' with API version '{{apiVersion}}' in namespace '{{namespace}}:{Environment.NewLine}{{ex}}'. Reconnecting...", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Options.Namespace, ex.ToString());
                            this.OnError(ex);
                            operationCancellationTokenSource.Cancel();
                        },
                        () =>
                        {
                            this.Logger.LogInformation("The connection of the event watcher for CRD of kind '{crdKind}' with API version '{apiVersion}' in namespace '{namespace}' has been closed. Reconnecting...", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Options.Namespace);
                            operationCancellationTokenSource.Cancel();
                        }))
                        {
                            if (string.IsNullOrWhiteSpace(this.Options.Namespace))
                                this.Logger.LogInformation("Started watching for events on CRD of kind '{crdKind}' with API version '{apiVersion}' in namespace '{namespace}'.", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Options.Namespace);
                            else
                                this.Logger.LogInformation("Started watching for events on CRD of kind '{crdKind}' with API version '{apiVersion}' in cluster.", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Options.Namespace);
                            while (!this._CancellationTokenSource.IsCancellationRequested
                                && !linkedCancellationTokenSource.IsCancellationRequested)
                            {

                            }
                        }
                    }
                }
                catch (HttpOperationException ex)
                {
                    this.Logger.LogError($"An exception occured while processing the events of the CRD of kind '{{crdKind}}' with API version '{{apiVersion}}'. The server responded with a '{{statusCode}}' status code:{Environment.NewLine}Details: {{responseContent}}. Reconnecting...", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, ex.Response.StatusCode, ex.Response.Content);
                    this.OnError(ex);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"An exception occured while processing the events of the CRD of kind '{{crdKind}}' with API version '{{apiVersion}}':{Environment.NewLine}Details: {{ex}}. Reconnecting...", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, ex.ToString());
                    this.OnError(ex);
                }
            }
            this.Subject?.OnCompleted();
            this.Logger.LogDebug("Stopped watching for events on CRD of kind '{crdKind}' with API version '{apiVersion}' in namespace '{namespace}'.", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Options.Namespace);
        }

        /// <inheritdoc/>
        public virtual async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (this._ExecutingTask == null)
                return;
            try
            {
                this._CancellationTokenSource.Cancel();
                this._CancellationTokenSource = null;
            }
            finally
            {
                await Task.WhenAny(this._ExecutingTask, Task.Delay(Timeout.Infinite, cancellationToken));
                this._ExecutingTask = null;
                this.Logger.LogDebug("Stopped listening for events on CRD of kind '{crdKind}', in group '{crdGroup}' and with version '{crdVersion}'", this.ResourceDefinition.Kind, this.ResourceDefinition.Group, this.ResourceDefinition.Version);
            }
        }

        /// <inheritdoc/>
        public virtual IDisposable Subscribe(IObserver<IResourceEvent<TResource>> observer)
        {
            return this.Subject.Subscribe(observer);
        }

        /// <summary>
        /// Handles resources events returned by the <see cref="IKubernetes"/> client
        /// </summary>
        /// <param name="type">The <see cref="WatchEventType"/></param>
        /// <param name="resource">The watched <see cref="ICustomResource"/></param>
        protected virtual void OnNext(WatchEventType type, TResource resource)
        {
            if(this.Options.Predicate == null
                || (this.Options.Predicate != null && this.Options.Predicate(type, resource)))
                this.Subject.OnNext(new ResourceEvent<TResource>(type, resource));
        }

        /// <summary>
        /// Handles <see cref="Exception"/>s thrown during the listening loop
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> to handle</param>
        protected virtual void OnError(Exception ex)
        {
            this.Subject.OnError(ex);
        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="ResourceWatcher{TResource}"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="ResourceWatcher{TResource}"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                    this._CancellationTokenSource.Cancel();
                this.Subject?.Dispose();
                this._Disposed = true;
                this.Logger.LogDebug("Disposed of the custom resource watcher for CRD of kind '{crdKind}', in group '{crdGroup}' and with version '{crdVersion}'", this.ResourceDefinition.Kind, this.ResourceDefinition.Group, this.ResourceDefinition.Version);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await Task.Run(() => this.Dispose());
        }

    }

}
