using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThoughtFocus.Domain.Request.FieldWork;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldWorkController : ControllerBase
    {
        public ILogger<FieldWorkController> _logger;
        public IFieldWorkService _fieldWorkService;
        public FieldWorkController(IFieldWorkService fieldWorkService
            , ILogger<FieldWorkController> logger)
        {
            _logger = logger;
            _fieldWorkService = fieldWorkService;
        }
        [HttpGet("GetFieldWorkList")]
        public IActionResult GetFieldWorkList(int userId)
        {
            List<FieldWorkResponse> lstFieldWork= _fieldWorkService.GetFieldWorkList(userId);
            return Ok(lstFieldWork);
        }
        [HttpGet("GetFieldWorkById")]
        public FieldWorkDataResponse GetFieldWorkById(int userId,int fieldWorkId)
        {
            FieldWorkDataResponse fieldWorkDataResponse = _fieldWorkService.GetFieldWorkDetailsById(userId, fieldWorkId);
            return fieldWorkDataResponse;
        }
        [HttpPost("UpdateFieldWorkValidation")]
        public string UpdateFieldWorkValidation(FieldWorkValidationRequest input)
        {
            string responseString = string.Empty;
            responseString = _fieldWorkService.UpdateFieldWorkValidation(input);
            return responseString;
        }
    }
}
