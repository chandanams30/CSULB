using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class DispositionMSCPFiledata
    {
        public class Attendance
        {
            public string label { get; set; }
            public List<Option> options { get; set; }
            public int key { get; set; }
            public string value { get; set; }
        }

        public class Communication
        {
            public string label { get; set; }
            public List<Option> options { get; set; }
            public int key { get; set; }
            public string value { get; set; }
        }

        public class Option
        {
            public string label { get; set; }
            public string value { get; set; }
        }

        public class Professional
        {
            public string label { get; set; }
            public List<Option> options { get; set; }
            public string value { get; set; }
            public int key { get; set; }
        }

        public class Rating
        {
            public string label { get; set; }
            public string value { get; set; }
        }
    }
}
