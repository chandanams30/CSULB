using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response;

namespace ThoughtFocus.Domain.Request.FieldWork
{
    public class FieldWorkActivityLogRequest
    {
        public int UserID { get; set; }
        public int FieldWorkID { get; set; }
        // public string ResponseSchema { get; set; }
        public int ActivityLogID { get; set; }
        public int CommunityDistrictID { get; set; }
        public int CommunitySchoolID { get; set; }
        public int CommunitySiteUserID { get; set; }
        public string CommunitySiteUserName { get; set; }
        public string CommunitySiteUserEmail { get; set; }
        public DateTime ActivityStartDate { get; set; }
        public DateTime ActivityEndDate { get; set; }
       
        public decimal Hours { get; set; }
        public string status { get; set; }
        public List<FieldWorkActivityLogStandards> standards { get; set; }
    }
    public class FieldWorkActivityLogStandards
    {
       // public int FieldWorkActivityLogID { get; set; }
        public int FieldWorkCoursesCategoryStandardID{ get; set; }
        public int FieldWorkCoursesCategorySchoolTypeID { get; set; }
        public string Details { get; set; }
        public decimal Hours { get; set; }
    }
    public class UpdateFieldWorkActivityLogStatusRequest
    {
        public int UserID { get; set; }
        public int FieldWorkID { get; set; }
        public int FieldWorkActivityLogID { get; set; }
        public string Status { get; set; }
    }
  
    public class UpdateFieldWorkActivityLogStatusListRequest
    {
        public List<UpdateFieldWorkActivityLogStatusRequest> logStatusList { get; set; }
    }
}
