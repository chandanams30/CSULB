using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class ActorDefinitionExecuteRule
    {
        public ActorDefinitionExecuteRule()
        {
            RestrictionDefinitions = new HashSet<RestrictionDefinition>();
        }

        public long Id { get; set; }
        public string RuleName { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Name { get; set; }

        public virtual WorkflowDefinition WorkflowDefinition { get; set; }
        public virtual ICollection<RestrictionDefinition> RestrictionDefinitions { get; set; }
    }
}
