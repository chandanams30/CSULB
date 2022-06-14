using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Form
{
    public class FormResponse
    {
        public int FormId { get; set; }
        public int ApplicantId { get; set; }
        public string ApplicantName { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public int SemesterId { get; set; }
        public string Semester { get; set; }
        public object Form { get; set; }
        public string ApplicationNumber { get; set; }
        public int ReviewerId { get; set; }
        public string ReviewerName { get; set; }
        public string ReviewerRecommendation { get; set; }
        public string CampusId { get; set; }
    }
}
