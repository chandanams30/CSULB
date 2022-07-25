using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [Table("ParameterDefinitionForAction", Schema = "WorkFlow")]
    public class ParameterDefinitionForAction
    {
        [Key]
        public long ID { get; set; }
        public bool IsInputParameter { get; set;}
        public virtual ParameterDefinition ParameterDefinition { get; set; }
        
        public virtual ActionDefinition ActionDefinition { get;  set; }
        public int Order { get; internal set; }
    }
}
