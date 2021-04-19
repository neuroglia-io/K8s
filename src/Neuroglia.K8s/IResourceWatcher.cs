using Microsoft.Extensions.Hosting;
using System;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Defines the fundamentals of a service used to listen for Kubernetes events about a specified <see cref="ICustomResource"/> type
    /// </summary>
    public interface IResourceWatcher
        : IHostedService, IDisposable, IAsyncDisposable
    {

        /// <summary>
        /// Gets the type of <see cref="ICustomResource"/> to listen Kubernetes events for
        /// </summary>
        Type ResourceType { get; }

    }

    /// <summary>
    /// Defines the fundamentals of a service used to listen for Kubernetes events about a specified <see cref="ICustomResource"/> type
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to listen Kubernetes events for</typeparam>
    public interface IResourceWatcher<TResource>
        : IResourceWatcher, IObservable<IResourceEvent<TResource>>
        where TResource : class, ICustomResource, new()
    {



    }

}
