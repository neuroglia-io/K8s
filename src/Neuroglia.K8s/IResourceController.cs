using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Defines the fundamentals of a service used to control <see cref="ICustomResource"/>s
    /// </summary>
    public interface IResourceController
        : IHostedService, IDisposable, IAsyncDisposable
    {

        /// <summary>
        /// Gets the type of <see cref="ICustomResource"/>s managed by the <see cref="IResourceController"/>
        /// </summary>
        Type ResourceType { get; }

    }

    /// <summary>
    /// Defines the fundamentals of a service used to control <see cref="ICustomResource"/>s
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/>s to control</typeparam>
    public interface IResourceController<TResource>
        : IResourceController
        where TResource : class, ICustomResource, new()
    {

        /// <summary>
        /// Reconciles the states of managed <see cref="ICustomResource"/>s
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        Task ReconcileAsync(CancellationToken cancellationToken = default);

    }

}
