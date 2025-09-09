using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ThoughtFocus.Service.Interfaces;
using ThoughtFocus.Domain.Response.Travel;
using System;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Request.Travel;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using ThoughtFocus.Common.Utilities.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TravelController : ControllerBase
    {
        public ILogger<TravelController> _logger;
        public ITravelService _travelService;
        private readonly IConfiguration _configuration;
        private readonly ICommonUtils _util;
        public TravelController(ITravelService travelService
            , ILogger<TravelController> logger
            , IConfiguration configuration
            , ICommonUtils util)
        {
            _logger = logger;
            _travelService = travelService;
            _configuration = configuration;
            _util=util;
        }

        [HttpGet("GetBusinessMileageSupervisorList")]
        public FacultySupervisorListResponse GetFacultySupervisorList(int UserID)
        {
            try
            {
                FacultySupervisorListResponse response = _travelService.GetFacultySupervisorList(UserID);
                return response;
            }
            catch (Exception ex)
            {
                FacultySupervisorListResponse response = new FacultySupervisorListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("UpsertBusinessMileageSupervisorUser")]
        public BaseResponse UpsertBusinessMileageSupervisorList(UpsertFacultySupervisorRequest input)
        {
            try
            {
                BaseResponse response = _travelService.UpsertBusinessMileageSupervisorList(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("UpsertBusinessMileageLog")]
        public BaseResponse UpsertBusinessMileageLog(UpsertBusinessMileageLogRequest input)
        {
            try
            {
                BaseResponse response = _travelService.UpsertBusinessMileageLog(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetBusinessMileageLogList")]
        public MileageLogListResponse GetBusinessMileageLogList(int BusinessMileageSupervisorID, int BusinessMileageSupervisorUserID)
        {
            try
            {
                MileageLogListResponse response = _travelService.GetBusinessMileageLogList(BusinessMileageSupervisorID, BusinessMileageSupervisorUserID);
                return response;
            }
            catch (Exception ex)
            {
                MileageLogListResponse response = new MileageLogListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("GetBusinessMileageLog")]
        public MileageLogResponse GetBusinessMileageLog(int BusinessMileageLogID)
        {
            try
            {
                MileageLogResponse response = _travelService.GetBusinessMileageLog(BusinessMileageLogID);
                return response;
            }
            catch (Exception ex)
            {
                MileageLogResponse response = new MileageLogResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpdatePrerequisitesDocument")]
        public BaseResponse UpdatePrerequisitesDocument(UpdatePrerequisitesDocumentRequest input)
        {
            try
            {
                #region testing with manual file , actual file will come as byte array 
                //-------------just for testing - comment it after testing
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\TBTEST.pdf";
                ////string filepath = "D:\\CSULB\\GitHub\\Documents\\test500kb.pdf";
                //byte[] fileContent = null;
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
                //long byteLength = new System.IO.FileInfo(filepath).Length;
                //fileContent = binaryReader.ReadBytes((Int32)byteLength);
                //input.FileContent = fileContent;
                //fs.Close();
                //fs.Dispose();
                //binaryReader.Close();
                //string fc = fileContent.ToString();
                //input.FileContent = fileContent;
                //----end comment----------------------------------------
                #endregion
                BaseResponse response = _travelService.UpdatePrerequisitesDocument(input,false);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("UpdatePrerequisitesValidation")]
        public BaseResponse UpdatePrerequisitesValidation(UpdatePrerequisitesDocumentRequest input)
        {
            try
            {
                #region testing with manual file , actual file will come as byte array 
                //-------------just for testing - comment it after testing
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\TBTEST.pdf";
                ////string filepath = "D:\\CSULB\\GitHub\\Documents\\test500kb.pdf";
                //byte[] fileContent = null;
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
                //long byteLength = new System.IO.FileInfo(filepath).Length;
                //fileContent = binaryReader.ReadBytes((Int32)byteLength);
                //input.FileContent = fileContent;
                //fs.Close();
                //fs.Dispose();
                //binaryReader.Close();
                //string fc = fileContent.ToString();
                //input.FileContent = fileContent;
                //----end comment----------------------------------------
                #endregion
                BaseResponse response = _travelService.UpdatePrerequisitesDocument(input,true);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to save data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("DownloadPrerequisitesDocument")]
        public IActionResult DownloadPrerequisitesDocument(int TravelPrerequisiteID, int BusinessMileageSupervisorUserID)
        {
            byte[] inputStream = null;
            string fileType = string.Empty;
            string fileName = string.Empty;

            TravelPrerequisiteAttachments obj = _travelService.DownloadPrerequisitesDocument(TravelPrerequisiteID, BusinessMileageSupervisorUserID);
            fileName = obj.FileName;
            inputStream = obj.FileContent;
            string[] fileSplit = obj.FileName.Split('.');
            string fileextension = obj.FileName.Split('.').Last();
            fileType = _util.GetFileType(fileextension);
            return File(inputStream, fileType, fileName);
        }

        [HttpPost("GetReportData")]
        public IActionResult GetReportData(ReportDataRequest input)
        {
            byte[] inputStream = null;
            string fileType = string.Empty;
            string fileName = string.Empty;

            ReportDataResponse obj = _travelService.GetReportData(input);
            fileName = obj.FileName;
            inputStream = obj.FileContent;
            string[] fileSplit = obj.FileName.Split('.');
            string fileextension = obj.FileName.Split('.').Last();
            fileType = _util.GetFileType(fileextension);
            return File(inputStream, fileType, fileName);
        }


    }
}
