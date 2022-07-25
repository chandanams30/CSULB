using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.Common.WorkFlowDataAccess.Models
{
    public partial class ParameterDefinitionForAction
    {
        public long Id { get; set; }
        public bool IsInputParameter { get; set; }
        public long? ParameterDefinitionId { get; set; }
        public int Order { get; set; }
        public long? ActionDefinitionId { get; set; }

        public virtual ActionDefinition ActionDefinition { get; set; }
        public virtual ParameterDefinition ParameterDefinition { get; set; }
    }
}
