using System;
using System.Collections.Generic;

namespace ThoughtFocus.Common.Workflow.Core.Runtime
{
    public interface IWorkflowRuleProvider
    {
        //bool CheckRule(long processId, Guid identityId, string ruleName, string parameter);

        bool CheckRule(long processId, Guid identityId, string ruleName, IDictionary<string, string> parameters);

        //IEnumerable<Guid> GetIdentitiesForRule(long processId, string ruleName, string parameter);

        IEnumerable<Guid> GetIdentitiesForRule(long processId, string ruleName, IDictionary<string, string> parameters);

        List<string> GetRules();
    }
}
