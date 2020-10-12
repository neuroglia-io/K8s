using k8s;
using k8s.Models;
using Newtonsoft.Json;
using System;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Represents an instance of a <see href="https://kubernetes.io/docs/tasks/extend-kubernetes/custom-resources/custom-resource-definitions/">Kubernetes Custom Resource Definition (CRD)</see>
    /// </summary>
    public abstract class CustomResource
        : KubernetesObject, ICustomResource
    {

        /// <summary>
        /// Initializes a new <see cref="CustomResource"/>
        /// </summary>
        /// <param name="definition">The <see cref="CustomResource"/>'s definition</param>
        protected CustomResource(ICustomResourceDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));
            this.Definition = definition;
            this.Kind = this.Definition.Kind;
            this.ApiVersion = this.Definition.ApiVersion;
        }

        /// <inheritdoc/>
        [JsonIgnore]
        public ICustomResourceDefinition Definition { get; }

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "metadata")]
        public V1ObjectMeta Metadata { get; set; }

        /// <summary>
        /// Validates the <see cref="CustomResource"/>
        /// </summary>
        public virtual void Validate()
        {

        }

    }

    /// <summary>
    /// Represents an instance of a <see href="https://kubernetes.io/docs/tasks/extend-kubernetes/custom-resources/custom-resource-definitions/">Kubernetes Custom Resource Definition (CRD)</see>
    /// </summary>
    /// <typeparam name="TSpec">The type of the custom object's specification</typeparam>
    public abstract class CustomResource<TSpec>
        : CustomResource, ICustomResource<TSpec>
    {

        /// <summary>
        /// Initializes a new <see cref="CustomResource"/>
        /// </summary>
        /// <param name="definition">The <see cref="CustomResource"/>'s definition</param>
        protected CustomResource(ICustomResourceDefinition definition) 
            : base(definition)
        {

        }

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "spec")]
        public TSpec Spec { get; set; }

    }

    /// <summary>
    /// Represents an instance of a <see href="https://kubernetes.io/docs/tasks/extend-kubernetes/custom-resources/custom-resource-definitions/">Kubernetes Custom Resource Definition (CRD)</see>
    /// </summary>
    /// <typeparam name="TSpec">The type of the custom object's specification</typeparam>
    /// <typeparam name="TStatus">The type of the custom object's status</typeparam>
    public abstract class CustomResource<TSpec, TStatus>
        : CustomResource<TSpec>, ICustomResource<TSpec, TStatus>
    {

        /// <summary>
        /// Initializes a new <see cref="CustomResource"/>
        /// </summary>
        /// <param name="definition">The <see cref="CustomResource"/>'s definition</param>
        protected CustomResource(ICustomResourceDefinition definition) 
            : base(definition)
        {

        }

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "status")]
        public TStatus Status { get; set; }

    }

}
