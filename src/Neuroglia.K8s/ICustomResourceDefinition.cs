namespace Neuroglia.K8s
{

    /// <summary>
    /// Defines the fundamentals of a <see href="https://kubernetes.io/docs/tasks/extend-kubernetes/custom-resources/custom-resource-definitions/">Kubernetes Custom Resource Definition (CRD)</see>
    /// </summary>
    public interface ICustomResourceDefinition
    {

        /// <summary>
        /// Gets the <see cref="ICustomResourceDefinition"/>'s api version
        /// </summary>
        string ApiVersion { get; }

        /// <summary>
        /// Gets the <see cref="ICustomResourceDefinition"/>'s kind
        /// </summary>
        string Kind { get; }

        /// <summary>
        /// Gets the <see cref="ICustomResourceDefinition"/>'s plural name
        /// </summary>
        string Plural { get; }

        /// <summary>
        /// Gets the <see cref="ICustomResourceDefinition"/>'s group
        /// </summary>
        string Group { get; }

        /// <summary>
        /// Gets the <see cref="ICustomResourceDefinition"/>'s version
        /// </summary>
        string Version { get; }

    }

}
