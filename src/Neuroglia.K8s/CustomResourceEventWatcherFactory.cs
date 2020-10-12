using Microsoft.Extensions.DependencyInjection;
using System;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Represents the default implementation of the <see cref="ICustomResourceEventWatcherFactory"/> interface.
    /// </summary>
    public class CustomResourceEventWatcherFactory
        : ICustomResourceEventWatcherFactory
    {

        /// <summary>
        /// Initializes a new <see cref="CustomResourceEventWatcherFactory"/>.
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/>.</param>
        public CustomResourceEventWatcherFactory(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <inheritdoc/>
        public virtual ICustomResourceEventWatcher<TResource> CreateController<TResource>(ICustomResourceDefinition resourceDefinition, CustomResourceEventDelegate<TResource> eventHandler, string ns = null) 
            where TResource : ICustomResource
        {
            ICustomResourceEventWatcher<TResource> controller = ActivatorUtilities.CreateInstance<CustomResourceEventWatcher<TResource>>(this.ServiceProvider, resourceDefinition, eventHandler, ns);
            return controller;
        }

    }

}
