using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class TransitionDefinition
    {
        public TransitionDefinition()
        {
            RestrictionDefinitions = new HashSet<RestrictionDefinition>();
            TransitionValidationDefinations = new HashSet<TransitionValidationDefination>();
        }

        public long Id { get; set; }
        public long ConditionId { get; set; }
        public int TransitionClassifierId { get; set; }
        public long TriggerId { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public long? FromId { get; set; }
        public long? ToId { get; set; }
        public string Name { get; set; }

        public virtual ConditionDefinition Condition { get; set; }
        public virtual ActivityDefinition From { get; set; }
        public virtual ActivityDefinition To { get; set; }
        public virtual TransitionClassifier TransitionClassifier { get; set; }
        public virtual TriggerDefinition Trigger { get; set; }
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }
        public virtual ICollection<RestrictionDefinition> RestrictionDefinitions { get; set; }
        public virtual ICollection<TransitionValidationDefination> TransitionValidationDefinations { get; set; }
    }
}
