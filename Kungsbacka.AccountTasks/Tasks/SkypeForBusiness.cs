using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kungsbacka.AccountTasks
{
    public class EnableCSUserTask : BasicTask
    {
        public EnableCSUserTask()
            : base("EnableCSUser", "Enable on-prem Skype")
        {
        }
    }

    public class GrantCSConferencingPolicyTask : BasicTask
    {
        [JsonIgnore]
        public static readonly string DefaultConferencingPolicy = "KBA_Standard_CAL_Conference_Policy";

        [JsonProperty(Order = 10)]
        public string ConferencingPolicy { get; private set; }

        public GrantCSConferencingPolicyTask(string conferencingPolicy)
            : base("GrantCSConferencingPolicy", "Grant on-prem Skype conferencing policy")
        {
            ConferencingPolicy = conferencingPolicy;
        }

        public GrantCSConferencingPolicyTask()
            : this(null)
        {
        }

        public bool ShouldSerializeConferencingPolicy()
        {
            return ConferencingPolicy != null;
        }
    }

    public class OnpremSkypeTask : SequenceTask
    {
        [JsonProperty(Order = 10)]
        public string ConferencingPolicy { get; private set; }
        public OnpremSkypeTask(string conferencingPolicy)
            : base("OnpremSkype", "On-prem Skype")
        {
            if (string.IsNullOrEmpty(conferencingPolicy))
            {
                throw new ArgumentException("conferencingPolicy");
            }
            ConferencingPolicy = conferencingPolicy;
            SetTasks(new List<AccountTask> {
                new EnableCSUserTask(),
                new WaitTask(5),
                new GrantCSConferencingPolicyTask()
            });
        }

        public OnpremSkypeTask(string conferencingPolicy, List<AccountTask> tasks)
            : base("OnpremSkype", "On-prem Skype")
        {
            if (string.IsNullOrEmpty(conferencingPolicy))
            {
                throw new ArgumentException("conferencingPolicy");
            }
            if (tasks == null || tasks.Count == 0)
            {
                throw new ArgumentException("tasks");
            }
            ConferencingPolicy = conferencingPolicy;
            SetTasks(tasks);
        }
    }
}
