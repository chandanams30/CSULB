using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class ConditionDefinition
    {
        public ConditionDefinition()
        {
            TransitionDefinitions = new HashSet<TransitionDefinition>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int ConditionTypeId { get; set; }
        public bool? ResultOnPreExecution { get; set; }
        public long? ActionId { get; set; }

        public virtual ActionDefinition Action { get; set; }
        public virtual ConditionType ConditionType { get; set; }
        public virtual ICollection<TransitionDefinition> TransitionDefinitions { get; set; }
    }
}
