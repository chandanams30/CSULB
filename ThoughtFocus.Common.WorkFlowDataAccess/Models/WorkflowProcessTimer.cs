using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.Common.WorkFlowDataAccess.Models
{
    public partial class WorkflowProcessTimer
    {
        public Guid WorkflowProcessTimerId { get; set; }
        public string Name { get; set; }
        public DateTime NextExecutionDateTime { get; set; }
        public long ProcessInstanceId { get; set; }
        public long WorkFlowDefinationId { get; set; }
        public bool Ignore { get; set; }
    }
}
