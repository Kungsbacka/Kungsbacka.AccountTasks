using Newtonsoft.Json;

namespace Kungsbacka.AccountTasks
{
    public class WaitTask : BasicTask
    {
        long? _minutes;
        [JsonProperty(Order = 10)]
        public long? Minutes
        {
            get
            {
                return _minutes;
            }
            private set
            {
                _minutes = value;
            }
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
}
