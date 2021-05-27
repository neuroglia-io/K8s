using k8s;
using k8s.Models;
using System;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Defines the fundamentals of a service used to build <see cref="IEventListener"/>s
    /// </summary>
    public interface IEventListenerOptionsBuilder
    {

        /// <summary>
        /// Configures the <see cref="IEventListener"/> to watch for <see cref="Corev1Event"/>s cluster-wide. Will ignore the namespace, if it has been set.
        /// </summary>
        /// <returns>The configured <see cref="IEventListenerOptionsBuilder"/></returns>
        IEventListenerOptionsBuilder InCluster();

        /// <summary>
        /// Sets the namespace in which to watch the <see cref="Corev1Event"/>s. If the namespace is not set, the <see cref="IEventListener"/> will watch for <see cref="Corev1Event"/>s cluster-wide
        /// </summary>
        /// <param name="namespace">The namespace to watch the <see cref="Corev1Event"/>s in</param>
        /// <returns>The configured <see cref="IEventListenerOptionsBuilder"/></returns>
        IEventListenerOptionsBuilder InNamespace(string @namespace);

        /// <summary>
        /// Sets the label selector by which to filter the <see cref="Corev1Event"/>s to watch
        /// </summary>
        /// <param name="selector">The label selector (ex: environment = production) to use</param>
        /// <returns>The configured <see cref="IEventListenerOptionsBuilder"/></returns>
        IEventListenerOptionsBuilder WithLabel(string selector);

        /// <summary>
        /// Sets the field selector by which to filter the <see cref="Corev1Event"/>s to watch
        /// </summary>
        /// <param name="selector">The field selector (ex: metadata.name) to use</param>
        /// <returns>The configured <see cref="IEventListenerOptionsBuilder"/></returns>
        IEventListenerOptionsBuilder WithField(string selector);

        /// <summary>
        /// Sets a predicate used to determine whether or not to handle the specified <see cref="Corev1Event"/>'s <see cref="WatchEventType"/>
        /// </summary>
        /// <param name="predicate">A predicate <see cref="Func{T1, T2, TResult}"/> used to determine whether or not to handle the specified <see cref="Corev1Event"/>'s <see cref="WatchEventType"/></param>
        /// <returns>The configured <see cref="IEventListenerOptionsBuilder"/></returns>
        IEventListenerOptionsBuilder When(Func<WatchEventType, Corev1Event, bool> predicate);

        /// <summary>
        /// Builds the <see cref="EventListenerOptions"/>
        /// </summary>
        /// <returns>A new <see cref="EventListenerOptions"/></returns>
        EventListenerOptions Build();

    }

}
