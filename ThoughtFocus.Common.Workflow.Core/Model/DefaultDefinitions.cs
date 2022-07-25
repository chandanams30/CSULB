using System;
using System.Collections.Generic;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    public static class DefaultDefinitions
    {
        public static readonly ParameterDefinition ParameterProcessId = new ParameterDefinition() { Name = "ProcessId", TypeAsString = (typeof(long)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterPreviousState = new ParameterDefinition() { Name = "PreviousState", TypeAsString = (typeof(string)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterCurrentState = new ParameterDefinition() { Name = "CurrentState", TypeAsString = (typeof(string)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterPreviousStateForDirect = new ParameterDefinition() { Name = "PreviousStateForDirect", TypeAsString = (typeof(string)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterPreviousStateForReverse = new ParameterDefinition() { Name = "PreviousStateForReverse", TypeAsString = (typeof(string)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterPreviousActivity = new ParameterDefinition() { Name = "PreviousActivity", TypeAsString = (typeof(string)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterCurrentActivity = new ParameterDefinition() { Name = "CurrentActivity", TypeAsString = (typeof(string)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterPreviousActivityForDirect = new ParameterDefinition() { Name = "PreviousActivityForDirect", TypeAsString = (typeof(string)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterPreviousActivityForReverse = new ParameterDefinition() { Name = "PreviousActivityForReverse", TypeAsString = (typeof(string)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterCurrentCommand = new ParameterDefinition() { Name = "CurrentCommand", TypeAsString = (typeof(string)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterExecutedActivityState = new ParameterDefinition() { Name = "ExecutedActivityState", TypeAsString = (typeof(string)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterConditionResult = new ParameterDefinition() { Name = "ConditionResult", TypeAsString = (typeof(ConditionResult)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterIdentityId = new ParameterDefinition() { Name = "IdentityId", TypeAsString = (typeof(Guid)).ToString(), Purpose = ParameterPurposeEnumeration.Persistence };
        public static readonly ParameterDefinition ParameterImpersonatedIdentityId = new ParameterDefinition() { Name = "ImpersonatedIdentityId", TypeAsString = (typeof(Guid)).ToString(), Purpose = ParameterPurposeEnumeration.Persistence };
        public static readonly ParameterDefinition ParameterSchemeId = new ParameterDefinition() { Name = "SchemeId", TypeAsString = (typeof(Guid)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterProcessName = new ParameterDefinition() { Name = "ProcessName", TypeAsString = (typeof(string)).ToString(), Purpose = ParameterPurposeEnumeration.System };
        public static readonly ParameterDefinition ParameterProcessInstance = new ParameterDefinition() { Name = "ProcessInstance", TypeAsString = "ThoughtFocus.Common.Workflow.Core.Model.ProcessInstance,ThoughtFocus.Common.Workflow.Core", Purpose = ParameterPurposeEnumeration.System };

        public static readonly ParameterDefinition ParameterIdentityIds = new ParameterDefinition()
                                                                     {
                                                                         Name = "IdentityIds",
                                                                         TypeAsString = (typeof(IEnumerable<Guid>)).ToString(),
                                                                         Purpose = ParameterPurposeEnumeration.System
                                                                     };
        public static CommandDefinition CommandAuto = new CommandDefinition() { Name = "Auto" };
        public static CommandDefinition CommandSetState = new CommandDefinition() { Name = "SetState" };


        public static readonly IEnumerable<ParameterDefinition> DefaultParameters = new List<ParameterDefinition>()
                                                                                        {
                                                                                            ParameterProcessId,
                                                                                            ParameterPreviousState,
                                                                                            ParameterCurrentState,
                                                                                            ParameterPreviousStateForDirect,
                                                                                            ParameterPreviousStateForReverse,
                                                                                            ParameterPreviousActivity,
                                                                                            ParameterPreviousActivityForDirect,
                                                                                            ParameterPreviousActivityForReverse,
                                                                                            ParameterCurrentCommand,
                                                                                            ParameterConditionResult,
                                                                                            ParameterIdentityId,
                                                                                            ParameterImpersonatedIdentityId,
                                                                                            ParameterExecutedActivityState,
                                                                                            ParameterCurrentActivity,
                                                                                            ParameterSchemeId,
                                                                                            ParameterIdentityIds,
                                                                                            ParameterProcessName
                                                                                        };
    }
}
