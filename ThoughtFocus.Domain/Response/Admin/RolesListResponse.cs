using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Admin
{
    public class RolesListResponse:BaseResponse
    {
      public List<RolesList> roles { get; set; }
    }
    public class RolesList
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }
    public class UpcomingSemesterListResponse : BaseResponse
    {
        public List<SemesterTerm> SemesterTerms { get; set; }
    }

    public class SemesterTerm
    {
        public string TermCode { get; set; }
        public string TermName { get; set; }
    }
    public class ApplicationProgramListResponse : BaseResponse
    {
        public List<ApplicationProgramsList> ApplicationProgramList { get; set; }
    }
    public class ApplicationProgramsList
    {
        public int ProgramID { get; set; }
        public string ProgramName { get; set; }
    }
}
