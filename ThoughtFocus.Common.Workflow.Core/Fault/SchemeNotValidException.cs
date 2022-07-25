using System;

namespace ThoughtFocus.Common.Workflow.Core.Fault
{
    public class WorkflowNotValidException : Exception
    {
        public WorkflowNotValidException(string msg) : base(msg) { }
    }
}
