using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.FormModels
{
    public class SSCP_MSCP_UDCP_Model
    {
        public personalInfo personalInfo { get; set; }
        public applicantRelatedAnswers applicantRelatedAnswers { get; set; }
        public List<recommendationForLongBeach> recommendationForLongBeach { get; set; }
        public signatureOfRecommender signatureOfRecommender { get; set; }
        public string comments { get; set; }
    }
    public class personalInfo
    {
        public string applicantFirstName { get; set; }
        public string applicantLastName { get; set; }
        public string credentialSubjectArea { get; set; }
        public string campusID { get; set; }
        public string recommenderFirstName { get; set; }
        public string recommenderLastName { get; set; }
        public string institution { get; set; }
        public string position_title { get; set; }
        public string telephoneContact { get; set; }
        public string email { get; set; }
    }
    public class applicantRelatedAnswers
    {
        public string answer1 { get; set; }
        public string answer2 { get; set; }
        public string answer3 { get; set; }
        public string answer4 { get; set; }
        public List<answer5> answer5 { get; set; }
    }
    public class answer5
    {
        public string qualities { get; set; }
        public bool belowAverageBottom { get; set; }
        public bool averageMiddle { get; set; }
        public bool goodTop { get; set; }
        public bool unusually { get; set; }
        public bool outstandingTop { get; set; }
        public bool trulyExcellentTop { get; set; }
        public bool cannotComment { get; set; }
    }
    public class recommendationForLongBeach
    {
        public string label { get; set; }
        public bool value { get; set; }
    }
    public class signatureOfRecommender
    {
        public string name { get; set; }
        public string date { get; set; }
    }
}
