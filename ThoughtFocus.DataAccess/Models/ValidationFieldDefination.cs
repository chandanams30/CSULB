using System;
using System.Collections.Generic;

#nullable disable

namespace ThoughtFocus.DataAccess.Models
{
    public partial class ValidationFieldDefination
    {
        public long ValidationFieldDefinationId { get; set; }
        public string ValidationFieldDefinationName { get; set; }
        public string ValidationFieldValue { get; set; }
        public long ValidationDefinationId { get; set; }

        public virtual ValidationDefination ValidationDefination { get; set; }
    }
}
