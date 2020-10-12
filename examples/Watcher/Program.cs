using k8s;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

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
                    services.AddHostedService<ResourceController>();
                });
        }

    }

}
