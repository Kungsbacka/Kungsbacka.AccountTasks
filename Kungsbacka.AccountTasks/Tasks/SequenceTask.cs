using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kungsbacka.AccountTasks
{
    public class SequenceTask : AccountTask
    {
        [JsonProperty(Order = 100)]
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
