using k8s;
using k8s.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Neuroglia.K8s;
using System;
using System.Threading;
using System.Threading.Tasks;
using Watcher.Resources;

namespace Watcher
{
    public class ResourceController
        : BackgroundService
    {

        private Task _ExecutingTask;

        public ResourceController(IServiceProvider serviceProvider, ILogger<ResourceController> logger, IKubernetes kubernetesClient)
        {
            this.ServiceProvider = serviceProvider;
            this.Logger = logger;
            this.KubernetesClient = kubernetesClient;
            this.CrdWatcher = ActivatorUtilities.CreateInstance<ICustomResourceEventWatcher<Test>>(this.ServiceProvider, CustomResourceDefinitions.Test, this.Namespace, new CustomResourceEventDelegate<Test>(this.OnTestEvent));
        }

        protected IServiceProvider ServiceProvider { get; }

        protected ILogger Logger { get; }

        protected IKubernetes KubernetesClient { get; }

        protected ICustomResourceEventWatcher CrdWatcher { get; }

        protected string Namespace => "test";

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this._ExecutingTask = this.CrdWatcher.StartAsync(stoppingToken);
            return this.KubernetesClient.CreateNamespacedCustomObjectAsync(new Test(new V1ObjectMeta() { Name = "test" }, new TestSpec() { Value = "Hello, world" }), this.Namespace);
        }

        protected void OnTestEvent(WatchEventType e, Test test)
        {
            switch (e)
            {
                case WatchEventType.Added:
                    this.Logger.LogInformation("Test added");
                    break;
                case WatchEventType.Modified:
                    this.Logger.LogInformation("Test modified");
                    break;
                case WatchEventType.Deleted:
                    this.Logger.LogInformation("Test deleted");
                    break;
                case WatchEventType.Bookmark:
                    this.Logger.LogInformation("Test version changed");
                    break;
                case WatchEventType.Error:
                    this.Logger.LogError("Test error");
                    break;
                default:
                    throw new NotSupportedException($"The specified watch event type '{e}' is not supported");
            }
        }

    }

}
