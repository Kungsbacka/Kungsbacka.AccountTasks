using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kungsbacka.AccountTasks
{
    public class ConfigureOnlineMailboxTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MailboxType? Type { get; private set; }

        public ConfigureOnlineMailboxTask(MailboxType? mailboxType)
            : base("ConfigureOnlineMailbox", "Configure online mailbox")
        {
            Type = mailboxType;
        }

        public ConfigureOnlineMailboxTask()
            : this(null)
        {
        }

        public bool ShouldSerializeType()
        {
            return Type != null;
        }
    }

    public class ConfigureOnlineOwaTask : BasicTask
    {
        public ConfigureOnlineOwaTask()
            : base("ConfigureOnlineOwa", "Configure online OWA")
        {
        }
    }
}
