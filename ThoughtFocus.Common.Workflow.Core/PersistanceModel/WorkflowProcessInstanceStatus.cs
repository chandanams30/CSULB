using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoughtFocus.Common.Workflow.Core.PersistanceModel
{
    [Table("WorkflowProcessInstanceStatus", Schema = "WorkFlow")]
    public class WorkflowProcessInstanceStatus
    {
        [Key]
        public long WorkflowProcessInstanceStatusID { get; set; }

        public Guid Lock { get; set; }

        public int Status { get; set; }

        public long ProcessInstanceID { get; set; }

        public long WorkFlowDefinationID { get; set; }

    }
}
