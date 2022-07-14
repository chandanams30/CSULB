using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public IActionResult GetMergedDocument(int userId,int formId)
        {
            Byte[] InputStream = null;
            string documentName = string.Empty;
            InputStream = _documentService.GetMergedDocument(userId, formId);
            return File(InputStream, "application/pdf;", documentName + ".pdf");
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
