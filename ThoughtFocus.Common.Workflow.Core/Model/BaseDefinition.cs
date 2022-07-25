using System.ComponentModel.DataAnnotations;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    public abstract class BaseDefinition
    {
        [Key]
        public long ID { get; set; }
        public virtual string Name { get; set; }   
        protected  BaseDefinition(){}
    }
}
