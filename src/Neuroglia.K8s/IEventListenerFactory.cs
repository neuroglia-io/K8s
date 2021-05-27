using System;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Defines the fundamentals of a service used to create <see cref="IEventListener"/>s
    /// </summary>
    public interface IEventListenerFactory
    {

        /// <summary>
        /// Create a new <see cref="IEventListener"/>
        /// </summary>
        /// <param name="configuration">An <see cref="Action{T}"/> used to configure the <see cref="IEventListener"/> to create</param>
        /// <returns>A new <see cref="IEventListener"/></returns>
        IEventListener Create(Action<IEventListenerOptionsBuilder> configuration);

    }

}
