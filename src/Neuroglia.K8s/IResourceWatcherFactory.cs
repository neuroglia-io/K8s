using System;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Defines the fundamentals of a service used to create <see cref="IResourceWatcher"/>s
    /// </summary>
    public interface IResourceWatcherFactory
    {

        /// <summary>
        /// Create a new <see cref="IResourceWatcher"/> for the specified <see cref="ICustomResource"/> type
        /// </summary>
        /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to watch</typeparam>
        /// <param name="configuration">An <see cref="Action{T}"/> used to configure the <see cref="IResourceWatcher"/> to create</param>
        /// <returns>A new <see cref="IResourceWatcher"/></returns>
        IResourceWatcher<TResource> Create<TResource>(Action<IResourceWatcherOptionsBuilder<TResource>> configuration)
            where TResource : class, ICustomResource, new();

    }

}
