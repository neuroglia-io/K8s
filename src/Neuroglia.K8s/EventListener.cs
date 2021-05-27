using k8s;
using k8s.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Neuroglia.K8s
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IEventListener"/> interface
    /// </summary>
    public class EventListener
        : BackgroundService, IEventListener
    {

        private CancellationTokenSource _CancellationTokenSource;
        private Task _ExecutingTask;

        /// <summary>
        /// Initializes a new <see cref="IEventListener"/>.
        /// </summary>
        /// <param name="logger">The service used to perform logging</param>
        /// <param name="kubernetes">The service used to communicate with Kubernetes.</param>
        /// <param name="options">The options used to configure the <see cref="EventListener"/></param>
        public EventListener(ILogger<EventListener> logger, IKubernetes kubernetes, EventListenerOptions options)
        {
            this.Logger = logger;
            this.Kubernetes = kubernetes;
            this.Options = options;
        }

        /// <summary>
        /// Gets the service used to perform logging
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the service used to communicate with the Kubernetes API
        /// </summary>
        protected IKubernetes Kubernetes { get; }

        /// <summary>
        /// Gets the options used to configure the <see cref="EventListener"/>
        /// </summary>
        protected EventListenerOptions Options { get; }

        /// <summary>
        /// Gets the <see cref="IEventListener"/>'s <see cref="Subject{T}"/>
        /// </summary>
        protected Subject<Corev1Event> Subject { get; } = new Subject<Corev1Event>();

        /// <inheritdoc/>
        public virtual IDisposable Subscribe(IObserver<Corev1Event> observer)
        {
            return this.Subject.Subscribe(observer);
        }

        /// <inheritdoc/>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (this._CancellationTokenSource != null
               && this._CancellationTokenSource.IsCancellationRequested)
            {
                this.Logger.LogDebug($"Failed to start the {nameof(IEventListener)} because it is being stopped.");
                this._CancellationTokenSource = null;
                this._ExecutingTask = null;
                return Task.CompletedTask;
            }
            if (this._ExecutingTask != null
                && this._CancellationTokenSource != null)
                this._CancellationTokenSource.Cancel();
            this._CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            this._ExecutingTask = Task.Run(async () => await this.ListenAsync(), this._CancellationTokenSource.Token);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Starts listening for Kubernetes <see cref="Corev1Event"/>s
        /// </summary>
        /// <returns>A new awaitable <see cref="Task"/></returns>
        protected virtual async Task ListenAsync()
        {
            while (!this._CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    using CancellationTokenSource operationCancellationTokenSource = new();
                    using CancellationTokenSource linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(this._CancellationTokenSource.Token, operationCancellationTokenSource.Token);
                    HttpOperationResponse<Corev1EventList> operationResponse;
                    if (string.IsNullOrWhiteSpace(this.Options.Namespace))
                    {
                        this.Logger.LogDebug("Creating a new watcher for cluster-wide Core V1 Events.");
                        operationResponse = await this.Kubernetes.ListEventForAllNamespacesWithHttpMessagesAsync(watch: true, cancellationToken: linkedCancellationTokenSource.Token).ConfigureAwait(false);
                    }
                    else
                    {
                        this.Logger.LogDebug("Creating a new watcher for Core V1 Events in namespace '{namespace}'.", this.Options.Namespace);
                        operationResponse = await this.Kubernetes.ListNamespacedEventWithHttpMessagesAsync(this.Options.Namespace, watch: true, cancellationToken: linkedCancellationTokenSource.Token).ConfigureAwait(false);
                    }
                    using (operationResponse)
                    {
                        using (Watcher<Corev1Event> watcher = operationResponse.Watch<Corev1Event, Corev1EventList>(this.OnNext,
                        (ex) =>
                        {
                            this.Logger.LogError($"An exception occured while watching over the Core V1 Events in namespace '{{namespace}}:{Environment.NewLine}{{ex}}'. Reconnecting...", this.Options.Namespace, ex.ToString());
                            this.OnError(ex);
                            operationCancellationTokenSource.Cancel();
                        },
                        () =>
                        {
                            this.Logger.LogInformation("The connection of the Core V1 Event watcher in namespace '{namespace}' has been closed. Reconnecting...", this.Options.Namespace);
                            operationCancellationTokenSource.Cancel();
                        }))
                        {
                            if (string.IsNullOrWhiteSpace(this.Options.Namespace))
                                this.Logger.LogInformation("Started watching for Core V1 Events in namespace '{namespace}'.", this.Options.Namespace);
                            else
                                this.Logger.LogInformation("Started watching for cluster-wide Core V1 Events.", this.Options.Namespace);
                            while (!this._CancellationTokenSource.IsCancellationRequested
                                && !linkedCancellationTokenSource.IsCancellationRequested)
                            {

                            }
                        }
                    }
                }
                catch (HttpOperationException ex)
                {
                    this.Logger.LogError($"An exception occured while processing Core V1 Events. The server responded with a '{{statusCode}}' status code:{Environment.NewLine}Details: {{responseContent}}. Reconnecting...", ex.Response.StatusCode, ex.Response.Content);
                    this.OnError(ex);
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"An exception occured while processing Core V1 Events:{Environment.NewLine}Details: {{ex}}. Reconnecting...", ex.ToString());
                    this.OnError(ex);
                }
            }
            this.Subject?.OnCompleted();
            this.Logger.LogDebug("Stopped watching for eCore V1 Events in namespace '{namespace}'.", this.Options.Namespace);
        }

        /// <summary>
        /// Handles resources events returned by the <see cref="IKubernetes"/> client
        /// </summary>
        /// <param name="type">The <see cref="WatchEventType"/></param>
        /// <param name="e">The watched <see cref="Corev1Event"/></param>
        protected virtual void OnNext(WatchEventType type, Corev1Event e)
        {
            if (this.Options.Predicate == null
                || (this.Options.Predicate != null && this.Options.Predicate(type, e)))
                this.Subject.OnNext(e);
        }

        /// <summary>
        /// Handles <see cref="Exception"/>s thrown during the listening loop
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> to handle</param>
        protected virtual void OnError(Exception ex)
        {
            this.Subject.OnError(ex);
        }

    }

}
