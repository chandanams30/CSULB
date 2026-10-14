using CSULB_COE.Models;
using CSULB_COE.ViewModels;
using iTextSharp.text.log;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using PasswordGenerator;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Request.Login;
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
            if (dtUserRegistration.Rows.Count > 0)
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
        public List<DistributionList> GetAllDistributionLists()
        {
            var distributionLists = new List<DistributionList>();
            try
            {

                //using (var entry = new DirectoryEntry())
                //using (var searcher = new DirectorySearcher(entry))
                //{
                //    // Search for all user accounts
                //    searcher.Filter = "(&(objectCategory=person)(objectClass=user))";
                //    searcher.PageSize = 1000;

                //    searcher.PropertiesToLoad.Add("sAMAccountName");
                //    searcher.PropertiesToLoad.Add("Name");
                //    searcher.PropertiesToLoad.Add("mail");
                //    searcher.PropertiesToLoad.Add("memberOf");

                //    using (SearchResultCollection users = searcher.FindAll())
                //    {
                //        foreach (SearchResult userResult in users)
                //        {
                //            string samAccountName =
                //                userResult.Properties["sAMAccountName"].Count > 0
                //                    ? userResult.Properties["sAMAccountName"][0]?.ToString() ?? ""
                //                    : "";

                //            string displayName =
                //                userResult.Properties["Name"].Count > 0
                //                    ? userResult.Properties["Name"][0]?.ToString() ?? ""
                //                    : "";

                //            string email =
                //                userResult.Properties["mail"].Count > 0
                //                    ? userResult.Properties["Email"][0]?.ToString() ?? ""
                //                    : "";

                //            _logger.LogInformation(
                //                "User: {DisplayName}, SamAccountName: {SamAccountName}, Email: {Email}",
                //                displayName,
                //                samAccountName,
                //                email);

                //            // Get all direct AD groups assigned to this user
                //            if (userResult.Properties.Contains("memberOf"))
                //            {
                //                foreach (var groupDn in userResult.Properties["memberOf"])
                //                {
                //                    string dn = groupDn?.ToString() ?? "";

                //                    if (string.IsNullOrWhiteSpace(dn))
                //                        continue;

                //                    try
                //                    {
                //                        using (var groupEntry = new DirectoryEntry("LDAP://" + dn))
                //                        using (var groupSearcher = new DirectorySearcher(groupEntry))
                //                        {
                //                            groupSearcher.Filter = "(objectCategory=group)";
                //                            groupSearcher.PropertiesToLoad.Add("cn");
                //                            groupSearcher.PropertiesToLoad.Add("sAMAccountName");

                //                            var groupResult = groupSearcher.FindOne();

                //                            if (groupResult != null)
                //                            {
                //                                string groupName =
                //                                    groupResult.Properties["cn"].Count > 0
                //                                        ? groupResult.Properties["cn"][0]?.ToString() ?? ""
                //                                        : "";

                //                                string groupSamAccountName =
                //                                    groupResult.Properties["sAMAccountName"].Count > 0
                //                                        ? groupResult.Properties["sAMAccountName"][0]?.ToString() ?? ""
                //                                        : "";

                //                                _logger.LogInformation(
                //                                    "User: {SamAccountName}, AD Group: {GroupName}, Group Account: {GroupAccount}",
                //                                    samAccountName,
                //                                    groupName,
                //                                    groupSamAccountName);
                //                            }
                //                        }
                //                    }
                //                    catch (Exception ex)
                //                    {
                //                        _logger.LogWarning(
                //                            ex,
                //                            "Unable to retrieve group details for user {SamAccountName}, Group DN: {GroupDN}",
                //                            samAccountName,
                //                            dn);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                using (var entry = new System.DirectoryServices.DirectoryEntry())
                using (var searcher = new DirectorySearcher(entry))
                {
                    // Search for all groups
                    searcher.Filter = "(objectCategory=group)";

                    searcher.PageSize = 1000;

                    searcher.PropertiesToLoad.Add("cn");
                searcher.PropertiesToLoad.Add("mail");
                searcher.PropertiesToLoad.Add("sAMAccountName");
                searcher.PropertiesToLoad.Add("groupType");

                _logger.LogInformation("Retrieve distribution lists from Active Directory.");

                using (SearchResultCollection results = searcher.FindAll())
                {
                    foreach (System.DirectoryServices.SearchResult result in results)
                    {
                        if (!result.Properties.Contains("groupType"))
                            continue;

                        int groupType = Convert.ToInt32(
                            result.Properties["groupType"][0]);

                        // Security-enabled groups have the high bit set.
                        bool isSecurityGroup =
                            (groupType & unchecked((int)0x80000000)) != 0;

                        // Keep distribution groups only
                        if (isSecurityGroup)
                            continue;

                        string name = result.Properties["cn"].Count > 0
                            ? result.Properties["cn"][0]?.ToString() ?? ""
                            : "";

                        string email = result.Properties["mail"].Count > 0
                            ? result.Properties["mail"][0]?.ToString() ?? ""
                            : "";

                        string samAccountName =
                            result.Properties["sAMAccountName"].Count > 0
                                ? result.Properties["sAMAccountName"][0]?.ToString() ?? ""
                                : "";
                        _logger.LogInformation("Name: " + name + "Email: " + email + "samAccountName" + samAccountName);

                        distributionLists.Add(new DistributionList
                        {
                            Name = name,
                            Email = email,
                            SamAccountName = samAccountName
                        });
                    }
                }
            }

                _logger.LogInformation("Retrieved {Count} distribution lists from Active Directory.",
                    distributionLists.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving distribution lists from Active Directory.");
            }

            return distributionLists;
        }

        public bool IsLoggedInUserEnabled(string loggedInSamAccountName)
        {
            bool isEnabled = false;

            try
            {
                using (var entry = new DirectoryEntry())
                using (var searcher = new DirectorySearcher(entry))
                {
                    // Search for user accounts
                    searcher.Filter = "(&(objectCategory=person)(objectClass=user))";
                    searcher.PageSize = 1000;

                    searcher.PropertiesToLoad.Add("cn");
                    searcher.PropertiesToLoad.Add("sAMAccountName");
                    searcher.PropertiesToLoad.Add("mail");
                    searcher.PropertiesToLoad.Add("userAccountControl");

                    using (SearchResultCollection results = searcher.FindAll())
                    {
                        foreach (System.DirectoryServices.SearchResult result in results)
                        {
                            string samAccountName =
                                result.Properties["sAMAccountName"].Count > 0 ? result.Properties["sAMAccountName"][0]?.ToString()
                                        ?? string.Empty
                                    : string.Empty;

                            // Compare with the logged-in user's account name
                            if (!string.Equals(
                                samAccountName,
                                loggedInSamAccountName,
                                StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            // Check whether the account is enabled
                            if (result.Properties["userAccountControl"].Count > 0)
                            {
                                int userAccountControl = Convert.ToInt32(
                                    result.Properties["userAccountControl"][0]);

                                // ACCOUNTDISABLE flag = 0x2
                                isEnabled = (userAccountControl & 0x2) == 0;
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error checking AD account status for {SamAccountName}",
                    loggedInSamAccountName);
            }

            return isEnabled;
        }


        public List<string> GetUsersByADGroup()
        {
            var samAccountNames = new List<string>();

            string groupDN =
                "CN=CED-TF-TEST-DataFeed,OU=Groups,OU=CED,OU=Delegated-OUs,DC=campus,DC=ad,DC=csulb,DC=edu";

            try
            {
                using (var entry = new DirectoryEntry())
                using (var searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter =
                        "(&(objectCategory=person)(objectClass=user)" +
                        "(memberOf=" + EscapeLdapFilterValue(groupDN) + "))";

                    searcher.PageSize = 1000;
                    searcher.PropertiesToLoad.Add("sAMAccountName");

                    using (SearchResultCollection results = searcher.FindAll())
                    {
                        foreach (SearchResult result in results)
                        {
                            try
                            {
                                if (result.Properties["sAMAccountName"].Count > 0)
                                {
                                    string samAccountName =
                                        result.Properties["sAMAccountName"][0]?.ToString() ?? "";

                                    if (!string.IsNullOrWhiteSpace(samAccountName))
                                    {
                                        samAccountNames.Add(samAccountName);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex,
                                    "Error processing an AD user. Continuing with the next user.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error retrieving users from AD group CED-TF-TEST-DataFeed. Continuing with the next process.");
            }

            _logger.LogInformation(
                "Retrieved {Count} AD users from CED-TF-TEST-DataFeed.",
                samAccountNames.Count);

            return samAccountNames;
        }

        private static string EscapeLdapFilterValue(string value)
    {
        return value
            .Replace("\\", "\\5c")
            .Replace("*", "\\2a")
            .Replace("(", "\\28")
            .Replace(")", "\\29")
            .Replace("\0", "\\00");
    }
}
}
