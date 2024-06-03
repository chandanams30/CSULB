using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class PrerequisiteExiredResponse : BaseResponse
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Identifier { get; set; }
        public string LogSummary { get; set; }
        public int TotalSent { get; set; }
        public int TotalFailure { get; set; }
        public int TriggeredBy { get; set; }
        public DateTime TriggeredDate { get; set; }

    }
}
