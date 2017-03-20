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
                case "EnableMailbox":
                {
                    returnTask = new EnableMailboxTask(dictionary.GetNullableMailboxType());
                    break;
                }
                case "DisableMailbox":
                {
                    returnTask = new DisableMailboxTask();
                    break;
                }
                case "ConnectMailbox":
                {
                    returnTask = new ConnectMailboxTask();
                    break;
                }
                case "ConfigureMailbox":
                {
                    returnTask = new ConfigureMailboxTask(dictionary.GetNullableMailboxType());
                    break;
                }
                case "ConfigureOwa":
                {
                    returnTask = new ConfigureOwaTask(dictionary.GetNullableMailboxType());
                    break;
                }
                case "ConfigureOnlineOwa":
                {
                    returnTask = new ConfigureOnlineOwaTask();
                    break;
                }
                case "ConfigureCalendar":
                {
                    returnTask = new ConfigureCalendarTask(dictionary.GetNullableMailboxType());
                    break;
                }
                case "ConfigureMessage":
                {
                    returnTask = new ConfigureMessageTask();
                    break;
                }
                case "ConfigureMailboxAutoReplyTask":
                {
                    if (!dictionary.ContainsKey("Enabled"))
                    {
                        throw new ArgumentException("Enabled");
                    }
                    bool enabled = (bool)dictionary["Enabled"];
                    string message = dictionary.GetValueOrDefault<string>("Message");
                    returnTask = new ConfigureMailboxAutoReplyTask(enabled, message);
                    break;
                }
                case "SendWelcomeMail":
                {
                    returnTask = new SendWelcomeMailTask(dictionary.GetNullableMailboxType());
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
                case "HomeFolder":
                {
                    string path = dictionary.GetValueOrDefault<string>("Path");
                    returnTask = new HomeFolderTask(path);
                    break;
                }
                case "SamlId":
                {
                    returnTask = new SamlIdTask();
                    break;
                }
                case "EnableCSUser":
                {
                    returnTask = new EnableCSUserTask();
                    break;
                }
                case "GrantCSConferencingPolicy":
                {
                    string conferencingPolicy = dictionary.GetValueOrDefault<string>("ConferencingPolicy");
                    returnTask = new GrantCSConferencingPolicyTask(conferencingPolicy);
                    break;
                }
                case "OnpremMailbox":
                {
                    string mailboxTypeString = dictionary.GetValueOrDefault<string>("Type");
                    MailboxType mailboxType;
                    if (!Enum.TryParse(mailboxTypeString, out mailboxType))
                    {
                        throw new ArgumentException("Type");
                    }
                    returnTask = new OnpremMailboxTask(
                        mailboxType,
                        GetTaskSequence(dictionary)
                    );
                    break;
                }
                case "ReconfigureOnpremMailbox":
                {
                    string mailboxTypeString = dictionary.GetValueOrDefault<string>("Type");
                    MailboxType mailboxType;
                    if (!Enum.TryParse(mailboxTypeString, out mailboxType))
                    {
                        throw new ArgumentException("Type");
                    }
                    returnTask = new ReconfigureOnpremMailboxTask(
                        mailboxType,
                        GetTaskSequence(dictionary)
                    );
                    break;
                }
                case "ReconnectOnpremMailbox":
                {
                    string mailboxTypeString = dictionary.GetValueOrDefault<string>("Type");
                    MailboxType mailboxType;
                    if (!Enum.TryParse(mailboxTypeString, out mailboxType))
                    {
                        throw new ArgumentException("Type");
                    }
                    returnTask = new ReconnectOnpremMailboxTask(
                        mailboxType,
                        GetTaskSequence(dictionary)
                    );
                    break;
                }
                case "OnpremSkype":
                {
                    returnTask = new OnpremSkypeTask(
                        dictionary.GetValueOrDefault<string>("ConferencingPolicy"),
                        GetTaskSequence(dictionary)
                    );
                    break;
                }
                case "MicrosoftOnline":
                {
                    returnTask = new MicrosoftOnlineTask(
                        dictionary.GetMsolLicenseArray(),
                        GetTaskSequence(dictionary)
                    );
                    break;
                }
                case "MicrosoftOnlineWithMailbox":
                {
                    returnTask = new MicrosoftOnlineWithMailboxTask(
                        dictionary.GetMsolLicenseArray(),
                        GetTaskSequence(dictionary)
                    );
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
