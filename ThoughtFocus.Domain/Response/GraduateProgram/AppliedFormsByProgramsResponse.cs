using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class AppliedFormsByProgramsResponse : BaseResponse
    {
        public List<AppliedFormsByPrograms> AppliedFormsByPrograms { get; set; }
        public HeaderDetails HeaderDetails { get; set; }

    }
    public class AppliedFormsByPrograms
    {
        public int FormID { get; set; }
        public string StudentName { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public int FormStateID { get; set; }
        public string FormState { get; set; }
        public DateTime AppliedDate { get; set; }
        public int ProgramID { get; set; }
        public string ProgramName { get; set; }
        public string Semester { get; set; }
        public string TermCode { get; set; }
        public string CSULBID { get; set; }
        public string ReviewersName { get; set; }
        public string BSRStatus { get; set; }
        public string CTCStatus { get; set; }
        public string GPAStatus { get; set; }
        public string SMCStatus { get; set; }
        public string TBTestStatus { get; set; }
    }

  
}
