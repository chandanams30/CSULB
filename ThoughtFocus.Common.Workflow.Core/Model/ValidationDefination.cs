using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [Table("ValidationDefination", Schema = "WorkFlow")]
    public class ValidationDefination
    {
        [Key]
        public long ValidationDefinationID { get; set; }

        public string ValidationDefinationName { get; set; }

        public string ValidationDefinationDescription { get; set; }

        public bool IsEnabled { get; set; }

        public string ValidationDefinationErrorMessage { get; set; }

        public long ValidationTypeID { get; set; }

        public virtual ValidationType ValidationType { get; set; }

        public virtual ICollection<ValidationFieldDefination> ValidationFieldDefinations { get; set; }

       
    }
}
