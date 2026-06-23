using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.Application;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IApplicationService
    {
        List<ApplicationListResponse> GetApplications(int userId, int roleId);
        StudentNotificationResponse GetStudentNotification(string CSULBID);
        List<ApplicationProgram> GetApplicationsForAdminPanel(int userId);
    }
}
