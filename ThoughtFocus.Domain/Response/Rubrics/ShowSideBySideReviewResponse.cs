using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Rubrics
{
    public class ShowSideBySideReviewResponse : BaseResponse
    {
        public List<ShowSideBySideReview> showSideBySideReview { get; set; }
    }
    public class ShowSideBySideReview
    {
        public int FilledRubricID { get; set; }
        public int PublishRubricID { get; set; }
        public int TemplateID { get; set; }
        public string StudentName { get; set; }
        public int CSULBID { get; set; }
        public string ProgramName { get; set; }
        public string TemplateName { get; set; }
        public int FormID { get; set; }
        public string TermName { get; set; }
        public string ReviewerName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ReviewerID { get; set; }
        public string TermCode { get; set; }
        public string State { get; set; }
        public string RubricForm { get; set; }
        public int ProgramID { get; set; }
        public string TemplateDescription { get; set; }
        public int TotalPoints { get; set; }
    }
}
