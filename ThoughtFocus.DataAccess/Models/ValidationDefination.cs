using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class ValidationDefination
    {
        public ValidationDefination()
        {
            TransitionValidationDefinations = new HashSet<TransitionValidationDefination>();
            ValidationFieldDefinations = new HashSet<ValidationFieldDefination>();
        }

        public long ValidationDefinationId { get; set; }
        public string ValidationDefinationName { get; set; }
        public string ValidationDefinationDescription { get; set; }
        public bool IsEnabled { get; set; }
        public string ValidationDefinationErrorMessage { get; set; }
        public long ValidationTypeId { get; set; }

        public virtual ValidationType ValidationType { get; set; }
        public virtual ICollection<TransitionValidationDefination> TransitionValidationDefinations { get; set; }
        public virtual ICollection<ValidationFieldDefination> ValidationFieldDefinations { get; set; }
    }
}
