using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class FormAddRecommendationRequest
    {
        public int FormID { get; set; }
        public string RecommenderIdentifier { get; set; }
        public int DocumentID { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }

        public List<FormAddRecommendationRequestAttachment> FormAddRecommendationRequestAttachment { get; set; }
    }
    public class FormAddRecommendationRequestAttachment
    {
        public int DocumentID { get; set; }

        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
    public class FormAddRecommendation
    {
        public int FormID { get; set; }
        public string RecommenderIdentifier { get; set; }
        public string LetterOfRecommendationJSON { get; set; }

        public List<FormAddRecommendationRequestAttachment> FormAddRecommendationRequestAttachment { get; set; }
    }
}
