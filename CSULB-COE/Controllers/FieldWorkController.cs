using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThoughtFocus.Domain.Request.FieldWork;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("[controller]")]
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
            try
            {
                FieldWorkListResponse lstFieldWork = _fieldWorkService.GetFieldWorkList(userId);
                return Ok(lstFieldWork);
            }
            catch (Exception ex)
            {
                FieldWorkListResponse response = new FieldWorkListResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return BadRequest(response);
            }
        }
        [HttpGet("GetFieldWorkById")]
        public FieldWorkDataResponse GetFieldWorkById(int userId,int fieldWorkId)
        {
            try
            {
                FieldWorkDataResponse fieldWorkDataResponse = _fieldWorkService.GetFieldWorkDetailsById(userId, fieldWorkId);
                return fieldWorkDataResponse;
            }
            catch (Exception ex)
            {
                FieldWorkDataResponse response = new FieldWorkDataResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("UpdateFieldWorkValidation")]
        public BaseResponse UpdateFieldWorkValidation(FieldWorkValidationRequest input)
        {
            try
            {
                BaseResponse response = _fieldWorkService.UpdateFieldWorkValidation(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("UploadFieldWorkRequiredDocuments")]
        public BaseResponse UploadFieldWorkRequiredDocuments(FieldWorkUploadDocumentsRequest input)
        {
            #region testing with manual file , actual file will come as byte array 
            //-------------just for testing - comment it after testing
            //string filepath = "D:\\CSULB\\GitHub\\Documents\\TBTEST.pdf";
            // string filepath = "D:\\CSULB\\GitHub\\Documents\\TB-TEST.docx";
            //string filepath = "D:\\CSULB\\GitHub\\Documents\\Student Clearance Form Sample.pdf";
            //byte[] fileContent = null;
            //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            //long byteLength = new System.IO.FileInfo(filepath).Length;
            //fileContent = binaryReader.ReadBytes((Int32)byteLength);
            //input.FileContent = fileContent;
            //fs.Close();
            //fs.Dispose();
            //binaryReader.Close();
            //----end comment----------------------------------------
            #endregion
            try
            {
                BaseResponse response = _fieldWorkService.UpdateFieldWorkDocumentValidation(input);
                return response;
            }
            catch (Exception ex)
            {
                BaseResponse response = new BaseResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpGet("DownloadRequiredDocuments")]
        public IActionResult DownloadRequiredDocuments(int userId,int fieldworkAttachmentId)
        {
            byte[] inputStream = null;
            string fileType = string.Empty;
            string fileName = string.Empty;

            FieldWorkProfileAttachments obj = _fieldWorkService.DownloadRequiredDocuments(userId,fieldworkAttachmentId);
            fileName = obj.FileName;
            inputStream = obj.FileContent;
            string[] fileSplit = obj.FileName.Split('.');

            fileType = GetFileType(fileSplit[1]);

            return File(inputStream, fileType, fileName);
        }
        [HttpGet("GetFieldWorkActivityLog")]
        public FieldWorkActivityLogResponse GetFieldWorkActivityLog(int userId, int fieldWorkId)
        {
            try
            {
                FieldWorkActivityLogResponse response = new FieldWorkActivityLogResponse();
                response = _fieldWorkService.GetFieldWorkActivityLog(userId, fieldWorkId);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkActivityLogResponse response = new FieldWorkActivityLogResponse();
                response.IsSuccess = false;
                response.Message = "Failed to retrieve data , please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }
        [HttpPost("UpdateFieldWorkActivityLog")]
        public FieldWorkActivityLogResponse UpdateFieldWorkActivityLog(FieldWorkActivityLogRequest input)
        {
            try
            {
                FieldWorkActivityLogResponse response = new FieldWorkActivityLogResponse();
                response = _fieldWorkService.UpdateFieldWorkActivityLog(input);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkActivityLogResponse response = new FieldWorkActivityLogResponse();
                response.IsSuccess = false;
                response.Message = "Failed to Save Activity Log, please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpPost("UploadFieldWorkActivityDocuments")]
        public FieldWorkAttachmentsResponse UploadFieldWorkActivityDocuments(FieldWorkAttachmentsRequest input)
        {
            try
            {
                // comment the below after testing 


                //string filepath = "D:\\CSULB\\GitHub\\Documents\\TBTEST.pdf";
                // string filepath = "D:\\CSULB\\GitHub\\Documents\\TB-TEST.docx";
                //string filepath = "D:\\CSULB\\GitHub\\Documents\\Student Clearance Form Sample.pdf";
                //byte[] fileContent = null;
                //System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
                //long byteLength = new System.IO.FileInfo(filepath).Length;
                //fileContent = binaryReader.ReadBytes((Int32)byteLength);
                //input.FileContent = fileContent;
                //fs.Close();
                //fs.Dispose();
                //binaryReader.Close();

                // end comment
                FieldWorkAttachmentsResponse response = new FieldWorkAttachmentsResponse();
                response = _fieldWorkService.UploadFieldWorkActivityDocuments(input);
                return response;
            }
            catch (Exception ex)
            {
                FieldWorkAttachmentsResponse response = new FieldWorkAttachmentsResponse();
                response.IsSuccess = false;
                response.Message = "Failed to upload activity attachments, please try after sometime";
                response.StackTrace = ex.Message;
                _logger.LogError(ex, ex.Message);
                return response;
            }
        }

        [HttpGet("DownloadActivityAttachments")]
        public IActionResult DownloadActivityAttachments(int userId, int fieldworkAttachmentId)
        {
            byte[] inputStream = null;
            string fileType = string.Empty;
            string fileName = string.Empty;

            FieldWorkProfileAttachments obj = _fieldWorkService.DownloadActivityAttachments(userId, fieldworkAttachmentId);
            fileName = obj.FileName;
            inputStream = obj.FileContent;
            string[] fileSplit = obj.FileName.Split('.');

            fileType = GetFileType(fileSplit[1]);

            return File(inputStream, fileType, fileName);
        }

        private string GetFileType(string fileExt)
        {
            string contentType = string.Empty;
            switch(fileExt.ToUpper())
            {
                case "PDF":
                    contentType = "application/pdf";
                    break;
                case "DOCX":
                    contentType = "Application/msword";
                    break;
                case "DOC":
                    contentType = "Application/msword";
                    break;
                case "XLSX":
                    contentType = "Application/x-msexcel";
                    break;
                case "XLS":
                    contentType = "Application/x-msexcel";
                    break;
                case "JPG":
                    contentType = "image/jpeg";
                    break;
                case "JPEG":
                    contentType = "image/jpeg";
                    break;

            }
            return contentType;
        }
    }
}
