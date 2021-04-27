using System;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Defines the fundamentals of a service used to build <see cref="IResourceControllerOptions{TResource}"/>
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/>s to manage</typeparam>
    public interface IResourceControllerOptionsBuilder<TResource>
        where TResource : class, ICustomResource, new()
    {

        /// <summary>
        /// Sets a boolean indicating whether or not to use automatic reconciliation
        /// </summary>
        /// <param name="enabled">A boolean indicating whether or not to use automatic reconciliation</param>
        /// <returns>The configured <see cref="IResourceControllerOptionsBuilder{TResource}"/></returns>
        IResourceControllerOptionsBuilder<TResource> UseAutomaticReconciliation(bool enabled = true);

        /// <summary>
        /// Sets the duration after which the <see cref="IResourceControllerOptionsBuilder{TResource}"/> will periodically auto-reconcile
        /// </summary>
        /// <param name="period">A <see cref="TimeSpan"/> representing the duration after which the <see cref="IResourceControllerOptionsBuilder{TResource}"/> will periodically auto-reconcile</param>
        /// <returns>The configured <see cref="IResourceControllerOptionsBuilder{TResource}"/></returns>
        IResourceControllerOptionsBuilder<TResource> ReconcileEvery(TimeSpan period);

        /// <summary>
        /// Configures the <see cref="IResourceWatcher{TResource}"/> used by the <see cref="IResourceController{TResource}"/> to configure
        /// </summary>
        /// <param name="watcherConfiguration">An <see cref="Action{T}"/> used to configure the <see cref="IResourceWatcher{TResource}"/> used by the <see cref="IResourceController{TResource}"/> to configure</param>
        /// <returns>The configured <see cref="IResourceControllerOptionsBuilder{TResource}"/></returns>
        IResourceControllerOptionsBuilder<TResource> ConfigureWatcher(Action<IResourceWatcherOptionsBuilder<TResource>> watcherConfiguration);

        /// <summary>
        /// Builds the <see cref="IResourceControllerOptions{TResource}"/>
        /// </summary>
        /// <returns>A new <see cref="IResourceControllerOptions{TResource}"/></returns>
        IResourceControllerOptions<TResource> Build();

    }

}
