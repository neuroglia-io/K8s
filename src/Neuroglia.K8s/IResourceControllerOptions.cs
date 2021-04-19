using System;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Defines the fundamentals of the options used to configure an <see cref="IResourceController"/>
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to manage</typeparam>
    public interface IResourceControllerOptions<TResource>
        where TResource : class, ICustomResource, new()
    {

        /// <summary>
        /// Gets a boolean indicating whether or not the <see cref="IResourceController"/> to configure should automatically and periodically reconcile
        /// </summary>
        bool AutoReconcile { get; }

        /// <summary>
        /// Gets the <see cref="TimeSpan"/> that represents the period of time after which to reconcile the <see cref="IResourceController"/> to configure
        /// </summary>
        TimeSpan ReconciliationPeriod { get; }

        /// <summary>
        /// Gets an <see cref="Action{T}"/> used to configure the <see cref="IResourceWatcher"/> used by the <see cref="IResourceController"/> to configure
        /// </summary>
        Action<IResourceWatcherOptionsBuilder<TResource>> WatchConfiguration { get; }

    }

}
