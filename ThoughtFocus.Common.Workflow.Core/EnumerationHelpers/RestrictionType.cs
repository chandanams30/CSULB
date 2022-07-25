using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThoughtFocus.Common.Workflow.Core.Model;

namespace ThoughtFocus.Common.Workflow.Core.EnumerationHelpers
{
    [Table("RestrictionType", Schema = "WorkFlow")]
    public class RestrictionType
    {
         public RestrictionType(RestrictionTypeEnumeration restrictionTypeEnumeration)
        {
            Id = (int)restrictionTypeEnumeration;
            Name = restrictionTypeEnumeration.ToString();
            Description = restrictionTypeEnumeration.GetEnumDescription();
        }

         public RestrictionType() { } //For EF

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public static implicit operator RestrictionType(RestrictionTypeEnumeration restrictionTypeEnumeration)
        {
            return new RestrictionType(restrictionTypeEnumeration);
        }

        public static implicit operator RestrictionTypeEnumeration(RestrictionType restrictionType)
        {
            return (RestrictionTypeEnumeration)restrictionType.Id;
        }
    }
}
