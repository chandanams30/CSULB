using System;

namespace ThoughtFocus.Common.Workflow.Core.Bus
{
    public sealed class ExecutionResponseEventArgs : EventArgs
    {
        public ExecutionResponseParameters Parameters { get; private set; }

        public ExecutionResponseEventArgs (ExecutionResponseParameters parameters)
        {
            Parameters = parameters;
        }
    }
}
