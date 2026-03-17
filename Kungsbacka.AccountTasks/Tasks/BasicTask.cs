using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Kungsbacka.AccountTasks
{
    public class BasicTask : AccountTask
    {
        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? RetryCount
        {
            get => _retryCount;
            set => _retryCount = value > 0 ? value : null;
        }
        private long? _retryCount;

        [JsonIgnore]
        public DateTime? WaitUntil { get; set; }

        [JsonPropertyName("WaitUntil")]
        [JsonPropertyOrder(3)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime? WaitUntilSerialized => WaitUntil.HasValue && WaitUntil >= DateTime.Now ? WaitUntil : null;

        public BasicTask(string taskName)
            : this(taskName, taskName) { }

        public BasicTask(string taskName, string displayName)
            : base(taskName, displayName)
        {
        }
    }
}
