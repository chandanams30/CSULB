//using CSULB_COE.ViewModels;
using Castle.Core.Internal;
using CSULB_COE.Models;
using CSULB_COE.ViewModels;
using Google.Apis.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using Owin;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.ConstrainedExecution;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Request;
using ThoughtFocus.Domain.Request.Login;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Admin;
using ThoughtFocus.Service.Interfaces;


namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserLoginService _userLoginService;
        public ILogger<LoginController> _logger;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;

        public LoginController(IUserLoginService userLoginService, ILogger<LoginController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _userLoginService = userLoginService;
            _logger = logger;
            _client = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        //[HttpPost("Authenticate")]
        //public IActionResult Login([FromBody] LoginRequest request)
        //{
        //    //_logger.LogInformation("Start : Authenticating for {userName}",userName);
        //    ViewModels.AuthenticateRequest authModel = new ViewModels.AuthenticateRequest();
        //    AuthenticateRequestRoles authenticateRequestRoles = new AuthenticateRequestRoles();
        //    authModel.Username = request.UserName;
        //    authModel.Password = request.Password;
        //    try
        //    {
        //        var response = _userLoginService.Authenticate(authModel);

        //        if (response.IsSuccess == true)
        //        {
        //            AuditLogRequest req = new AuditLogRequest();
        //            req.UserID = response.UserId;
        //            req.Type = "Login";
        //            req.IPAddress = GetClientIPAddress();
        //            req.AuthenticationType = "Basic";
        //            var auditResponse = _userLoginService.SaveAuditLog(req);

        //        }

        //        //var groups = new List<string>();
        //        var groupsTest = new List<string>();
        //        //string username = "yash.jayaram@csulb.edu";
        //        string username = _configuration["ApplicationKeys:LoggedInUserName"];
        //        _logger.LogInformation("Fetching MEMBER OF groups for {Username}", username);

        //        //using (var entry = new DirectoryEntry()) // implicit credentials
        //        //using (var searcher = new DirectorySearcher(entry))
        //        //{
        //        //    searcher.Filter =
        //        //        $"(&(objectClass=user)(userPrincipalName={username}))";

        //        //    searcher.PropertiesToLoad.Add("memberOf");

        //        //    var result = searcher.FindOne();

        //        //    if (result == null)
        //        //    {
        //        //        _logger.LogWarning("AD user not found: {Username}", username);
        //        //    }
        //        //    else
        //        //    {
        //        //        if (!result.Properties.Contains("memberOf"))
        //        //        {
        //        //            _logger.LogInformation(
        //        //                "User {Username} has no direct group memberships",
        //        //                username
        //        //            );
        //        //        }
        //        //        foreach (var groupDn in result.Properties["memberOf"])
        //        //        {
        //        //            if (groupDn == null)
        //        //                continue;

        //        //            var dn = groupDn.ToString();
        //        //            if (string.IsNullOrWhiteSpace(dn))
        //        //                continue;

        //        //            var commaIndex = dn.IndexOf(',');
        //        //            if (commaIndex < 0)
        //        //                continue;

        //        //            var groupName = dn.Substring(3, commaIndex - 3);
        //        //            if (groupName.StartsWith("CED-TF-", StringComparison.OrdinalIgnoreCase))
        //        //            {
        //        //                groups.Add(groupName);
        //        //                _logger.LogInformation("Directory Search");
        //        //                _logger.LogInformation("Assigned Group: {GroupName}", groupName);
        //        //            }

        //        //        }
        //        //        _logger.LogInformation(
        //        //    "User {Username} MEMBER OF groups: {Groups}",
        //        //    username,
        //        //    string.Join(" | ", groups)
        //        //);
        //        //    }
        //        //}
        //        var groups = _configuration["ApplicationKeys:ADGroups"].ToString();
        //        groupsTest = groups
        //              .Split(',')
        //              .Select(id => id.Trim())
        //              .ToList();
        //        List<RoleATID> rolesList = new List<RoleATID>();
        //            List<string> roles = new List<string>();
        //            ViewModels.UserInfoRequest userInfo = new ViewModels.UserInfoRequest();

        //            foreach (var groupName in groupsTest)
        //            {
        //                long roleId = _userLoginService.GetRoleIdFromGroup(groupName);
        //            long applicationTypeId = _userLoginService.GetApplicationTypeIdFromGroup(groupName);

        //                // Skip invalid or non-matching groups
        //                if (roleId == 0 || applicationTypeId == 0)
        //                    continue;

        //                rolesList.Add(new RoleATID
        //                {
        //                    RoleId = roleId,
        //                    ApplicationTypeId = applicationTypeId,
        //                });
        //            authenticateRequestRoles.Email = response.Email;
        //            authenticateRequestRoles.CSULBID = response.CSULBID;
        //            authenticateRequestRoles.RoleID = roleId;
        //            authenticateRequestRoles.ApplicationTypeID = applicationTypeId;

        //            var upsertADRoles = _userLoginService.SaveRolesFromAD(authenticateRequestRoles);
        //        }

        //        userInfo.Email = response.Email;
        //        userInfo.CSULBID = response.CSULBID;
        //        var getADRoles = _userLoginService.GetIntegratedUserRoles(userInfo);
        //            response.Roles = getADRoles.RolesList;

        //        return Ok(response);

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return BadRequest();
        //    }
        //}
        [HttpPost("Authenticate")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            //_logger.LogInformation("Start : Authenticating for {userName}",userName);
            ViewModels.AuthenticateRequest authModel = new ViewModels.AuthenticateRequest();
            AuthenticateRequestRoles authenticateRequestRoles = new AuthenticateRequestRoles();
            authModel.Username = request.UserName;
            authModel.Password = request.Password;
            try
            {
                var response = _userLoginService.Authenticate(authModel);

                if (response.IsSuccess == true)
                {
                    AuditLogRequest req = new AuditLogRequest();
                    req.UserID = response.UserId;
                    req.Type = "Login";
                    req.IPAddress = GetClientIPAddress();
                    req.AuthenticationType = "Basic";
                    var auditResponse = _userLoginService.SaveAuditLog(req);

                }
                var groups = new List<string>();
                string _samAccountName = string.Empty;
                string username = _configuration["ApplicationKeys:LoggedInUserName"];
                //string username = "yash.jayaram@csulb.edu";
                //string username = response.Email;
                _logger.LogInformation("Fetching MEMBER OF groups for {Username}", username);

                using (var entry = new DirectoryEntry()) // implicit credentials
                using (var searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter =
                        $"(&(objectClass=user)(userPrincipalName={username}))";

                    searcher.PropertiesToLoad.Add("memberOf");

                    var result = searcher.FindOne();

                    if (result == null)
                    {
                        _logger.LogWarning("AD user not found: {Username}", username);
                    }
                    else
                    {
                        if (!result.Properties.Contains("memberOf"))
                        {
                            _logger.LogInformation(
                                "User {Username} has no direct group memberships",
                                username
                            );
                        }
                        foreach (var groupDn in result.Properties["memberOf"])
                        {
                            if (groupDn == null)
                                continue;

                            var dn = groupDn.ToString();
                            if (string.IsNullOrWhiteSpace(dn))
                                continue;

                            var commaIndex = dn.IndexOf(',');
                            if (commaIndex < 0)
                                continue;

                            var groupName = dn.Substring(3, commaIndex - 3);
                            //if (groupName.StartsWith(_configuration["ApplicationKeys:ADGroupsKey"], StringComparison.OrdinalIgnoreCase))
                            //{
                            //    groups.Add(groupName);
                            //    _logger.LogInformation("Directory Search");
                            //    _logger.LogInformation("Assigned Group: {GroupName}", groupName);
                            //}

                            //Find SamAccountName
                            using (var groupEntry = new DirectoryEntry($"LDAP://{dn}"))
                            using (var groupSearcher = new DirectorySearcher(groupEntry))
                            {
                                groupSearcher.Filter = "(objectClass=group)";
                                groupSearcher.PropertiesToLoad.Add("sAMAccountName");

                                var groupResult = groupSearcher.FindOne();

                                if (groupResult != null &&
                                    groupResult.Properties.Contains("sAMAccountName"))
                                {
                                    var samAccountName =
                                        groupResult.Properties["sAMAccountName"][0]?.ToString();
                                    _samAccountName = samAccountName;

                                    _logger.LogInformation(
                                        "AD Group: {GroupName}, sAMAccountName: {SamAccountName}",
                                        groupName,
                                        samAccountName);
                                }

                                var adGroupsKey = _configuration["ApplicationKeys:ADGroupsKey"];

                                var allowedGroups = (_configuration["ApplicationKeys:AllowedADGroups"] ?? "")
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(x => x.Trim());

                                if ((!string.IsNullOrEmpty(adGroupsKey) &&
                                 groupName.StartsWith(adGroupsKey, StringComparison.OrdinalIgnoreCase))
                                || allowedGroups.Any(g => string.Equals(
                                    groupName,
                                    g,
                                    StringComparison.OrdinalIgnoreCase)))
                                {
                                    groups.Add(groupName);
                                    _logger.LogInformation("Directory Search");
                                    _logger.LogInformation("Assigned Group: {GroupName}", groupName);
                                }

                            }
                            _logger.LogInformation(
                        "User {Username} MEMBER OF groups: {Groups}",
                        username,
                        string.Join(" | ", groups)
                    );
                        }
                    }
                }
                List<RoleATID> rolesList = new List<RoleATID>();
                ViewModels.UserInfoRequest userInfo = new ViewModels.UserInfoRequest();
                //////////TEST
                var groupsTest = new List<string>();
                var adGroupsKey1 = _configuration["ApplicationKeys:ADGroupsKey"];
                var groupName1 = "CN=CED-TF-TEST-DataFeed";
                // ---------------------------
                var allowedGroups1 = (_configuration["ApplicationKeys:AllowedADGroups"] ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim());

                if ((!string.IsNullOrEmpty(adGroupsKey1) &&
                 groupName1.StartsWith(adGroupsKey1, StringComparison.OrdinalIgnoreCase))
                || allowedGroups1.Any(g => string.Equals(
                    groupName1,
                    g,
                    StringComparison.OrdinalIgnoreCase)))
                {
                    groups.Add(groupName1);
                    _logger.LogInformation("Directory Search");
                    _logger.LogInformation("Assigned Group: {GroupName}", groupName1);
                }

                if (groups != null)
                {
                    groupsTest.Add("CED-TF-ProgramCoordinator-Doctoral");
                    //groupsTest.Add("CED-TF-ProgramAdmin-Graduate");
                    //groupsTest.Add(groupName1);
                    ////////// END
                    var SamAccountNameLstForGradeAdmin = (_configuration["ApplicationKeys:SamAccountNameForGradeAdmin"] ?? "")
                                                             .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                             .Select(x => x.Trim());
                    //foreach (var groupName in groups)
                    foreach (var groupName in groupsTest)
                    {
                        long roleId = 0;
                        //test _samAccountName
                        _samAccountName = "008728484";
                        if (SamAccountNameLstForGradeAdmin.Any(x =>
                            string.Equals(
                                x,
                                _samAccountName,
                                StringComparison.OrdinalIgnoreCase)))
                        {
                            roleId = RoleConstants.StudentProfileGradeAdmin;
                        }
                        else
                        {
                            roleId = _userLoginService.GetRoleIdFromGroup(groupName);
                        }

                        long applicationTypeId = _userLoginService.GetApplicationTypeIdFromGroup(groupName);

                        // Skip invalid or non-matching groups
                        if (roleId == 0 || applicationTypeId == 0)
                            continue;

                        rolesList.Add(new RoleATID
                        {
                            RoleId = roleId,
                            ApplicationTypeId = applicationTypeId,
                        });
                        authenticateRequestRoles.Email = response.Email;
                        authenticateRequestRoles.CSULBID = response.CSULBID;
                        authenticateRequestRoles.RoleID = roleId;
                        authenticateRequestRoles.ApplicationTypeID = applicationTypeId;

                        var upsertADRoles = _userLoginService.SaveRolesFromAD(authenticateRequestRoles);
                    }

                    userInfo.Email = response.Email;
                    userInfo.CSULBID = response.CSULBID;
                    var getADRoles = _userLoginService.GetIntegratedUserRoles(userInfo);
                    response.Roles = getADRoles.RolesList;
                }
                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }


        [HttpPost("AuthenticateSSOToken")]
        public async Task<IActionResult> AuthenticateSSOToken([FromBody] LoginSSORequest request)
        {
            AuthenticateResponse response = new AuthenticateResponse();
            try
            {
                _logger.LogInformation(request.Token.ToString(), request);
                string accessToken = request.Token;
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                var graphResponse = await _client.GetAsync("https://graph.microsoft.com/beta/me");
                string emplid = string.Empty;
                string displayName = string.Empty;
                string mail = string.Empty;
                string LastName = string.Empty;
                string FirstName = string.Empty;
                if (graphResponse.IsSuccessStatusCode)
                {
                    
                    var graphResults=graphResponse.Content.ReadAsStringAsync().Result;
                    var json = new NewtonsoftJsonSerializer();
                    dynamic dynObj = JsonConvert.DeserializeObject(graphResults);
                    foreach (var data in dynObj)
                    {
                        if (data.Name == "employeeId")
                        {
                            emplid = data.Value;
                        }
                        if (data.Name == "displayName")
                        {
                            displayName = data.Value;
                        }
                        if (data.Name == "mail")
                        {
                            mail = data.Value;
                        }
                        if (data.Name == "surname")
                        {
                            LastName = data.Value;
                        }
                        if (data.Name == "givenName")
                        {
                            FirstName = data.Value;
                        }
                    }

                response = _userLoginService.AuthenticateSSO(emplid,displayName,mail,LastName,FirstName);
                    if (response.IsSuccess == true)
                    {
                        AuditLogRequest req = new AuditLogRequest();
                        req.UserID = response.UserId;
                        req.Type = "Login";
                        req.IPAddress=GetClientIPAddress();
                        req.AuthenticationType = "SSO";
                        var auditResponse = _userLoginService.SaveAuditLog(req);

                    }
                    var groups = new List<string>();
                    string _samAccountName = string.Empty;

                    string username = _configuration["ApplicationKeys:LoggedInUserName"];
                    //string username = mail;
                    _logger.LogInformation("Fetching MEMBER OF groups for {Username}", username);

                    using (var entry = new DirectoryEntry()) // implicit credentials
                    using (var searcher = new DirectorySearcher(entry))
                    {
                        searcher.Filter =
                            $"(&(objectClass=user)(userPrincipalName={username}))";

                        searcher.PropertiesToLoad.Add("memberOf");

                        var result = searcher.FindOne();

                        if (result == null)
                        {
                            _logger.LogWarning("AD user not found: {Username}", username);
                        }
                        else
                        {
                            if (!result.Properties.Contains("memberOf"))
                            {
                                _logger.LogInformation(
                                    "User {Username} has no direct group memberships",
                                    username
                                );
                            }
                            foreach (var groupDn in result.Properties["memberOf"])
                            {
                                if (groupDn == null)
                                    continue;

                                var dn = groupDn.ToString();
                                if (string.IsNullOrWhiteSpace(dn))
                                    continue;

                                var commaIndex = dn.IndexOf(',');
                                if (commaIndex < 0)
                                    continue;

                                var groupName = dn.Substring(3, commaIndex - 3);

                                //Find SamAccountName
                                using (var groupEntry = new DirectoryEntry($"LDAP://{dn}"))
                                using (var groupSearcher = new DirectorySearcher(groupEntry))
                                {
                                    groupSearcher.Filter = "(objectClass=group)";
                                    groupSearcher.PropertiesToLoad.Add("sAMAccountName");

                                    var groupResult = groupSearcher.FindOne();

                                    if (groupResult != null &&
                                        groupResult.Properties.Contains("sAMAccountName"))
                                    {
                                        var samAccountName =
                                            groupResult.Properties["sAMAccountName"][0]?.ToString();
                                        _samAccountName = samAccountName;

                                        _logger.LogInformation(
                                            "AD Group: {GroupName}, sAMAccountName: {SamAccountName}",
                                            groupName,
                                            samAccountName);
                                    }

                                    var adGroupsKey = _configuration["ApplicationKeys:ADGroupsKey"];

                                    var allowedGroups = (_configuration["ApplicationKeys:AllowedADGroups"] ?? "")
                                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(x => x.Trim());

                                    if ((!string.IsNullOrEmpty(adGroupsKey) &&
                                     groupName.StartsWith(adGroupsKey, StringComparison.OrdinalIgnoreCase))
                                    || allowedGroups.Any(g => string.Equals(
                                        groupName,
                                        g,
                                        StringComparison.OrdinalIgnoreCase)))
                                    {
                                        groups.Add(groupName);
                                        _logger.LogInformation("Directory Search");
                                        _logger.LogInformation("Assigned Group: {GroupName}", groupName);
                                    }
                                }

                            }
                            _logger.LogInformation(
                        "User {Username} MEMBER OF groups: {Groups}",
                        username,
                        string.Join(" | ", groups)
                    );
                        }
                    }
                    if (groups != null)
                    {
                        AuthenticateRequestRoles authenticateRequestRoles = new AuthenticateRequestRoles();
                        List<RoleATID> rolesList = new List<RoleATID>();
                        ViewModels.UserInfoRequest userInfo = new ViewModels.UserInfoRequest();
                        var SamAccountNameLstForGradeAdmin = (_configuration["ApplicationKeys:SamAccountNameForGradeAdmin"] ?? "")
                                                             .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                             .Select(x => x.Trim());

                        foreach (var groupName in groups)
                        {
                            long roleId = 0;
                            if (SamAccountNameLstForGradeAdmin.Any(x =>
                                string.Equals(
                                    x,
                                    _samAccountName,
                                    StringComparison.OrdinalIgnoreCase)))
                            {
                                roleId = RoleConstants.StudentProfileGradeAdmin;
                            }
                            else
                            {
                                roleId = _userLoginService.GetRoleIdFromGroup(groupName);
                            }
                            long applicationTypeId = _userLoginService.GetApplicationTypeIdFromGroup(groupName);

                            // Skip invalid or non-matching groups
                            if (roleId == 0 || applicationTypeId == 0)
                                continue;

                            rolesList.Add(new RoleATID
                            {
                                RoleId = roleId,
                                ApplicationTypeId = applicationTypeId,
                            });
                            authenticateRequestRoles.Email = response.Email;
                            authenticateRequestRoles.CSULBID = response.CSULBID;
                            authenticateRequestRoles.RoleID = roleId;
                            authenticateRequestRoles.ApplicationTypeID = applicationTypeId;

                            var upsertADRoles = _userLoginService.SaveRolesFromAD(authenticateRequestRoles);
                        }

                        userInfo.Email = response.Email;
                        userInfo.CSULBID = response.CSULBID;
                        var getADRoles = _userLoginService.GetIntegratedUserRoles(userInfo);
                        response.Roles = getADRoles.RolesList;
                    }

                    return Ok(response);
                }
                else
                {

                    response.IsSuccess = false;
                    response.message = "Authentication Failed";
                    return BadRequest(response);
                }
                   
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }
        }

        #region AuthenticateSSO old method 
        //[HttpPost("AuthenticateSSO")]
        //public IActionResult AuthenticateSSO(string CSULBID)
        //{
        //    try
        //    {
        //        // logic to connect to graph API 
        //        //string accessToken = CSULBID;
        //        //_client.DefaultRequestHeaders.Clear();
        //        //_client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        //        //var graphResponse = await _client.GetAsync("https://graph.microsoft.com/beta/me");
        //        //_logger.LogInformation(graphResponse.ToString(), graphResponse);
        //        ////string id = Convert.ToString(graphResponse.Id);
        //        //string id = string.Empty;
        //        //var response = _userLoginService.AuthenticateSSO(id);
        //        var response = _userLoginService.AuthenticateSSO(CSULBID);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return BadRequest();
        //    }
        //}
        #endregion

        [HttpPost("SaveUserRegistration")]
        public BaseResponse SaveUserRegistration([FromBody] LoginUserRegistrationRequest request)
        {
            try
            {
                var response = _userLoginService.SaveUserRegistration(request);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("SaveAuditLog")]
        public BaseResponse SaveAuditLog(AuditLogRequest request)
        {
            try
            {
                var response = _userLoginService.SaveAuditLog(request);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        private string GetClientIPAddress()
        {
            var ip = string.Empty;
            ip= Convert.ToString(Request.HttpContext.Connection.RemoteIpAddress);  
            return ip.ToString();

        }
    }
}
