using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Repository.Interfaces.User;

namespace ThoughtFocus.Repository.Implementation.User
{
    public class UserActivityImpl : AbstractEFApplicationBaseRepository<UserActivityLog>, IUserActivityRepository
    {
        private CSULB_DBContext _Context;
        public UserActivityImpl(CSULB_DBContext context)
         : base(context)
        {
            this._Context = context;
        }

        public string AddActivityLog(long userId)
        {
            UserActivityLog activityLog = new UserActivityLog();
            activityLog.Id = 0;
            activityLog.UserId = userId;
            activityLog.LoginDateTime = System.DateTime.Now;
            try
            {
                Add(activityLog);
                this._Context.SaveChanges();
                return "Success";
            }
            catch(Exception ex)
            {
                // log error 
                return "Failed";
            }
           
        }
    }
}
