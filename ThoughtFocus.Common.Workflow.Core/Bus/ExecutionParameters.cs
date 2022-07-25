using System;
using System.Collections.Generic;
using System.Linq;
using ThoughtFocus.Common.Workflow.Core.Model;

namespace ThoughtFocus.Common.Workflow.Core.Bus
{
    [Serializable]
    public sealed class ExecutionParameters
    {
        [Serializable]
        public class MethodToExecuteParameterInfo
        {
            public int Order { get; internal set; }
            public object Value { get; internal set; }
            public Type Type { get; internal set; }
        }

        [Serializable]
        public  class MethodToExecuteInfo
        {
            public int Order { get; internal set; }
            public string Type { get; internal set; }
            public string MethodName { get; internal set; }
            public IEnumerable<MethodToExecuteParameterInfo> InputParameters { get; internal set; }
            public IEnumerable<MethodToExecuteParameterInfo> OutputParameters { get; internal set; }
        }

        public long ProcessId { get; internal set; }

        public IEnumerable<MethodToExecuteInfo> Methods { get; internal set; }
        
        
        public static ExecutionParameters Create (long processId, IDictionary<string,object> parameters, ActivityDefinition activityToExecute)
        {
            if (activityToExecute.Implementation.Count < 1)
                throw new InvalidOperationException();

            var methods = new List<MethodToExecuteInfo>(activityToExecute.Implementation.Count);
            var executionParameters = new ExecutionParameters
                                          {
                                     ProcessId = processId,
                                     Methods = methods,
                                     
                                 };

            foreach (var action in activityToExecute.Implementation)
            {
                var inputParameters = new List<MethodToExecuteParameterInfo>(action.ActionDefinition.InputParameters.Count());
                var outputParameters = new List<MethodToExecuteParameterInfo>(action.ActionDefinition.OutputParameters.Count());
                var method = new MethodToExecuteInfo
                                 {
                                     Type = action.ActionDefinition.Type.AssemblyQualifiedName,
                                     MethodName = action.ActionDefinition.MethodName,
                                     InputParameters = inputParameters,
                                     OutputParameters = outputParameters,
                                     Order = action.Order
                                 };

                inputParameters.AddRange(
                    action.ActionDefinition.InputParameters.Select(
                        inParameter =>
                        new MethodToExecuteParameterInfo
                            {
                                Order = inParameter.Order,
                                Value = parameters[inParameter.ParameterDefinition.Name],
                                Type = inParameter.ParameterDefinition.Type
                            }));

                outputParameters.AddRange(
                    action.ActionDefinition.OutputParameters.Select(
                        outParameter =>
                        new MethodToExecuteParameterInfo
                            {
                                Order = outParameter.Order,
                                Value =
                                    parameters.ContainsKey(outParameter.ParameterDefinition.Name)
                                        ? parameters[outParameter.ParameterDefinition.Name]
                                        : null,
                                Type = outParameter.ParameterDefinition.Type
                            }));

                methods.Add(method);
            }

            return executionParameters;
        }
    }
}
