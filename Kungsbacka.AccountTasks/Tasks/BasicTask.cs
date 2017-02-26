using System;
using Newtonsoft.Json;

namespace Kungsbacka.AccountTasks
{
    public class BasicTask : AccountTask
    {
        [JsonProperty(Order = 2)]
        public long? RetryCount { get; set; }

        [JsonProperty(Order = 3)]
        public DateTime? WaitUntil { get; set; }

        public BasicTask(string taskName)
            : this(taskName, taskName) { }

        public BasicTask(string taskName, string displayName)
            : base(taskName, displayName)
        {
        }

        public bool ShouldSerializeRetryCount()
        {
            return RetryCount > 0;
        }

        public bool ShouldSerializeWaitUntil()
        {
            return WaitUntil >= DateTime.Now;
        }
    }
}
