using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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
        private ThoughtFocus.DocumentManager.IFileConverter _fileConverter;
        private readonly IConfiguration _configuration;

        public DocumentController(IDocumentService documentService
            , ILogger<DocumentController> logger
            , ThoughtFocus.DocumentManager.IFileConverter fileConverter
            , IConfiguration configuration)
        {
            _logger = logger;
            _documentService = documentService;
            _configuration = configuration;
        }
        [HttpGet("GetDocument")]
        //public IActionResult GetMergedDocument(int userId,int formId)
        public IActionResult GetDocument(string filename)
        {
            // read the file and convert it into byte array 
            //string filepath = "D:\\CSULB\\GitHub\\Documents\\MYDOCS.png";
            //string filepath = "D:\\CSULB\\GitHub\\Documents\\logo.jpeg";
            //string filepath = "D:\\CSULB\\GitHub\\Documents\\logo.jpg";
            // string filepath = "D:\\CSULB\\GitHub\\Documents\\pic1.jpg";
            // string filepath = "D:\\CSULB\\GitHub\\Documents\\pic2.jpg";
            string deadLine = DateTime.Now.ToString("MM/dd/yyyy");
            string filepath = "D:\\CSULB\\GitHub\\Documents\\MyDOC.docx";
            var tmpPath = _configuration["ApplicationKeys:FileRepository"];
            string tmpPDFFileName = "MyDOC" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf";
            string destinationPath = Path.Combine(tmpPath, tmpPDFFileName);
            Byte[] InputStream = null;
            InputStream=word2PDF(filepath, destinationPath);
            
            // byte[] fileContent = null;
            // System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            // System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            // long byteLength = new System.IO.FileInfo(filepath).Length;
            // fileContent = binaryReader.ReadBytes((Int32)byteLength);
            // fs.Close();
            // fs.Dispose();
            // binaryReader.Close();
            // Byte[] InputStream = null;
            // #region for word file download as pdf 

            // var tmpFile = "MyDOC" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".docx";
            // var tmpPath = _configuration["ApplicationKeys:FileRepository"];
            // var tmpFilePath = Path.Combine(tmpPath, tmpFile);
            // System.IO.File.WriteAllBytes(tmpFilePath, fileContent);

            // Microsoft.Office.Interop.Word.ApplicationClass word; word = new Microsoft.Office.Interop.Word.ApplicationClass();
            // object UnknownType = Type.Missing;
            // object readOnly = true;
            // object InputLocation = tmpFilePath;
            // word.Visible = false;
            // //To Open the Word Document
            // word.Documents.Open(ref InputLocation,    //Input File Name Location
            //     ref UnknownType,    // Conversion Conformation
            //     ref readOnly,       // Set ReadOnly Property
            //     ref UnknownType,    // Add to the Recent Files
            //     ref UnknownType,    // Document Password Setting
            //     ref UnknownType,    // Password Templete
            //     ref UnknownType,    // Revert
            //     ref UnknownType,    // Write Password to Document
            //     ref UnknownType,    // Write Password Template
            //     ref UnknownType,    // File Format
            //     ref UnknownType,    // Encoding File
            //     ref UnknownType,    // Visibility
            //     ref UnknownType,    // To Open or Repair
            //     ref UnknownType,    // Document Direction
            //     ref UnknownType,    // Encoding Dialog
            //     ref UnknownType);   // XML Text Transform
            // //Microsoft.Office.Interop.Word.Document doc = word.Documents.Open(ref InputLocation,    //Input File Name Location
            // //    ref UnknownType,    // Conversion Conformation
            // //    ref readOnly,       // Set ReadOnly Property
            // //    ref UnknownType,    // Add to the Recent Files
            // //    ref UnknownType,    // Document Password Setting
            // //    ref UnknownType,    // Password Templete
            // //    ref UnknownType,    // Revert
            // //    ref UnknownType,    // Write Password to Document
            // //    ref UnknownType,    // Write Password Template
            // //    ref UnknownType,    // File Format
            // //    ref UnknownType,    // Encoding File
            // //    ref UnknownType,    // Visibility
            // //    ref UnknownType,    // To Open or Repair
            // //    ref UnknownType,    // Document Direction
            // //    ref UnknownType,    // Encoding Dialog
            // //    ref UnknownType);   // XML Text Transform
            //// doc.Activate();

            // //To Get Document in PDF Format
            // string tmpPDFFileName = "MyDOC" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf"; 
            // object SavePDFFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;
            // object OutputLocation = Path.Combine(tmpPath, tmpPDFFileName); 
            // word.Visible = false;
            // word.DisplayAlerts = Microsoft.Office.Interop.Word.WdAlertLevel.wdAlertsNone;
            // word.Options.SavePropertiesPrompt = false;
            // word.Options.SaveNormalPrompt = false;
            // word.ActiveDocument.SaveAs(ref OutputLocation, //Output File Location
            // ref SavePDFFormat,    // File Format
            // ref UnknownType,    // Comment to PDF File
            // ref UnknownType,    // Password
            // ref UnknownType,    // Add to Recent File
            // ref UnknownType,    // Write Password
            // ref UnknownType,    // ReadOnly Propert
            // ref UnknownType,    // Original Font Embeding
            // ref UnknownType,    // Save Picture
            // ref UnknownType,    // Saving Form Datas
            // ref UnknownType,    // Save as AOVE Letter
            // ref UnknownType,    // Encoding
            // ref UnknownType,    // Inserting Line Breakes
            // ref UnknownType,    // Allow Substitution
            // ref UnknownType,    // Line Ending
            // ref UnknownType);   // Add BiDi Marks


            // //To Close the Document File
            // word.Documents.Close(ref UnknownType, ref UnknownType, ref UnknownType);

            // //To Exit the Word Application
            // word.Quit(ref UnknownType, ref UnknownType, ref UnknownType);


            // // read the file and convert to byte array 

            // //System.IO.FileStream fsPDF = new System.IO.FileStream(Path.Combine(tmpPath, tmpPDFFileName), System.IO.FileMode.Open, System.IO.FileAccess.Read);
            // //System.IO.BinaryReader binaryReaderPDF = new System.IO.BinaryReader(fsPDF);
            // //long byteLengthPDF = new System.IO.FileInfo(Path.Combine(tmpPath, tmpPDFFileName)).Length;
            // //InputStream = binaryReader.ReadBytes((Int32)byteLengthPDF);
            // //fs.Close();
            // //fs.Dispose();
            // //binaryReader.Close();



            // // delete the word file 

            // FileInfo finfo = new FileInfo(tmpFilePath);
            // finfo.Delete();

//#endregion
            #region for images download as pdf 
            //string documentName = string.Empty;
            ////InputStream = _documentService.GetMergedDocument(userId, formId);
            //using (MemoryStream stream = new System.IO.MemoryStream())
            //{
            //    //Initialize the PDF document object.
            //    using (Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f))
            //    {
            //        PdfWriter.GetInstance(pdfDoc, stream).SetFullCompression();
            //        pdfDoc.Open();

            //        //Add the Image file to the PDF document object.
            //       // iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(fileContent);
            //        iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(fileContent);

            //        //Scaling the image
            //        if (pic.Height > pic.Width)
            //        {
            //            float percentage = 0.0f;
            //            percentage = 700 / pic.Height;
            //            pic.ScalePercent(percentage * 100);
            //        }
            //        else
            //        {
            //            float percentage = 0.0f;
            //            percentage = 540 / pic.Width;
            //            pic.ScalePercent(percentage * 100);
            //        }
            //        pdfDoc.Add(pic);
            //        pdfDoc.Close();
            //        InputStream = stream.ToArray();
            //    }
            //}
            //return File(InputStream, "application/pdf;", filename + ".pdf");
            #endregion
            return File(InputStream, "application/pdf;","Converted.pdf");

            #region File name split logic 
            //string uploadedFileName = filename;
            //string[] splitter = uploadedFileName.Split('.');
            //StringBuilder fileNameAppender = new StringBuilder();
            //string fileExtension = uploadedFileName.Split('.').Last();
            //int length = splitter.Length;
            //for(int i=0;i<splitter.Length;i++)
            //{
            //    if (i == length - 2 && length > 2)
            //        fileNameAppender.Append(splitter[i]);
            //    else if (length == 2)
            //    {
            //        fileNameAppender.Append(splitter[i]);
            //        break;
            //    }
            //    else
            //    {
            //        if(i!=length-1)
            //            fileNameAppender.Append(splitter[i] + ".");
            //    }
            //}
            //return Ok("File Name- "+fileNameAppender.ToString()+"  File Extension- "+fileExtension);
            #endregion
        }
        private byte[] word2PDF(object Source, object Target)
        {
            Microsoft.Office.Interop.Word.ApplicationClass MSdoc;
            Byte[] InputStream = null;
            //Use for the parameter whose type are not known or say Missing
            object Unknown = Type.Missing;
            //Creating the instance of Word Application
            MSdoc = new Microsoft.Office.Interop.Word.ApplicationClass();

            try
            {
                MSdoc.Visible = false;
                MSdoc.Documents.Open(ref Source, ref Unknown,
                     ref Unknown, ref Unknown, ref Unknown,
                     ref Unknown, ref Unknown, ref Unknown,
                     ref Unknown, ref Unknown, ref Unknown,
                     ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown);
                MSdoc.Application.Visible = false;
                MSdoc.WindowState = Microsoft.Office.Interop.Word.WdWindowState.wdWindowStateMinimize;

                object format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;

                MSdoc.ActiveDocument.SaveAs(ref Target, ref format,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                        ref Unknown, ref Unknown, ref Unknown,
                       ref Unknown, ref Unknown);
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
            finally
            {
                if (MSdoc != null)
                {
                    MSdoc.Documents.Close(ref Unknown, ref Unknown, ref Unknown);
                    //WordDoc.Application.Quit(ref Unknown, ref Unknown, ref Unknown);
                }
                // for closing the application
                MSdoc.Quit(ref Unknown, ref Unknown, ref Unknown);
                // read the file stream 
                System.IO.FileStream fsPDF = new System.IO.FileStream(Target.ToString(), System.IO.FileMode.Open, System.IO.FileAccess.Read);
                System.IO.BinaryReader binaryReaderPDF = new System.IO.BinaryReader(fsPDF);
                long byteLengthPDF = new System.IO.FileInfo(Target.ToString()).Length;
                InputStream = binaryReaderPDF.ReadBytes((Int32)byteLengthPDF);
                fsPDF.Close();
                fsPDF.Dispose();
                binaryReaderPDF.Close();

            }
            return InputStream;
        }

        private byte[] Merge(String OutFile)
        {
            byte[] inputStream = null;
            var folderPath = _configuration["ApplicationKeys:FileRepository"];
            string workingFolderName = Path.Combine(folderPath, "WorkingFolder");
            string MergedPDFFolderName= Path.Combine(folderPath, "MergedPDF");
            OutFile = Path.Combine(MergedPDFFolderName, OutFile);
            string[] mergelist = Directory.GetFiles(workingFolderName, "*.pdf");
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            PdfCopy copyProvider;

            if ((new FileInfo(OutFile)).Exists)
            {
                copyProvider = new PdfCopy(document, new System.IO.FileStream(OutFile, System.IO.FileMode.Append));
            }
            else
            {
                copyProvider = new PdfCopy(document, new System.IO.FileStream(OutFile, System.IO.FileMode.Create));
            }
            document.Open();

            foreach (string file in mergelist)
            {
                if (!string.IsNullOrEmpty(file))
                {
                    //Console.WriteLine("Merging: " + file);
                    //PdfReader.AllowOpenWithFullPermissions = true;
                    iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                   // pdfReader.setUnethicalReading(true);
                    copyProvider.AddDocument(pdfReader);
                    pdfReader.Close();
                }
            }
            document.Close();
            System.IO.FileStream fsPDF = new System.IO.FileStream(OutFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReaderPDF = new System.IO.BinaryReader(fsPDF);
            long byteLengthPDF = new System.IO.FileInfo(OutFile).Length;
            inputStream = binaryReaderPDF.ReadBytes((Int32)byteLengthPDF);
            fsPDF.Close();
            fsPDF.Dispose();
            binaryReaderPDF.Close();
            return inputStream;
        }

        [HttpGet("GetMergedDocument")]
        public IActionResult GetMergedDocument(string DocumentName)
        {
            Byte[] InputStream = null;
            //string documentName = string.Empty;
            //InputStream = _documentService.GetMergedDocument(userId);
            InputStream = Merge(DocumentName + ".pdf");
            return File(InputStream, "application/pdf;", DocumentName + ".pdf");
            
        }
    }
}
