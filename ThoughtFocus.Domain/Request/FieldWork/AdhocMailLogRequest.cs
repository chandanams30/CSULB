using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response;

namespace ThoughtFocus.Domain.Request.FieldWork
{
    public class PrerequisiteExpiredRequest
    {
        public int UserID { get; set; }
        public string Type { get; set; }
        public string Identifier { get; set; }
        public string TermCode { get; set; }
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
    public class UnapprovedPartnerUserMailRequest
    {
        public int UserID { get; set; }
        public string Type { get; set; }
        public string Identifier { get; set; }
        public string TermCode { get; set; }
    }
}
