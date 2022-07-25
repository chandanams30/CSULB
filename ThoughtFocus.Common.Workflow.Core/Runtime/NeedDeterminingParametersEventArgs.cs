using System;
using System.Collections.Generic;

namespace ThoughtFocus.Common.Workflow.Core.Runtime
{
    public class NeedDeterminingParametersEventArgs : EventArgs
    {
        public long ProcessId { get; set; }
        public IDictionary<string, IEnumerable<object>> DeterminingParameters { get; set; }
    }
}
