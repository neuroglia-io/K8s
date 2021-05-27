using k8s.Models;
using Microsoft.Extensions.Hosting;
using System;

namespace Neuroglia.K8s
{

    /// <summary>
    /// Defines the fundamentals of a service used to listen to Kubernetes <see cref="Corev1Event"/>s
    /// </summary>
    public interface IEventListener
        : IHostedService, IObservable<Corev1Event>
    {



    }

}
