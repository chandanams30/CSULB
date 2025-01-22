using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class DecisionLettersResponse : BaseResponse
    {
        public DecisionLetters decisionLetters { get; set; }
        public List<FinalDecision> finalDecision { get; set; }
    }
    public class DecisionLetters
    {
        public int DecisionLettersID { get; set; }
        public string ProgramIdentifier { get; set; }
        public string  OfferedCategories { get; set; }
        public string DecisionType { get; set; }
        public string MailBody { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }
    public class FinalDecision
    {
        public int ID { get; set; }
        public string Decision {  get; set; }
    }
}
