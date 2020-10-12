using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Defines the fundamentals of a service used to listen for Kubernetes events about a specified <see cref="ICustomResource"/> type
    /// </summary>
    public interface ICustomResourceEventWatcher
        : IDisposable, IAsyncDisposable
    {

        /// <summary>
        /// Gets the type of <see cref="ICustomResource"/> to listen Kubernetes events for
        /// </summary>
        Type ObjectType { get; }

        /// <summary>
        /// Starts listening for Kubernetes events concerning the specified <see cref="ICustomResource"/> type
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops listening for Kubernetes events
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task StopAsync(CancellationToken cancellationToken = default);

    }

    /// <summary>
    /// Defines the fundamentals of a service used to listen for Kubernetes events about a specified <see cref="ICustomResource"/> type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to listen Kubernetes events for</typeparam>
    public interface ICustomResourceEventWatcher<TResource>
        : ICustomResourceEventWatcher
        where TResource : ICustomResource
    {



    }

}
