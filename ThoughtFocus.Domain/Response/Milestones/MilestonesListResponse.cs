using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Milestones
{
    public class MilestonesListResponse:BaseResponse
    {
        public Milestones Milestones { get; set; }
        //public List<Milestones> MilestonesList { get; set; }
    }
    public class Milestones
    {
        //public int MilestoneID { get; set; }
        //public string MilestoneName { get; set; }
        //public string MilestoneDescription { get; set; }
        //public string MilestoneForm { get; set; }
        //public bool isPublished { get; set; }
        //public int CreatedBy { get; set; }
        //public DateTime CreatedDate { get; set; }
       public string MilestoneList { get; set; }
    }

    public class PublishedMilestonesListResponse : BaseResponse
    {
        public PublishedMilestonesList MilestonesByProgramTerm { get; set; }
    }
    public class PublishedMilestonesList
    {
        public string MilestonesByProgramTerm { get; set; }
    }
}
