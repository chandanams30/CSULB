using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThoughtFocus.Common.Workflow.Core.Model;
namespace ThoughtFocus.Common.Workflow.Core.EnumerationHelpers
{

    [Table("TransitionClassifier", Schema = "WorkFlow")]
    public class TransitionClassifier 
    {
        public TransitionClassifier(TransitionClassifierEnumeration transitionClassifierEnumeration)
        {
            Id = (int)transitionClassifierEnumeration;
            Name = transitionClassifierEnumeration.ToString();
            Description = transitionClassifierEnumeration.GetEnumDescription();
        }

        public TransitionClassifier() { } //For EF

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public static implicit operator TransitionClassifier(TransitionClassifierEnumeration transitionClassifierEnumeration)
        {
            return new TransitionClassifier(transitionClassifierEnumeration);
        }

        public static implicit operator TransitionClassifierEnumeration(TransitionClassifier transitionClassifier)
        {
            return (TransitionClassifierEnumeration)transitionClassifier.Id;
        }
    }
}
