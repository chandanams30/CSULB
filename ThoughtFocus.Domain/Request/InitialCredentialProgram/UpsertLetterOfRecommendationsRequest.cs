using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.InitialCredentialProgram
{
    public class UpdateFormStudentMessageBoardRequest
    {
        public int LetterOfRecommendationID { get; set; }
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public string RecommenderName { get; set; }
        public string RecommenderEmail { get; set; }
    }
    public class UpdateLetterOfRecommendationsJSONRequest
    {
        public int LetterOfRecommendationID { get; set; }
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public string LetterOfRecommendationJSON { get; set; }
    }
}
