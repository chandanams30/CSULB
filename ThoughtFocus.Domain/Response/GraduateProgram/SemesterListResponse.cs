using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.InitialCredentialProgram;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class SemesterListResponse : BaseResponse
    {
        public List<Semester> Semesters { get; set; }
    }
    public class Semester
    {
        public string TermCode { get; set; }
        public string TermName { get; set; }
    }
    public class RecommenderMailBodyResponse : BaseResponse
    {
        public RecommenderBody recommenderResponse { get; set; }
    }
    public class RecommenderBody
    {
        public int ID { get; set; }
        public string RecommenderMailBody { get; set; }
        public string Identifier { get; set; }
    }
    public class EvaluatorMailBodyResponse : BaseResponse
    {
        public EvaluatorBody evaluatorResponse { get; set; }
    }
    public class EvaluatorBody
    {
        public int ID { get; set; }
        public string EvaluatorMailBody { get; set; }
        public string Identifier { get; set; }
    }
    public class AdmissionRequirementsBodyResponse : BaseResponse
    {
        public AdmissionRequirementBody admissionRequirementResponse { get; set; }
    }
    public class AdmissionRequirementBody
    {
        public int ID { get; set; }
        public string EmailBody { get; set; }
        public string Subject { get; set; }
    }
}
