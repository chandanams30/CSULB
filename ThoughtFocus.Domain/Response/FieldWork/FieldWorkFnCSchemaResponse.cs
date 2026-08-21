using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkFnCSchemaResponse:BaseResponse
    {
        public string schema { get; set; }
    }
    public class PUNS_GetCommunitySiteSupervisorDemonstrationTeacherListResponse : BaseResponse
    {
       public List<PUNS_GetCommunitySiteSupervisorDemonstrationTeacherList> listPartnerUser { get; set; }
    }


    public class PUNS_GetCommunitySiteSupervisorDemonstrationTeacherList
    {
        public int CSSDTID { get; set; }
        public string CommunitySiteUserName { get; set; }
        public string CommunitySiteUserEmail { get; set; }
        //public DateTime? EmailSentOn { get; set; }
        public string EmailSentOn { get; set; }
    }

    public class PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail : BaseResponse
    {
        public int CSSDTID { get; set; }
        public string CommunitySiteUserName { get; set; }
        public string CommunitySiteUserEmail { get; set; }
        public string CommunitySiteUserIdentifier { get; set; }
        public string StudentName { get; set; }

    }
    public class PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents : BaseResponse
    {
        public int CSSDTID { get; set; }
        public string CommunitySiteUserName { get; set; }
        public string CommunitySiteUserEmail { get; set; }
        public string CommunitySiteUserIdentifier { get; set; }
        public string StudentName { get; set; }

    }
    public class UpdateCommunitySiteSupervisorDemonstrationTeacherListResponse : BaseResponse
    {
        public int CssdtID { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
    }

}
