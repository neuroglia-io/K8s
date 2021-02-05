using k8s;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Rest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Represents the default implementation of the <see cref="ICustomResourceWatcher{TResource}"/> interface.
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to listen the Kubernetes events for.</typeparam>
    public class CustomResourceWatcher<TResource>
        : ICustomResourceWatcher<TResource>
        where TResource : class, ICustomResource, new()
    {

        private CancellationTokenSource _CancellationTokenSource;
        private Task _ExecutingTask;
        private object _SubscriptionLock = new object();
        private object _ResourcesLock = new object();

        /// <summary>
        /// Initializes a new <see cref="ICustomResourceWatcher{TResource}"/>.
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="kubernetes">The service used to communicate with Kubernetes.</param>
        /// <param name="options">The current <see cref="CustomResourceWatcherOptions"/></param>
        public CustomResourceWatcher(ILogger<CustomResourceWatcher<TResource>> logger, IKubernetes kubernetes, CustomResourceWatcherOptions options)
        {
            this.Logger = logger;
            this.Kubernetes = kubernetes;
            this.ResourceDefinition = new TResource().Definition;
            this.Options = options;
            this.StartAsync();
        }

        Type ICustomResourceWatcher.ResourceType
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
        /// Gets the current <see cref="CustomResourceWatcherOptions"/>
        /// </summary>
        protected CustomResourceWatcherOptions Options { get; }

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all registered <see cref="ICustomResource"/>s
        /// </summary>
        private List<TResource> Resources = new List<TResource>();

        /// <summary>
        /// Gets a <see cref="List{T}"/> containing all active subscriptions to the <see cref="IResourceEvent{TResource}"/> produced by the <see cref="CustomResourceWatcher{TResource}"/>
        /// </summary>
        private List<CustomResourceSubscription<TResource>> _Subscriptions = new List<CustomResourceSubscription<TResource>>();

        /// <inheritdoc/>
        public virtual Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (this._CancellationTokenSource != null
                && this._CancellationTokenSource.IsCancellationRequested)
            {
                this.Logger.LogDebug($"Failed to start the {nameof(ICustomResourceWatcher)} because it is being stopped.");
                this._CancellationTokenSource = null;
                this._ExecutingTask = null;
                return Task.CompletedTask;
            } 
            if (this._ExecutingTask != null
                && this._CancellationTokenSource != null)
                this._CancellationTokenSource.Cancel();
            this._CancellationTokenSource = new CancellationTokenSource();
            this._ExecutingTask = this.ListenAsync();
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
                    this.Logger.LogDebug("Retrieving all registered resources of kind '{kind}' (apiVersion: {apiVersion}) with label selector {labelSelector} and field selector {fieldSelector}...", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Options.LabelSelector, this.Options.FieldSelector);
                    if (string.IsNullOrWhiteSpace(this.Options.Namespace))
                        this.Resources = (await this.Kubernetes.ListClusterCustomObjectAsync<TResource>(this.ResourceDefinition.Group, this.ResourceDefinition.Version, this.ResourceDefinition.Plural, labelSelector: this.Options.LabelSelector, fieldSelector: this.Options.FieldSelector, cancellationToken: this._CancellationTokenSource.Token)).Items.ToList();
                    else
                        this.Resources = (await this.Kubernetes.ListNamespacedCustomObjectAsync<TResource>(this.ResourceDefinition.Group, this.ResourceDefinition.Version, this.Options.Namespace, this.ResourceDefinition.Plural, fieldSelector: this.Options.FieldSelector, cancellationToken: this._CancellationTokenSource.Token)).Items.ToList();
                    this.Logger.LogDebug("Retrieved {count} resources of kind '{kind}' (apiVersion: {apiVersion})", this.Resources.Count, this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"An exception occured while listing all registered resources of kind '{{kind}}'.{Environment.NewLine}Details: {{ex}}", this.ResourceDefinition.Kind, ex.Message);
                    throw;
                }
                try
                {
                    CancellationTokenSource operationCancellationTokenSource = new CancellationTokenSource();
                    CancellationTokenSource linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._CancellationTokenSource.Token, operationCancellationTokenSource.Token);
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
                    using (Watcher<TResource> watcher = operationResponse.Watch<TResource, object>(this.OnNext,
                        (ex) => 
                        {
                            this.Logger.LogError($"An exception occured while watching over the CRD of kind '{{crdKind}}' with API version '{{apiVersion}}' in namespace '{{namespace}}:{Environment.NewLine}{{ex}}'. Reconnecting...", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Options, ex.ToString());
                            this._Subscriptions.ToList().ForEach(s => s.Observer.OnError(ex));
                            operationCancellationTokenSource.Cancel();
                        },
                        () => 
                        {
                            this.Logger.LogInformation("The connection of the event watcher for CRD of kind '{crdKind}' with API version '{apiVersion}' in namespace '{namespace}' has been closed. Reconnecting...", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Options);
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
                catch (HttpOperationException ex)
                {
                    this.Logger.LogError($"An exception occured while processing the events of the CRD of kind '{{crdKind}}' with API version '{{apiVersion}}'. The server responded with a '{{statusCode}}' status code:{Environment.NewLine}Details: {{responseContent}}. Reconnecting...", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, ex.Response.StatusCode, ex.Response.Content);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"An exception occured while processing the events of the CRD of kind '{{crdKind}}' with API version '{{apiVersion}}':{Environment.NewLine}Details: {{ex}}. Reconnecting...", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, ex.ToString());
                }
            }
            this._Subscriptions?.ToList().ForEach(s => s.Observer.OnCompleted());
            this.Logger.LogDebug("Stopped watching for events on CRD of kind '{crdKind}' with API version '{apiVersion}' in namespace '{namespace}'.", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Options);
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
            CustomResourceSubscription<TResource> subscription = new CustomResourceSubscription<TResource>(observer);
            subscription.Disposed += this.OnSubscriptionDisposed;
            lock (this._SubscriptionLock)
            {
                this._Subscriptions.Add(subscription);
            }
            return subscription;
        }

        /// <summary>
        /// Handles resources events returned by the <see cref="IKubernetes"/> client
        /// </summary>
        /// <param name="type">The <see cref="WatchEventType"/></param>
        /// <param name="resource">The watched <see cref="ICustomResource"/></param>
        protected virtual void OnNext(WatchEventType type, TResource resource)
        {
            ResourceEvent<TResource> e = new ResourceEvent<TResource>(type, resource);
            switch (e.Type)
            {
                case WatchEventType.Added:
                    lock (this._ResourcesLock)
                    {
                        this.Resources.Add(resource);
                    }
                    break;
                case WatchEventType.Modified:
                    lock (this._ResourcesLock)
                    {
                        TResource match = this.Resources.FirstOrDefault(r => r.Metadata.Uid == resource.Metadata.Uid);
                        if (match != null)
                        {
                            this.Resources.Remove(match);
                            this.Resources.Add(resource);
                        }   
                    }
                    break;
                case WatchEventType.Deleted:
                    lock (this._ResourcesLock)
                    {
                        TResource match = this.Resources.FirstOrDefault(r => r.Metadata.Uid == resource.Metadata.Uid);
                        if (match != null)
                            this.Resources.Remove(match);
                    }
                    break;
            }
            this._Subscriptions.ToList().ForEach(s => s.Observer.OnNext(e));
        }

        /// <summary>
        /// Handles the event fired whenever a <see cref="CustomResourceSubscription{TResource}"/> has been disposed of
        /// </summary>
        /// <param name="sender">The <see cref="CustomResourceSubscription{TResource}"/> that has been disposed of</param>
        /// <param name="e">The <see cref="EventArgs"/></param>
        protected virtual void OnSubscriptionDisposed(object sender, EventArgs e)
        {
            lock (this._SubscriptionLock)
            {
                this._Subscriptions.Remove((CustomResourceSubscription<TResource>)sender);
            }
        }

        /// <inheritdoc/>
        public virtual IEnumerator<TResource> GetEnumerator()
        {
            return this.Resources.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="CustomResourceWatcher{TResource}"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="CustomResourceWatcher{TResource}"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                    this._CancellationTokenSource.Cancel();
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
            await this.StopAsync();
        }

    }

}
