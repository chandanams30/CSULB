using System;
using System.Collections.Generic;
using System.Linq;
using ThoughtFocus.Common.Workflow.Core.Runtime;

namespace ThoughtFocus.Workflow
{
    public class WorkflowRule : IWorkflowRuleProvider
    {
        //private ISiteVisitMemberInfoRepository _siteVisitMemberInfoRepository;

        private class RuleFunction
        {
            public Func<long, string, IDictionary<string, string>, IEnumerable<string>> GetFunction { get; set; }
            public Func<long, Guid, IDictionary<string, string>, bool> CheckFunction { get; set; }
        }

        private Dictionary<string, RuleFunction> _funcs =
            new Dictionary<string, RuleFunction>();

        public bool CheckRule(long processId, Guid identityId, string ruleName, IDictionary<string, string> parameters)
        {
            return _funcs.ContainsKey(ruleName) && _funcs[ruleName].CheckFunction.Invoke(processId, identityId, parameters);
        }

        public IEnumerable<Guid> GetIdentitiesForRule(long processId, string ruleName, IDictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        public List<string> GetRules()
        {
            return _funcs.Select(c => c.Key).ToList();
        }
    }
}