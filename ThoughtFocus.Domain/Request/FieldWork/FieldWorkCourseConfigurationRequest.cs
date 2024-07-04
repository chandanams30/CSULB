using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response;

namespace ThoughtFocus.Domain.Request.FieldWork
{
    public class FieldWorkCourseConfigurationRequest
    {
        public int CourseId { get; set; }
        public bool EnableActivityLog { get; set; }
        public bool AutoCompute { get; set; }
        public bool RecordByDate { get; set; }
        
        public int CategoryID { get; set; }
        public int FieldWorkHours { get; set; }

        public List<FieldWorkDocumentConfig> DocumentsConfig { get; set; }
    }
    public class FieldWorkDocumentConfig
    {
      
        public int CourseId { get; set; }
        public int DocumentID { get; set; }
        public bool IsRestricted { get; set; }
        public bool IsRequiredPrerequisite { get; set; }
    }
}
