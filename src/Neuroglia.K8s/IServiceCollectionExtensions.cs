using k8s;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddSingleton<IKubernetes>(new Kubernetes(configuration));
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
        /// Adds an <see cref="ICustomResourceEventWatcherFactory"/> service of the specified type
        /// </summary>
        /// <typeparam name="TFactory">The type of <see cref="ICustomResourceEventWatcherFactory"/> to use</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddCustomResourceEventWatcherFactory<TFactory>(this IServiceCollection services)
            where TFactory : class, ICustomResourceEventWatcherFactory
        {
            services.AddSingleton<ICustomResourceEventWatcherFactory, TFactory>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="ICustomResourceEventWatcherFactory"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddCustomResourceEventWatcherFactory(this IServiceCollection services)
        {
            services.AddCustomResourceEventWatcherFactory<CustomResourceEventWatcherFactory>();
            return services;
        }

    }

}
