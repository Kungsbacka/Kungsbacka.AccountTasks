using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Kungsbacka.AccountTasks
{
    public class MsolEnableSyncTask : BasicTask
    {
        public MsolEnableSyncTask()
            : base("MsolEnableSync", "Synchronize to Azure AD")
        {
        }
    }

    public class MsolLicenseTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        public MsolLicense[] License { get; private set; }

        public MsolLicenseTask(MsolLicense[] license)
            : base("MsolLicense", "Directly assign Microsoft 365 licenses")
        {
            License = license;
        }

        public MsolLicenseTask()
            : this(null)
        {
        }

        public bool ShouldSerializeLicense()
        {
            return License != null;
        }
    }

    public class MsolLicenseGroupTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        public Guid[] LicenseGroups { get; private set; }
        [JsonProperty(Order = 20)]
        public bool SkipSyncCheck { get; private set; }
        [JsonProperty(Order = 30)]
        public bool SkipDynamicGroupCheck { get; private set; }

        public MsolLicenseGroupTask(Guid[] licenseGroups, bool skipSyncCheck, bool skipDynamicGroupCheck)
            : base("MsolLicenseGroup", "Add to Microsoft 365 license groups")
        {
            LicenseGroups = licenseGroups;
            SkipSyncCheck = skipSyncCheck;
            SkipDynamicGroupCheck = skipDynamicGroupCheck;
        }

        public MsolLicenseGroupTask(Guid[] licenseGroups)
            : this(licenseGroups, false, false)
        {
        }

        public MsolLicenseGroupTask(bool skipSyncCheck)
            : this(null, skipSyncCheck, false)
        {
        }

        public MsolLicenseGroupTask(bool skipSyncCheck, bool skipDynamicGroupCheck)
            : this(null, skipSyncCheck, skipDynamicGroupCheck)
        {
        }

        public MsolLicenseGroupTask()
            : this(null, false, false)
        {
        }

        public bool ShouldSerializeLicenseGroups()
        {
            return LicenseGroups != null;
        }

        public bool ShouldSerializeSkipSyncCheck()
        {
            return SkipSyncCheck;
        }

        public bool ShouldSerializeSkipDynamicGroupCheck()
        {
            return SkipDynamicGroupCheck;
        }
    }

    public class MsolRemoveLicenseGroupTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        public Guid[] LicenseGroups { get; private set; }
        [JsonProperty(Order = 20)]
        public bool SkipBaseLicenseCheck { get; private set; }

        public MsolRemoveLicenseGroupTask(Guid[] licenseGroups, bool skipBaseLicenseCheck)
            : base("MsolRemoveLicenseGroup", "Remove from Microsoft 365 license groups")
        {
            LicenseGroups = licenseGroups;
            SkipBaseLicenseCheck = skipBaseLicenseCheck;
        }

        public MsolRemoveLicenseGroupTask(Guid[] licenseGroups)
            : this(licenseGroups, false)
        {
        }

        public bool ShouldSerializeSkipBaseLicenseCheck()
        {
            return SkipBaseLicenseCheck;
        }
    }

    public class MsolRemoveAllLicenseGroupTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        public bool SkipStashLicense { get; private set; }

        public MsolRemoveAllLicenseGroupTask(bool skipStashLicense)
            : base("MsolRemoveAllLicenseGroup", "Remove from all Microsoft 365 license groups")
        {
            SkipStashLicense = skipStashLicense;
        }

        public MsolRemoveAllLicenseGroupTask()
            : this(false)
        {
        }

        public bool ShouldSerializeSkipStashLicense()
        {
            return SkipStashLicense;
        }
    }


    public class MsolRestoreLicenseGroupTask : BasicTask
    {
        [JsonProperty(Order = 10)]
        public bool SkipSyncCheck { get; private set; }

        public MsolRestoreLicenseGroupTask(bool skipSyncCheck)
            : base("MsolRestoreLicenseGroup", "Re-add to stashed Microsoft 365 license groups")
        {
            SkipSyncCheck = skipSyncCheck;
        }

        public MsolRestoreLicenseGroupTask()
            : this(false)
        {
        }

        public bool ShouldSerializeSkipSyncCheck()
        {
            return SkipSyncCheck;
        }
    }

    public class MicrosoftOnlineTask : SequenceTask
    {
        [JsonProperty(Order = 10)]
        public Guid[] LicenseGroups { get; private set; }

        public MicrosoftOnlineTask(Guid[] licenseGroups)
            : base("MicrosoftOnline", "Microsoft 365")
        {
            if (licenseGroups == null || licenseGroups.Length == 0)
            {
                throw new ArgumentException(nameof(licenseGroups));
            }
            LicenseGroups = licenseGroups;
            SetTasks(new List<AccountTask> {
                new MsolEnableSyncTask(),
                new WaitTask(1),
                new MsolLicenseGroupTask()
            });
        }

        public MicrosoftOnlineTask(Guid[] licenseGroups, List<AccountTask> tasks)
            : base("MicrosoftOnline", "Microsoft 365")
        {
            if (licenseGroups == null || licenseGroups.Length == 0)
            {
                throw new ArgumentException(nameof(licenseGroups));
            }
            if (tasks == null || tasks.Count == 0)
            {
                throw new ArgumentException(nameof(tasks));
            }
            LicenseGroups = licenseGroups;
            SetTasks(tasks);
        }
    }

    public class MicrosoftOnlineWithMailboxTask : SequenceTask
    {
        [JsonProperty(Order = 10)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MailboxType Type { get; private set; }

        [JsonProperty(Order = 20)]
        public Guid[] LicenseGroups { get; private set; }

        public MicrosoftOnlineWithMailboxTask(MailboxType type, Guid[] licenseGroups)
            : base("MicrosoftOnlineWithMailbox", "Microsoft 365 with mailbox")
        {
            if (licenseGroups == null || licenseGroups.Length == 0)
            {
                throw new ArgumentException(nameof(licenseGroups));
            }
            Type = type;
            LicenseGroups = licenseGroups;
            List<AccountTask> tasks = new List<AccountTask> {
                new EnableRemoteMailboxTask(),
                new WaitTask(5),
                new ConfigureRemoteMailboxTask(),
                new MsolEnableSyncTask(),
                new MsolLicenseGroupTask(skipSyncCheck: true, skipDynamicGroupCheck: false),
                new WaitTask(120),
                new ConfigureOnlineMailboxTask(),
                new ConfigureOnlineOwaTask()
            };
            if (type == MailboxType.Employee || type == MailboxType.Faculty)
            {
                tasks.Add(new SendWelcomeMailTask());
            }
            SetTasks(tasks);
        }

        public MicrosoftOnlineWithMailboxTask(MailboxType type, Guid[] licenseGroups, List<AccountTask> tasks)
            : base("MicrosoftOnlineWithMailbox", "Microsoft 365 with mailbox")
        {
            if (licenseGroups == null || licenseGroups.Length == 0)
            {
                throw new ArgumentException(nameof(licenseGroups));
            }
            if (tasks == null || tasks.Count == 0)
            {
                throw new ArgumentException(nameof(tasks));
            }
            Type = type;
            LicenseGroups = licenseGroups;
            SetTasks(tasks);
        }
    }

    public class MicrosoftOnlinePostExpireTask : SequenceTask
    {
        public MicrosoftOnlinePostExpireTask()
            : base("MicrosoftOnlinePostExpire", "Clean up user resources after account expires")
        {
            SetTasks(new List<AccountTask>
            {
                new AddToOnpremGroupTask("U-exch-ndr-mailbox"),
                new SetOnlineMailboxTypeTask(ExchangeMailboxType.Shared),
                new SetHiddenFromAddressListTask(true),
                new MsolRemoveAllLicenseGroupTask(),
            });
        }

        public MicrosoftOnlinePostExpireTask(List<AccountTask> tasks)
            : base("MicrosoftOnlinePostExpire", "Clean up user resources after account expires")
        {
            if (tasks == null || tasks.Count == 0)
            {
                throw new ArgumentException(nameof(tasks));
            }
            SetTasks(tasks);
        }
    }

    public class MicrosoftOnlineRestoreTask : SequenceTask
    {
        [JsonProperty(Order = 10)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MailboxType Type { get; private set; }

        public MicrosoftOnlineRestoreTask(MailboxType type)
            : base("MicrosoftOnlineRestore", "Restore Microsoft 365 licenses and configure mailbox")
        {
            Type = type;
            SetTasks(new List<AccountTask>
            {
                new MsolRestoreLicenseGroupTask(),
                new WaitTask(120),
                new RemoveFromOnpremGroupTask("U-exch-ndr-mailbox"),
                new SetOnlineMailboxTypeTask(ExchangeMailboxType.Regular),
                new SetHiddenFromAddressListTask(false),
                new ConfigureOnlineMailboxTask(),
                new ConfigureOnlineOwaTask()
            });
        }

        public MicrosoftOnlineRestoreTask(MailboxType type, List<AccountTask> tasks)
            : base("MicrosoftOnlineRestore", "Restore Microsoft 365 licenses and configure mailbox")
        {
            if (tasks == null || tasks.Count == 0)
            {
                throw new ArgumentException(nameof(tasks));
            }
            Type = type;
            SetTasks(tasks);
        }
    }
}