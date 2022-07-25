using System;

namespace ThoughtFocus.Common.Workflow.Core.Runtime
{
    public class SchemaWasChangedEventArgs : EventArgs
    {
        public long ProcessId { get; set; }
        public Guid SchemeId { get; set; }
        public bool SchemaWasObsolete { get; set; }
        public bool DeterminingParametersWasChanged { get; set; }
    }
}
