using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.SearchApplication;

namespace ThoughtFocus.Service.Interfaces
{
    public interface ISearchApplication
    {
        StudentSearchResponse GetStudentSearchData(string searchString);
    }
}
