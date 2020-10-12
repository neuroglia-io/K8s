using k8s;
using k8s.Models;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Defines the fundamentals of a <see href="https://kubernetes.io/docs/tasks/extend-kubernetes/custom-resources/custom-resource-definitions/">Kubernetes Custom Resource Definition (CRD)</see> instance
    /// </summary>
    public interface ICustomResource
        : IKubernetesObject<V1ObjectMeta>
    {

        /// <summary>
        /// Gets the <see cref="ICustomResource"/>'s definition
        /// </summary>
        ICustomResourceDefinition Definition { get; }

    }

    /// <summary>
    /// Defines the fundamentals of a <see href="https://kubernetes.io/docs/tasks/extend-kubernetes/custom-resources/custom-resource-definitions/">Kubernetes Custom Resource Definition (CRD)</see> instance
    /// </summary>
    /// <typeparam name="TSpec">The type of the <see cref="ICustomResource"/>'s specification</typeparam>
    public interface ICustomResource<TSpec>
        : ICustomResource
    {

        /// <summary>
        /// Gets the <see cref="ICustomResource"/>'s specifications.
        /// </summary>
        TSpec Spec { get; }

    }

    /// <summary>
    /// Defines the fundamentals of a <see href="https://kubernetes.io/docs/tasks/extend-kubernetes/custom-resources/custom-resource-definitions/">Kubernetes Custom Resource Definition (CRD)</see> instance
    /// </summary>
    /// <typeparam name="TSpec">The type of the custom object's specification</typeparam>
    /// <typeparam name="TStatus">The type of the <see cref="ICustomResource"/>'s status</typeparam>
    public interface ICustomResource<TSpec, TStatus>
        : ICustomResource<TSpec>
    {

        /// <summary>
        /// Gets the <see cref="ICustomResource"/>'s status.<para></para>
        /// Note that the status sub resource must be enabled in the <see href="https://kubernetes.io/docs/tasks/extend-kubernetes/custom-resources/custom-resource-definitions/">CRD</see> 
        /// as described in the <see href="https://kubernetes.io/docs/tasks/extend-kubernetes/custom-resources/custom-resource-definitions/#subresources">Kubernetes documentation</see>
        /// </summary>
        TStatus Status { get; }

    }

}
