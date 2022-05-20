using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Repository.Interfaces.User
{
    public interface IUserActivityRepository:IEFApplicationBaseRepository<ThoughtFocus.DataAccess.Models.UserActivityLog>
    {
        string AddActivityLog(long userId);
    }
}
