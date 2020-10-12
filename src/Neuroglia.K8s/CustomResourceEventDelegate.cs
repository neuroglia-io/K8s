using k8s;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Represents the delegate to invoke whenever a new Kubernetes event for the specified <see cref="ICustomResource"/> type has been received
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to listen Kubernetes events for</typeparam>
    /// <param name="e">The type of the watch event</param>
    /// <param name="resource">The <see cref="ICustomResource"/></param>
    public delegate void CustomResourceEventDelegate<TResource>(WatchEventType e, TResource resource)
        where TResource : ICustomResource;

}
