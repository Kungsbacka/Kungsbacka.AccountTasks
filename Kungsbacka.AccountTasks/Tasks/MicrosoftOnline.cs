using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Kungsbacka.AccountTasks
{
    public class MsolEnableSyncTask : BasicTask
    {
        public MsolEnableSyncTask()
            : base("MsolEnableSync", "Synchronize to Office 365")
        {
        }
    }

    public class MsolLicenseTask : BasicTask
    {    
        [JsonProperty(Order = 10)]
        public MsolLicense[] License { get; private set; }

        public MsolLicenseTask(MsolLicense[] license)
            : base("MsolLicense", "Office 365 License")
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

    public class MicrosoftOnlineTask : SequenceTask
    {
        [JsonProperty(Order = 10)]
        public MsolLicense[] License { get; private set; }

        public MicrosoftOnlineTask(MsolLicense[] license)
            : base("MicrosoftOnline", "Office 365")
        {
            if (license == null || license.Length == 0)
            {
                throw new ArgumentException("license");
            }
            License = license;
            SetTasks(new List<AccountTask> {
                new MsolEnableSyncTask(),
                new WaitTask(35),
                new MsolLicenseTask()
            });
        }

        public MicrosoftOnlineTask(MsolLicense[] license, List<AccountTask> tasks)
            : base("MicrosoftOnline", "Office 365")
        {
            if (license == null || license.Length == 0)
            {
                throw new ArgumentException("license");
            }
            if (tasks == null || tasks.Count == 0)
            {
                throw new ArgumentException("tasks");
            }
            License = license;
            SetTasks(tasks);
        }
    }

    public class MicrosoftOnlineWithMailboxTask : SequenceTask
    {
        [JsonProperty(Order = 10)]
        public MsolLicense[] License { get; private set; }

        private void CheckLicense(MsolLicense[] license)
        {
            // Check if Exchange service plan is disabled
            bool exchangeDisabled = license.Any(t1 =>
                t1.DisabledPlans != null && t1.DisabledPlans.Any(t2 => 
                    t2.Equals("9aaf7827-d63c-4b61-89c3-182f06f82e5c", StringComparison.OrdinalIgnoreCase)
                )
            );
            if (license == null || license.Length == 0 || exchangeDisabled)
            {
                throw new ArgumentException("license");
            }
        }

        public MicrosoftOnlineWithMailboxTask(MsolLicense[] license)
            : base("MicrosoftOnlineWithMailbox", "Office 365 with mailbox")
        {
            CheckLicense(license);
            License = license;
            SetTasks(new List<AccountTask> {
                new EnableRemoteMailboxTask(),
                new WaitTask(5),
                new ConfigureRemoteMailboxTask(),
                new MsolEnableSyncTask(),
                new WaitTask(35),
                new MsolLicenseTask(),
                new WaitTask(5),
                new ConfigureOnlineOwaTask()
            });
        }

        public MicrosoftOnlineWithMailboxTask(MsolLicense[] license, List<AccountTask> tasks)
            : base("MicrosoftOnlineWithMailbox", "Office 365 with mailbox")
        {
            CheckLicense(license);
            if (tasks == null || tasks.Count == 0)
            {
                throw new ArgumentException("tasks");
            }
            License = license;
            SetTasks(tasks);
        }
    }
}
