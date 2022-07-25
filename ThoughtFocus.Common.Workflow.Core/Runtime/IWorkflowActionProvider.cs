using ThoughtFocus.Common.Workflow.Core.Model;
using System.Collections.Generic;

namespace ThoughtFocus.Common.Workflow.Core.Runtime
{
    public interface IWorkflowActionProvider
    {                       
        void ExecuteAction(string name, ProcessInstance processInstance, string actionParameter);
        bool ExecuteCondition(string name, ProcessInstance processInstance, string actionParameter);
        List<string> GetActions();
    }
}
