using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThoughtFocus.Common.Workflow.Core.Model;

namespace ThoughtFocus.Common.Workflow.Core.EnumerationHelpers
{
    [Table("TriggerType", Schema = "WorkFlow")]
    public class TriggerType
    {
        public TriggerType(TriggerTypeEnumeration triggerTypeEnumeration)
        {
            Id = (int)triggerTypeEnumeration;
            Name = triggerTypeEnumeration.ToString();
            Description = triggerTypeEnumeration.GetEnumDescription();
        }

        public TriggerType() { } //For EF

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public String Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public static implicit operator TriggerType(TriggerTypeEnumeration triggerTypeEnumeration)
        {
            return new TriggerType(triggerTypeEnumeration);
        }

        public static implicit operator TriggerTypeEnumeration(TriggerType triggerType)
        {
            return (TriggerTypeEnumeration)triggerType.Id;
        }
    }
}
