using k8s;
using k8s.Models;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Defines extensions for <see cref="IKubernetes"/>
    /// </summary>
    public static class IKubernetesExtensions
    {

        /// <summary>
        /// Creates a new namespaced custom object of the specified type
        /// </summary>
        /// <typeparam name="TResource">The type of custom object to create</typeparam>
        /// <param name="kubernetes">The extended <see cref="IKubernetes"/></param>
        /// <param name="resource">The resource to create</param>
        /// <param name="namespaceParameter">The namespace for which to create the specified custom object</param>
        /// <param name="pretty">A boolean indicating whether or not the resource should be pretty-printed</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The newly created resource</returns>
        public static async Task<TResource> CreateNamespacedCustomObjectAsync<TResource>(this IKubernetes kubernetes, TResource resource, string namespaceParameter, bool pretty = false, CancellationToken cancellationToken = default)
            where TResource : class, ICustomResource
        {
            return await kubernetes.CreateNamespacedCustomObjectAsync(resource, resource.Definition.Group, resource.Definition.Version, namespaceParameter, resource.Definition.Plural, null, null, pretty ? "true" : null, cancellationToken) as TResource;
        }

        /// <summary>
        /// Creates a new namespaced custom object of the specified type
        /// </summary>
        /// <typeparam name="TResource">The type of custom object to create</typeparam>
        /// <param name="kubernetes">The extended <see cref="IKubernetes"/></param>
        /// <param name="resource">The resource to create</param>
        /// <param name="pretty">A boolean indicating whether or not the resource should be pretty-printed</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>The newly created resource</returns>
        public static async Task<TResource> CreateClusterCustomObjectAsync<TResource>(this IKubernetes kubernetes, TResource resource, bool pretty = false, CancellationToken cancellationToken = default)
            where TResource : class, ICustomResource
        {
            return await kubernetes.CreateClusterCustomObjectAsync(resource, resource.Definition.Group, resource.Definition.Version, resource.Definition.Plural, null, null, pretty ? "true" : null, cancellationToken) as TResource;
        }

        /// <summary>
        /// Lists namespaced custom objects
        /// </summary>
        /// <typeparam name="T">The type of custom resource to list</typeparam>
        /// <param name="kubernetes">The extended <see cref="IKubernetes"/></param>
        /// <param name="group">The custom resource's group name</param>
        /// <param name="version">The custom resource's version</param>
        /// <param name="namespaceParameter">The custom resource's namespace</param>
        /// <param name="plural">The custom resource's plural name. For TPRs this would be lowercase plural kind.</param>
        /// <param name="continueParameter">The continue option should be set when retrieving more results from the server.</param>
        /// <param name="fieldSelector">A selector to restrict the list of returned objects by their fields. Defaults to everything.</param>
        /// <param name="labelSelector">A selector to restrict the list of returned objects by their labels. Defaults to everything.</param>
        /// <param name="limit">Limit is a maximum number of responses to return for a list call.</param>
        /// <param name="resourceVersion"> When specified with a watch call, shows changes that occur after that particular version of a resource.</param>
        /// <param name="timeoutSeconds">Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.</param>
        /// <param name="watch"> Watch for changes to the described resources and return them as a stream of add, update, and remove notifications.</param>
        /// <param name="pretty">A boolean indicating whether or not to pretty print the output</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="KubernetesList{T}"/> containing custom objects of the specified type</returns>
        public static async Task<KubernetesList<T>> ListNamespacedCustomObjectAsync<T>(this IKubernetes kubernetes, string group, string version, string namespaceParameter, string plural, string continueParameter = null, string fieldSelector = null,
            string labelSelector = null, int? limit = null, string resourceVersion = null, int? timeoutSeconds = null, bool? watch = null, bool pretty = false, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject
        {
            JObject result = (JObject)await kubernetes.ListNamespacedCustomObjectAsync(group, version, namespaceParameter, plural, continueParameter, fieldSelector, labelSelector, limit, resourceVersion, timeoutSeconds, watch, pretty ? "true" : null, cancellationToken);
            return result.ToObject<KubernetesList<T>>();
        }

        /// <summary>
        /// Lists cluster-wide custom objects
        /// </summary>
        /// <typeparam name="T">The type of custom resource to list</typeparam>
        /// <param name="kubernetes">The extended <see cref="IKubernetes"/></param>
        /// <param name="group">The custom resource's group name</param>
        /// <param name="version">The custom resource's version</param>
        /// <param name="plural">The custom resource's plural name. For TPRs this would be lowercase plural kind.</param>
        /// <param name="continueParameter">The continue option should be set when retrieving more results from the server.</param>
        /// <param name="fieldSelector">A selector to restrict the list of returned objects by their fields. Defaults to everything.</param>
        /// <param name="labelSelector">A selector to restrict the list of returned objects by their labels. Defaults to everything.</param>
        /// <param name="limit">Limit is a maximum number of responses to return for a list call.</param>
        /// <param name="resourceVersion"> When specified with a watch call, shows changes that occur after that particular version of a resource.</param>
        /// <param name="timeoutSeconds">Timeout for the list/watch call. This limits the duration of the call, regardless of any activity or inactivity.</param>
        /// <param name="watch"> Watch for changes to the described resources and return them as a stream of add, update, and remove notifications.</param>
        /// <param name="pretty">A boolean indicating whether or not to pretty print the output</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/></param>
        /// <returns>A new <see cref="KubernetesList{T}"/> containing custom objects of the specified type</returns>
        public static async Task<KubernetesList<T>> ListClusterCustomObjectAsync<T>(this IKubernetes kubernetes, string group, string version, string plural, string continueParameter = null, string fieldSelector = null,
            string labelSelector = null, int? limit = null, string resourceVersion = null, int? timeoutSeconds = null, bool? watch = null, bool pretty = false, CancellationToken cancellationToken = default)
            where T : class, IKubernetesObject
        {
            JObject result = (JObject)await kubernetes.ListClusterCustomObjectAsync(group, version, plural, continueParameter, fieldSelector, labelSelector, limit, resourceVersion, timeoutSeconds, watch, pretty ? "true" : null, cancellationToken);
            return result.ToObject<KubernetesList<T>>();
        }

    }

}
