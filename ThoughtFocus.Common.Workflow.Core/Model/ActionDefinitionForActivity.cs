using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [Table("ActionDefinitionForActivity", Schema = "WorkFlow")]
    public class ActionDefinitionForActivity
    {
        [Key]
        public long ID { get; set; }
        public virtual ActionDefinition ActionDefinition { get; set; }
        public virtual ActivityDefinition ActivityDefinition { get; set; }
        public bool IsPostExecution { get; set; } 
        [NotMapped]
        public long ActivityDefinition_ID { get; set; }
        [NotMapped]
        public long ActionDefinition_ID { get; set; }
        public int Order { get; set; }
    }
}
