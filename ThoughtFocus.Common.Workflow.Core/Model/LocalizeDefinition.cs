using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThoughtFocus.Common.Workflow.Core.EnumerationHelpers;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    public enum LocalizeTypeEnumeration
    {
        Command=1, State=2
    }
    [Table("LocalizeDefinition", Schema = "WorkFlow")]
    public class LocalizeDefinition
    {
        [Key]
        public  long ID { get;  set; }
        [NotMapped]
        public int TypeId { get; set; }
        public int LocalizeTypeID { get; set; }     

         public bool IsDefault { get;  set; }

        public string ObjectName { get;  set; }

        public string Culture { get;  set; }

        public string Value { get;  set; }  
         public long WorkflowDefinitionID { get; set; } 

        [ForeignKey("LocalizeTypeID")] 
        public virtual LocalizeType LocalizeType { get;  set; }    

       [ForeignKey("WorkflowDefinitionID")]        
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }

        public static LocalizeDefinition Create(string objectName, string type, string culture, string value, string isDefault)
        {
            LocalizeTypeEnumeration parsedType;
            Enum.TryParse(type, true, out parsedType);

            return new LocalizeDefinition
                       {
                           Culture = culture,
                           IsDefault = !string.IsNullOrEmpty(isDefault) && bool.Parse(isDefault),
                           ObjectName = objectName,
                           LocalizeType = parsedType,
                           Value = value
                       };

        }
    }
}
