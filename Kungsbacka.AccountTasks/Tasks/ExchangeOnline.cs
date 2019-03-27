using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kungsbacka.AccountTasks
{
    public class ConfigureOnlineMailboxTask : BasicTask
    {
        // Not optional, but declare as nullable anyway since mailbox type
        // can also come from the sequence task container.
        [JsonProperty(Order = 10)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MailboxType? Type { get; private set; }

        public ConfigureOnlineMailboxTask(MailboxType? type)
            : base("ConfigureOnlineMailbox", "Configure online mailbox")
        {
            Type = type;
        }

        public ConfigureOnlineMailboxTask()
            : this(null)
        {
        }
    }

    public class ConfigureOnlineOwaTask : BasicTask
    {
        public ConfigureOnlineOwaTask()
            : base("ConfigureOnlineOwa", "Configure online OWA")
        {
        }
    }

    public class SetOnlineMailboxTypeTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ExchangeMailboxType MailboxType { get; private set; }

        public SetOnlineMailboxTypeTask(ExchangeMailboxType mailboxType)
            : base("SetOnlineMailboxType", "Sets the exchange mailbox type")
        {
            MailboxType = mailboxType;
        }
    }
}
