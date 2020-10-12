using Newtonsoft.Json;

namespace Watcher.Resources
{

    public class TestStatus
    {

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

    }

}
