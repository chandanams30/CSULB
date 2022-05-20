using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class TransitionClassifier
    {
        public TransitionClassifier()
        {
            TransitionDefinitions = new HashSet<TransitionDefinition>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<TransitionDefinition> TransitionDefinitions { get; set; }
    }
}
