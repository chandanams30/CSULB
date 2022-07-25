using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [Table("TransitionValidationDefination", Schema = "WorkFlow")]
    public class TransitionValidationDefination
    {
         [Key]
        public long TransitionValidationDefinationID { get; set; }

         public string TransitionValidationName { get; set; }

        public bool IsEnabled { get; set; }

        public long ValidationDefinationID { get; set; }

        public virtual ValidationDefination ValidationDefination { get; set; }

       
        public long TransitionDefinitionID { get; set; }

         [ForeignKey("TransitionDefinitionID")]
        public virtual TransitionDefinition TransitionDefinition { get; set; }


    }
}
