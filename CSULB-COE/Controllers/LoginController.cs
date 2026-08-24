//using CSULB_COE.ViewModels;
using CSULB_COE.Models;
using CSULB_COE.ViewModels;
using Google.Apis.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Newtonsoft.Json;
using Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using ThoughtFocus.Domain.Request;
using ThoughtFocus.Domain.Request.Login;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Service.Interfaces;
using System.DirectoryServices;
using Microsoft.Extensions.Configuration;


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

        [HttpPost("Authenticate")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            //_logger.LogInformation("Start : Authenticating for {userName}",userName);
            ViewModels.AuthenticateRequest authModel = new ViewModels.AuthenticateRequest();
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
                //string username = mail;
                _logger.LogInformation("Fetching MEMBER OF groups for {Username}", username);

                try
                    {
                        using (var entry = new System.DirectoryServices.DirectoryEntry()) // implicit credentials
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
                                    try
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
                                        try
                                        {
                                            using (var groupEntry = new System.DirectoryServices.DirectoryEntry($"LDAP://{dn}"))
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
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(
                                                ex,
                                                "Error retrieving sAMAccountName for group {GroupName}",
                                                groupName);

                                            continue;
                                        }

                                        //var adGroupsKey = _configuration["ApplicationKeys:ADGroupsKey"];

                                        //var allowedGroups = (_configuration["ApplicationKeys:AllowedADGroups"] ?? "")
                                        //.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        //.Select(x => x.Trim());

                                        //if ((!string.IsNullOrEmpty(adGroupsKey) &&
                                        // groupName.StartsWith(adGroupsKey, StringComparison.OrdinalIgnoreCase))
                                        //|| allowedGroups.Any(g => string.Equals(
                                        //    groupName,
                                        //    g,
                                        //    StringComparison.OrdinalIgnoreCase)))
                                        //{
                                        //    groups.Add(groupName);
                                        //    _logger.LogInformation("Directory Search");
                                        //    _logger.LogInformation("Assigned Group: {GroupName}", groupName);
                                        //}

                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(
                                            ex,
                                            "Error processing AD group for user {Username}",
                                            username);
                                        continue;
                                    }


                                }
                                _logger.LogInformation(
                            "User {Username} MEMBER OF groups: {Groups}",
                            username,
                            string.Join(" | ", groups));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error while fetching AD groups for user {Username}",
                            username);
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
                    _logger.LogInformation("Graph Result - " + graphResults);

                    _logger.LogInformation("LOGIN USER INFORMATION - CSULBID: {emplid} , DisplayName: {DisplayName}, Email: {Email}, FirstName: {FirstName}, LastName: {LastName}",
                     emplid, displayName, mail, FirstName, LastName);
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

                    try
                    {
                        using (var entry = new System.DirectoryServices.DirectoryEntry()) // implicit credentials
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
                                    try
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
                                        try
                                        {
                                            using (var groupEntry = new System.DirectoryServices.DirectoryEntry($"LDAP://{dn}"))
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
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(
                                                ex,
                                                "Error retrieving sAMAccountName for group {GroupName}",
                                                groupName);

                                            continue;
                                        }

                                        //var adGroupsKey = _configuration["ApplicationKeys:ADGroupsKey"];

                                        //var allowedGroups = (_configuration["ApplicationKeys:AllowedADGroups"] ?? "")
                                        //.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        //.Select(x => x.Trim());

                                        //if ((!string.IsNullOrEmpty(adGroupsKey) &&
                                        // groupName.StartsWith(adGroupsKey, StringComparison.OrdinalIgnoreCase))
                                        //|| allowedGroups.Any(g => string.Equals(
                                        //    groupName,
                                        //    g,
                                        //    StringComparison.OrdinalIgnoreCase)))
                                        //{
                                        //    groups.Add(groupName);
                                        //    _logger.LogInformation("Directory Search");
                                        //    _logger.LogInformation("Assigned Group: {GroupName}", groupName);
                                        //}

                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(
                                            ex,
                                            "Error processing AD group for user {Username}",
                                            username);
                                        continue;
                                    }


                                }
                                _logger.LogInformation(
                            "User {Username} MEMBER OF groups: {Groups}",
                            username,
                            string.Join(" | ", groups));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error while fetching AD groups for user {Username}",
                            username);
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

        //[HttpPost("AuthenticateSSOToken")]
        //public async Task<IActionResult> AuthenticateSSOToken1([FromBody] LoginSSORequest request)
        //{
        //    AuthenticateResponse response = new AuthenticateResponse();
        //    try
        //    {
        //        _logger.LogInformation(request.Token.ToString(), request);
        //        string accessToken = request.Token;
        //        _client.DefaultRequestHeaders.Clear();
        //        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);


        //        bool isTestMode = true;
        //        if (isTestMode)
        //        {
        //            string graphResults = string.Empty;

        //            graphResults = @"{
        //      ""employeeId"": ""123456"",
        //      ""displayName"": ""John Doe"",
        //      ""mail"": ""john.doe@domain.com"",
        //      ""surname"": ""Doe"",
        //      ""givenName"": ""John""
        //      }";

        //            // Deserialize response
        //            dynamic dynObj = JsonConvert.DeserializeObject(graphResults);

        //            string emplid = dynObj.employeeId != null ? dynObj.employeeId.ToString() : string.Empty;
        //            string displayName = dynObj.displayName != null ? dynObj.displayName.ToString() : string.Empty;
        //            string mail = dynObj.mail != null ? dynObj.mail.ToString() : string.Empty;
        //            string LastName = dynObj.surname != null ? dynObj.surname.ToString() : string.Empty;
        //            string FirstName = dynObj.givenName != null ? dynObj.givenName.ToString() : string.Empty;

        //            _logger.LogInformation("LOGIN USER INFORMATION - CSULBID: {emplid} , DisplayName: {DisplayName}, Email: {Email}, FirstName: {FirstName}, LastName: {LastName}",
        //          emplid, displayName, mail, FirstName, LastName);

        //            return Ok(response);
        //        }
        //        else
        //        {
        //            var graphResponse = await _client.GetAsync("https://graph.microsoft.com/beta/me");

        //            string emplid = string.Empty;
        //            string displayName = string.Empty;
        //            string mail = string.Empty;
        //            string LastName = string.Empty;
        //            string FirstName = string.Empty;
        //            if (graphResponse.IsSuccessStatusCode)
        //            {

        //                var graphResults = graphResponse.Content.ReadAsStringAsync().Result;
        //                var json = new NewtonsoftJsonSerializer();
        //                dynamic dynObj = JsonConvert.DeserializeObject(graphResults);
        //                foreach (var data in dynObj)
        //                {
        //                    if (data.Name == "employeeId")
        //                    {
        //                        emplid = data.Value;
        //                    }
        //                    if (data.Name == "displayName")
        //                    {
        //                        displayName = data.Value;
        //                    }
        //                    if (data.Name == "mail")
        //                    {
        //                        mail = data.Value;
        //                    }
        //                    if (data.Name == "surname")
        //                    {
        //                        LastName = data.Value;
        //                    }
        //                    if (data.Name == "givenName")
        //                    {
        //                        FirstName = data.Value;
        //                    }
        //                }
        //                _logger.LogInformation("Graph Result - "+ graphResults);
        //                _logger.LogInformation("LOGIN USER INFORMATION - CSULBID: {emplid} , DisplayName: {DisplayName}, Email: {Email}, FirstName: {FirstName}, LastName: {LastName}",
        //                emplid, displayName, mail, FirstName, LastName);
        //                response = _userLoginService.AuthenticateSSO(emplid, displayName, mail, LastName, FirstName);
        //                if (response.IsSuccess == true)
        //                {
        //                    AuditLogRequest req = new AuditLogRequest();
        //                    req.UserID = response.UserId;
        //                    req.Type = "Login";
        //                    req.IPAddress = GetClientIPAddress();
        //                    req.AuthenticationType = "SSO";
        //                    var auditResponse = _userLoginService.SaveAuditLog(req);

        //                }
        //                return Ok(response);
        //            }
        //            else
        //            {

        //                response.IsSuccess = false;
        //                response.message = "Authentication Failed";
        //                return BadRequest(response);
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        return BadRequest();
        //    }
        //}
    }
}
