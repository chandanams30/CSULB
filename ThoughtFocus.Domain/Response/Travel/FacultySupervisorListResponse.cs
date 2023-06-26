using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Travel
{
    public class FacultySupervisorListResponse:BaseResponse
    {
        public List<FacultySupervisor> FacultySupervisorList { get; set; }
    }
    public class FacultySupervisor
    {
        
        public int BusinessMileageSupervisorUserID { get; set; }
        public string BusinessMileageSupervisorName { get; set; }
        public string Email { get; set; }
        public string CSULBID { get; set; }
        public string Phone { get; set; }
        public string MailingAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int? BusinessMileageSupervisorID { get; set; }
        public Boolean isPersonalInfoCompleted { get; set; }
        public Boolean IsTravelPrerequisitesCompleted { get; set; }
    }
}
