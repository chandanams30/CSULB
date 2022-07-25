using System;
using System.ComponentModel.DataAnnotations.Schema;
using ThoughtFocus.Common.Workflow.Core.EnumerationHelpers;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [NotMapped]
    public sealed class ParameterDefinitionWithValue : ParameterDefinition
    {
        public override string Name
        {
            get
            {
                return ParameterDefinition.Name;
            }
            set
            {
                ParameterDefinition.Name = value;
            }
        }

        public override ParameterPurpose Purpose
        {
            get
            {
                return ParameterDefinition.Purpose;
            }
            internal set
            {
                ParameterDefinition.Purpose = value;
            }
        }

        public override Type Type
        {
            get
            {
                return ParameterDefinition.Type;
            }
            internal set
            {
                ParameterDefinition.Type = value;
            }
        }

        internal ParameterDefinition ParameterDefinition { private get;  set; }
        public object Value { get; internal set; }
    }
}
