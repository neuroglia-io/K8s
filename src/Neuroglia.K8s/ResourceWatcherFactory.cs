using Microsoft.Extensions.DependencyInjection;
using System;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Represents the default implementation of the <see cref="IResourceWatcherFactory"/> interface
    /// </summary>
    public class ResourceWatcherFactory
        : IResourceWatcherFactory
    {

        /// <summary>
        /// Initializes a new <see cref="ResourceWatcherFactory"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        public ResourceWatcherFactory(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <inheritdoc/>
        public virtual IResourceWatcher<TResource> Create<TResource>(Action<IResourceWatcherOptionsBuilder<TResource>> configuration)
            where TResource : class, ICustomResource, new()
        {
            IResourceWatcherOptionsBuilder<TResource> optionsBuilder = new ResourceWatcherOptionsBuilder<TResource>();
            configuration?.Invoke(optionsBuilder);
            IResourceWatcherOptions<TResource> options = optionsBuilder.Build();
            return ActivatorUtilities.CreateInstance<ResourceWatcher<TResource>>(this.ServiceProvider, options);
        }

    }

}
