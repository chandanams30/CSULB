using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class ActionDefinition
    {
        public ActionDefinition()
        {
            ActionDefinitionForActivities = new HashSet<ActionDefinitionForActivity>();
            ConditionDefinitions = new HashSet<ConditionDefinition>();
            ParameterDefinitionForActions = new HashSet<ParameterDefinitionForAction>();
        }

        public long Id { get; set; }
        public string TypeAsString { get; set; }
        public string FullTypeName { get; set; }
        public string MethodName { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Name { get; set; }

        public virtual WorkflowDefinition WorkflowDefinition { get; set; }
        public virtual ICollection<ActionDefinitionForActivity> ActionDefinitionForActivities { get; set; }
        public virtual ICollection<ConditionDefinition> ConditionDefinitions { get; set; }
        public virtual ICollection<ParameterDefinitionForAction> ParameterDefinitionForActions { get; set; }
    }
}
