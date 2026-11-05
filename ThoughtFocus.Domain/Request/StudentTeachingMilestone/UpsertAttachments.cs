using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.StudentTeachingMilestone
{
    public class UpsertAttachments
    {
        public int FormID { get; set; }
        public int DocumentID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public byte[] FileContent { get; set; }
        public int CreatedBy { get; set; }
    }
    public class DeleteAttachementRequest
    {
        public Guid UniqueID { get; set; }
    }
    public class AddCredentialAreaRequest
    {
        public string Name { get; set; }
    }
    public class StudentTeachingMilestoneRequest
    {
        public object JSON { get; set; }

    }
    public class AssignApproverAssignerRequest
    {
        public int FormID { get; set; }
        public string CSULBID { get; set; }
        public int ProgramID { get; set; }

    }
    public class AddExternalApproverRequest
    {
        public int FormID { get; set; }
        public string CSULBID { get; set; }
        public int ProgramID { get; set; }
        public int ExternalApproverID { get; set; }

    }
    public class ApproverAssignmentsRequest
    {
        public int FormID { get; set; }
        public string CSULBID { get; set; }
        public int ProgramID { get; set; }
    }
    public class UpdateFormStateRequest
    {
        public int FormID { get; set; }
        public int StateID { get; set; }
    }
}
