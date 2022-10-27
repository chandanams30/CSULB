using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class FormReviewerReviewRequest
    {
        public int FormID { get; set; }
        public int ReviewID { get; set; }
        public int ReviewerID { get; set; }
        public string ReviewerRecommendation { get; set; }
        public string ReviewerComments { get; set; }
    }
}
