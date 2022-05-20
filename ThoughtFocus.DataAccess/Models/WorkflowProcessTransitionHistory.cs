using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class WorkflowProcessTransitionHistory
    {
        public Guid WorkflowProcessTransitionHistoryId { get; set; }
        public long ProcessInstanceId { get; set; }
        public long WorkFlowDefinationId { get; set; }
        public string ActorIdentityId { get; set; }
        public string ExecutorIdentityId { get; set; }
        public bool IsFinalised { get; set; }
        public string FromActivityName { get; set; }
        public string FromStateName { get; set; }
        public string ToActivityName { get; set; }
        public string ToStateName { get; set; }
        public string TransitionClassifier { get; set; }
        public DateTime TransitionTime { get; set; }
        public string TriggerName { get; set; }
    }
}
