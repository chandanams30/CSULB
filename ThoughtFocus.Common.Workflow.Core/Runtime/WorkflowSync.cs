using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ThoughtFocus.Common.Workflow.Core.Persistence;

namespace ThoughtFocus.Common.Workflow.Core.Runtime
{
    public sealed class WorkflowSync : IDisposable
    {
        private bool _isDisposed;

        private readonly WorkflowRuntime _runtime;

        private readonly long _processId;
        private readonly long _workflowID;

        private readonly AutoResetEvent _handle;

        private List<ProcessStatus> _statusesForWaiting;

        private bool _wasSet;

        public WorkflowSync(WorkflowRuntime runtime, long processId,long workflowID)
        {
            if (runtime == null) throw new ArgumentNullException("runtime");
            if (processId < 0) throw new ArgumentOutOfRangeException("processId");

            _isDisposed = false;
            _runtime = runtime;
            _processId = processId;
            _workflowID = workflowID;
            _handle = new AutoResetEvent(false);
        }

        public void StatrtWaitingFor(IEnumerable<ProcessStatus> statuses)
        {
            _handle.Reset();
            _wasSet = false;

            _statusesForWaiting = statuses.ToList();

            _runtime.ProcessStatusChanged += RuntimeProcessStatusChanged;

            if (!_wasSet)
            {
                var currentStatus = _runtime.GetProcessStatus(_processId, _workflowID);
                if (_statusesForWaiting.Contains(currentStatus))
                    _handle.Set();
            }
        }

        private void RuntimeProcessStatusChanged(object sender, ProcessStatusChangedEventArgs e)
        {
            if (_statusesForWaiting.Contains(e.NewStatus))
            {
                _handle.Set();
                _wasSet = true;
            }
        }

        public void Wait (TimeSpan timeout)
        {
            _handle.WaitOne(timeout);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _runtime.ProcessStatusChanged -= RuntimeProcessStatusChanged;
                _isDisposed = true;
            }
        }
    }
}
