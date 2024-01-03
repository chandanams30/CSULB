using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.StudentProfile;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IStudentProfile
    {
        StudentProfileResponse GetStudentProfileData(string CsuldId);
        StudentProfileSearchResponse GetStudentProfileSearchData(string searchString);
    }
}
