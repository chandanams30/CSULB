using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Repository.Interfaces.User;

namespace ThoughtFocus.Repository.Implementation.User
{
    public class UserDetailsImpl : AbstractEFApplicationBaseRepository<ThoughtFocus.DataAccess.Models.User>, IUserDetailsRepository
    {
        private CSULB_DBContext _Context;
        public UserDetailsImpl(CSULB_DBContext context)
         : base(context)
        {
            this._Context = context;
        }
      public ThoughtFocus.DataAccess.Models.User GetUserDetails(int userId)
        {
            var query = GetAll().FirstOrDefault(x => x.Id==Convert.ToInt32(userId));
            
            return query;
        }

    }
}
