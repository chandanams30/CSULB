using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class ParameterDefinition
    {
        public ParameterDefinition()
        {
            ParameterDefinitionForActions = new HashSet<ParameterDefinitionForAction>();
        }

        public long Id { get; set; }
        public string TypeAsString { get; set; }
        public int PurposeId { get; set; }
        public string SerializedDefaultValue { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Name { get; set; }

        public virtual ParameterPurpose Purpose { get; set; }
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }
        public virtual ICollection<ParameterDefinitionForAction> ParameterDefinitionForActions { get; set; }
    }
}
