using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkSemesterList : BaseResponse
    {
        public List<FieldWorkSemester> Semesters { get; set; }
    }
    public class FieldWorkSemester
    {
        public string Value { get; set; }
        public string Label { get; set; }
    }
}
