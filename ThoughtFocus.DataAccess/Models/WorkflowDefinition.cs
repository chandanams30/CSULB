using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class WorkflowDefinition
    {
        public WorkflowDefinition()
        {
            ActionDefinitions = new HashSet<ActionDefinition>();
            ActivityDefinitions = new HashSet<ActivityDefinition>();
            ActorDefinitionExecuteRules = new HashSet<ActorDefinitionExecuteRule>();
            ActorDefinitionIsIdentities = new HashSet<ActorDefinitionIsIdentity>();
            ActorDefinitionIsInRoles = new HashSet<ActorDefinitionIsInRole>();
            CommandDefinitions = new HashSet<CommandDefinition>();
            LocalizeDefinitions = new HashSet<LocalizeDefinition>();
            ParameterDefinitions = new HashSet<ParameterDefinition>();
            TransitionDefinitions = new HashSet<TransitionDefinition>();
            WorkflowProcessInstances = new HashSet<WorkflowProcessInstance>();
        }

        public long Id { get; set; }
        public string DesignerModel { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ActionDefinition> ActionDefinitions { get; set; }
        public virtual ICollection<ActivityDefinition> ActivityDefinitions { get; set; }
        public virtual ICollection<ActorDefinitionExecuteRule> ActorDefinitionExecuteRules { get; set; }
        public virtual ICollection<ActorDefinitionIsIdentity> ActorDefinitionIsIdentities { get; set; }
        public virtual ICollection<ActorDefinitionIsInRole> ActorDefinitionIsInRoles { get; set; }
        public virtual ICollection<CommandDefinition> CommandDefinitions { get; set; }
        public virtual ICollection<LocalizeDefinition> LocalizeDefinitions { get; set; }
        public virtual ICollection<ParameterDefinition> ParameterDefinitions { get; set; }
        public virtual ICollection<TransitionDefinition> TransitionDefinitions { get; set; }
        public virtual ICollection<WorkflowProcessInstance> WorkflowProcessInstances { get; set; }
    }
}
