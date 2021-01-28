namespace Neuroglia.K8s
{
    /// <summary>
    /// Represents the default implementation of the <see cref="ICustomResourceNamespace{TResource}"/> interface
    /// </summary>
    /// <typeparam name="TResource">The type of the namespaced custom resource</typeparam>
    public class CustomResourceNamespace<TResource>
        : ICustomResourceNamespace<TResource>
        where TResource : ICustomResource, new()
    {

        /// <summary>
        /// Initializes a new <see cref="CustomResourceNamespace{TResource}"/>
        /// </summary>
        /// <param name="ns">The custom resource's namespace</param>
        public CustomResourceNamespace(string ns)
        {
            this.Namespace = ns;
        }

        /// <summary>
        /// Gets the custom resource's namespace
        /// </summary>
        public string Namespace { get; }

    }

}
