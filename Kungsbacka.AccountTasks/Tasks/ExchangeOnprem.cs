using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kungsbacka.AccountTasks
{
    public enum MailboxType
    {
        Employee,
        Student,
        Shared
    }

    public class EnableMailboxTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MailboxType? Type { get; private set; }

        public EnableMailboxTask(MailboxType? mailboxType)
            : base("EnableMailbox", "Enable on-prem mailbox")
        {
            Type = mailboxType;
        }

        public EnableMailboxTask()
            : this(null)
        {
        }

        public bool ShouldSerializeType()
        {
            return Type != null;
        }
    }

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

    public class ConfigureMailboxTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MailboxType? Type { get; private set; }

        public ConfigureMailboxTask(MailboxType? mailboxType)
            : base("ConfigureMailbox", "Configure on-prem mailbox")
        {
            Type = mailboxType;
        }

        public ConfigureMailboxTask()
            : this(null)
        {
        }

        public bool ShouldSerializeType()
        {
            return Type != null;
        }
    }

    public class ConfigureOwaTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MailboxType? Type { get; private set; }

        public ConfigureOwaTask(MailboxType? mailboxType)
            : base("ConfigureOwa", "Configure on-prem OWA")
        {
            Type = mailboxType;
        }

        public ConfigureOwaTask()
            : this(null)
        {
        }

        public bool ShouldSerializeType()
        {
            return Type != null;
        }
    }

    public class ConfigureCalendarTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MailboxType? Type { get; private set; }

        public ConfigureCalendarTask(MailboxType? mailboxType)
            : base("ConfigureCalendar", "Configure on-prem calendar")
        {
            Type = mailboxType;
        }

        public ConfigureCalendarTask()
            : this(null)
        {
        }

        public bool ShouldSerializeType()
        {
            return Type != null;
        }
    }

    public class ConfigureMessageTask : BasicTask
    {
        public ConfigureMessageTask()
            : base("ConfigureMessage", "Configure mailbox message")
        {
        }
    }

    public class ConfigureMailboxAutoReplyTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        public bool Enabled { get; private set; }
        [JsonProperty(Order = 11)]
        public string Message { get; private set; }

        public ConfigureMailboxAutoReplyTask(bool enabled)
            : base("ConfigureMailboxAutoReplyTask", "Set On-prem Mailbox Auto Reply State")
        {
            Enabled = enabled;
        }

        public ConfigureMailboxAutoReplyTask(bool enabled, string message)
            : base("ConfigureMailboxAutoReplyTask", "Set On-prem Mailbox Auto Reply State")
        {
            Enabled = enabled;
            Message = message;
        }

        public bool ShouleSerializeMessage()
        {
            return !Enabled || string.IsNullOrEmpty(Message);
        }
    }

    public class SendWelcomeMailTask : BasicTask
    {
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

    public class OnpremMailboxTask : SequenceTask
    {
        [JsonProperty(Order = 10)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MailboxType Type { get; private set; }

        public OnpremMailboxTask(MailboxType mailboxType)
            : base("OnpremMailbox", "On-prem Mailbox")
        {
            Type = mailboxType;
            var sequence = new List<AccountTask>(5);
            sequence.Add(new EnableMailboxTask());
            sequence.Add(new WaitTask(5));
            sequence.Add(new ConfigureOwaTask());
            sequence.Add(new ConfigureCalendarTask());
            sequence.Add(new ConfigureMessageTask());
            if (mailboxType == MailboxType.Employee)
            {
                sequence.Add(new SendWelcomeMailTask());
            }
            SetTasks(sequence);
        }

        public OnpremMailboxTask(MailboxType mailboxType, List<AccountTask> tasks)
            : base("OnpremMailbox", "On-prem Mailbox")
        {
            if (tasks == null || tasks.Count == 0)
            {
                throw new ArgumentException("tasks");
            }
            Type = mailboxType;
            SetTasks(tasks);
        }
    }

    public class OnpremMailboxReconfigureTask : SequenceTask
    {
        [JsonProperty(Order = 10)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MailboxType Type { get; private set; }

        public OnpremMailboxReconfigureTask(MailboxType mailboxType)
            : base("OnpremMailboxReconfigure", "Reconfigure On-prem Mailbox")
        {
            Type = mailboxType;
            var sequence = new List<AccountTask>(5);
            sequence.Add(new ConfigureOwaTask());
            sequence.Add(new ConfigureCalendarTask());
            sequence.Add(new ConfigureMessageTask());
            SetTasks(sequence);
        }

        public OnpremMailboxReconfigureTask(MailboxType mailboxType, List<AccountTask> tasks)
            : base("OnpremMailboxReconfigure", "Reconfigure On-prem Mailbox")
        {
            if (tasks == null || tasks.Count == 0)
            {
                throw new ArgumentException("tasks");
            }
            Type = mailboxType;
            SetTasks(tasks);
        }
    }
}
