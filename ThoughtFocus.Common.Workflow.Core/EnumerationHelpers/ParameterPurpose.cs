using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThoughtFocus.Common.Workflow.Core.Model;

namespace ThoughtFocus.Common.Workflow.Core.EnumerationHelpers
{
    [Table("ParameterPurpose", Schema = "WorkFlow")]
    public class ParameterPurpose
    {
          public ParameterPurpose(ParameterPurposeEnumeration parameterPurposeEnumeration)
        {
            Id = (int)parameterPurposeEnumeration;
            Name = parameterPurposeEnumeration.ToString();
            Description = parameterPurposeEnumeration.GetEnumDescription();
        }

          public ParameterPurpose() { } //For EF

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        public static implicit operator ParameterPurpose(ParameterPurposeEnumeration parameterPurposeEnumeration)
        {
            return new ParameterPurpose(parameterPurposeEnumeration);
        }

        public static implicit operator ParameterPurposeEnumeration(ParameterPurpose parameterPurpose)
        {
            return (ParameterPurposeEnumeration)parameterPurpose.Id;
        }
    }
    
}
