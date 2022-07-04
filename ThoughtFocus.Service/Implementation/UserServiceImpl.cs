using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Domain.Response.Users;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class UserServiceImpl : IUserService
    {
        private readonly ISqlDBUtility _helper;
        public UserServiceImpl(ISqlDBUtility helper)
        {
            _helper = helper;
        }
        public List<UsersResponse> GetUserById(int userId)
        {
            List<UsersResponse> obj = new List<UsersResponse>();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@roleid", SqlDbType.Int, 50) { Value =0  },
                                          new SqlParameter("@userId", SqlDbType.Int, 50) { Value = userId },
                                        };

            DataTable dtUsers = _helper.GetDataTable("[dbo].[GetUsersByRole]", parameters);
            if (dtUsers.Rows.Count > 0)
            {
                obj = dtUsers.AsEnumerable().Select(row =>
                                         new UsersResponse
                                         {
                                             UserId = Convert.ToInt32(row["UserId"]),
                                             Name = Convert.ToString(row["Name"]),
                                             EmailId = Convert.ToString(row["Email"]),
                                             RoleId = Convert.ToInt32(row["RoleId"]),
                                             Role = Convert.ToString(row["Description"])
                                         }).ToList();
            }

            return obj;
        }

        public List<UsersResponse> GetUsersByRole(int roleId)
        {
            List<UsersResponse> obj = new List<UsersResponse>();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@roleid", SqlDbType.Int, 50) { Value = roleId },
                                          new SqlParameter("@userId", SqlDbType.Int, 50) { Value = 0 },
                                        };

            DataTable dtUsers = _helper.GetDataTable("[dbo].[GetUsersByRole]", parameters);
            if (dtUsers.Rows.Count > 0)
            {
                obj = dtUsers.AsEnumerable().Select(row =>
                                         new UsersResponse
                                         {
                                            UserId= Convert.ToInt32(row["UserId"]),
                                            Name=Convert.ToString(row["Name"]),
                                            EmailId = Convert.ToString(row["Email"]),
                                            RoleId = Convert.ToInt32(row["RoleId"]),
                                            Role = Convert.ToString(row["Description"])
                                         }).ToList();
            }

            return obj;
        }
    }
}
