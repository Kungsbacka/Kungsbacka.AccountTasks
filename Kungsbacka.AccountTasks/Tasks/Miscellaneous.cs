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

    public class HomeFolderTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        public string Path { get; private set; }
        public HomeFolderTask(string path)
            : base("HomeFolder", "Home Folder")
        {
            Path = path;
        }

        public HomeFolderTask()
            : this(null)
        {
        }

        public bool ShouldSerializePath()
        {
            return Path != null;
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
