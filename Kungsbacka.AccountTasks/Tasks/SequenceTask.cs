using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Kungsbacka.AccountTasks
{
    public class SequenceTask : AccountTask
    {
        [JsonProperty(Order = 100)]
        public ReadOnlyCollection<AccountTask> Tasks
        {
            get
            {
                return _tasks.AsReadOnly();
            }
        }
        List<AccountTask> _tasks;

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
