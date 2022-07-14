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
        public BaseResponse UpdateFieldWorkValidation(FieldWorkValidationRequest input)
        {
            string responseString = string.Empty;
            BaseResponse response = _fieldWorkService.UpdateFieldWorkValidation(input);
            return response;
        }

        [HttpPost("UploadFieldWorkRequiredDocuments")]
        public BaseResponse UploadFieldWorkRequiredDocuments(FieldWorkUploadDocumentsRequest input)
        {
            string responseString = string.Empty;
            // -------------just for testing -  comment it after testing 
            //string filepath = "D:\\CSULB\\GitHub\\Documents\\TBTEST.pdf";
            string filepath = "D:\\CSULB\\GitHub\\Documents\\TB-TEST.docx";
            //string filepath = "D:\\CSULB\\GitHub\\Documents\\Student Clearance Form Sample.pdf";
            byte[] fileContent = null;
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            long byteLength = new System.IO.FileInfo(filepath).Length;
            fileContent = binaryReader.ReadBytes((Int32)byteLength);
            input.FileContent = fileContent;
            fs.Close();
            fs.Dispose();
            binaryReader.Close();
            //----end comment----------------------------------------
            BaseResponse response = _fieldWorkService.UpdateFieldWorkDocumentValidation(input);
            return response;
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
