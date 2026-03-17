using System.Text.Json.Serialization;

namespace Kungsbacka.AccountTasks
{
    public class WaitTask : BasicTask
    {
        private long? _minutes;

        [JsonPropertyOrder(10)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? Minutes
        {
            get => _minutes;
            private set => _minutes = value;
        }

        public WaitTask(long? minutes)
            : base("Wait", "Wait before next task")
        {
            Minutes = minutes;
        }

        public WaitTask()
            : this(null)
        {
        }
    }

    public class SamlIdTask : BasicTask
    {
        public SamlIdTask()
            : base("SamlId", "SAML ID")
        {
        }
    }

    public class AddToOnpremGroupTask : BasicTask
    {
        [JsonPropertyOrder(10)]
        public string Group { get; private set; }

        public AddToOnpremGroupTask(string group)
            : base("AddToOnpremGroup", "Add user to an onprem group")
        {
            Group = group;
        }
    }

    public class RemoveFromOnpremGroupTask : BasicTask
    {
        [JsonPropertyOrder(10)]
        public string Group { get; private set; }

        public RemoveFromOnpremGroupTask(string group)
            : base("RemoveFromOnpremGroup", "Remove user from an onprem group")
        {
            Group = group;
        }
    }
}
