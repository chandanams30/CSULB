using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.StudentProfile
{
    public class UpsertProfileAttachmentResponse : BaseResponse
    {
        public int ProfileDocumentID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public int UserID { get; set; }
    }
    public class DownloadProfileAttachmentResponse : BaseResponse
    {
        public int ProfileDocumentID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public int UserID { get; set; }
        public byte[] FileContent { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class ProfileAttachmentDetailsResponse : BaseResponse
    {
        public List<ProfileAttachmentDetails> ProfileAttachmentDetails { get; set; }
    }
    public class ProfileAttachmentDetails
    {
        public int ProfileDocumentID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public int UserID { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CanView { get; set; }
        public string CreatedByUsername { get; set; }
    }
    public class PrerequisitesResponse : BaseResponse
    {
        public List<PrerequisiteDetails> FormPrerequisites { get; set; }
    }
    public class PrerequisiteDetails
    {
        public int fieldWorkAttachmentID { get; set; }
        public int UserID { get; set; }
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public DateTime? ValidatedDate { get; set; }
        public string DocumentStatus { get; set; }
    }
    public class ProgramPlannerCourseListResponse:BaseResponse
    {
        public List<ProgramPlannerCourseList> ProgramPlannerCourseList { get; set; }
    }

    public class ProgramPlannerCourseList
    {

        public int MasterID { get; set; }
        public int DetailID { get; set; }
        public string CourseName { get; set; }
        public string TermCode { get; set; }
        public string Term { get; set; }
        public string Year { get; set; }
        public string Notes { get; set; }
        public string CSULBID { get; set; }
    }
}
