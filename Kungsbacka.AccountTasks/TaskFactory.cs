using System;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;

namespace Kungsbacka.AccountTasks
{
    public static class TaskFactory
    {
        public static List<AccountTask> DeserializeTasks(string taskJson)
        {
            dynamic deserializedTasks;
            if (taskJson[0] == '[')
            {
                deserializedTasks = JsonConvert.DeserializeObject<List<ExpandoObject>>(taskJson);
            }
            else
            {
                ExpandoObject tmp = JsonConvert.DeserializeObject<ExpandoObject>(taskJson);
                deserializedTasks = new List<ExpandoObject> { tmp };
            }
            var taskList = new List<AccountTask>(deserializedTasks.Count);
            foreach (dynamic task in deserializedTasks)
            {
                taskList.Add(TaskFactory.CreateTask(task));
            }
            return taskList;
        }

        public static AccountTask CreateTask(IDictionary<string, object> dictionary)
        {
            AccountTask returnTask = null;
            string taskName = dictionary.GetValueOrDefault<string>("TaskName");
            switch (taskName)
            {
                case "Wait" :
                    {
                        long? minutes = dictionary.GetValueOrDefault<long?>("Minutes");
                        returnTask = new WaitTask(minutes);
                        break;
                    }
                case "MsolEnableSync":
                    {
                        returnTask = new MsolEnableSyncTask();
                        break;
                    }
                case "MsolLicense":
                    {
                        returnTask = new MsolLicenseTask(dictionary.GetMsolLicenseArray());
                        break;
                    }
                case "MsolLicenseGroup":
                    {
                        bool skipSyncCheck = dictionary.GetValueOrDefault<bool>("SkipSyncCheck");
                        bool skipDynamicGroupCheck = dictionary.GetValueOrDefault<bool>("SkipDynamicGroupCheck");
                        returnTask = new MsolLicenseGroupTask(dictionary.GetGuidArray("LicenseGroups"), skipSyncCheck, skipDynamicGroupCheck);
                        break;
                    }
                case "MsolRemoveLicenseGroup":
                    {
                        bool skipBaseLicenseCheck = dictionary.GetValueOrDefault<bool>("SkipBaseLicenseCheck");
                        returnTask = new MsolRemoveLicenseGroupTask(dictionary.GetGuidArray("LicenseGroups"), skipBaseLicenseCheck);
                        break;
                    }
                case "MsolRemoveAllLicenseGroup":
                    {
                        bool skipStashLicense = dictionary.GetValueOrDefault<bool>("SkipStashLicense");
                        returnTask = new MsolRemoveAllLicenseGroupTask(skipStashLicense);
                        break;
                    }
                case "MsolRestoreLicenseGroup":
                    {
                        bool skipSyncCheck = dictionary.GetValueOrDefault<bool>("SkipSyncCheck");
                        returnTask = new MsolRestoreLicenseGroupTask(skipSyncCheck);
                        break;
                    }
                case "ConfigureOnlineMailbox":
                    {
                        returnTask = new ConfigureOnlineMailboxTask(
                            dictionary.GetNullableEnum<MailboxType>("Type")
                        );
                        break;
                    }
                case "ConfigureOnlineOwa":
                    {
                        returnTask = new ConfigureOnlineOwaTask();
                        break;
                    }
                case "SetOnlineMailboxType":
                    {
                        returnTask = new SetOnlineMailboxTypeTask(
                            dictionary.GetEnum<ExchangeMailboxType>("MailboxType")    
                        );
                        break;
                    }
                case "SendWelcomeMail":
                    {
                        returnTask = new SendWelcomeMailTask(
                            dictionary.GetNullableEnum<MailboxType>("Type")
                        );
                        break;
                    }
                case "SetHiddenFromAddressList":
                    {
                        bool? hidden = dictionary.GetValueOrDefault<bool>("Hidden");
                        if (hidden == null)
                        {
                            throw new ArgumentException("Hidden");
                        }
                        returnTask = new SetHiddenFromAddressListTask((bool)hidden);
                        break;
                    }
                case "EnableRemoteMailbox":
                    {
                        returnTask = new EnableRemoteMailboxTask();
                        break;
                    }
                case "ConfigureRemoteMailbox":
                    {
                        returnTask = new ConfigureRemoteMailboxTask();
                        break;
                    }
                case "SamlId":
                    {
                        returnTask = new SamlIdTask();
                        break;
                    }
                case "MicrosoftOnline":
                    {
                        returnTask = new MicrosoftOnlineTask(
                            dictionary.GetGuidArray("LicenseGroups"),
                            GetTaskSequence(dictionary)
                        );
                        break;
                    }
                case "MicrosoftOnlineWithMailbox":
                    {
                        returnTask = new MicrosoftOnlineWithMailboxTask(
                            dictionary.GetEnum<MailboxType>("Type"),
                            dictionary.GetGuidArray("LicenseGroups"),
                            GetTaskSequence(dictionary)
                        );
                        break;
                    }
                case "MicrosoftOnlinePostExpire":
                    {
                        returnTask = new MicrosoftOnlinePostExpireTask(GetTaskSequence(dictionary));
                        break;
                    }
                case "MicrosoftOnlineRestore":
                    {
                        returnTask = new MicrosoftOnlineRestoreTask(
                            dictionary.GetEnum<MailboxType>("Type"),
                            GetTaskSequence(dictionary)
                        );
                        break;
                    }
                case "AddToOnpremGroup":
                    {
                        string group = dictionary.GetValueOrDefault<string>("Group");
                        if (string.IsNullOrEmpty(group))
                        {
                            throw new ArgumentException("Group");
                        }
                        returnTask = new AddToOnpremGroupTask(group);
                        break;
                    }
                case "RemoveFromOnpremGroup":
                    {
                        string group = dictionary.GetValueOrDefault<string>("Group");
                        if (string.IsNullOrEmpty(group))
                        {
                            throw new ArgumentException("Group");
                        }
                        returnTask = new RemoveFromOnpremGroupTask(group);
                        break;
                    }
                default:
                    {
                        throw new ArgumentException(string.Format("Invalid TaskName: {0}", taskName));
                    }
            }
            returnTask.Id = dictionary.GetValueOrDefault<long?>("Id");
            if (returnTask is BasicTask)
            {
                ((BasicTask)returnTask).RetryCount = dictionary.GetValueOrDefault<long?>("RetryCount");
                ((BasicTask)returnTask).WaitUntil = dictionary.GetValueOrDefault<DateTime?>("WaitUntil");
            }
            return returnTask;
        }

        static List<AccountTask> GetTaskSequence(IDictionary<string, object> dictionary)
        {
            dynamic taskSequence = dictionary.GetValueOrDefault<dynamic>("Tasks");
            if (!(taskSequence is List<object>) || taskSequence.Count == 0)
            {
                throw new ArgumentException("Tasks");
            }
            var taskList = new List<AccountTask>(taskSequence.Count);
            foreach (dynamic dynamicTask in taskSequence)
            {
                AccountTask task = TaskFactory.CreateTask(dynamicTask);
                taskList.Add(task);
            }
            return taskList;
        }
    }
}
