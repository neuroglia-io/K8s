using k8s;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neuroglia.K8s;
using System.Threading.Tasks;
using Watcher.Resources;

namespace Watcher
{

    class Program
    {

        static async Task Main(string[] args)
        {
            using (var host = CreateHost(args).Build())
            {
                await host.RunAsync();
            }
        }

        static IHostBuilder CreateHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddLogging();
                    services.AddSingleton<IKubernetes>(new Kubernetes(KubernetesClientConfiguration.InClusterConfig()));
                    services.AddResourceWatcher<Test>(watch => watch.InNamespace("test"));
                    services.AddHostedService<ResourceController>();
                });
        }

    }

}
