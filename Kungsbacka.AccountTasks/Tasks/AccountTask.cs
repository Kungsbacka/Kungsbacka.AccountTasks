using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kungsbacka.AccountTasks
{
    public abstract class AccountTask
    {
        internal static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions();
        private static readonly JsonSerializerOptions _indentedOptions = new JsonSerializerOptions { WriteIndented = true };

        [JsonIgnore]
        public string DisplayName { get; private set; }

        [JsonPropertyOrder(1)]
        public string TaskName { get; private set; }

        [JsonPropertyOrder(20)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? Id
        {
            get => _id;
            set => _id = value > 0 ? value : null;
        }
        private long? _id;

        protected AccountTask(string taskName)
            : this(taskName, taskName)
        {
        }

        protected AccountTask(string taskName, string displayName)
        {
            TaskName = taskName;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public string ToJson(bool indented = false)
            => JsonSerializer.Serialize(this, GetType(), indented ? _indentedOptions : DefaultOptions);
    }
}
