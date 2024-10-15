using Newtonsoft.Json;
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
        public DownloadDetail downloadDetails { get; set; }
        public List<FieldWorkActivityLogResponse> fieldWorkList { get; set; }
        public FieldWorkActivityLogHandler activityLogHandler { get; set; }
    }
    public class PUFieldWorkActivityLogListResponse : BaseResponse
    {
        public List<FieldWorkActivityLogResponse> fieldWorkList { get; set; }
        public FieldWorkActivityLogHandler activityLogHandler { get; set; }
        public FieldWorkSummary fieldWorkSummary { get; set; }
    }
    public class DownloadDetail
    {
        public FieldWorkInformation fieldWorkInformation { get; set; }
        public Summary summary { get; set; }
    }
    public class FieldWorkInformation
    {
        [JsonProperty("Student Name")]
        public string StudentName { get; set; }
        [JsonProperty("Student ID")]
        public string StudentID { get; set; }
        [JsonProperty("Course Name")]
        public string CourseName { get; set; }
        [JsonProperty("Semester")]
        public string Semester { get; set; }
        [JsonProperty("Instructor")]
        public string Instructor { get; set; }
        [JsonProperty("Course Number")]
        public string CourseNumber { get; set; }
        [JsonProperty("Section")]
        public int Section {  get; set; }
    }
    public class Summary
    {
        [JsonProperty("Expected Hours")]
        public decimal ExpectedHours { get; set; }
        [JsonProperty("Logged Hours")]
        public decimal LoggedHours { get; set; }
        [JsonProperty("Sent For Approval")]
        public decimal SentForApproval { get; set; }
        [JsonProperty("Approved Hours")]
        public decimal ApprovedHours { get; set; }

    }

}
