using k8s;
using k8s.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Represents a list of <see cref="ICustomResource"/>s
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IKubernetesObject"/>s contained by the <see cref="KubernetesList{TObject}"/></typeparam>
    public class KubernetesList<T>
        : KubernetesObject<V1ListMeta>
        where T : class, IKubernetesObject
    {

        /// <summary>
        /// Gets/sets an <see cref="IList{T}"/> containing the <see cref="IKubernetesObject"/>s the <see cref="KubernetesList{TObject}"/> is made out of
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public IList<T> Items { get; set; }

    }

}
