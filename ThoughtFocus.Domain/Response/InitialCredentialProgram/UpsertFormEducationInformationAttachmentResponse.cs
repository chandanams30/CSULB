using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.InitialCredentialProgram
{
    public class UpsertFormEducationInformationAttachmentResponse:BaseResponse
    {
        public int FormEducationInformationAttachmentID { get; set; }
        public int FormID { get; set; }
        public Guid UniqueID { get; set; }
        public string FileName { get; set; }
        public string FileExtn { get; set; }
        public string FolderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
