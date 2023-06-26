using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Travel
{
    public class ReportDataRequest
    {
        public int BusinessMileageSupervisorID { get; set; }
        public string ReportStartMonth { get; set; }
        public string ReportStartYear { get; set; }
        public string ReportEndMonth { get; set; }
        public string ReportEndYear { get; set; }
    }
}
