using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Milestones
{
    public class MilestoneUsersListResponse:BaseResponse
    {
        public string MilestoneUsersList { get; set; }
    }
    
    public class MilestoneProgramTermListResponse : BaseResponse
    {
        public string MilestoneProgramTermList { get; set; }
    }

    public class MilestonePublishedFormsListResponse : BaseResponse
    {
        public string MilestonePublishedFormsList { get; set; }
    }
    public class StudentMilestoneListResponse : BaseResponse
    {
        public List<StudentMilestoneList> studentsMilestone { get; set; }
    }
    public class StudentMilestoneList
    {
        public int MilestoneFormID { get; set; }
        public int MilestonePublishedFormID { get; set; }
        public string ApproverName { get; set; }
        public string StudentName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string State { get; set; }
        public int CSULBID { get; set; }
        public string ProgramName { get; set; }
        public string MilestoneName { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public int FormId { get; set; }
        public string TermName { get; set; }
    }

}
