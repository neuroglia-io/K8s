namespace Neuroglia.K8s
{

    /// <summary>
    /// Defines the fundamentals of a service used to create new <see cref="ICustomResourceEventWatcher"/>s.
    /// </summary>
    public interface ICustomResourceEventWatcherFactory
    {

        /// <summary>
        /// Creates a new <see cref="ICustomResourceEventWatcher"/> instance for the specified <see cref="ICustomResource"/> type.
        /// </summary>
        /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to create a new <see cref="ICustomResourceEventWatcher"/> for.</typeparam>
        /// <param name="resourceDefinition">The <see cref="ICustomResourceDefinition"/> that defines the <see cref="ICustomResource"/> type to create a new <see cref="ICustomResourceEventWatcher"/> for.</param>
        /// <param name="eventHandler">The <see cref="CustomResourceEventDelegate{TResource}"/> to invoke whenever a new Kubernetes event for the specified <see cref="ICustomResource"/> type has been received.</param>
        /// <param name="ns">The namespace in which to listen for Kubernetes events for the specified <see cref="ICustomResource"/> type. If null or empty, the controller will listen for cluster-wide events instead.</param>
        /// <returns>A new <see cref="ICustomResourceEventWatcher"/> instance</returns>
        ICustomResourceEventWatcher<TResource> CreateController<TResource>(ICustomResourceDefinition resourceDefinition, CustomResourceEventDelegate<TResource> eventHandler, string ns = null)
            where TResource : ICustomResource; 

    }

}
