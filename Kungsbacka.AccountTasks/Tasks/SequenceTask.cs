using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kungsbacka.AccountTasks
{
    internal class AccountTaskCollectionConverter : JsonConverter<ReadOnlyCollection<AccountTask>>
    {
        public override ReadOnlyCollection<AccountTask> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException("Deserialization is handled by TaskFactory.");

        public override void Write(Utf8JsonWriter writer, ReadOnlyCollection<AccountTask> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var task in value)
                JsonSerializer.Serialize(writer, task, task.GetType(), options);
            writer.WriteEndArray();
        }
    }

    public class SequenceTask : AccountTask
    {
        [JsonPropertyOrder(100)]
        [JsonConverter(typeof(AccountTaskCollectionConverter))]
        public ReadOnlyCollection<AccountTask> Tasks => _tasks.AsReadOnly();

        private List<AccountTask> _tasks;

        public SequenceTask(string taskName)
            : base(taskName, taskName)
        {
        }

        public SequenceTask(string taskName, string displayName)
            : base(taskName, displayName)
        {
        }

        protected void SetTasks(List<AccountTask> tasks)
        {
            _tasks = tasks;
        }
    }
}
