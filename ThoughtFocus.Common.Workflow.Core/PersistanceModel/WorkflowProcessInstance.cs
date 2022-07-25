using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThoughtFocus.Common.Workflow.Core.Model;

namespace ThoughtFocus.Common.Workflow.Core.PersistanceModel
{
    [Table("WorkflowProcessInstance", Schema = "WorkFlow")]
    public class WorkflowProcessInstance
    {
        [Key]
        public long WorkflowProcessInstanceID { get; set; }

        public long  ProcessInstanceID { get; set; }

        public Guid SchemeId { get; set; }

        public string ActivityName { get; set; }

        public string StateName { get; set; }    

        public long WorkflowDefinitionID { get; set; }

        public string PreviousActivity { get; set; }
        public string PreviousState { get; set; }
        public string PreviousActivityForDirect { get; set; }
        public string PreviousStateForDirect { get; set; }
        public string PreviousActivityForReverse { get; set; }
        public string PreviousStateForReverse { get; set; }
        public bool IsDeterminingParametersChanged { get; set; }

        [ForeignKey("WorkflowDefinitionID")]
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }
    }
}
