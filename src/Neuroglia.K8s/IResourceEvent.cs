using k8s;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Defines the fundamentals of an event originating from a Kubernetes custom resource of the specified type 
    /// </summary>
    /// <typeparam name="TResource">The type of the custom resource the event concerns</typeparam>
    public interface IResourceEvent<TResource>
        where TResource : class, ICustomResource, new()
    {

        /// <summary>
        /// Gets the event's type
        /// </summary>
        WatchEventType Type { get; }

        /// <summary>
        /// Gets the custom resource that has produced the event
        /// </summary>
        TResource Resource { get; }

    }

}
