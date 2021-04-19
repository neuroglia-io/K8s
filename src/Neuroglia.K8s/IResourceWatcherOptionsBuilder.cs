using k8s;
using System;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Defines the fundamentals of a service used to build <see cref="IResourceWatcherOptions{TResource}"/>
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to watch</typeparam>
    public interface IResourceWatcherOptionsBuilder<TResource>
        where TResource : class, ICustomResource, new()
    {

        /// <summary>
        /// Configures the <see cref="IResourceWatcher"/> to watch for <see cref="ICustomResource"/>s cluster-wide. Will ignore the namespace, if it has been set.
        /// </summary>
        /// <returns>The configured <see cref="IResourceWatcherOptionsBuilder{TResource}"/></returns>
        IResourceWatcherOptionsBuilder<TResource> InCluster();

        /// <summary>
        /// Sets the namespace in which to watch the <see cref="ICustomResource"/>s. If the namespace is not set, the <see cref="IResourceWatcher"/> will watch for <see cref="ICustomResource"/>s cluster-wide
        /// </summary>
        /// <param name="namespace">The namespace to watch the <see cref="ICustomResource"/>s in</param>
        /// <returns>The configured <see cref="IResourceWatcherOptionsBuilder{TResource}"/></returns>
        IResourceWatcherOptionsBuilder<TResource> InNamespace(string @namespace);

        /// <summary>
        /// Sets the label selector by which to filter the <see cref="ICustomResource"/>s to watch
        /// </summary>
        /// <param name="selector">The label selector (ex: environment = production) to use</param>
        /// <returns>The configured <see cref="IResourceWatcherOptionsBuilder{TResource}"/></returns>
        IResourceWatcherOptionsBuilder<TResource> WithLabel(string selector);

        /// <summary>
        /// Sets the field selector by which to filter the <see cref="ICustomResource"/>s to watch
        /// </summary>
        /// <param name="selector">The field selector (ex: metadata.name) to use</param>
        /// <returns>The configured <see cref="IResourceWatcherOptionsBuilder{TResource}"/></returns>
        IResourceWatcherOptionsBuilder<TResource> WithField(string selector);

        /// <summary>
        /// Sets a predicate used to determine whether or not to handle the specified <see cref="ICustomResource"/>'s <see cref="WatchEventType"/>
        /// </summary>
        /// <param name="predicate">A predicate <see cref="Func{T1, T2, TResult}"/> used to determine whether or not to handle the specified <see cref="ICustomResource"/>'s <see cref="WatchEventType"/></param>
        /// <returns>The configured <see cref="IResourceWatcherOptionsBuilder{TResource}"/></returns>
        IResourceWatcherOptionsBuilder<TResource> When(Func<WatchEventType, TResource, bool> predicate);

        /// <summary>
        /// Builds the <see cref="IResourceWatcherOptions{TResource}"/>
        /// </summary>
        /// <returns>A new <see cref="IResourceWatcherOptions{TResource}"/></returns>
        IResourceWatcherOptions<TResource> Build();

    }

}
