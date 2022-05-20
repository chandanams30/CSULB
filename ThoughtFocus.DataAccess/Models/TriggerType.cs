using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class TriggerType
    {
        public TriggerType()
        {
            TriggerDefinitions = new HashSet<TriggerDefinition>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TriggerDefinition> TriggerDefinitions { get; set; }
    }
}
