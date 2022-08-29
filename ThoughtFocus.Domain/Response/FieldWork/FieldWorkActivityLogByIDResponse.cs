using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkActivityLogByIDResponse:BaseResponse
    {
        public FieldWorkActivityLogByID DataByID { get; set; }
        public List<FieldWorkActivityLogStandardList> standardList { get; set; }
    }
    public class FieldWorkActivityLogStandardList
    {
       public int FieldWorkActivityLogID { get; set; }
        public int FieldWorkCoursesCategoryStandardID { get; set; }
        public string Details { get; set; }
        public int Hours { get; set; }
    }
    public class FieldWorkActivityLogByID
    {
        public int ActivityLogID { get; set; }
        public int FieldWorkID { get; set; }
        public int CommunitySiteID { get; set; }
        public int CommunitySiteUserID { get; set; }
        public DateTime ActivityStartDate { get; set; }
        public DateTime ActivityEndDate { get; set; }
        public int Hours { get; set; }
        public string Status { get; set; }
    }
}
