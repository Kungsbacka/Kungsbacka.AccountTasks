using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Kungsbacka.AccountTasks
{
    public static class TaskFactory
    {
        public static List<AccountTask> DeserializeTasks(string taskJson)
        {
            using (var document = JsonDocument.Parse(taskJson))
            {
                if (document.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var taskList = new List<AccountTask>(document.RootElement.GetArrayLength());
                    foreach (var element in document.RootElement.EnumerateArray())
                        taskList.Add(CreateTask(ConvertObject(element)));
                    return taskList;
                }
                else
                {
                    return new List<AccountTask> { CreateTask(ConvertObject(document.RootElement)) };
                }
            }
        }

        private static Dictionary<string, object> ConvertObject(JsonElement element)
        {
            var dict = new Dictionary<string, object>();
            foreach (var prop in element.EnumerateObject())
                dict[prop.Name] = ConvertElement(prop.Value);
            return dict;
        }

        private static object ConvertElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    return ConvertObject(element);
                case JsonValueKind.Array:
                    var list = new List<object>();
                    foreach (var item in element.EnumerateArray())
                        list.Add(ConvertElement(item));
                    return list;
                case JsonValueKind.String:
                    if (element.TryGetDateTime(out DateTime dt))
                        return dt;
                    return element.GetString();
                case JsonValueKind.Number:
                    return element.GetInt64();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                    return null;
                default:
                    throw new JsonException($"Unsupported JsonValueKind: {element.ValueKind}");
            }
        }

        public static AccountTask CreateTask(IDictionary<string, object> dictionary)
        {
            string taskName = dictionary.GetValueOrDefault<string>("TaskName");
            AccountTask returnTask;
            switch (taskName)
            {
                case "Wait":
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
                case "DisableMailbox":
                    {
                        returnTask = new DisableMailboxTask();
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
                            dictionary.GetTaskSequence()
                        );
                        break;
                    }
                case "MicrosoftOnlineWithMailbox":
                    {
                        returnTask = new MicrosoftOnlineWithMailboxTask(
                            dictionary.GetEnum<MailboxType>("Type"),
                            dictionary.GetGuidArray("LicenseGroups"),
                            dictionary.GetTaskSequence()
                        );
                        break;
                    }
                case "MicrosoftOnlineAutomaticLicenseChangeTask":
                    {
                        returnTask = new MicrosoftOnlineAutomaticLicenseChangeTask(
                            dictionary.GetEnum<MailboxType>("Type"),
                            dictionary.GetGuidArray("LicenseGroups"),
                            dictionary.GetTaskSequence()
                        );
                        break;
                    }
                case "MicrosoftOnlinePostExpire":
                    {
                        returnTask = new MicrosoftOnlinePostExpireTask(dictionary.GetTaskSequence());
                        break;
                    }
                case "MicrosoftOnlineRestore":
                    {
                        returnTask = new MicrosoftOnlineRestoreTask(
                            dictionary.GetEnum<MailboxType>("Type"),
                            dictionary.GetTaskSequence()
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
            if (returnTask is BasicTask task)
            {
                task.RetryCount = dictionary.GetValueOrDefault<long?>("RetryCount");
                task.WaitUntil = dictionary.GetValueOrDefault<DateTime?>("WaitUntil");
            }
            return returnTask;
        }
    }
}
