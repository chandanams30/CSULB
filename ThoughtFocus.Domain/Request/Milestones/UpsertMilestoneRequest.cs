using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response;

namespace ThoughtFocus.Domain.Request.Milestones
{
    public class UpsertMilestoneRequest
    {
        public int MilestoneID { get; set; }
        public string MilestoneName { get; set; }
        public string MilestoneDescription { get; set; }
        public string MilestoneForm { get; set; }
        public bool isPublished { get; set; }
        public bool isMandatory { get; set; }
        public int createdByUserID { get; set; }
        public List<MileStoneApprovers> MileStoneApprovers { get; set; }
    }
    public class MileStoneApprovers
    {
        public int ApproverUserID { get; set; }
        public int Sequence { get; set; }
    }
    public class AssignMilestoneToProgramRequest
    {
        public List<AssignMilestoneToProgram> AssignMilestones { get; set; }
    }
    public class AssignMilestoneToProgram
    {
        public int ProgramID { get; set; }
        public int MilestoneID { get; set; }
        public string TermCode { get; set; }
    }
}
