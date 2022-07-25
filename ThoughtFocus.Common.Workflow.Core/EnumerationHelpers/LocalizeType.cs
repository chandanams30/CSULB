using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThoughtFocus.Common.Workflow.Core.Model;

namespace ThoughtFocus.Common.Workflow.Core.EnumerationHelpers
{
    [Table("LocalizeType", Schema = "WorkFlow")]
    public class LocalizeType
    {
        [Key]
        public int Id { get; set; }
        public LocalizeType(LocalizeTypeEnumeration localizeTypeEnumeration)
        {
            Id = (int)localizeTypeEnumeration;
            Name = localizeTypeEnumeration.ToString();
            Description = localizeTypeEnumeration.GetEnumDescription();
        }

         public LocalizeType() { } //For EF

        

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public static implicit operator LocalizeType(LocalizeTypeEnumeration localizeTypeEnumeration)
        {
            return new LocalizeType(localizeTypeEnumeration);
        }

        public static implicit operator LocalizeTypeEnumeration(LocalizeType localizeType)
        {
            return (LocalizeTypeEnumeration)localizeType.Id;
        }
    }
}
