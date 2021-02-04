namespace Neuroglia.K8s
{
    /// <summary>
    /// Defines the fundamentals of a service used to build <see cref="CustomResourceWatcherOptions"/>
    /// </summary>
    public interface ICustomResourceWatcherOptionsBuilder
    {

        /// <summary>
        /// Sets the namespace in which to watch the <see cref="ICustomResource"/>s. If the namespace is not set, the <see cref="ICustomResourceWatcher"/> will watch for <see cref="ICustomResource"/>s cluster-wide
        /// </summary>
        /// <param name="namespace">The namespace to watch the <see cref="ICustomResource"/>s in</param>
        /// <returns>The configured <see cref="ICustomResourceWatcherOptionsBuilder"/></returns>
        ICustomResourceWatcherOptionsBuilder WatchIn(string @namespace);

        /// <summary>
        /// Sets the label selector by which to filter the <see cref="ICustomResource"/>s to watch
        /// </summary>
        /// <param name="selector">The label selector (ex: environment = production) to use</param>
        /// <returns>The configured <see cref="ICustomResourceWatcherOptionsBuilder"/></returns>
        ICustomResourceWatcherOptionsBuilder SelectByLabel(string selector);

        /// <summary>
        /// Sets the field selector by which to filter the <see cref="ICustomResource"/>s to watch
        /// </summary>
        /// <param name="selector">The field selector (ex: metadata.name) to use</param>
        /// <returns>The configured <see cref="ICustomResourceWatcherOptionsBuilder"/></returns>
        ICustomResourceWatcherOptionsBuilder SelectByField(string selector);

        /// <summary>
        /// Builds the <see cref="CustomResourceWatcherOptions"/>
        /// </summary>
        /// <returns>A new <see cref="CustomResourceWatcherOptions"/></returns>
        CustomResourceWatcherOptions Build();

    }

}
