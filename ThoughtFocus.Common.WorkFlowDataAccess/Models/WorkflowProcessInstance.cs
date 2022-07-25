using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.Common.WorkFlowDataAccess.Models
{
    public partial class WorkflowProcessInstance
    {
        public long WorkflowProcessInstanceId { get; set; }
        public long ProcessInstanceId { get; set; }
        public Guid SchemeId { get; set; }
        public string ActivityName { get; set; }
        public string StateName { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string PreviousActivity { get; set; }
        public string PreviousState { get; set; }
        public string PreviousActivityForDirect { get; set; }
        public string PreviousStateForDirect { get; set; }
        public string PreviousActivityForReverse { get; set; }
        public string PreviousStateForReverse { get; set; }
        public bool IsDeterminingParametersChanged { get; set; }

        public virtual WorkflowDefinition WorkflowDefinition { get; set; }
    }
}
