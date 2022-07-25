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

namespace ThoughtFocus.Service.Implementation
{
    public class UserLoginServiceImpl : IUserLoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserDetailsRepository _userDetailsRepository;
        private readonly IUserActivityRepository _userActivityRepository;
        private readonly CSULB_DBContext _context;
        private readonly IConfiguration _config;
        public UserLoginServiceImpl(IUserRepository userRepository,
                                    IUserDetailsRepository userDetailsRepository, 
                                    IUserActivityRepository userActivityRepository,
                                    CSULB_DBContext context,
                                    IConfiguration config 
                                    )
        {
            _userRepository = userRepository;
            _userDetailsRepository = userDetailsRepository;
            _userActivityRepository = userActivityRepository;
            _config = config;
            _context = context;
        }
        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            AuthenticateResponse response = new AuthenticateResponse();

            // validate credential
            UserCred _cred = _userRepository.GetUserLogin(model.Username, model.Password);
            if (_cred == null)
            {
                response.message = "Incorrect UserName / Password";
                return response;
            }
            else
            {
                // adding data to UserActivityLog
                string activityStatus = _userActivityRepository.AddActivityLog(_cred.UserId);
                User _user = _userDetailsRepository.GetUserDetails(Convert.ToInt32(_cred.UserId));
                if (_user != null)
                { 
                        List<Roles> roles = new List<Roles>();
                     
                        roles = GetUserRoles(_cred.UserId);
                        response.UserName = model.Username;
                        response.message = "Success";
                        response.FirstName = _user.FirstName; 
                        response.LastName = _user.LastName; 
                        response.Roles = roles; // pull the roles based on the userID 
                        response.JWTToken = GetJWTString(_user);  
                }
            }
         

            return response;
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
