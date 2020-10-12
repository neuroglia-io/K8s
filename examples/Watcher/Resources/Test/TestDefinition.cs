using Neuroglia.K8s;

namespace Watcher.Resources
{

    public class TestDefinition
        : CustomResourceDefinition
    {

        public TestDefinition()
          : base("k8s.neuroglia.io/v1alpha1", "Test", "tests")
        {

        }

    }

}
