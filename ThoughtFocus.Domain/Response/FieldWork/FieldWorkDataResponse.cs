using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkDataResponse :BaseResponse
    {
        public FieldWorkResponse FieldWork { get; set; }
        public List<FieldWorkRoles> FieldWorkRoles { get; set; }
        public List<FieldWorkProfileAttachments> FieldWorkAttachments { get; set; }
    }
    public class FieldWorkRoles
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int FieldWorkID { get; set; }
        public int RoleID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class FieldWorkProfileAttachments
    {
        public int FieldWorkAttachmentID { get; set; }
        public int UserID { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        //public string FileExtn { get; set; }
       // public string FolderName { get; set; }
        public bool? IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ValidatedDate { get; set; }
       // public int CreatedBy { get; set; }
       // public DateTime? CreatedDate { get; set; }
        public DateTime? ValidTill { get; set; }
        public byte[] FileContent { get; set; }
        public string RejectReason { get; set; }
    }
}
