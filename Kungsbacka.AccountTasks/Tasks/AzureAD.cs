using System.Text.Json.Serialization;

namespace Kungsbacka.AccountTasks
{
    public class AzureADGroup
    {
        [JsonPropertyOrder(10)]
        public AzureADGroupType Type { get; private set; }
        [JsonPropertyOrder(20)]
        public string Name { get; private set; }
    }
}
