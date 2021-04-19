using System;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Represents a <see href="https://kubernetes.io/docs/tasks/extend-kubernetes/custom-resources/custom-resource-definitions/">Kubernetes Custom Resource Definition (CRD)</see>.
    /// </summary>
    public class CustomResourceDefinition
        : ICustomResourceDefinition
    {

        /// <summary>
        /// Initializes a new <see cref="CustomResourceDefinition"/>.
        /// </summary>
        /// <param name="apiVersion">The <see cref="CustomResourceDefinition"/>'s api version.</param>
        /// <param name="kind">The <see cref="CustomResourceDefinition"/>'s kind.</param>
        /// <param name="plural">The <see cref="CustomResourceDefinition"/>'s plural name.</param>
        public CustomResourceDefinition(string apiVersion, string kind, string plural)
        {
            if (string.IsNullOrWhiteSpace(apiVersion))
                throw new ArgumentNullException(nameof(apiVersion));
            if (string.IsNullOrWhiteSpace(kind))
                throw new ArgumentNullException(nameof(kind));
            if (string.IsNullOrWhiteSpace(plural))
                throw new ArgumentNullException(nameof(plural));
            this.ApiVersion = apiVersion;
            this.Kind = kind;
            this.Plural = plural;
        }

        /// <inheritdoc/>
        public string ApiVersion { get; }

        /// <inheritdoc/>
        public string Kind { get; }

        /// <inheritdoc/>
        public string Plural { get; }

        /// <inheritdoc/>
        public string Group
        {
            get
            {
                return this.ApiVersion?.Split("/")[0];
            }
        }

        /// <inheritdoc/>
        public string Version
        {
            get
            {
                return this.ApiVersion?.Split("/")[1];
            }
        }

    }

}
