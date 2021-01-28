using k8s;
using Newtonsoft.Json;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Represents the base class for all generic <see cref="IKubernetesObject"/>s
    /// </summary>
    public abstract class KubernetesObject<TMetadata>
        : IKubernetesObject<TMetadata>, IMetadata<TMetadata>
        where TMetadata : class
    {

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "apiVersion")]
        public string ApiVersion { get; set; }

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        /// <inheritdoc/>
        [JsonProperty(PropertyName = "metadata")]
        public TMetadata Metadata { get; set; }

    }

}
