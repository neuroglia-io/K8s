using k8s;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IResourceEvent{TResource}"/> interface
    /// </summary>
    /// <typeparam name="TResource">The type of the custom resource the event concerns</typeparam>
    public class ResourceEvent<TResource>
        : IResourceEvent<TResource>
         where TResource : ICustomResource, new()
    {

        /// <summary>
        /// Initializes a new <see cref="ResourceEvent{TResource}"/>
        /// </summary>
        /// <param name="type">The event's type</param>
        /// <param name="resource">The custom resource that has produced the event</param>
        public ResourceEvent(WatchEventType type, TResource resource)
        {
            this.Type = type;
            this.Resource = resource;
        }

        /// <inheritdoc/>
        public WatchEventType Type { get; }

        /// <inheritdoc/>
        public TResource Resource { get; }

    }

}
