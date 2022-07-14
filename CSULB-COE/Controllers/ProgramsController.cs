using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThoughtFocus.Service.Interfaces;
using Microsoft.Extensions.Logging;
using ThoughtFocus.Domain.Response.Program;
using ThoughtFocus.Domain.Request.Program;

namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProgramsController : ControllerBase
    {
        public ILogger<ProgramsController> _logger;
        private readonly IProgramsService _programService;
        public ProgramsController(IProgramsService programService
                                 ,ILogger<ProgramsController> logger)
        {
            _programService = programService;
            _logger = logger;
        }

        [HttpGet("GetProgramsList")]
        public IActionResult GetProgramsList(int applicationTypeId, int semesterId, int stateId, int userId)
        {
            try
            {
                List<ProgramResponse> response = _programService.GetProgramList(applicationTypeId,semesterId,stateId,userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }

        }

        [HttpGet("GetProgram")]
        public IActionResult GetProgram(int programId, int semesterId, int userId)
        {
            try
            {
                ProgramResponse response = _programService.GetProgram(programId,semesterId, userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }

        }
        [HttpGet("GetProgramApplications")]
        public IActionResult GetProgramApplications(int programId,int semesterId,int stateId,int userId)
        {
            List<ProgramApplicationListResponse> response = _programService.GetProgramApplications(programId,semesterId,stateId,userId);
            return Ok(response);
        }
        [HttpPost("AddProgram")]
        public IActionResult AddUpdateProgram(AddUpdateRequest model)
        {

            return Ok();
        }
    }
}
