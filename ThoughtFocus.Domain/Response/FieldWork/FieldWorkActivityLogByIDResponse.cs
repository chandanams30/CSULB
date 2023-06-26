using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkActivityLogByIDResponse:BaseResponse
    {
        public FieldWorkActivityLogByID DataByID { get; set; }
        public List<FieldWorkActivityLogStandardList> standardList { get; set; }
        public FieldWorkActivityLogHandler   ActivityLogHandler { get; set; }
    }
    public class FieldWorkActivityLogStandardList
    {
       public int FieldWorkActivityLogID { get; set; }
        public int FieldWorkCoursesCategoryStandardID { get; set; }
        public string FieldWorkCoursesCategoryStandard { get; set; }
        public int FieldWorkCoursesCategorySchoolTypeID { get; set; }
        public string FieldWorkCoursesCategorySchoolType { get; set; }
        public string Details { get; set; }
        public decimal Hours { get; set; }
    }
    public class FieldWorkActivityLogByID
    {
        public int ActivityLogID { get; set; }
        public int FieldWorkID { get; set; }
        public int CommunityDistrictID { get; set; }
        public string CommunityDistrictName { get; set; }
        public int CommunitySchoolID { get; set; }
        public string CommunitySchoolName { get; set; }
        public int CommunitySiteUserID { get; set; }
        public string CommunitySiteUserName { get; set; }       
        public DateTime ActivityStartDate { get; set; }
        public DateTime ActivityEndDate { get; set; }
        public decimal Hours { get; set; }
        public string Status { get; set; }
        public string ApprovedByUser { get; set; }
        public DateTime? ApprovedDateTime { get; set; }
    }
    public class FieldWorkActivityLogHandler
    {
        public string ActivityLogHandler { get; set; }
    }
}
