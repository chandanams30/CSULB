using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class ParameterPurpose
    {
        public ParameterPurpose()
        {
            ParameterDefinitions = new HashSet<ParameterDefinition>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ParameterDefinition> ParameterDefinitions { get; set; }
    }
}
