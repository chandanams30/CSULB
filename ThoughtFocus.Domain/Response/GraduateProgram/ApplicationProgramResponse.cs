using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class ApplicationProgramResponse : BaseResponse
    {
      public List<ApplicationPrograms> ApplicationPrograms { get; set; }
      public HeaderDetails HeaderDetails { get; set; }
      public List<Semester> Semesters { get; set; }
    }
    public class ApplicationPrograms
    {
        public int programID { get; set; }
        public string programName { get; set; }
        public string semester { get; set; }
        public string TermCode { get; set; }
        public DateTime applicationOpens { get; set; }
        public DateTime applicationCloseDate { get; set; }
        public int TotalCount { get; set; }
        public int AcceptedCount { get; set; }
        public bool showApply { get; set; }
        public bool showView { get; set; }
        public string ProgramSetting { get; set; }
        public int SubmittedCount { get; set; }
    }

    public class HeaderDetails
    {
        public string semester { get; set; }
        public string TermCode { get; set; }
        public bool showApply { get; set; }
        public bool showView { get; set; }
        public bool showAssignApplicationToReviewers { get; set; }
        public bool showSettings { get; set; }
        public string programName { get; set; }
        public int programID { get; set; }
        //public bool showBulkDeny { get; set; }
        //public bool showBulkOffer { get; set; }
    }
}
