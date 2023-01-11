using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.SearchApplication
{
    public class StudentSearchResponse:BaseResponse
    {
        public List<StudentSearch> studentSearch { get; set; }
    }
    public class StudentSearch
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EMAIL { get; set; }
        public string CSULBID { get; set; }
        public string Type { get; set; }
        public int UserID { get; set; }
        public string TermCode { get; set; }
        public int? ProgramID { get; set; }
        public int ApplicationTypeID { get; set; }
    }
}
