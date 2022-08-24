using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtFocus.Service.Interfaces;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        public ILogger<DocumentController> _logger;
        public IDocumentService _documentService;

        public DocumentController(IDocumentService documentService
            , ILogger<DocumentController> logger)
        {
            _logger = logger;
            _documentService = documentService;
        }
        [HttpGet("GetMergedDocument")]
        //public IActionResult GetMergedDocument(int userId,int formId)
        public IActionResult GetMergedDocument(string filename)
        {
            //Byte[] InputStream = null;
            //string documentName = string.Empty;
            //InputStream = _documentService.GetMergedDocument(userId, formId);
            //return File(InputStream, "application/pdf;", documentName + ".pdf");
            string uploadedFileName = filename;
            string[] splitter = uploadedFileName.Split('.');
            StringBuilder fileNameAppender = new StringBuilder();
            string fileExtension = uploadedFileName.Split('.').Last();
            int length = splitter.Length;
            for(int i=0;i<splitter.Length;i++)
            {
                if (i == length - 2 && length > 2)
                    fileNameAppender.Append(splitter[i]);
                else if (length == 2)
                {
                    fileNameAppender.Append(splitter[i]);
                    break;
                }
                else
                {
                    if(i!=length-1)
                        fileNameAppender.Append(splitter[i] + ".");
                }
            }
            return Ok("File Name- "+fileNameAppender.ToString()+"  File Extension- "+fileExtension);
        }
        [HttpGet("GetDocument")]
        public IActionResult GetDocument(int userId,int formAttachmentId)
        {
            Byte[] InputStream = null;
            string documentName = string.Empty;
            InputStream = _documentService.GetMergedDocument(userId, formAttachmentId);
            return File(InputStream, "application/pdf;", documentName + ".pdf");
            
        }
    }
}
