using k8s.Models;
using Neuroglia.K8s;

namespace Watcher.Resources
{

    public class Test
        : CustomResource<TestSpec, TestStatus>
    {

        public Test()
          : base(CustomResourceDefinitions.Test)
        {

        }

        public Test(V1ObjectMeta metadata, TestSpec spec)
          : this()
        {
            this.Metadata = metadata;
            this.Spec = spec;
        }

    }
}
