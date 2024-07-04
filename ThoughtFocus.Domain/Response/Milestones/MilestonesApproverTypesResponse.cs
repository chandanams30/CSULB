using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Milestones
{
    public class MilestonesApproverTypesResponse:BaseResponse
    {
        public List<MilestonesApproverTypes> ApproverTypes { get; set; }
    }

    public class MilestonesApproverTypes
    {
        public int ApproverTypeID { get; set; }
        public string Name { get; set; }
    }
}
