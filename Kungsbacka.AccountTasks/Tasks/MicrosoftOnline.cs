using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kungsbacka.AccountTasks
{
    public class MsolEnableSyncTask : BasicTask
    {
        public MsolEnableSyncTask()
            : base("MsolEnableSync", "Synchronize to Azure AD")
        {
        }
    }

    //public class MsolLicenseTask : BasicTask
    //{
    //    [JsonPropertyOrder(10)]
    //    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //    public MsolLicense[] License { get; private set; }

    //    public MsolLicenseTask(MsolLicense[] license)
    //        : base("MsolLicense", "Directly assign Microsoft 365 licenses")
    //    {
    //        License = license;
    //    }

    //    public MsolLicenseTask()
    //        : this(null)
    //    {
    //    }
    //}

    public class MsolLicenseGroupTask : BasicTask
    {
        [JsonPropertyOrder(10)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid[] LicenseGroups { get; private set; }

        [JsonPropertyOrder(20)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool SkipSyncCheck { get; private set; }

        [JsonPropertyOrder(30)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
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
    }

    public class MsolRemoveLicenseGroupTask : BasicTask
    {
        [JsonPropertyOrder(10)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid[] LicenseGroups { get; private set; }

        [JsonPropertyOrder(20)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
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
    }

    public class MsolRemoveAllLicenseGroupTask : BasicTask
    {
        [JsonPropertyOrder(10)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
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
    }

    public class MsolClearStashedLicenseTask : BasicTask
    {
        public MsolClearStashedLicenseTask()
            : base("MsolClearStashedLicense", "Clears attribute that store stashed licenses")
        {
        }
    }

    public class MsolRestoreLicenseGroupTask : BasicTask
    {
        [JsonPropertyOrder(10)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
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
    }

    public class MicrosoftOnlineTask : SequenceTask
    {
        [JsonPropertyOrder(10)]
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
        [JsonPropertyOrder(10)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MailboxType Type { get; private set; }

        [JsonPropertyOrder(20)]
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
        [JsonPropertyOrder(10)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
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

    public class MicrosoftOnlineAutomaticLicenseChangeTask : SequenceTask
    {
        [JsonPropertyOrder(10)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MailboxType Type { get; private set; }

        [JsonPropertyOrder(20)]
        public Guid[] LicenseGroups { get; private set; }

        public MicrosoftOnlineAutomaticLicenseChangeTask(MailboxType type, Guid[] licenseGroups, CurrenLicensingStatus currenLicensingStatus, NewLicenseingStatus newLicenseingStatus)
            : base("MicrosoftOnlineAutomaticLicenseChangeTask", "Change Microsoft 365 license")
        {
            Type = type;
            LicenseGroups = licenseGroups;

            // Note: Branches producing identical tasks are kept separate intentionally to allow future divergence

            // Unlicensed -> Unlicensed: not allowed
            if (currenLicensingStatus == CurrenLicensingStatus.Unlicensed && newLicenseingStatus == NewLicenseingStatus.Unlicensed)
            {
                throw new ArgumentException("Transitioning from unlicensed to unlicensed is not allowed.");
            }

            // Unlicensed -> License w/ mail
            if (currenLicensingStatus == CurrenLicensingStatus.Unlicensed && newLicenseingStatus == NewLicenseingStatus.MailEnabled)
            {
                SetTasks(new List<AccountTask> {
                    new EnableRemoteMailboxTask(),
                    new WaitTask(5),
                    new ConfigureRemoteMailboxTask(),
                    new MsolEnableSyncTask(),
                    new MsolLicenseGroupTask(skipSyncCheck: true, skipDynamicGroupCheck: false),
                    new WaitTask(120),
                    new ConfigureOnlineMailboxTask(),
                    new ConfigureOnlineOwaTask()
                });
            }
            // Unlicensed -> License w/o mail
            else if (currenLicensingStatus == CurrenLicensingStatus.Unlicensed && newLicenseingStatus == NewLicenseingStatus.MailDisabled)
            {
                SetTasks(new List<AccountTask> {
                    new MsolEnableSyncTask(),
                    new MsolLicenseGroupTask(skipSyncCheck: true, skipDynamicGroupCheck: false)
                });
            }
            // Active license w/ mail -> License w/ mail
            else if (currenLicensingStatus == CurrenLicensingStatus.ActiveMailEnabled && newLicenseingStatus == NewLicenseingStatus.MailEnabled)
            {
                SetTasks(new List<AccountTask> {
                    new MsolLicenseGroupTask(skipSyncCheck: true, skipDynamicGroupCheck: false)
                });
            }
            // Active license w/ mail -> License w/o mail
            else if (currenLicensingStatus == CurrenLicensingStatus.ActiveMailEnabled && newLicenseingStatus == NewLicenseingStatus.MailDisabled)
            {
                SetTasks(new List<AccountTask> {
                    new MsolLicenseGroupTask(skipSyncCheck: true, skipDynamicGroupCheck: false)
                });
            }
            // Active license w/ mail -> Unlicensed
            else if (currenLicensingStatus == CurrenLicensingStatus.ActiveMailEnabled && newLicenseingStatus == NewLicenseingStatus.Unlicensed)
            {
                SetTasks(new List<AccountTask> {
                    new MsolRemoveAllLicenseGroupTask()
                });
            }
            // Active license w/o mail -> License w/ mail
            else if (currenLicensingStatus == CurrenLicensingStatus.ActiveMailDisabled && newLicenseingStatus == NewLicenseingStatus.MailEnabled)
            {
                SetTasks(new List<AccountTask> {
                    new EnableRemoteMailboxTask(),
                    new WaitTask(5),
                    new ConfigureRemoteMailboxTask(),
                    new MsolEnableSyncTask(),
                    new MsolLicenseGroupTask(skipSyncCheck: true, skipDynamicGroupCheck: false),
                    new WaitTask(120),
                    new ConfigureOnlineMailboxTask(),
                    new ConfigureOnlineOwaTask()
                });
            }
            // Active license w/o mail -> License w/o mail
            else if (currenLicensingStatus == CurrenLicensingStatus.ActiveMailDisabled && newLicenseingStatus == NewLicenseingStatus.MailDisabled)
            {
                SetTasks(new List<AccountTask> {
                    new MsolLicenseGroupTask(skipSyncCheck: true, skipDynamicGroupCheck: false)
                });
            }
            // Active license w/o mail -> Unlicensed
            else if (currenLicensingStatus == CurrenLicensingStatus.ActiveMailDisabled && newLicenseingStatus == NewLicenseingStatus.Unlicensed)
            {
                SetTasks(new List<AccountTask> {
                    new MsolRemoveAllLicenseGroupTask()
                });
            }
            // Stashed license w/ mail -> License w/ mail
            else if (currenLicensingStatus == CurrenLicensingStatus.StashedMailEnabled && newLicenseingStatus == NewLicenseingStatus.MailEnabled)
            {
                SetTasks(new List<AccountTask>
                {
                    new MsolClearStashedLicenseTask(),
                    new MsolEnableSyncTask(),
                    new WaitTask(1),
                    new MsolLicenseGroupTask(skipSyncCheck: true, skipDynamicGroupCheck: false),
                    new WaitTask(120),
                    new RemoveFromOnpremGroupTask("U-exch-ndr-mailbox"),
                    new SetOnlineMailboxTypeTask(ExchangeMailboxType.Regular),
                    new SetHiddenFromAddressListTask(false),
                    new ConfigureOnlineMailboxTask(),
                    new ConfigureOnlineOwaTask()
                });
            }
            // Stashed license w/ mail -> License w/o mail
            else if (currenLicensingStatus == CurrenLicensingStatus.StashedMailEnabled && newLicenseingStatus == NewLicenseingStatus.MailDisabled)
            {
                SetTasks(new List<AccountTask>
                {
                    new MsolClearStashedLicenseTask(),
                    new MsolEnableSyncTask(),
                    new WaitTask(1),
                    new MsolLicenseGroupTask(skipSyncCheck: true, skipDynamicGroupCheck: false),
                    new RemoveFromOnpremGroupTask("U-exch-ndr-mailbox"),
                    new SetOnlineMailboxTypeTask(ExchangeMailboxType.Regular),
                    new SetHiddenFromAddressListTask(false)
                });
            }
            // Stashed license w/ mail -> Unlicensed
            else if (currenLicensingStatus == CurrenLicensingStatus.StashedMailEnabled && newLicenseingStatus == NewLicenseingStatus.Unlicensed)
            {
                SetTasks(new List<AccountTask>
                {
                    new MsolClearStashedLicenseTask(),
                    new WaitTask(1),
                    new MsolLicenseGroupTask(skipSyncCheck: true, skipDynamicGroupCheck: false),
                    new RemoveFromOnpremGroupTask("U-exch-ndr-mailbox"),
                    new SetOnlineMailboxTypeTask(ExchangeMailboxType.Regular),
                    new SetHiddenFromAddressListTask(false)
                });
            }
            // Stashed license w/o mail -> License w/ mail
            else if (currenLicensingStatus == CurrenLicensingStatus.StashedMailDisabled && newLicenseingStatus == NewLicenseingStatus.MailEnabled)
            {
                SetTasks(new List<AccountTask> {
                    new MsolClearStashedLicenseTask(),
                    new EnableRemoteMailboxTask(),
                    new WaitTask(5),
                    new ConfigureRemoteMailboxTask(),
                    new MsolEnableSyncTask(),
                    new MsolLicenseGroupTask(skipSyncCheck: true, skipDynamicGroupCheck: false),
                    new WaitTask(120),
                    new ConfigureOnlineMailboxTask(),
                    new ConfigureOnlineOwaTask()
                });
            }
            // Stashed license w/o mail -> License w/o mail
            else if (currenLicensingStatus == CurrenLicensingStatus.StashedMailDisabled && newLicenseingStatus == NewLicenseingStatus.MailDisabled)
            {
                SetTasks(new List<AccountTask> {
                    new MsolClearStashedLicenseTask(),
                    new WaitTask(1),
                    new MsolLicenseGroupTask(skipSyncCheck: true, skipDynamicGroupCheck: false)
                });
            }
            // Stashed license w/o mail -> Unlicensed
            else if (currenLicensingStatus == CurrenLicensingStatus.StashedMailDisabled && newLicenseingStatus == NewLicenseingStatus.Unlicensed)
            {
                SetTasks(new List<AccountTask> {
                    new MsolClearStashedLicenseTask(),
                    new WaitTask(1),
                    new MsolRemoveAllLicenseGroupTask()
                });
            }
            else
            {
                throw new ArgumentOutOfRangeException("Unhandled combination of licensing states.");
            }
        }

        public MicrosoftOnlineAutomaticLicenseChangeTask(MailboxType type, Guid[] licenseGroups, List<AccountTask> tasks)
            : base("MicrosoftOnlineAutomaticLicenseChangeTask", "Change Microsoft 365 license")
        {
            if (tasks == null || tasks.Count == 0)
            {
                throw new ArgumentException(nameof(tasks));
            }
            Type = type;
            LicenseGroups = licenseGroups;
            SetTasks(tasks);
        }
    }

    public enum CurrenLicensingStatus
    {
        ActiveMailEnabled,
        ActiveMailDisabled,
        StashedMailEnabled,
        StashedMailDisabled,
        Unlicensed
    }

    public enum NewLicenseingStatus
    {
        MailEnabled,
        MailDisabled,
        Unlicensed
    }
}
