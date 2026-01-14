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
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginController(IUserLoginService userLoginService, ILogger<LoginController> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _userLoginService = userLoginService;
            _logger = logger;
            _client = httpClientFactory.CreateClient();
            _httpContextAccessor = httpContextAccessor;
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
                //string jsonData = @"{
                //""roles"": [""Program Admin"", ""Reviewer"", ""Program Coordinator""]}";
                //var roleData = System.Text.Json.JsonSerializer.Deserialize<Roles>(jsonData);
                //List<Roles> rolesList = new List<Roles>();
                //List<string> roles = new List<string>();

                //using (JsonDocument doc = JsonDocument.Parse(jsonData))
                //{
                //    if (doc.RootElement.TryGetProperty("roles", out JsonElement rolesElement))
                //    {
                //        foreach (var role in rolesElement.EnumerateArray())
                //            roles.Add(role.GetString());
                //    }
                //}

                //foreach (var roleName in roles)
                //{
                //    int roleId = GetRoleIdFromName(roleName);

                //    rolesList.Add(new Roles
                //    {
                //        RoleId = roleId,
                //        RoleName = roleName
                //    });
                //}
                //response.Roles = rolesList;

                //var group = _httpContextAccessor.HttpContext.Request.Headers["group"];
                //var ADgroup = _httpContextAccessor.HttpContext.User.Claims
                //.Where(c => c.Type == "groups")
                //.Select(c => c.Value)
                //.ToList();
                //Console.WriteLine("Group" + group );
                //Console.WriteLine("ADGroups" + ADgroup);
                //_logger.LogInformation("Group" + group);
                //_logger.LogInformation("ADGroups" + ADgroup);

                // string userID = _httpContextAccessor.HttpContext.Request.Headers["userID"];
                //Console.WriteLine("UserID" + userID);
                //_logger.LogInformation("UserID" + userID);


                var groupHeader = Request.Headers["group"].ToString();
                var userID = Request.Headers["userID"].ToString();
                var adGroups = User.Claims
           .Where(c =>
               c.Type == "groups" ||
               c.Type == ClaimTypes.GroupSid ||
               c.Type.Contains("groups"))
           .Select(c => c.Value)
           .ToList();
                _logger.LogInformation("Group header: {Group}", groupHeader);
                _logger.LogInformation("UserID header: {UserID}", userID);
                _logger.LogInformation("AD Groups: {Groups}", string.Join(",", adGroups));


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
                    response = _userLoginService.AuthenticateSSO(emplid, displayName, mail, LastName, FirstName);

        //            string jsonData = @"{
        //    ""roles"": ""Program Admin, Reviewer, Program Coordinator""
        //}";
        //            RoleData roleData = System.Text.Json.JsonSerializer.Deserialize<RoleData>(jsonData);
        //            List<Roles> rolesList = new List<Roles>();

        //            //for (int i = 0; i < roleData.Roles.Count; i++)
        //            //{
        //            //    roles.Add(roleData.Roles[i]);
        //            //}
        //            foreach (var roleName in roleData.Roles)
        //            {
        //                int roleId = GetRoleIdFromName(roleName);

        //                rolesList.Add(new Roles
        //                {
        //                    RoleId = roleId,
        //                    RoleName = roleName      
        //                });
        //            }
        //            response.Roles = rolesList;

                    if (response.IsSuccess == true)
                    {
                        AuditLogRequest req = new AuditLogRequest();
                        req.UserID = response.UserId;
                        req.Type = "Login";
                        req.IPAddress=GetClientIPAddress();
                        req.AuthenticationType = "SSO";
                        var auditResponse = _userLoginService.SaveAuditLog(req);

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
        public class RoleData
        {
            public List<string> Roles { get; set; }
        }
        //static int GetRoleIdFromName(string roleName)
        //{
        //    return roleName switch
        //    {
        //        "Program Admin" => RoleConstants.ProgramAdmin,
        //        "Reviewer" => RoleConstants.Reviewer,
        //        "Program Coordinator" => RoleConstants.ProgramCoordinator,
        //        _ => 0 // default or unknown role
        //    };
        //}

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
