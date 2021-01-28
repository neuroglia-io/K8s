using k8s;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Defines extensions for <see cref="IServiceCollection"/>s
    /// </summary>
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// Adds and configures an <see cref="IKubernetes"/> service
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="configuration">The <see cref="KubernetesClientConfiguration"/> to use</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddKubernetesClient(this IServiceCollection services, KubernetesClientConfiguration configuration)
        {
            services.TryAddSingleton<IKubernetes>(new Kubernetes(configuration));
            return services;
        }

        /// <summary>
        /// Adds <see cref="IKubernetes"/> service with an <see cref="KubernetesClientConfiguration.InClusterConfig"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddKubernetesClient(this IServiceCollection services)
        {
            services.AddKubernetesClient(KubernetesClientConfiguration.InClusterConfig());
            return services;
        }

        /// <summary>
        /// Adds an <see cref="ICustomResourceWatcher"/> for the specified custom resource type
        /// </summary>
        /// <typeparam name="TResource">The type of the custom resource to watch</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="namespace">The namespace of the custom resources to watch</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddCustomResourceWatcher<TResource>(this IServiceCollection services, string @namespace)
            where TResource : ICustomResource, new()
        {
            if(!string.IsNullOrWhiteSpace(@namespace))
                services.TryAddSingleton<ICustomResourceNamespace<TResource>>(new CustomResourceNamespace<TResource>(@namespace));
            services.TryAddSingleton(typeof(ICustomResourceWatcher<>), typeof(CustomResourceWatcher<>));
            return services;
        }

        /// <summary>
        /// Adds an <see cref="ICustomResourceWatcher"/> for the specified custom resource type
        /// </summary>
        /// <typeparam name="TResource">The type of the custom resource to watch</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddCustomResourceWatcher<TResource>(this IServiceCollection services)
            where TResource : ICustomResource, new()
        {
            return services.AddCustomResourceWatcher<TResource>(null);
        }

    }

}
