using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class ActionDefinitionForActivity
    {
        public long Id { get; set; }
        public long? ActionDefinitionId { get; set; }
        public long? ActivityDefinitionId { get; set; }
        public bool IsPostExecution { get; set; }
        public int Order { get; set; }

        public virtual ActionDefinition ActionDefinition { get; set; }
        public virtual ActivityDefinition ActivityDefinition { get; set; }
    }
}
