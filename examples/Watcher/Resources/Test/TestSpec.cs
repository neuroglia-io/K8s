using Newtonsoft.Json;

namespace Watcher.Resources
{

    public class TestSpec
    {

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

    }

}
