using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Travel
{
    public class ReportDataResponse
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
