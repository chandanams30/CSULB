using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.FieldWork
{
    public class UpsertEvaluationRequest
    {
        public int EvaluationID { get; set; }
        public int UserID { get; set; }
        public int FieldWorkID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public string EvaluatorName { get; set; }
        public string EvaluatorEmail { get; set; }
        public string ApplicationType { get; set; }

    }
    public class UpdateEvaluationJSONRequest
    {
        public int EvaluationID { get; set; }
        public int UserID { get; set; }
        public int FieldWorkID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public string EvaluationJSON { get; set; }
        public string ApplicationType { get; set; }

    }
    public class DownloadAttachment
    {
        public string evaluationjson { get; set; }
    }
    public class FieldWorkActivityLogsAttachmentRequest
    {
        public int UserID { get; set; }
        public int FieldWorkID { get; set; }
    }
}
