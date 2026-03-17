using System.Text.Json.Serialization;

namespace Kungsbacka.AccountTasks
{
    public class ConfigureOnlineMailboxTask : BasicTask
    {
        // Not optional, but declare as nullable anyway since mailbox type
        // can also come from the sequence task container.
        [JsonPropertyOrder(10)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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
        [JsonPropertyOrder(10)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExchangeMailboxType MailboxType { get; private set; }

        public SetOnlineMailboxTypeTask(ExchangeMailboxType mailboxType)
            : base("SetOnlineMailboxType", "Sets the exchange mailbox type")
        {
            MailboxType = mailboxType;
        }
    }
}
