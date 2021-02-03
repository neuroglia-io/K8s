namespace Neuroglia.K8s
{
    /// <summary>
    /// Represents the default implementation of the <see cref="ICustomResourceWatcherOptionsBuilder"/> interface
    /// </summary>
    public class CustomResourceWatcherOptionsBuilder
        : ICustomResourceWatcherOptionsBuilder
    {

        /// <summary>
        /// Initializes a new <see cref="CustomResourceWatcherOptionsBuilder"/>
        /// </summary>
        /// <param name="options">The <see cref="CustomResourceWatcherOptions"/> to configure</param>
        public CustomResourceWatcherOptionsBuilder(CustomResourceWatcherOptions options)
        {
            this.Options = options;
        }

        /// <summary>
        /// Initializes a new <see cref="CustomResourceWatcherOptionsBuilder"/>
        /// </summary>
        public CustomResourceWatcherOptionsBuilder()
            : this(new CustomResourceWatcherOptions())
        {

        }

        /// <summary>
        /// Gets the <see cref="CustomResourceWatcherOptions"/> to configure
        /// </summary>
        protected CustomResourceWatcherOptions Options { get; }

        /// <inheritdoc/>
        public virtual ICustomResourceWatcherOptionsBuilder WatchIn(string @namespace)
        {
            this.Options.Namespace = @namespace;
            return this;
        }

        /// <inheritdoc/>
        public virtual ICustomResourceWatcherOptionsBuilder SelectByLabel(string selector)
        {
            this.Options.LabelSelector = selector;
            return this;
        }

        /// <inheritdoc/>
        public virtual ICustomResourceWatcherOptionsBuilder SelectByField(string selector)
        {
            this.Options.FieldSelector = selector;
            return this;
        }

        /// <inheritdoc/>
        public virtual CustomResourceWatcherOptions Build()
        {
            return this.Options;
        }

    }

}
