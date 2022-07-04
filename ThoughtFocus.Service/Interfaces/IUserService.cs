using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.Users;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IUserService
    {
        List<UsersResponse> GetUsersByRole(int roleId);
        List<UsersResponse> GetUserById(int userId);
    }
}
