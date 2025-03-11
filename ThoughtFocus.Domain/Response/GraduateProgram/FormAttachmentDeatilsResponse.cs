using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class FormAttachmentDeatilsResponse : BaseResponse
    {
        public FormAttachmentDeatils formAttachmentDeatils { get; set; }
    }
    public class FormAttachmentDeatils
    {
        public int formID { get; set; }
        public int formAttachmentID { get; set; }
        public int documentID { get; set; }
        public int programID { get; set; }
        public string attachmentTitle { get; set; }
        public string fileName { get; set; }
        public string fileExtn { get; set; }
        public bool IsOptional { get; set; }
    }
}
