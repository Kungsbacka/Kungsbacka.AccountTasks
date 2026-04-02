using System.Text.Json.Serialization;

namespace Kungsbacka.AccountTasks
{
    public class EnableRemoteMailboxTask : BasicTask
    {
        public EnableRemoteMailboxTask()
            : base("EnableRemoteMailbox", "Enable remote mailbox")
        {
        }
    }

    public class DisableMailboxTask : BasicTask
    {
        public bool KeepSyncing { get; private set; }        

        public DisableMailboxTask(bool keepSyncing)
            : base("DisableMailbox", "Disable mailbox")
        {
            KeepSyncing = keepSyncing;
        }
    }

    public class ConfigureRemoteMailboxTask : BasicTask
    {
        public ConfigureRemoteMailboxTask()
            : base("ConfigureRemoteMailbox", "Configure remote mailbox")
        {
        }
    }

    public class SendWelcomeMailTask : BasicTask
    {
        // Not optional, but declare as nullable anyway since mailbox type
        // can also come from the sequence task container.
        [JsonPropertyOrder(10)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MailboxType? Type { get; private set; }

        public SendWelcomeMailTask(MailboxType? mailboxType)
            : base("SendWelcomeMail", "Send welcome mail")
        {
            Type = mailboxType;
        }

        public SendWelcomeMailTask()
            : this(null)
        {
        }
    }

    public class SetHiddenFromAddressListTask : BasicTask
    {
        [JsonPropertyOrder(10)]
        public bool Hidden { get; private set; }

        public SetHiddenFromAddressListTask(bool hidden)
            : base("SetHiddenFromAddressList", "Show in or hide from address list")
        {
            Hidden = hidden;
        }
    }
}
