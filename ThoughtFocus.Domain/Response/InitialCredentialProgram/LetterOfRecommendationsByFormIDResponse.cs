using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ThoughtFocus.Domain.Response.GraduateProgram;

namespace ThoughtFocus.Domain.Response.InitialCredentialProgram
{
    public class LetterOfRecommendationsByFormIDResponse : BaseResponse
    {
        public List<LetterOfRecommendationsByFormID> LetterOfRecommendationsByFormID { get; set; }
    }
    public class LetterOfRecommendationsByFormID
    {
        public int LetterOfRecommendationID { get; set; }
        public int FormID { get; set; }
        public string RecommenderName { get; set; }
        public string RecommenderEmail { get; set; }
        public BigInteger CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string RecommenderURL { get; set; }
        public DateTime RecommenderURLValidTill { get; set; }
        public string RecommenderIdentifier { get; set; }
        public string isMailSent { get; set; }
        public string LetterOfRecommendationJSON { get; set; }
        public string CanView { get; set; }
        public string FileLink { get; set; }
    }
}
