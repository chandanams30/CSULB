using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.GraduateProgram
{
    public class SemesterListResponse : BaseResponse
    {
        public List<Semester> Semesters { get; set; }
    }
    public class Semester
    {
        public string TermCode { get; set; }
        public string TermName { get; set; }
    }
}
