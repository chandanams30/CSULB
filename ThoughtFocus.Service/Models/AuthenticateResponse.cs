using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response;

namespace CSULB_COE.Models
{
    public class AuthenticateResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string CSULBID { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string JWTToken { get; set; }
        //public List<long> RoleID { get; set; }
        //public List<string> RoleName { get; set; }

        public List<Roles> Roles { get; set; }
        public List<RoleATID> RoleATIDList { get; set; }

        public string message { get; set; }

        public bool IsSuccess { get; set; }

    }

    public class Roles
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class RoleATID
    {
        public int RoleId { get; set; }
        public int ApplicationTypeId { get; set; }

    }
    public static class RoleConstants
    {
        public const int ProgramAdmin = 4;
        public const int Reviewer = 6;
        public const int ProgramCoordinator = 11;

    }
    public static class ApplicationTypeConstants
    {
        public const int Doctoral = 3;
        public const int ICP = 1;
        public const int Graduate = 2;
    }
    public class RoleADResponse : BaseResponse
    {

        public List<Roles> RolesList { get; set; }
        public List<RoleATID> RoleATIDList { get; set; }

    }

}
