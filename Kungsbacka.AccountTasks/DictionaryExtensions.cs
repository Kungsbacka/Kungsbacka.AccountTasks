using System;
using System.Collections.Generic;
using System.Linq;

namespace Kungsbacka.AccountTasks
{
    public static class DictionaryExtensions
    {
        public static T GetValueOrDefault<T>(this IDictionary<string, object> dictionary, string key)
        {
            if (dictionary.TryGetValue(key, out object value))
            {
                try
                {
                    return (T)value;
                }
                catch
                {
                    return default;
                }
            }
            return default;
        }

        public static List<AccountTask> GetTaskSequence(this IDictionary<string, object> dictionary)
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

        public static T? GetNullableEnum<T>(this IDictionary<string, object> dictionary, string key) where T : struct
        {
            string enumString = dictionary.GetValueOrDefault<string>(key);
            if (string.IsNullOrEmpty(enumString))
            {
                return null;
            }
            if (!Enum.TryParse(enumString, out T result))
            {
                throw new ArgumentException("Could not parse string as enum");
            }
            return result;
        }

        public static T GetEnum<T>(this IDictionary<string, object> dictionary, string key)
        {
            string enumString = dictionary.GetValueOrDefault<string>(key);
            return (T)Enum.Parse(typeof(T), enumString);
        }

        public static MsolLicense[] GetMsolLicenseArray(this IDictionary<string, object> dictionary)
        {
            var deserializedLicense = dictionary.GetValueOrDefault<List<object>>("License");
            if (deserializedLicense == null)
            {
                return null;
            }
            List<MsolLicense> license = new List<MsolLicense>(deserializedLicense.Count);
            foreach (object item in deserializedLicense)
            {
                string skuId = ((IDictionary<string, object>)item).GetValueOrDefault<string>("SkuId");
                List<object> disabledPlans = ((IDictionary<string, object>)item).GetValueOrDefault<List<object>>("DisabledPlans");
                license.Add(new MsolLicense(skuId, disabledPlans?.Select(t => (string)t).ToArray()));
            }
            return license.ToArray();
        }

        public static Guid[] GetGuidArray(this IDictionary<string, object> dictionary, string key)
        {
            var objectList = dictionary.GetValueOrDefault<List<object>>(key);
            if (objectList == null)
            {
                return null;
            }
            var guidList = new List<Guid>(objectList.Count);
            foreach (object obj in objectList)
            {
                guidList.Add(Guid.Parse((string)obj));
            }
            return guidList.ToArray();
        }
    }
}
