namespace Kungsbacka.AccountTasks
{
    public class AzureADGroup
    {
        [JsonProperty(Order = 10)]
        public AzureADGroupType Type { get; private set; }
        [JsonProperty(Order = 20)]
        public string Name { get; private set; }
    }
}