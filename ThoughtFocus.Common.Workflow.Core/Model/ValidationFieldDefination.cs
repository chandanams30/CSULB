using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [Table("ValidationFieldDefination", Schema = "WorkFlow")]
    public class ValidationFieldDefination
    {
        [Key]
        public long ValidationFieldDefinationID { get; set; }
        public string ValidationFieldDefinationName { get; set; }
        public string ValidationFieldValue { get; set; }
        public long ValidationDefinationID { get; set; }

        public virtual ValidationDefination ValidationDefination { get; set; }
    }
}