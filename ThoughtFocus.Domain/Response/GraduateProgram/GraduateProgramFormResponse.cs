using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class GraduateProgramFormResponse : BaseResponse
    {
        public FormBasicInformation FormBasicInformation { get; set; }
        public FormStateHandler FormStateHandler { get; set; }
        public List<FormAttachmentsInformation> FormAttachmentsInformation { get; set; }
        public RecommendersInformation RecommendersInformation { get; set; }
        public ReviewerInformation ReviewerInformation { get; set; }
    }
    public class FormBasicInformation
    {
        public int FormID { get; set; }
        public int StudentID { get; set; }
        public int ProgramID { get; set; }
        public string ProgramName { get; set; }
        public string TermCode { get; set; }
        public string Semester { get; set; }
        public string Form { get; set; }
        public int FormStateID { get; set; }
        public string FormState { get; set; }
        public string ApplicationNumber { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public int ModifiedBy { get; set; }
        public string MessageBoard { get; set; }
        public string CompletingYourApplication { get; set; }
    }
    public class FormStateHandler
    {
        public string StateHandler { get; set; }
    }
    public class FormAttachmentsInformation
    {
        public int FormAttachmentID { get; set; }
        public int DocumentID { get; set; }
        public int ProgramID { get; set; }
        public int FormID { get; set; }
        public string AttachmentTitle { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public bool IsOptional { get; set; }
    }
    public class RecommendersInformation
    {
        public string Recommendations { get; set; }
        #region Old section
        //public int RecomendationID { get; set; }
        //public int FormID { get; set; }
        //public string RecommenderName { get; set; }
        //public string RecommenderEmail { get; set; }
        //public string FileName { get; set; }
        //public string FileExtn { get; set; }
        //public string FolderName { get; set; }
        //public DateTime UploadedDate { get; set; }
        //public int CreatedBy { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public string RecommenderURL { get; set; }
        //public DateTime RecommenderURLValidTill { get; set; }
        //public bool AllowUpload { get; set; }
        //public string RecommenderIdentifier { get; set; }
        #endregion
    }
    
    public class ReviewerInformation
    {
        public string Reviewer { get; set; }
    }
}
