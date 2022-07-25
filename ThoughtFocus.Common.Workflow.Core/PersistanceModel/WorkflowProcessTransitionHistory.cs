using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoughtFocus.Common.Workflow.Core.PersistanceModel
{
    [Table("WorkflowProcessTransitionHistory", Schema = "WorkFlow")]
    public class WorkflowProcessTransitionHistory
    {
         [Key]
        public Guid WorkflowProcessTransitionHistoryID { get; set; }

        public long ProcessInstanceID { get; set; }

        public long WorkFlowDefinationID { get; set; }

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
