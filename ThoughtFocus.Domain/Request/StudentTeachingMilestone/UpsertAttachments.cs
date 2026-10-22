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
}
