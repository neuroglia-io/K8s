using Microsoft.Extensions.DependencyInjection;
using System;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IEventListener"/> interface
    /// </summary>
    public class EventListenerFactory
        : IEventListenerFactory
    {

        /// <summary>
        /// Initializes a new <see cref="EventListenerFactory"/>
        /// </summary>
        /// <param name="serviceProvider">The current <see cref="IServiceProvider"/></param>
        public EventListenerFactory(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the current <see cref="IServiceProvider"/>
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <inheritdoc/>
        public IEventListener Create(Action<IEventListenerOptionsBuilder> configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            IEventListenerOptionsBuilder builder = new EventListenerOptionsBuilder();
            configuration(builder);
            EventListenerOptions options = builder.Build();
            return ActivatorUtilities.CreateInstance<EventListener>(this.ServiceProvider, options);
        }

    }

}
