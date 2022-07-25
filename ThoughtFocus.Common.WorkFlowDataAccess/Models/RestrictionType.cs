using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.Common.WorkFlowDataAccess.Models
{
    public partial class RestrictionType
    {
        public RestrictionType()
        {
            RestrictionDefinitions = new HashSet<RestrictionDefinition>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RestrictionDefinition> RestrictionDefinitions { get; set; }
    }
}
