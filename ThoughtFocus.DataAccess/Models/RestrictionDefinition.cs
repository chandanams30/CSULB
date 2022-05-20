using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class RestrictionDefinition
    {
        public long Id { get; set; }
        public int? RestrictionTypeId { get; set; }
        public long? TransitionId { get; set; }
        public long? ActorDefinitionExecuteRuleId { get; set; }
        public long? ActorDefinitionIsIdentityId { get; set; }
        public long? ActorDefinitionIsInRoleId { get; set; }

        public virtual ActorDefinitionExecuteRule ActorDefinitionExecuteRule { get; set; }
        public virtual ActorDefinitionIsIdentity ActorDefinitionIsIdentity { get; set; }
        public virtual ActorDefinitionIsInRole ActorDefinitionIsInRole { get; set; }
        public virtual RestrictionType RestrictionType { get; set; }
        public virtual TransitionDefinition Transition { get; set; }
    }
}
