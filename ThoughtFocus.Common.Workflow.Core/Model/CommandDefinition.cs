using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [Table("CommandDefinition", Schema = "WorkFlow")]
    public class CommandDefinition : BaseDefinition
    {

        [Key]
        public long ID { get; set; }

        public string CommandIconClass { get; set; }
        public long WorkflowDefinitionID { get; set; }
        
        [NotMapped]
        public Dictionary<string, ParameterDefinition> InputParameters { get; internal set; }

        public static CommandDefinition Create(string name)
        {
            return new CommandDefinition()
                       {Name = name, InputParameters = new Dictionary<string, ParameterDefinition>()};
        }

       
        [ForeignKey("WorkflowDefinitionID")]
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }

       
        

        public void AddParameterRef(string name, ParameterDefinition parameter)
        {
            InputParameters.Add(name,parameter);
        }
    }

}
