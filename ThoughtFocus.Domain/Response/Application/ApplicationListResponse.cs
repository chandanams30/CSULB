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

    public class StudentNotificationResponse
    {
        public bool ShowNotification { get; set; }
        public string Message { get; set; }
    }
    public class ApplicationProgram
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
