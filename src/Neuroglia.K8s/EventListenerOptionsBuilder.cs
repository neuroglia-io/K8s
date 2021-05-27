using k8s;
using k8s.Models;
using System;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IEventListenerOptionsBuilder"/> interface
    /// </summary>
    public class EventListenerOptionsBuilder
        : IEventListenerOptionsBuilder
    {

        /// <summary>
        /// Initializes a new <see cref="EventListenerOptionsBuilder"/>
        /// </summary>
        /// <param name="options">The <see cref="EventListenerOptions"/> to configure</param>
        public EventListenerOptionsBuilder(EventListenerOptions options)
        {
            this.Options = options;
        }

        /// <summary>
        /// Initializes a new <see cref="EventListenerOptionsBuilder"/>
        /// </summary>
        public EventListenerOptionsBuilder()
            : this(new EventListenerOptions())
        {

        }

        /// <summary>
        /// Gets the <see cref="EventListenerOptions"/> to configure
        /// </summary>
        protected EventListenerOptions Options { get; }

        /// <inheritdoc/>
        public virtual IEventListenerOptionsBuilder InCluster()
        {
            this.Options.Namespace = null;
            return this;
        }

        /// <inheritdoc/>
        public virtual IEventListenerOptionsBuilder InNamespace(string @namespace)
        {
            this.Options.Namespace = @namespace;
            return this;
        }

        /// <inheritdoc/>
        public virtual IEventListenerOptionsBuilder WithLabel(string selector)
        {
            this.Options.LabelSelector = selector;
            return this;
        }

        /// <inheritdoc/>
        public virtual IEventListenerOptionsBuilder WithField(string selector)
        {
            this.Options.FieldSelector = selector;
            return this;
        }

        /// <inheritdoc/>
        public virtual IEventListenerOptionsBuilder When(Func<WatchEventType, Corev1Event, bool> predicate)
        {
            this.Options.Predicate = predicate;
            return this;
        }

        /// <inheritdoc/>
        public virtual EventListenerOptions Build()
        {
            return this.Options;
        }
    }

}
