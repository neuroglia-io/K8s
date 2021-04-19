using k8s;
using System;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IResourceControllerOptionsBuilder{TResource}"/> interface
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to watch</typeparam>
    public class ResourceWatcherOptionsBuilder<TResource>
        : IResourceWatcherOptionsBuilder<TResource>
        where TResource : class, ICustomResource, new()
    {

        /// <summary>
        /// Initializes a new <see cref="ResourceWatcherOptions{TResource}"/>
        /// </summary>
        /// <param name="options">The <see cref="ResourceWatcherOptions{TResource}"/> to configure</param>
        public ResourceWatcherOptionsBuilder(ResourceWatcherOptions<TResource> options)
        {
            this.Options = options;
        }

        /// <summary>
        /// Initializes a new <see cref="ResourceWatcherOptions{TResource}"/>
        /// </summary>
        public ResourceWatcherOptionsBuilder()
            : this(new ResourceWatcherOptions<TResource>())
        {
            
        }

        /// <summary>
        /// Gets the <see cref="ResourceWatcherOptions{TResource}"/> to configure
        /// </summary>
        protected ResourceWatcherOptions<TResource> Options { get; }

        /// <inheritdoc/>
        public virtual IResourceWatcherOptionsBuilder<TResource> InCluster()
        {
            this.Options.Namespace = null;
            return this;
        }

        /// <inheritdoc/>
        public virtual IResourceWatcherOptionsBuilder<TResource> InNamespace(string @namespace)
        {
            this.Options.Namespace = @namespace;
            return this;
        }

        /// <inheritdoc/>
        public virtual IResourceWatcherOptionsBuilder<TResource> WithLabel(string selector)
        {
            this.Options.LabelSelector = selector;
            return this;
        }

        /// <inheritdoc/>
        public virtual IResourceWatcherOptionsBuilder<TResource> WithField(string selector)
        {
            this.Options.FieldSelector = selector;
            return this;
        }

        /// <inheritdoc/>
        public virtual IResourceWatcherOptionsBuilder<TResource> When(Func<WatchEventType, TResource, bool> predicate)
        {
            this.Options.Predicate = predicate;
            return this;
        }

        /// <inheritdoc/>
        public virtual ResourceWatcherOptions<TResource> Build()
        {
            return this.Options;
        }

        IResourceWatcherOptions<TResource> IResourceWatcherOptionsBuilder<TResource>.Build()
        {
            return this.Options;
        }

    }
}
