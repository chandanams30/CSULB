using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Program
{
    public class ProgramApplicationListResponse
    {
        public int FormId { get; set; }
        public string ProgramName { get; set; }
        public string SemesterName { get; set; }
        public string ApplicationNumber { get; set; }
        public string ApplicantName { get; set; }
        public string CampusID { get; set; }
        public string ReviewerName { get; set; }
        public string ReviewerRecommendation { get; set; }

    }
}
