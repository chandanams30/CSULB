using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.Common.WorkFlowDataAccess.Models
{
    public partial class LocalizeType
    {
        public LocalizeType()
        {
            LocalizeDefinitions = new HashSet<LocalizeDefinition>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<LocalizeDefinition> LocalizeDefinitions { get; set; }
    }
}
