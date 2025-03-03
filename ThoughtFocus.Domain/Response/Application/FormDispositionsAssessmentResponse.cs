using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.FieldWork;
using static ThoughtFocus.Domain.Request.GraduateProgram.DispositionMSCPFiledata;

namespace ThoughtFocus.Domain.Response.Application
{
    public class FormDispositionsAssessmentResponse:BaseResponse
    {
        public int FormDispositionsAssessmentID { get; set; }
        public int FormID { get; set; }
        public string DispositionsAssessmentForm { get; set; }
    }

    public class FormSubSectionResponse : BaseResponse
    {
        public int FormSubSectionID { get; set; }
        public int FormID { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public string SubSectionForm { get; set; }
        public bool showUpdateFormSubSection { get; set; }
    }

    public class FormSectionAttachmentResponse : BaseResponse
    {
        public List<FormSectionAttachmentList> FormSectionAttachmentList { get; set; }
    }

    public class FormSectionAttachmentList
    {
        public int FormSubSectionAttachmentID { get; set; }
        public int FormID { get; set; }
        public int FormSubSectionID { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public bool CanView { get; set; }
    }

    public class FormSubsectionAttachmentDownloadResponse : BaseResponse
    {
       
        public int ID { get; set; }
        public int FormID { get; set; }
        public int FormSubSectionID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public byte[] FileContent { get; set; }
    }

    public class DownloadEducationalInformationalAttachment: BaseResponse
    {
        public int FormEducationInformationAttachmentID { get; set; }
        public int FormID { get; set; }
        public Guid FormUniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public string FolderName { get; set; }
        public byte[] FileContent { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
    public class ClinicalPracticeEquivalencyAttachmentList : BaseResponse
    {
        public Attachments attachements {  get; set; } 
       
    }
    public class Attachments
    {
        public PV pv { get; set; }
        public PC pc { get; set; }
    }
    public class PV
    {
        public ProfessionalVerificationForm professionalVerificationForm { get; set; }
        public ChildDevelopmentPermit childDevelopmentPermit { get; set; }
    }
    public class ProfessionalVerificationForm
    {
        public int FormSubSectionAttachmentID { get; set; }
        public int FormID { get; set; }
        public int FormSubSectionID { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public bool CanView { get; set; }
    }
    public class ChildDevelopmentPermit
    {
        public int FormSubSectionAttachmentID { get; set; }
        public int FormID { get; set; }
        public int FormSubSectionID { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public bool CanView { get; set; }
    }
    public class PC
    {
        public CourseSyllabi courseSyllabi { get; set; }
        public List<Transcripts> transcripts { get; set; }
    }
    public class CourseSyllabi
    {
        public int FormSubSectionAttachmentID { get; set; }
        public int FormID { get; set; }
        public int FormSubSectionID { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public bool CanView { get; set; }
    }
    public class Transcripts
    {
        public int FormSubSectionAttachmentID { get; set; }
        public int FormID { get; set; }
        public int FormSubSectionID { get; set; }
        public string SubSectionIdentifiers { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public bool CanView { get; set; }
    }
}
