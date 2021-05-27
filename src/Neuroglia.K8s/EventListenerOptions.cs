using k8s;
using k8s.Models;
using System;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Represents the options used to configure an <see cref="IEventListener"/>
    /// </summary>
    public class EventListenerOptions
    {

        /// <summary>
        /// Gets the namespace in which to listen for <see cref="Corev1Event"/>s
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets the label selector by which to filter the <see cref="Corev1Event"/>s to listen for
        /// </summary>
        public string LabelSelector { get; set; }

        /// <summary>
        /// Gets the field selector by which to filter the <see cref="Corev1Event"/>s to listen for
        /// </summary>
        public string FieldSelector { get; set; }

        /// <inheritdoc/>
        public Func<WatchEventType, Corev1Event, bool> Predicate { get; set; }

    }

}
