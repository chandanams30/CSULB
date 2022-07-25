using System;
using System.Collections.Generic;
using ThoughtFocus.Common.Workflow.Core.Model;
using ThoughtFocus.Common.Workflow.Core.Persistence;

namespace ThoughtFocus.Common.Workflow.Core.Runtime
{
    public class ProcessStatusChangedEventArgs : EventArgs
    {
        public long ProcessId { get; private set; }
        public ProcessStatus OldStatus { get; private set; }
        public ProcessStatus NewStatus { get; private set; }
        public List<ParameterDefinitionWithValue> ProcessParameters { get; internal set; }
        public string ProcessName { get; internal set; }
        public ProcessInstance ProcessInstance { get; internal set; }
        public long workflowID { get; set; }

        public ProcessStatusChangedEventArgs (long processId, ProcessStatus oldStatus, ProcessStatus newStatus,long workFlowID,ProcessInstance ProcessInstance)
        {
            ProcessId = processId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            workflowID = workFlowID;
            this.ProcessInstance = ProcessInstance;
        }
    }
}
