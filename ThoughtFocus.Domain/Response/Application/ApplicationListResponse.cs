using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Application
{
    public class ApplicationListResponse
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public int Count { get; set; }
    }
}
