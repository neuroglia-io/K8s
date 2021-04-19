using k8s;
using System;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Represents the options used to configure an <see cref="IResourceWatcher"/>
    /// </summary>
    public class ResourceWatcherOptions<TResource>
        : IResourceWatcherOptions<TResource>
        where TResource : class, ICustomResource, new()
    {

        /// <inheritdoc/>
        public string Namespace { get; set; }

        /// <inheritdoc/>
        public string LabelSelector { get; set; }

        /// <inheritdoc/>
        public string FieldSelector { get; set; }

        /// <inheritdoc/>
        public Func<WatchEventType, TResource, bool> Predicate { get; set; }

    }

}
