using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [Table("ValidationType", Schema = "WorkFlow")]
    public class ValidationType
    {
        [Key]
         public long ValidationTypeID { get; set; }
        public string ValidationTypeName { get; set; }
        public string ValidationTypeDescription { get; set; }
    }
}
