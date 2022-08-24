using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using CSULB_COE.Models;
using CSULB_COE.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Repository.Interfaces;
using ThoughtFocus.Repository.Interfaces.User;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class UserLoginServiceImpl : IUserLoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserDetailsRepository _userDetailsRepository;
        private readonly IUserActivityRepository _userActivityRepository;
        private readonly CSULB_DBContext _context;
        private readonly IConfiguration _config;
        private readonly ISqlDBUtility _helper;
        public UserLoginServiceImpl(IUserRepository userRepository,
                                    IUserDetailsRepository userDetailsRepository, 
                                    IUserActivityRepository userActivityRepository,
                                    CSULB_DBContext context,
                                    IConfiguration config,
                                    ISqlDBUtility helper
                                    )
        {
            _userRepository = userRepository;
            _userDetailsRepository = userDetailsRepository;
            _userActivityRepository = userActivityRepository;
            _config = config;
            _context = context;
            _helper = helper;
        }
        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            #region Old entity framework codes 
            //AuthenticateResponse response = new AuthenticateResponse();

            //// validate credential
            //UserCred _cred = _userRepository.GetUserLogin(model.Username, model.Password);
            //if (_cred == null)
            //{
            //    response.message = "Incorrect UserName / Password";
            //    return response;
            //}
            //else
            //{
            //    // adding data to UserActivityLog
            //    string activityStatus = _userActivityRepository.AddActivityLog(_cred.UserId);
            //    User _user = _userDetailsRepository.GetUserDetails(Convert.ToInt32(_cred.UserId));
            //    if (_user != null)
            //    { 
            //            List<Roles> roles = new List<Roles>();

            //            roles = GetUserRoles(_cred.UserId);
            //            response.UserName = model.Username;
            //            response.message = "Success";
            //            response.FirstName = _user.FirstName; 
            //            response.LastName = _user.LastName; 
            //            response.Roles = roles; // pull the roles based on the userID 
            //            response.JWTToken = GetJWTString(_user);
            //            response.UserId = Convert.ToInt32(_user.Id);
            //    }
            //}


            //return response;
            #endregion

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
                                                  Email = Convert.ToString(row["Email"]),
                                                  CSULBID= Convert.ToString(row["CSULBID"])
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
                    obj.CSULBID = _user.CSULBID;
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

        public AuthenticateResponse AuthenticateSSO(string CSULBID)
        {
            AuthenticateResponse obj = new AuthenticateResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@CSULBID", SqlDbType.NVarChar, 100) { Value = CSULBID }
                                        };
            DataSet dsUserValidationData = _helper.GetDataSet("[dbo].[AuthenticateSSOUsers]", parameters);

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
                                                  Email = Convert.ToString(row["Email"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"])
                                              }).FirstOrDefault();

                    obj.Roles = dsUserValidationData.Tables[1].AsEnumerable().Select(row =>
                                              new Roles
                                              {
                                                  RoleId = Convert.ToInt32(row["RoleId"]),
                                                  RoleName = Convert.ToString(row["RoleName"])
                                              }).ToList();

                    obj.UserId = Convert.ToInt32(_user.Id);
                   // obj.UserName = model.Username;
                    obj.FirstName = _user.FirstName;
                    obj.LastName = _user.LastName;
                    obj.Email = _user.Email;
                    obj.CSULBID = _user.CSULBID;
                    obj.JWTToken = GetJWTString(_user);


                    obj.message = "Data Retrieved Successfully";

                }
                else
                    obj.message = "You don't have access to MyCED, Please contact Administrator.";
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
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim("UserID",user.Id.ToString())
              }),
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var cToken = tokenHandler.CreateToken(tokenDescriptor);
            token = tokenHandler.WriteToken(cToken);

            return token;

        }

        private List<Roles> GetUserRoles(int userId)
        {
            List<Roles> roles = new List<Roles>();

            var query = _context.Roles
                         .Join(_context.UserRoles.Where(x=>x.UserId== userId),
                         role => role.Id,
                         userrole => userrole.RoleId,
                         (role, userrole) => new Roles
                         {
                             RoleId=Convert.ToInt32(role.Id),
                             RoleName=role.Description
                         }).ToList();

            return query;
        }

      
    }
}
