using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThoughtFocus.Service.Interfaces;
using Microsoft.Extensions.Logging;
using ThoughtFocus.Domain.Response.Form;
using ThoughtFocus.Domain.Request.Form;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormsController : ControllerBase
    {
        public ILogger<FormsController> _logger;
        private readonly IFormsService _formsService;
        public FormsController(IFormsService formsService 
                              ,ILogger<FormsController> logger)
        {
            _formsService = formsService;
            _logger = logger;
        }

        [HttpGet("GetFormsList")]
        public IActionResult GetFormsList(int programId,int semesterId, int userId)
        {
            try
            {
                // gets the forms list 
                List<FormResponse> response = _formsService.GetFormList(programId, semesterId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }

        }

        [HttpGet("GetForm")]
        public IActionResult GetForm(int formId,int programId, int userId)
        {
            try
            {
                FormResponse response = _formsService.GetForm(formId,programId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }

        }
        [HttpPost("SaveForm")]
        public IActionResult SaveForm(FormAddRequest request)
        {
            try
            {
                string response = _formsService.SaveForm(request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest();
            }

        }
    }
}
