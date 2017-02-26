﻿using Newtonsoft.Json;

namespace Kungsbacka.AccountTasks
{
    public abstract class AccountTask
    {
        [JsonIgnore]
        public string DisplayName { get; private set; }

        [JsonProperty(Order = 1)]
        public string TaskName { get; private set; }

        [JsonProperty(Order = 20)]
        public long? Id { get; set; }

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

        public bool ShouldSerializeId()
        {
            return Id > 0;
        }

        public string ToJson(bool indented = false)
        {
            if (indented)
            {
                using (var stringWriter = new System.IO.StringWriter())
                {
                    var jsonTextWriter = new JsonTextWriter(stringWriter);
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.Indentation = 3;
                    var jsonSerializer = new JsonSerializer();
                    jsonSerializer.Serialize(jsonTextWriter, this);
                    return stringWriter.ToString();
                }
            }
            return JsonConvert.SerializeObject(this);
        }
    }
}
