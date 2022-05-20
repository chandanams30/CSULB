using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class ValidationType
    {
        public ValidationType()
        {
            ValidationDefinations = new HashSet<ValidationDefination>();
        }

        public long ValidationTypeId { get; set; }
        public string ValidationTypeName { get; set; }
        public string ValidationTypeDescription { get; set; }

        public virtual ICollection<ValidationDefination> ValidationDefinations { get; set; }
    }
}
