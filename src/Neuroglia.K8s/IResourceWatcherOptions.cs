using k8s;
using System;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Defines the fundamentals of the options used to configure an <see cref="IResourceWatcher{TResource}"/>
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to watch</typeparam>
    public interface IResourceWatcherOptions<TResource>
        where TResource : class, ICustomResource, new()
    {

        /// <summary>
        /// Gets the namespace in which to listen for <see cref="ICustomResource"/>s
        /// </summary>
        string Namespace { get; }

        /// <summary>
        /// Gets the label selector by which to filter the <see cref="ICustomResource"/>s to watch
        /// </summary>
        string LabelSelector { get; }

        /// <summary>
        /// Gets the field selector by which to filter the <see cref="ICustomResource"/>s to watch
        /// </summary>
        string FieldSelector { get; }

        /// <summary>
        /// Gets a predicate <see cref="Func{T, TResult}"/> to determine whether or not to handle an event fired by the watched <see cref="ICustomResource"/>
        /// </summary>
        /// <inheritdoc/>
        public Func<WatchEventType, TResource, bool> Predicate { get; }

    }

}
