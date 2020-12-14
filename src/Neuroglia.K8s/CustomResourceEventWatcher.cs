using k8s;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Represents the default implementation of the <see cref="ICustomResourceEventWatcher{TResource}"/> interface.
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to listen the Kubernetes events for.</typeparam>
    public class CustomResourceEventWatcher<TResource>
        : ICustomResourceEventWatcher<TResource>
        where TResource : ICustomResource
    {

        private readonly CancellationTokenSource _StoppingCts = new CancellationTokenSource();
        private Task _ExecutingTask;

        /// <summary>
        /// Initializes a new <see cref="ICustomResourceEventWatcher{TResource}"/>.
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="kubernetesClient">The service used to communicate with Kubernetes.</param>
        /// <param name="resourceDefinition">The <see cref="ICustomResourceDefinition"/> of the <see cref="ICustomResource"/> type to to listen the Kubernetes events for.</param>
        /// <param name="namespaceProperty">The namespace in which to listen for Kubernetes events for the specified <see cref="ICustomResource"/> type. If null or empty, the controller will listen for cluster-wide events instead.</param>
        /// <param name="eventHandler">The <see cref="CustomResourceEventDelegate{TResource}"/> to invoke whenever a Kubernetes event for the specified <see cref="ICustomResource"/> type is received.</param>
        public CustomResourceEventWatcher(ILogger<CustomResourceEventWatcher<TResource>> logger, IKubernetes kubernetesClient, ICustomResourceDefinition resourceDefinition, string namespaceProperty, CustomResourceEventDelegate<TResource> eventHandler)
        {
            this.Logger = logger;
            this.KubernetesClient = kubernetesClient;
            this.ResourceDefinition = resourceDefinition;
            this.Namespace = namespaceProperty;
            this.EventHandler = eventHandler;
        }

        Type ICustomResourceEventWatcher.ObjectType
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
        protected IKubernetes KubernetesClient { get; }

        /// <summary>
        /// Gets the <see cref="ICustomResourceDefinition"/> of the <see cref="ICustomResource"/> type to to listen the Kubernetes events for.
        /// </summary>
        protected ICustomResourceDefinition ResourceDefinition { get; }

        /// <summary>
        /// Gets the <see cref="CustomResourceEventDelegate{TResource}"/> to invoke whenever a Kubernetes event for the specified <see cref="ICustomResource"/> type is received.
        /// </summary>
        protected CustomResourceEventDelegate<TResource> EventHandler { get; }

        /// <summary>
        /// Gets the namespace in which to listen for Kubernetes events for the specified <see cref="ICustomResource"/> type. If null or empty, the controller will listen for cluster-wide events instead.
        /// </summary>
        protected string Namespace { get; }

        /// <inheritdoc/>
        public virtual Task StartAsync(CancellationToken cancellationToken = default)
        {
            this._ExecutingTask = this.ListenAsync(this._StoppingCts.Token);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Starts listening for Kubernetes events concerning the specified <see cref="ICustomResource"/> type
        /// </summary>
        /// <param name="stoppingToken">A <see cref="CancellationToken"/> used to stop listening</param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task ListenAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                HttpOperationResponse<object> operationResponse;
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        if (!string.IsNullOrWhiteSpace(this.Namespace))
                            operationResponse = await this.KubernetesClient.ListNamespacedCustomObjectWithHttpMessagesAsync(this.ResourceDefinition.Group, this.ResourceDefinition.Version, this.Namespace, this.ResourceDefinition.Plural, watch: true).ConfigureAwait(false);
                        else
                            operationResponse = await this.KubernetesClient.ListClusterCustomObjectWithHttpMessagesAsync(this.ResourceDefinition.Group, this.ResourceDefinition.Version, this.ResourceDefinition.Plural, watch: true).ConfigureAwait(false);
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        this.Logger.LogInformation("Creating a new watcher for events on CRD of kind '{crdKind}' with API version '{apiVersion}' in namespace '{namespace}'.", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Namespace);
                        using (Watcher<TResource> watcher = operationResponse.Watch<TResource, object>((type, item) => this.EventHandler(type, item),
                            ex =>
                            {
                                cancellationTokenSource.Cancel();
                                this.Logger.LogError($"An exception occured while watching over the CRD of kind '{{crdKind}}' with API version '{{apiVersion}}' in namespace '{{namespace}}:{Environment.NewLine}{{ex}}'. Reconnecting...", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Namespace, ex.ToString());
                            },
                            () =>
                            {
                                cancellationTokenSource.Cancel();
                                this.Logger.LogInformation("The connection of the event watcher for CRD of kind '{crdKind}' with API version '{apiVersion}' in namespace '{namespace}' has been closed. Reconnecting...", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Namespace);
                            }))
                        {
                            this.Logger.LogInformation("Started listening for events on CRD of kind '{crdKind}' with API version '{apiVersion}' in namespace '{namespace}'.", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Namespace);
                            while (!stoppingToken.IsCancellationRequested
                                && !cancellationTokenSource.IsCancellationRequested)
                            {

                            }
                        }
                    }
                }
                catch (HttpOperationException ex)
                {
                    this.Logger.LogError($"An exception occured while processing the events of the CRD {{apiVersion}}. The server responded with a '{{statusCode}}' status code:{Environment.NewLine}{{responseContent}}. Reconnecting...", this.ResourceDefinition.ApiVersion, ex.Response.StatusCode, ex.Response.Content);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"An exception occured while processing the events of the CRD {{apiVersion}}:{Environment.NewLine}{{ex}}. Reconnecting...", this.ResourceDefinition.ApiVersion, ex.ToString());
                }
            }
            this.Logger.LogInformation("Stopped listening for events on CRD of kind '{crdKind}' with API version '{apiVersion}' in namespace '{namespace}'.", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion, this.Namespace);
        }

        /// <inheritdoc/>
        public virtual async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (this._ExecutingTask == null)
                return;
            try
            {
                this._StoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(this._ExecutingTask, Task.Delay(Timeout.Infinite, cancellationToken));
                this._ExecutingTask = null;
                this.Logger.LogInformation("Stopped listening for events on CRD of kind '{crdKind}', in group '{crdGroup}' and with version '{crdVersion}'", this.ResourceDefinition.Kind, this.ResourceDefinition.Group, this.ResourceDefinition.Version);
            }
        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="CustomResourceEventWatcher{TResource}"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="CustomResourceEventWatcher{TResource}"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                    this._StoppingCts.Cancel();
                this._Disposed = true;
                this.Logger.LogInformation("Disposed of the custom resource event watcher for CRD of kind '{crdKind}', in group '{crdGroup}' and with version '{crdVersion}'", this.ResourceDefinition.Kind, this.ResourceDefinition.Group, this.ResourceDefinition.Version);
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
