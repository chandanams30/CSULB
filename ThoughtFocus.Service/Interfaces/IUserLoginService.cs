using System;
using System.Collections.Generic;
using System.Text;
using CSULB_COE.Models;
using CSULB_COE.ViewModels;
using ThoughtFocus.Domain.Response;
//using ThoughtFocus.Domain.Request;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IUserLoginService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        AuthenticateResponse AuthenticateSSO(string CSULBID);
    }
}
