using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Admin
{
    public class UpsertCSULBIDForIntern
    {
        public string CSULBIDs { get; set; }
    }
    public class SupervisorForCourses
    {
        public string CSULBIDs { get; set; }
        public string Subjects { get; set; }

    }
}
