using Nancy.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.InitialCredentialProgram
{
    public class FormDispositionsAssessmentRequest
    {
        //public int FormDispositionsAssessmentID { get; set; }
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
    }

    public class UpsertFormDispositionsAssessmentRequest
    {
        public int FormDispositionsAssessmentID { get; set; }
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public string DispositionsAssessmentForm { get; set; }

    }

    public class FormSubsectionRequest
    {
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public int RoleId { get; set; }

    }

    public class UpsertFormSubSectionRequest
    {
        public int FormSubSectionID { get; set; }
        public int UserID { get; set; }
        public int FormID { get; set; }
        public int ProgramID { get; set; }
        public string TermCode { get; set; }
        public string SubSectionForm { get; set; }
        public string SubSectionIdentifiers { get; set; }
    }

    public class SaveFormSubSectionAttachmentRequest
    {
        public int FormSubSectionID { get; set; }
        public int FormID { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public string TermCode { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string FileExtn { get; set; }
        public string SavedFileName { get; set; }
        public int UserID { get; set; }

    }

    public class SubsectionAttachmentDownloadRequest
    {
        public int FormID { get; set; }
        public int FormSubSectionID { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public int FormSubSectionAttachmentID { get; set; }

    }

    public class UpdateFormSubSectionApproveralRequest
    {
        public int FormID { get; set; }
        public int FormSubSectionID { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public int ApproverUserID { get; set; }
        public string ApproverComments { get; set; }
        public bool IsApproved { get; set; }
        public string Status { get; set; }
    }
    public class SaveClinicalPracticeEquivalencyAttachmentRequest
    {
        public int FormSubSectionID { get; set; }
        public int FormID { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public string TermCode { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
        public string FileExtn { get; set; }
        public string SavedFileName { get; set; }
        public int UserID { get; set; }

    }
    public class ClinicalPracticeEquivalencyAttachmentRequest
    {
        public int FormID { get; set; }
        public int FormSubSectionID { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public int FormSubSectionAttachmentID { get; set; }

    }
    public class AdmissionRequirementsBody
    {
        public int ID { get; set; }
        public string EmailBody { get; set; }
        public string Identifier { get; set; }
    }
}
