using System;
using System.ComponentModel.DataAnnotations.Schema;
using ThoughtFocus.Common.Workflow.Core.EnumerationHelpers;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [Table("ParameterDefinition", Schema = "WorkFlow")]
    public class ParameterDefinition : BaseDefinition
    {
        public string TypeAsString { get; internal set; }

         public int PurposeID { get; set; }

        public string SerializedDefaultValue { get; internal set; } 

        [ForeignKey("WorkflowDefinitionID")]
        public virtual WorkflowDefinition WorkflowDefinition { get; set; }
        public long WorkflowDefinitionID { get; set; }

        [NotMapped]
        public virtual Type Type { get { return Type.GetType(TypeAsString); } internal set { } }
      
        [ForeignKey("PurposeID")]
        public virtual ParameterPurpose Purpose { get; internal set; }   


        
        

        public static ParameterDefinition Create(string name, string type, string purpose, string serializedValue)
        {
            ParameterPurposeEnumeration parsedPurpose;
            Enum.TryParse(purpose, true, out parsedPurpose);
            return new ParameterDefinition
                       {
                           Name = name,
                           Type = Type.GetType(type),
                           Purpose = parsedPurpose,
                           SerializedDefaultValue = serializedValue
                       };
        }

        public static ParameterDefinition Create(string name, string type, string serializedValue)
        {
            return Create(name, type, ParameterPurposeEnumeration.Temporary.ToString("G"),serializedValue);
        }

        public static ParameterDefinitionForAction Create(ParameterDefinition parameterDefinition, string order)
        {
            var parsedOrder = int.Parse(order);
            return new ParameterDefinitionForAction {ParameterDefinition = parameterDefinition, Order = parsedOrder};
        }

        
        public static ParameterDefinitionWithValue Create(ParameterDefinition parameterDefinition, object value)
        {


            if (value != null && value.GetType().Namespace == "System.Data.Entity.DynamicProxies")
            {
                if (value != null && !value.GetType().BaseType.Equals(parameterDefinition.Type) && !parameterDefinition.Type.IsAssignableFrom(value.GetType().BaseType))
                    throw new InvalidOperationException();
                return new ParameterDefinitionWithValue { ParameterDefinition = parameterDefinition, Value = value };
            }

            else
            {
                if (value != null && !value.GetType().Equals(parameterDefinition.Type) && !parameterDefinition.Type.IsAssignableFrom(value.GetType()))
                    throw new InvalidOperationException();
                return new ParameterDefinitionWithValue { ParameterDefinition = parameterDefinition, Value = value };
            }
             

            
        }
           //  a= thisType.BaseType;
    }

}
