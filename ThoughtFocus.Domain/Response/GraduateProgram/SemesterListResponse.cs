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
    public class RecommenderMailBodyResponse : BaseResponse
    {
        public RecommenderBody recommenderResponse { get; set; }
    }
    public class RecommenderBody
    {
        public int ID { get; set; }
        public string RecommenderMailBody { get; set; }
    }
}
