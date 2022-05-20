using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.DataAccess.Models;

namespace ThoughtFocus.Repository.Interfaces.User
{
    public interface IUserDetailsRepository : IEFApplicationBaseRepository<ThoughtFocus.DataAccess.Models.User>
    {
        ThoughtFocus.DataAccess.Models.User GetUserDetails(int userId);
    }
}
