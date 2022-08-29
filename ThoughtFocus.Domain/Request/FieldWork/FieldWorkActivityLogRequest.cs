using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.FieldWork
{
    public class FieldWorkActivityLogRequest
    {
        public int UserID { get; set; }
        public int FieldWorkID { get; set; }
        // public string ResponseSchema { get; set; }
        public int ActivityLogID { get; set; }
        public DateTime ActivityStartDate { get; set; }
        public DateTime ActivityEndDate { get; set; }
        public int CommunitySiteID { get; set; }
        public int CommunitySiteUserID { get; set; }
        public int Hours { get; set; }
        public string status { get; set; }
        public List<FieldWorkActivityLogStandards> standards { get; set; }
    }
    public class FieldWorkActivityLogStandards
    {
       // public int FieldWorkActivityLogID { get; set; }
        public int FieldWorkCoursesCategoryStandardID{ get; set; }
        public string Details { get; set; }
        public int Hours { get; set; }
    }
}
