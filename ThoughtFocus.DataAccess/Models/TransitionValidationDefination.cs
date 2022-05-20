using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class TransitionValidationDefination
    {
        public long TransitionValidationDefinationId { get; set; }
        public string TransitionValidationName { get; set; }
        public bool IsEnabled { get; set; }
        public long ValidationDefinationId { get; set; }
        public long TransitionDefinitionId { get; set; }

        public virtual TransitionDefinition TransitionDefinition { get; set; }
        public virtual ValidationDefination ValidationDefination { get; set; }
    }
}
