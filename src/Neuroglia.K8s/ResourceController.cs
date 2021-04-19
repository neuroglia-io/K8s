using k8s;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Represents the default base class for all <see cref="IResourceController"/> implementations
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to watch</typeparam>
    public abstract class ResourceController<TResource>
        : IResourceController<TResource>
        where TResource : class, ICustomResource, new()
    {

        private Timer _ReconcilitationTimer;

        /// <summary>
        /// Initializes a new <see cref="ResourceController{TResource}"/>
        /// </summary>
        /// <param name="loggerFactory">The service used to create <see cref="ILogger"/>s</param>
        /// <param name="kubernetes">The Kubernets API client service</param>
        /// <param name="resourceWatcherFactory">The service used to create <see cref="IResourceWatcherFactory"/> instances</param>
        /// <param name="options">The service used to retrieve the <see cref="ResourceController{TResource}"/>'s <see cref="ResourceControllerOptions{TResource}"/></param>
        protected ResourceController(ILoggerFactory loggerFactory, IKubernetes kubernetes, IResourceWatcherFactory resourceWatcherFactory, IOptions<ResourceControllerOptions<TResource>> options)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
            this.Kubernetes = kubernetes;
            this.Options = options.Value;
            this.ResourceWatcher = resourceWatcherFactory.Create(this.Options.WatchConfiguration);
            this.ResourceDefinition = this.ResourceDefinition = new TResource().Definition;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the Kubernets API client service
        /// </summary>
        protected IKubernetes Kubernetes { get; }

        /// <summary>
        /// Gets the service used to watch <see cref="ICustomResource"/>s of the specified type
        /// </summary>
        protected IResourceWatcher<TResource> ResourceWatcher { get; }

        /// <summary>
        /// Gets the <see cref="ICustomResourceDefinition"/> of the controlled <see cref="ICustomResource"/> type
        /// </summary>
        protected ICustomResourceDefinition ResourceDefinition { get; }

        /// <summary>
        /// Gets the options used to configure the <see cref="ResourceController{TResource}"/>
        /// </summary>
        protected ResourceControllerOptions<TResource> Options { get; }

        Type IResourceController.ResourceType
        {
            get
            {
                return typeof(TResource);
            }
        }

        /// <inheritdoc/>
        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await this.ReconcileAsync(cancellationToken);
            this.ResourceWatcher.Subscribe(this.OnEvent, this.OnError, this.OnCompleted);
            await this.ResourceWatcher.StartAsync(cancellationToken);
            if(this.Options.AutoReconcile)
                this._ReconcilitationTimer = new Timer(this.OnReconcileTimerHeartbeat, null, this.Options.ReconciliationPeriod, this.Options.ReconciliationPeriod);
        }

        /// <inheritdoc/>
        public abstract Task ReconcileAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            this._ReconcilitationTimer?.Dispose();
            this._ReconcilitationTimer = null;
            await this.ResourceWatcher.StopAsync(cancellationToken);
        }

        private void OnReconcileTimerHeartbeat(object state)
        {
            _ = this.ReconcileAsync();
        }

        /// <summary>
        /// Handles the specified <see cref="IResourceEvent{TResource}"/>
        /// </summary>
        /// <param name="e">The <see cref="IResourceEvent{TResource}"/> to handle</param>
        protected virtual void OnEvent(IResourceEvent<TResource> e)
        {
            this.Logger.LogDebug("An event of type '{type}' has been received concerning a resource of kind '{kind}' (apiVersion={apiVersion})", e.Type, this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion);
        }

        /// <summary>
        /// Handles an <see cref="Exception"/> that has occured while watching the <see cref="ICustomResource"/>s of the specified type
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/></param>
        protected virtual void OnError(Exception ex)
        {
            this.Logger.LogError("An exception has occured while watching custom resources of kind '{kind}' (apiVersion={apiVersion})", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion);
            _ = this.ReconcileAsync();
        }

        /// <summary>
        /// Handles the completion of the subscription's stream
        /// </summary>
        protected virtual void OnCompleted()
        {
            this.Logger.LogDebug("Completed watching custom resources of kind '{kind}' (apiVersion={apiVersion})", this.ResourceDefinition.Kind, this.ResourceDefinition.ApiVersion);
        }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="IResourceController"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="IResourceController"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    this.StopAsync(default).GetAwaiter().GetResult();
                    this._ReconcilitationTimer?.Dispose();
                    this.ResourceWatcher?.Dispose();
                }
                this._Disposed = true;
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
