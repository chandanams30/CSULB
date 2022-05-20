using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Repository.Interfaces;

namespace ThoughtFocus.Repository.Implementation
{
    public class UserRepositoryImpl : AbstractEFApplicationBaseRepository<UserCred>, IUserRepository
    {
        private CSULB_DBContext _Context;
        public UserRepositoryImpl(CSULB_DBContext context)
         : base(context)
        {
            this._Context = context;
        }

        public UserCred GetUserLogin(string UserName, string Password)
        {
            //var query = GetAll().FirstOrDefault(x => x.UserName == UserName && x.Password == Password);
            var query = this._Context.UserCreds.FirstOrDefault(x => x.UserName == UserName && x.Password == Password);
            return query;
        }
    }
}
