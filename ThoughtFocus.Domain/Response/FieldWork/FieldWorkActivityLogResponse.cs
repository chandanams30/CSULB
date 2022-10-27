using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkActivityLogResponse
    {
        public int ActivityLogID { get; set; }
        public string DisplayID { get; set; }
        public int FieldWorkID { get; set; }
        public int CommunitySiteID { get; set; }
        public string SiteName { get; set; }
        public DateTime ActivityStartDate { get; set; }
        public DateTime ActivityEndDate { get; set; }
        public decimal Hours { get; set; }
        public string status { get; set; }
        public bool ShowCheckbox { get; set; }
        //public string BaseSchema { get; set; }
        //public string ResponseSchema { get; set; }
    }
    public class FieldWorkActivityLogListResponse : BaseResponse
    {
        public List<FieldWorkActivityLogResponse> fieldWorkList { get; set; }
        public FieldWorkActivityLogHandler activityLogHandler { get; set; }
    }
}
