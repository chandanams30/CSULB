using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkEvaluationByIDResponse : BaseResponse
    {
        public List<FieldWorkEvaluationByID> FieldWorkEvaluationByID { get; set; }
    }
    public class FieldWorkEvaluationByID
    {
        public int? EvaluationID { get; set; }
        public int? FieldWorkId { get; set; }
        public string EvaluatorName { get; set; }
        public string EvaluatorEmail { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string EvaluationURL { get; set; }
        public DateTime? EvaluationURLValidTill { get; set; }
        public string EvaluationIdentifier { get; set; }
        public bool isMailSent { get; set; }
        public string EvaluationJSON { get; set; }
        public bool CanView { get; set; }
        public string FileLink { get; set; }
    }
}
