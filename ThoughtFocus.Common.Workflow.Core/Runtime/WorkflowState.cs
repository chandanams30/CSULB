using System;
using System.Runtime.Serialization;

namespace ThoughtFocus.Common.Workflow.Core.Runtime
{

    [Serializable]
    [DataContract]
    public class WorkflowState
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string VisibleName { get; set; }
        [DataMember]
        public string WorkFlowName { get; set; }

        [DataMember]
        public long  WorkFlowID { get; set; }
    }
}
