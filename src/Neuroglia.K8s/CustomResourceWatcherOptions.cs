namespace Neuroglia.K8s
{
    /// <summary>
    /// Represents the options used to configure an <see cref="ICustomResourceWatcher"/>
    /// </summary>
    public class CustomResourceWatcherOptions
    {

        /// <summary>
        /// Gets/sets the namespace in which to listen for <see cref="ICustomResource"/>s
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets/sets the label selector by which to filter the <see cref="ICustomResource"/>s to watch
        /// </summary>
        public string LabelSelector { get; set; }

        /// <summary>
        /// Gets/sets the field selector by which to filter the <see cref="ICustomResource"/>s to watch
        /// </summary>
        public string FieldSelector { get; set; }

    }

}
