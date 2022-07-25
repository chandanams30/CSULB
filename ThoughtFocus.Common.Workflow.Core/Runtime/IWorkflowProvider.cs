using ThoughtFocus.Common.Workflow.Core.Model;
using System.Collections.Generic;

namespace ThoughtFocus.Common.Workflow.Core.Runtime
{
    public interface IWorkflowProvider
    {
        ProcessInstance CreateNewWorkflow(long processId,
                                        string workflowName,
                                        IDictionary<string, IEnumerable<object>> parameters);

        ProcessInstance CreateNewWorkflow(long processId,
                                        long workFlowID, IDictionary<string, IEnumerable<object>> parameters);

        ProcessInstance CreateNewWorkflowScheme(long processId,
                                               string workflowName,
                                               IDictionary<string, IEnumerable<object>> parameters);

        ProcessInstance GetWorkflowInstance(long processId,long workFlowDefinationID);

        WorkflowDefinition GetWorkflowDefinition(long workflowId);

        WorkflowDefinition GetWorkflowDefinition(string workflowName);

        WorkflowDefinition GetWorkflowDefinition(string workflowName, IDictionary<string, IEnumerable<object>> parameters);

        WorkflowDefinition GetWorkflowDefinition(long workFlowID, IDictionary<string, IEnumerable<object>> parameters);
    }
}
