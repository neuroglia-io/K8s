using System;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IResourceControllerOptionsBuilder{TResource}"/> interface
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to manage</typeparam>
    public class ResourceControllerOptionsBuilder<TResource>
        : IResourceControllerOptionsBuilder<TResource>
         where TResource : class, ICustomResource, new()
    {

        /// <summary>
        /// Initializes a new <see cref="ResourceControllerOptionsBuilder{TResource}"/>
        /// </summary>
        /// <param name="options">The <see cref="ResourceControllerOptions{TResource}"/> to configure</param>
        public ResourceControllerOptionsBuilder(ResourceControllerOptions<TResource> options)
        {
            this.Options = options;
        }

        /// <summary>
        /// Initializes a new <see cref="ResourceControllerOptionsBuilder{TResource}"/>
        /// </summary>
        public ResourceControllerOptionsBuilder()
            : this(new ResourceControllerOptions<TResource>())
        {

        }

        /// <summary>
        /// Gets the <see cref="IResourceControllerOptions{TResource}"/> to configure
        /// </summary>
        protected ResourceControllerOptions<TResource> Options { get; }

        /// <inheritdoc/>
        public virtual IResourceControllerOptionsBuilder<TResource> UseAutomaticReconciliation(bool autoReconcile = true)
        {
            this.Options.AutoReconcile = autoReconcile;
            return this;
        }

        /// <inheritdoc/>
        public virtual IResourceControllerOptionsBuilder<TResource> ReconcileEvery(TimeSpan period)
        {
            this.Options.AutoReconcile = true;
            this.Options.ReconciliationPeriod = period;
            return this;
        }

        /// <inheritdoc/>
        public virtual IResourceControllerOptionsBuilder<TResource> Watch(Action<IResourceWatcherOptionsBuilder<TResource>> wacthConfiguration)
        {
            this.Options.WatchConfiguration = wacthConfiguration;
            return this;
        }

        /// <inheritdoc/>
        public virtual ResourceControllerOptions<TResource> Build()
        {
            return this.Options;
        }

        IResourceControllerOptions<TResource> IResourceControllerOptionsBuilder<TResource>.Build()
        {
            return this.Build();
        }

    }

}
