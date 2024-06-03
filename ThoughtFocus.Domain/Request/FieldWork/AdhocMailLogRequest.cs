using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.FieldWork
{
    public class AdhocMailLogRequest
    {
        public class PrerequisiteExiredRequest
        {
            public int UserID { get; set; }
            public string Type { get; set; }
            public string Identifier { get; set; }
        }
        public class PrerequisiteApprovedRequest
        {
            public int UserID { get; set; }
            public string Type { get; set; }
            public string Identifier { get; set; }
            public string TermCode { get; set; }
        }
        public class PendingRecommendationsRequest
        {
            public int UserID { get; set; }
            public string Type { get; set; }
            public string Identifier { get; set; }
            public string TermCode { get; set; }
            public int ProgramID { get; set; }
        }
    }
}
