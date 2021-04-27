using System;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Represents the options used to configure an <see cref="IResourceController"/>
    /// </summary>
    /// <typeparam name="TResource">The type of <see cref="ICustomResource"/> to manage</typeparam>
    public class ResourceControllerOptions<TResource>
        : IResourceControllerOptions<TResource>
        where TResource : class, ICustomResource, new()
    {

        /// <summary>
        /// Initializes a new <see cref="ResourceControllerOptions{TResource}"/>
        /// </summary>
        public ResourceControllerOptions()
        {
            this.AutoReconcile = true;
            this.ReconciliationPeriod = TimeSpan.FromSeconds(60);
        }

        /// <inheritdoc/>
        public bool AutoReconcile { get; set; }

        /// <inheritdoc/>
        public TimeSpan ReconciliationPeriod { get; set; }

        /// <inheritdoc/>
        public Action<IResourceWatcherOptionsBuilder<TResource>> WatchConfiguration { get; set; } = builder => { };

    }

}
