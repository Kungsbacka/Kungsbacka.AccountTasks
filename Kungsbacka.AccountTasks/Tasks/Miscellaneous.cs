using Newtonsoft.Json;

namespace Kungsbacka.AccountTasks
{
    public class WaitTask : BasicTask
    {
        private long? _minutes;
        [JsonProperty(Order = 10)]
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

        public bool ShouldSerializeMinutes()
        {
            return Minutes != null;
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
        [JsonProperty(Order = 10)]
        public string Group { get; private set; }

        public AddToOnpremGroupTask(string group)
            : base("AddToOnpremGroup", "Add user to an onprem group")
        {
            Group = group;
        }
    }

    public class RemoveFromOnpremGroupTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        public string Group { get; private set; }

        public RemoveFromOnpremGroupTask(string group)
            : base("RemoveFromOnpremGroup", "Remove user from an onprem group")
        {
            Group = group;
        }
    }
}
