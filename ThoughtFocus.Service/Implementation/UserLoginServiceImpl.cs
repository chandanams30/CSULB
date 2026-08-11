using CSULB_COE.Models;
using CSULB_COE.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PasswordGenerator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Enumeration;
using ThoughtFocus.Domain.Request.Login;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Admin;
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
        private readonly ISendMail _sendMail;
        public ILogger<UserLoginServiceImpl> _logger;
        private readonly IConfiguration _configuration;
        public UserLoginServiceImpl(IUserRepository userRepository,
                                    IUserDetailsRepository userDetailsRepository, 
                                    IUserActivityRepository userActivityRepository,
                                    CSULB_DBContext context,
                                    IConfiguration config,
                                    ISqlDBUtility helper,
                                    ISendMail sendMail,
                                    ILogger<UserLoginServiceImpl> logger,
                                    IConfiguration configuration
                                    )
        {
            _userRepository = userRepository;
            _userDetailsRepository = userDetailsRepository;
            _userActivityRepository = userActivityRepository;
            _config = config;
            _context = context;
            _helper = helper;
            _sendMail = sendMail;
            _logger = logger;
            _configuration = configuration;
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
                    var jsonObj = JObject.Parse(File.ReadAllText(@"SupportFiles/MycedConfigurations/CSULBCEDConfig.json"));
                    var superAdminCSULBIDs = jsonObj["SuperAdminCSULBID"]?.ToString().Split(',').Select(id => id.Trim());

                    // Get the CSULBID from the DataTable
                    var currentCSULBID = Convert.ToString(dsUserValidationData.Tables[0].Rows[0]["CSULBID"]);
                    User _user = dsUserValidationData.Tables[0].AsEnumerable().Select(row =>
                                              new User
                                              {
                                                  Id = Convert.ToInt64(row["UserId"]),
                                                  FirstName = Convert.ToString(row["FirstName"]),
                                                  LastName = Convert.ToString(row["LastName"]),
                                                  Email = Convert.ToString(row["Email"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),
                                              }).FirstOrDefault();
                    obj.Roles = dsUserValidationData.Tables[1].AsEnumerable().Select(row =>
                                              new Roles
                                              {
                                                  RoleId = Convert.ToInt32(row["RoleId"]),
                                                  RoleName = Convert.ToString(row["RoleName"])
                                              }).ToList();
                    // Check if the current CSULBID is in the SuperAdmin list
                    if((currentCSULBID == "" && Convert.ToInt32(dsUserValidationData.Tables[1].Rows[0]["RoleId"]) == 1) || superAdminCSULBIDs.Contains(currentCSULBID)) 
                    {
                        obj.IsSuperAdmin = true;
                    }

                    obj.UserId = Convert.ToInt32(_user.Id);
                    obj.UserName = model.Username;
                    obj.FirstName = _user.FirstName;
                    obj.LastName = _user.LastName;
                    obj.Email = _user.Email;
                    obj.CSULBID = _user.CSULBID;
                    obj.JWTToken = GetJWTString(_user);

                    obj.IsSuccess = true;
                    obj.message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.message = "Incorrect UserName / Password";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.message = "Data Retrieval Failed , Please contact site admin ";
                //obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public AuthenticateResponse AuthenticateSSO(string CSULBID,string displayName,string mail,string lastName,string firstName)
        {
            AuthenticateResponse obj = new AuthenticateResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@CSULBID", SqlDbType.NVarChar, 50) { Value = CSULBID },
                                          new SqlParameter("@displayName", SqlDbType.NVarChar, 255) { Value = displayName },
                                          new SqlParameter("@mail", SqlDbType.NVarChar, 255) { Value = mail },
                                          new SqlParameter("@LastName", SqlDbType.NVarChar, 255) { Value = lastName },
                                          new SqlParameter("@FirstName", SqlDbType.NVarChar, 255) { Value = firstName },
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

                    obj.IsSuccess = true;
                    obj.message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.message = "You don't have access to MyCED, Please contact Administrator.";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
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
                Expires = DateTime.UtcNow.AddMinutes(60),
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

        public BaseResponse SaveUserRegistration(LoginUserRegistrationRequest request)
        {
            BaseResponse obj = new BaseResponse();
            string userPassword = string.Empty;
            // get the autogenerated password here 
            var pwd = new Password(includeLowercase: true, includeUppercase: true, includeNumeric: true, includeSpecial: true, passwordLength: 12);
            userPassword = pwd.Next();
            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@FirstName", SqlDbType.NVarChar, 255) { Value = request.FirstName },
                                          new SqlParameter("@LastName", SqlDbType.NVarChar, 255) { Value = request.LastName },
                                          new SqlParameter("@Email", SqlDbType.NVarChar, 255) { Value = request.Email },
                                          new SqlParameter("@CSULBID", SqlDbType.NVarChar, 50) { Value = request.CSULBID },
                                          new SqlParameter("@Password", SqlDbType.NVarChar, 50) { Value = userPassword }
                                        };
            DataTable dtUserRegistration = _helper.GetDataTable("[dbo].[SaveUserRegistration]", parameters);
            if (dtUserRegistration !=null && dtUserRegistration.Rows.Count > 0)
            {
                int status = Convert.ToInt32(dtUserRegistration.Rows[0]["Status"]);
                string message = Convert.ToString(dtUserRegistration.Rows[0]["Message"]);
                // if status is 1 , then send email to user with password 
                // status is 0 , then set the base response to false and error msg back
                if (status == 1)
                {
                    string logoText = "cid:myImageID";
                    // trigger email with the user name 
                    string bodyUsername = GetMailBodyTemplate("Student_Username.html");
                    string subjectUserName = "Username for MyCED Application";
                    bodyUsername = bodyUsername.Replace("[[logoPath]]", logoText)
                               .Replace("[[ApplicantName]]", request.FirstName + " " + request.LastName)
                               .Replace("[[UserName]]", request.CSULBID);
                    // mail send to student email
                    //_sendMail.SendEmail(request.Email, "", "COMMON", subjectUserName, bodyUsername, "");
                    var BCCCommonMails = _configuration["EmailNotifications:BCCCommon"];
                    _sendMail.SendEmail(BCCCommonMails, "", "COMMON", subjectUserName, bodyUsername, "");// mail to BCCCommon email
                    // trigger email with the password 
                    string bodyPassword = GetMailBodyTemplate("Student_Password.html");
                    string subjectPassword = "Password for MyCED Application";
                    bodyPassword = bodyPassword.Replace("[[logoPath]]", logoText)
                               .Replace("[[ApplicantName]]", request.FirstName + " " + request.LastName)
                               .Replace("[[Password]]", userPassword);
                    //_sendMail.SendEmail(request.Email, "","COMMON", subjectPassword, bodyPassword, "");
                    _sendMail.SendEmail(BCCCommonMails, "", "COMMON", subjectPassword, bodyPassword, "");// mail to BCCCommon email
                    obj.IsSuccess = true;
                    obj.Message = message;
                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = message;
                }
                 
            }
            return obj;
        }
        private string GetMailBodyTemplate(string templateName)
        {
            string body = string.Empty;
            string filepath = Path.Combine("SupportFiles/EmailTemplates", templateName);
            using (StreamReader reader = new StreamReader(Path.GetFullPath(filepath)))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }

        public BaseResponse SaveAuditLog(AuditLogRequest request)
        {
            BaseResponse obj = new BaseResponse();
            SqlParameter[] parameters =
                                        {
                                          
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = request.UserID },
                                          new SqlParameter("@Type", SqlDbType.NVarChar, 25) { Value = request.Type },
                                          new SqlParameter("@IPAddress", SqlDbType.NVarChar, 25) { Value = request.IPAddress },
                                          new SqlParameter("@AuthenticationType", SqlDbType.NVarChar, 25) { Value = request.AuthenticationType }
                                        };
            int ID = _helper.InsertTable("[User].[InsertLoginLogout]", parameters);
            obj.IsSuccess = true;
            obj.Message = "";
            return obj;
        }
        public BaseResponse SaveRolesFromAD(AuthenticateRequestRoles request)
        {
            BaseResponse obj = new BaseResponse();
            DataTable roleTable = new DataTable();


            //roleTable.Columns.Add("RoleID", typeof(long));
            //roleTable.Columns.Add("ApplicationTypeID", typeof(long));


            //foreach (var role in request.roleIDList)
            //{
            //    roleTable.Rows.Add(role.RoleId, role.ApplicationTypeId);
            //}
            //string rolesJson = JsonConvert.SerializeObject(request.roleIDList);
            //        string rolesJson = "[" + string.Join(",", request.roleIDList.Select(r =>
            //$"{{\"RoleId\":{r.RoleId},\"ApplicationTypeId\":{r.ApplicationTypeId}}}")) + "]";
            //        rolesJson.Replace("\"","");

            string rolesJson = ConvertToJson(request.roleIDList);

            SqlParameter[] parameters =
 {
    new SqlParameter("@Email", SqlDbType.NVarChar, 255) { Value = request.Email?.Trim() ?? "" },
    new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = request.RoleID },
    new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt) { Value = request.ApplicationTypeID },
    new SqlParameter("@CSULBID", SqlDbType.VarChar, 50) { Value = request.CSULBID?.Trim() ?? "" }
};
            var ID = _helper.InsertTable("[dbo].[SaveUserRolesFromAD]", parameters);
            //using (var conn = new SqlConnection(connectionString))
            //{
            //    conn.Open();
            //    using (var cmd = new SqlCommand("[dbo].[SaveUserRolesFromAD]", conn))
            //    {
            //        cmd.CommandType = CommandType.StoredProcedure;

            //        cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 255) { Value = request.Email.Trim() });
            //        cmd.Parameters.Add(new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = request.RoleID });
            //        cmd.Parameters.Add(new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt) { Value = request.ApplicationTypeID });
            //        cmd.Parameters.Add(new SqlParameter("@CSULBID", SqlDbType.VarChar, 50) { Value = request.CSULBID.Trim() });

            //        int rowsAffected = cmd.ExecuteNonQuery();
            //        Console.WriteLine($"Rows affected: {rowsAffected}");
            //    }
            //}

            obj.IsSuccess = true;
            obj.Message = "";
            return obj;
        }
        public string ConvertToJson<T>(T obj)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using var ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public RoleADResponse GetIntegratedUserRoles(UserInfoRequest request)
        {
            RoleADResponse obj = new RoleADResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@Email", SqlDbType.NVarChar, 255) { Value = request.Email },
                                            new SqlParameter("@CSULBID", SqlDbType.VarChar, 50) { Value = request.CSULBID }
                                        };

            DataTable dtRolesList = _helper.GetDataTable("[dbo].[GetIntegratedUserRoles]", parameters);
            try
            {
                if (dtRolesList !=null && dtRolesList.Rows.Count > 0)
                {


                    obj.RolesList = dtRolesList.AsEnumerable().Select(row =>
                                              new Roles
                                              {
                                                  RoleId = Convert.ToInt16(row["RoleID"]),
                                                  RoleName = Convert.ToString(row["RoleName"])
                                              }).ToList();
                    obj.RoleATIDList = dtRolesList.AsEnumerable().Select(row =>
                                              new RoleATID
                                              {
                                                  RoleId = Convert.ToInt16(row["RoleID"]),
                                                  ApplicationTypeId = Convert.ToInt16(row["ApplicationTypeId"])
                                              }).ToList();

                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }
       public long GetRoleIdFromGroup(string groupName)
        {
            
                if (string.IsNullOrWhiteSpace(groupName))
                    return 0;

                // Expected format: CED-TF-RoleName-ApplicationType
                var parts = groupName.Split('-');

                if (parts.Length < 4)
                    return 0;

                string rolePart = parts[2].Trim(); // Extract role segment

                return rolePart switch
                {
                    "ProgramAdmin" => RoleConstants.ProgramAdmin,
                    "Reviewer" => RoleConstants.Reviewer,
                    "ProgramCoordinator" => RoleConstants.ProgramCoordinator,
                    "Administrator" => RoleConstants.Administrator,
                    "FacultySupervisor" => RoleConstants.FacultySupervisor,
                    "Instructor" => RoleConstants.Instructor,
                    "StudentProfileStaff" => RoleConstants.StudentProfileStaff,
                    "StudentProfileGradeAdmin" => RoleConstants.StudentProfileGradeAdmin,
                    _ => 0 // Unknown role
                };
            
        }
        public long GetApplicationTypeIdFromGroup(string groupName)
        {
            
                if (string.IsNullOrWhiteSpace(groupName))
                    return 0;

                // Expected format: CED-TF-RoleName-ApplicationType
                var parts = groupName.Split('-');

                if (parts.Length < 4)
                    return 0;

                string appTypePart = parts[3].Trim(); // Extract application type segment

                return appTypePart switch
                {
                    "Doctoral" => ApplicationTypeConstants.Doctoral,
                    "ICP" => ApplicationTypeConstants.ICP,
                    "Graduate" => ApplicationTypeConstants.Graduate,
                    _ => 0 // Unknown application type
                };
          
        }


    }
}
