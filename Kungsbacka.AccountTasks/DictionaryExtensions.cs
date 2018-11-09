using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kungsbacka.AccountTasks
{
    public static class DictionaryExtensions
    {
        public static T GetValueOrDefault<T>(this IDictionary<string, object> dictionary, string key)
        {
            object value;
            if (dictionary.TryGetValue(key, out value))
            {
                try
                {
                    return (T)value;
                }
                catch
                {
                    return default(T);
                }
            }
            return default(T);
        }

        public static MailboxType? GetNullableMailboxType(this IDictionary<string, object> dictionary)
        {
            string mailboxTypeString = dictionary.GetValueOrDefault<string>("Type");
            if (string.IsNullOrEmpty(mailboxTypeString))
            {
                return null;
            }
            MailboxType mailboxType;
            if (!Enum.TryParse(mailboxTypeString, out mailboxType))
            {
                throw new ArgumentException("Unknown MailboxType");
            }
            return mailboxType;
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
