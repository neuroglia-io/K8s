using k8s;
using k8s.Models;
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

        public ResourceController(IServiceProvider serviceProvider, ILogger<ResourceController> logger, IKubernetes kubernetesClient, IResourceWatcher<Test> crdWatcher)
        {
            this.ServiceProvider = serviceProvider;
            this.Logger = logger;
            this.KubernetesClient = kubernetesClient;
            this.CrdWatcher = crdWatcher;
        }

        protected IServiceProvider ServiceProvider { get; }

        protected ILogger Logger { get; }

        protected IKubernetes KubernetesClient { get; }

        protected IResourceWatcher<Test> CrdWatcher { get; }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.CrdWatcher.Subscribe(this.OnTestEvent);
            return this.KubernetesClient.CreateNamespacedCustomObjectAsync(new Test(new V1ObjectMeta() { Name = "test" }, new TestSpec() { Value = "Hello, world" }), "test", cancellationToken: stoppingToken);
        }

        protected void OnTestEvent(IResourceEvent<Test> e)
        {
            switch (e.Type)
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
