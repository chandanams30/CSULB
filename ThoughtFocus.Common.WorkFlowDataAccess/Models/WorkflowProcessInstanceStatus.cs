using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.Common.WorkFlowDataAccess.Models
{
    public partial class WorkflowProcessInstanceStatus
    {
        public long WorkflowProcessInstanceStatusId { get; set; }
        public Guid Lock { get; set; }
        public int Status { get; set; }
        public long ProcessInstanceId { get; set; }
        public long WorkFlowDefinationId { get; set; }
    }
}
