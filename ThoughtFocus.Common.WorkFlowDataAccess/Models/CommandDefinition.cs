using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.Common.WorkFlowDataAccess.Models
{
    public partial class CommandDefinition
    {
        public CommandDefinition()
        {
            TriggerDefinitions = new HashSet<TriggerDefinition>();
        }

        public long Id { get; set; }
        public string CommandIconClass { get; set; }
        public long WorkflowDefinitionId { get; set; }
        public string Name { get; set; }

        public virtual WorkflowDefinition WorkflowDefinition { get; set; }
        public virtual ICollection<TriggerDefinition> TriggerDefinitions { get; set; }
    }
}
