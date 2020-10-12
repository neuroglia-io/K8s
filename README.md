# Neuroglia K8s
.NET Standard 2.1 library that provides abstractions and default implementations to manage Kubernetes Custom Resource Definitions thanks to an [IKubernetes client](https://github.com/kubernetes-client/csharp)

# Usage

[Nuget Package](https://www.nuget.org/packages/KubernetesClient/)

```
  dotnet add package Neuroglia.K8s
```

## Sample Code

### Defining a Custom Resource Definition

```c#
  //Example of a 'TEST' CRD
  public class TestDefinition
    : CustomResourceDefinition
  {

      public TestDefinition()
        : base("k8s.neuroglia.io/v1alpha1", "Test", "tests")
      {
      
      }

  }

  public static class CustomResourceDefinitions
  {
  
      public static TestDefinition Test = new TestDefinition();
  
  }

```

### Defining a Custom Resource

```c#
  //Example of an instance of a 'Test' CRD
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
  
  //The spec of the Test CRD
  public class TestSpec
  {
  
      [JsonProperty(PropertyName = "value")]
      public string Value { get; set; }

  }
  
  //The status of the Test CRD
  public class TestStatus
  {
  
      [JsonProperty(PropertyName = "status")]
      public string Status { get; set; }
  
  }
```

### Installing the CRD on Kubernetes

1. Create the CRD yaml file

```yaml
apiVersion: apiextensions.k8s.io/v1beta1
kind: CustomResourceDefinition
metadata:
  name: tests.k8s.neuroglia.io #must be the concatenation of the plural and the group parameters supplied in your CRD class ({plural}.{group})
spec:
  group: k8s.neuroglia.io #must be the same group than provided in your CRD class, in the ApiVersion parameter ({group}/{version})
  subresources: #add this block if your CRD uses a status, otherwise remove
    status: {}
  versions:
    - name: v1alpha1 #must be the same version than provided in your CRD class, in the ApiVersion parameter ({group}/{version})
      served: true
      storage: true
  scope: Namespaced #add this line if the CRD is to be namespaced, otherwise remove
  names:
    plural: tests #must be the same than provided in your CRD class
    singular: test #should be the singularized version of the plural value supplied in your CRD class
    kind: Test #must be the same than provided in your CRD class
```

2. Apply the file using kubectl

```powershell
kubectl apply -f test-crd.yaml
```

### Creating a new Custom Resource programatically (thanks to the IKubernetes C# client)

```c#
var kube = new Kubernetes(KubernetesClientConfiguration.InClusterConfig());
var test = await kube.CreateNamespacedCustomObjectAsync(new Test(new TestSpec() { Value = "Hello, world!" }), "mynamespace");
```

Check whether the Custom Resource has been successfully created

```powershell
kubectl get tests -n mynamespace
```

### Creating a new Custom Resource declaratively

1. Create the Custom Resource's yaml file

```yaml
apiVersion: k8s.neuroglia.io/v1alpha1
kind: Test
metadata:
  name: test
spec:
  value: "hello, world"
```

2. Apply the file using kubectl

```powershell
kubectl apply -f test.yaml
```

3. Check whether the Custom Resource has been successfully created

```powershell
kubectl get tests
```

### Watching events on a Custom Resource Definition

```c#
var logger = LoggerFactory.Create<CustomResourceEventWatcher<Test>>();
var kube = new Kubernetes(KubernetesClientConfiguration.InClusterConfig());
var crd = CustomResourceDefinitions.Test;
var handler = new CustomResourceEventDelegate<Test>((e, test) => 
{
    switch (e)
    {
        case WatchEventType.Added:
            logger.LogInformation("Test added");
            break;
        case WatchEventType.Modified:
            logger.LogInformation("Test modified");
            break;
        case WatchEventType.Deleted:
            logger.LogInformation("Test deleted");
            break;
        case WatchEventType.Bookmark:
            logger.LogInformation("Test version changed");
            break;
        case WatchEventType.Error:
            logger.LogError("Test error");
            break;
        default:
            throw new NotSupportedException($"The specified watch event type '{e}' is not supported");
    }
});
var watcher = new CustomResourceEventWatcher<Test>(logger, kube, crd, namespaceProperty, handler);
await watcher.StartAsync();
```

## Examples

There is an extensive example in the [examples directory](https://github.com/neuroglia-io/K8s/tree/master/examples)

### Running the examples

```powershell
git clone git@github.com:neuroglia-io/K8s.git
cd K8s\examples\watcher
dotnet run
```

# Contributing

Please see [CONTRIBUTING.md](https://github.com/neuroglia-io/K8s/blob/master/CONTRIBUTING.md) for instructions on how to contribute.
