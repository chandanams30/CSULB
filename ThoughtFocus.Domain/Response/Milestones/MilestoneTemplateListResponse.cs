using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Milestones
{
    public class MilestoneTemplateListResponse:BaseResponse
    {
        public MilestoneTemplateList MilestoneTemplateList { get; set; }
    }

    public class MilestoneTemplateList
    {
        public string MilestoneList { get; set; }
    }
}
