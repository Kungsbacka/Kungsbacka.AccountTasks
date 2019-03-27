using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kungsbacka.AccountTasks
{
    public class EnableRemoteMailboxTask : BasicTask
    {
        public EnableRemoteMailboxTask()
            : base("EnableRemoteMailbox", "Enable remote mailbox")
        {
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
        [JsonProperty(Order = 10)]
        [JsonConverter(typeof(StringEnumConverter))]
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

        public bool ShouldSerializeType()
        {
            return Type != null;
        }
    }

    public class SetHiddenFromAddressListTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        public bool Hidden { get; private set; }

        public SetHiddenFromAddressListTask(bool hidden)
            : base("SetHiddenFromAddressList", "Show in or hide from address list")
        {
            Hidden = hidden;
        }
    }
}
