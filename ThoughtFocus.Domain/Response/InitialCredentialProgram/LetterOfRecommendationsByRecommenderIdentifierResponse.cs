using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ThoughtFocus.Domain.Response.GraduateProgram;

namespace ThoughtFocus.Domain.Response.InitialCredentialProgram
{
    public class LetterOfRecommendationsByRecommenderIdentifierResponse : BaseResponse
    {
        public LetterOfRecommendationsByRecommenderIdentifier LetterOfRecommendationsByRecommenderIdentifier { get; set; }
    }
    public class LetterOfRecommendationsByRecommenderIdentifier
    {
        public int LetterOfRecommendationID { get; set; }
        public int FormID { get; set; }
        public string RecommenderName { get; set; }
        public string LetterOfRecommendationJSON { get; set; }
        public string StudentName { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
       public string CSULBID { get; set; }
        public string StudentEmail { get;set; }
        public string ProgramName { get; set; }
        public string TermName { get; set; }
      
    }
}
