using System;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Represents a subscription to an <see cref="ICustomResource"/>
    /// </summary>
    /// <typeparam name="TResource">The type of the <see cref="ICustomResource"/> to subscribe to</typeparam>
    public class CustomResourceSubscription<TResource>
        : IDisposable
         where TResource : ICustomResource, new()
    {

        /// <summary>
        /// Represents the event fired whenever the <see cref="CustomResourceSubscription{TResource}"/> has been disposed of
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Initializes a new <see cref="CustomResourceSubscription{TResource}"/>
        /// </summary>
        /// <param name="observer">The owner of the <see cref="CustomResourceSubscription{TResource}"/></param>
        public CustomResourceSubscription(IObserver<IResourceEvent<TResource>> observer)
        {
            this.Observer = observer;
        }

        /// <summary>
        /// Gets the owner of the <see cref="CustomResourceSubscription{TResource}"/>
        /// </summary>
        public IObserver<IResourceEvent<TResource>> Observer { get; }

        private bool _Disposed;
        /// <summary>
        /// Disposes of the <see cref="CustomResourceSubscription{TResource}"/>
        /// </summary>
        /// <param name="disposing">A boolean indicating whether or not the <see cref="CustomResourceSubscription{TResource}"/> is being disposed of</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    this.Disposed?.Invoke(this, new EventArgs());
                }
                this._Disposed = true;
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }

}
