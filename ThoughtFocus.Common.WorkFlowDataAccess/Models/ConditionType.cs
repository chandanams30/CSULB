using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.Common.WorkFlowDataAccess.Models
{
    public partial class ConditionType
    {
        public ConditionType()
        {
            ConditionDefinitions = new HashSet<ConditionDefinition>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ConditionDefinition> ConditionDefinitions { get; set; }
    }
}
