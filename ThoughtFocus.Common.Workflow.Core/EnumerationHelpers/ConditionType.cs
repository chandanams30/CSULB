using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThoughtFocus.Common.Workflow.Core.Model;

namespace ThoughtFocus.Common.Workflow.Core.EnumerationHelpers
{
    [Table("ConditionType", Schema = "WorkFlow")]
     public class ConditionType
    {
        public ConditionType(ConditionTypeEnumeration conditionTypeEnumeration)
        {
            Id = (int)conditionTypeEnumeration;
            Name = conditionTypeEnumeration.ToString();
            Description = conditionTypeEnumeration.GetEnumDescription();
        }

         public ConditionType() { } //For EF

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public static implicit operator ConditionType(ConditionTypeEnumeration conditionTypeEnumeration)
        {
            return new ConditionType(conditionTypeEnumeration);
        }

        public static implicit operator ConditionTypeEnumeration(ConditionType conditionType)
        {
            return (ConditionTypeEnumeration)conditionType.Id;
        }
    }
}
