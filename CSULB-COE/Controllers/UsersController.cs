using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class UsersController : ControllerBase
    {
        public ILogger<UsersController> _logger;
        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }
        [HttpGet("GetUsersByRole")]
        public IActionResult GetUsersByRole(int roleId)
        {
            return Ok();
        }
        [HttpGet("GetUserById")]
        public IActionResult GetUserById(int userId)
        {
            return Ok();
        }
        [HttpPost("AssignUsersToProgram")]
        public IActionResult AssignUsersToProgram()
        {
            return Ok();
        }

        [HttpPost("AddUsers")]
        public IActionResult AddUsers()
        {
            return Ok();
        }

    }
}
