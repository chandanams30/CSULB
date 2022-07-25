using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.Common.WorkFlowDataAccess.Models
{
    public partial class LocalizeDefinition
    {
        public long Id { get; set; }
        public int LocalizeTypeId { get; set; }
        public bool IsDefault { get; set; }
        public string ObjectName { get; set; }
        public string Culture { get; set; }
        public string Value { get; set; }
        public long WorkflowDefinitionId { get; set; }

        public virtual LocalizeType LocalizeType { get; set; }
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }
    }
}
