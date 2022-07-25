using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.Common.WorkFlowDataAccess.Models
{
    public partial class WorkflowProcessInstancePersistence
    {
        public Guid WorkflowProcessInstancePersistenceId { get; set; }
        public string ParameterName { get; set; }
        public long ProcessInstanceId { get; set; }
        public long WorkFlowDefinationId { get; set; }
        public string Value { get; set; }
    }
}
