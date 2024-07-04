using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class EvaluationByEvaluationIdentifierResponse : BaseResponse
    {
        public EvaluationByEvaluationIdentifier evaluationByEvaluationIdentifier { get; set; }
    }
    public class EvaluationByEvaluationIdentifier
    {
        public int EvaluationID { get; set; }
        public int FieldWorkID { get; set; }
        public string EvaluatorName { get; set; }
        public string EvaluationJSON { get; set; }
        public string StudentName { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string CSULBID { get; set; }
        public string StudentEmail { get;set; }
        public string CourseTitle { get; set; }
        public string TermName { get; set; }
      
    }
}
