using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using CSULB_COE.Models;
using CSULB_COE.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Repository.Interfaces;
using ThoughtFocus.Repository.Interfaces.User;
using ThoughtFocus.Service.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using ThoughtFocus.DataAccess.DBHelper;

namespace ThoughtFocus.Service.Implementation
{
    public class UserLoginServiceImpl : IUserLoginService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _config;

        public UserLoginServiceImpl(ISqlDBUtility helper, 
                                    IConfiguration config 
                                    )
        {
            _helper = helper;
            _config = config;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            AuthenticateResponse obj = new AuthenticateResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@Username", SqlDbType.NVarChar, 100) { Value = model.Username },
                                          new SqlParameter("@Password", SqlDbType.NVarChar, 100) { Value = model.Password }
                                        };
            DataSet dsUserValidationData = _helper.GetDataSet("[dbo].[AuthenticateUsers]", parameters);

            try
            {
                if (dsUserValidationData.Tables.Count > 0)
                {
                    User _user = dsUserValidationData.Tables[0].AsEnumerable().Select(row =>
                                              new User
                                              {
                                                  Id = Convert.ToInt64(row["UserId"]),
                                                  FirstName = Convert.ToString(row["FirstName"]),
                                                  LastName = Convert.ToString(row["LastName"]),
                                                  Email = Convert.ToString(row["Email"])
                                              }).FirstOrDefault();

                    obj.Roles = dsUserValidationData.Tables[1].AsEnumerable().Select(row =>
                                              new Roles
                                              {
                                                  RoleId = Convert.ToInt32(row["RoleId"]),
                                                  RoleName = Convert.ToString(row["RoleName"])
                                              }).ToList();

                    obj.UserId = Convert.ToInt32(_user.Id);
                    obj.UserName = model.Username;
                    obj.FirstName = _user.FirstName;
                    obj.LastName = _user.LastName;
                    obj.Email = _user.Email;
                    obj.JWTToken = GetJWTString(_user);

     
                    obj.message = "Data Retrieved Successfully";

                }
                else
                    obj.message = "Incorrect UserName / Password";
            }
            catch (Exception ex)
            {
  
                obj.message = "Data Retrieval Failed , Please contact site admin ";
                //obj.StackTrace = ex.Message;
            }
            return obj;
        }

        private string GetJWTString(User user)
        {
            string token = string.Empty;

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_config["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
              {
                    new Claim(ClaimTypes.Name,user.Email)
              }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var cToken = tokenHandler.CreateToken(tokenDescriptor);
            token = tokenHandler.WriteToken(cToken);

            return token;

        }

        //private List<Roles> GetUserRoles(int userId)
        //{
        //    List<Roles> roles = new List<Roles>();

        //    var query = _context.Roles
        //                 .Join(_context.UserRoles.Where(x=>x.UserId== userId),
        //                 role => role.Id,
        //                 userrole => userrole.RoleId,
        //                 (role, userrole) => new Roles
        //                 {
        //                     RoleId=Convert.ToInt32(role.Id),
        //                     RoleName=role.Description
        //                 }).ToList();

        //    return query;
        //}

      
    }
}
