using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;
 
namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkCourseConfiguration : BaseResponse
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string CourseNumber { get; set; }
        public string ClassSection { get; set; }
        public FieldWorkActivityConfig Configs { get; set; }
    }
    public class FieldWorkDocuments
    {
        public string DocumentName { get; set; }
        public int DocumentId { get; set; }
        public bool IsRestricted { get; set; }
        public bool isRequiredPrerequisite { get; set; }
    }
    public class FieldWorkActivityConfig
    {
        public bool EnableActivityLog { get; set; }
        public bool AutoCompute { get; set; }
        public string FieldWorkHours { get; set; }
        public string CategoryID { get; set; }
        public bool RecordByDate { get; set; }
        public List<FieldWorkDocuments> DocumentsConfig { get; set; }

    }

}
