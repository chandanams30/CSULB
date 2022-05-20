using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.DataAccess.Models;

namespace ThoughtFocus.Repository.Interfaces
{
    public interface IUserRepository : IEFApplicationBaseRepository<UserCred>
    {
        UserCred GetUserLogin(string UserName, string Password);
    }
}
