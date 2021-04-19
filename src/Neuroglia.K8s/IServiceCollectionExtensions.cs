﻿using k8s;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;

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
            services.TryAddSingleton(Options.Create(configuration));
            services.TryAddSingleton<IKubernetes>(provider => new Kubernetes(provider.GetRequiredService<IOptions<KubernetesClientConfiguration>>().Value));
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
        /// Adds a <see cref="IResourceWatcherFactory"/>
        /// </summary>
        /// <typeparam name="TFactory">The type of the <see cref="IResourceWatcherFactory"/> to add</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddResourceWatcherFactory<TFactory>(this IServiceCollection services)
            where TFactory : class, IResourceWatcherFactory
        {
            services.TryAddSingleton<IResourceWatcherFactory, TFactory>();
            return services;
        }

        /// <summary>
        /// Adds the default <see cref="IResourceWatcherFactory"/>
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddResourceWatcherFactory(this IServiceCollection services)
        {
            services.AddResourceWatcherFactory<ResourceWatcherFactory>();
            return services;
        }

        /// <summary>
        /// Adds and configures a new <see cref="IResourceWatcher{TResource}"/>
        /// </summary>
        /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to watch</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="configuration">An <see cref="Action{T}"/> used to configure the <see cref="IResourceWatcher"/></param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddResourceWatcher<TResource>(this IServiceCollection services, Action<IResourceWatcherOptionsBuilder<TResource>> configuration)
            where TResource : class, ICustomResource, new()
        {
            services.AddSingleton(provider => provider.GetRequiredService<IResourceWatcherFactory>().Create(configuration));
            services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<IResourceWatcher<TResource>>());
            return services;
        }

        /// <summary>
        /// Adds and configures a new <see cref="IResourceController{TResource}"/>
        /// </summary>
        /// <typeparam name="TController">The type of the <see cref="IResourceController{TResource}"/> implementation to add</typeparam>
        /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to manage</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to configure</param>
        /// <param name="configuration">An <see cref="Action{T}"/> used to configure the <see cref="IResourceController"/> to add</param>
        /// <returns>The configured <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddResourceController<TController, TResource>(this IServiceCollection services, Action<IResourceControllerOptionsBuilder<TResource>> configuration)
            where TController : class, IResourceController<TResource>
            where TResource : class, ICustomResource, new()
        {
            ResourceControllerOptionsBuilder<TResource> optionsBuilder = new();
            configuration?.Invoke(optionsBuilder);
            ResourceControllerOptions<TResource> options = optionsBuilder.Build();
            services.AddSingleton(Options.Create(options));
            services.AddHostedService<ResourceController<TResource>>();
            return services;
        }

    }

}
