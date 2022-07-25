using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.Common.WorkFlowDataAccess.Models
{
    public partial class ActivityDefinition
    {
        public ActivityDefinition()
        {
            ActionDefinitionForActivities = new HashSet<ActionDefinitionForActivity>();
            TransitionDefinitionFroms = new HashSet<TransitionDefinition>();
            TransitionDefinitionTos = new HashSet<TransitionDefinition>();
        }

        public long Id { get; set; }
        public string State { get; set; }
        public bool IsInitial { get; set; }
        public bool IsFinal { get; set; }
        public bool IsForSetState { get; set; }
        public bool IsAutoSchemeUpdate { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Name { get; set; }

        public virtual WorkflowDefinition WorkflowDefinition { get; set; }
        public virtual ICollection<ActionDefinitionForActivity> ActionDefinitionForActivities { get; set; }
        public virtual ICollection<TransitionDefinition> TransitionDefinitionFroms { get; set; }
        public virtual ICollection<TransitionDefinition> TransitionDefinitionTos { get; set; }
    }
}
