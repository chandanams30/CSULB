using System;
using System.Collections.Generic;
using System.Text;
using CSULB_COE.Models;
using CSULB_COE.ViewModels;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Request.Login;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IUserLoginService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        AuthenticateResponse AuthenticateSSO(string CSULBID, string displayName, string mail, string lastName, string firstName);
        BaseResponse SaveUserRegistration(LoginUserRegistrationRequest request);
        BaseResponse SaveAuditLog(AuditLogRequest request);
    }
}
