using System.Collections.Generic;
using ThoughtFocus.Domain.CustomView;
using ThoughtFocus.Domain.Response;

namespace ThoughtFocus.Domain.CustomView
{
    public class WorkFlowCommandResponse:BaseResponse
    {
        public List<WorkflowCommandViewEntity> WorkFlowCommands { get; set; }
    }
}
