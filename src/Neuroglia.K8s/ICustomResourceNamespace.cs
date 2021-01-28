namespace Neuroglia.K8s
{
    /// <summary>
    /// Defines the fundamentals of an object used to describe the namespace of a custom resource type
    /// </summary>
    /// <typeparam name="TResource">The type of the namespaced custom resource</typeparam>
    public interface ICustomResourceNamespace<TResource>
        where TResource : ICustomResource, new()
    {

        /// <summary>
        /// Gets the custom resource's namespace
        /// </summary>
        string Namespace { get; }

    }

}
