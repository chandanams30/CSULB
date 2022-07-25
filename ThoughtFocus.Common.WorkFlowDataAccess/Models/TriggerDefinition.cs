using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.Common.WorkFlowDataAccess.Models
{
    public partial class TriggerDefinition
    {
        public TriggerDefinition()
        {
            TransitionDefinitions = new HashSet<TransitionDefinition>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? CommandId { get; set; }
        public int? TypeId { get; set; }

        public virtual CommandDefinition Command { get; set; }
        public virtual TriggerType Type { get; set; }
        public virtual ICollection<TransitionDefinition> TransitionDefinitions { get; set; }
    }
}
