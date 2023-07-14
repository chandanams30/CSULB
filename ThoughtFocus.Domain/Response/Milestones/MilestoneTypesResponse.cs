using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Milestones
{
    public class MilestoneTypesResponse:BaseResponse
    {
        public List<MilestoneTypes> MilestoneTypes { get; set; }
    }
    public class MilestoneTypes
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
