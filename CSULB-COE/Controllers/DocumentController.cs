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
using ThoughtFocus.Domain.TemplateModels;
using Newtonsoft.Json.Linq;
using iTextSharp.text.html.simpleparser;

namespace CSULB_COE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class DocumentController : ControllerBase
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

        [HttpPost("GetTravelDocument")]
        public IActionResult GetTravelDocument(MileageReportRequest input)
        {

            // First fetch the start and end location
            Byte[] InputStream = null;
            StringBuilder strRows = new StringBuilder();
            JArray fields = JArray.Parse(input.DirectionsJSON);
            foreach (JObject jObject in fields)
            {
                string turnTypePath = string.Empty;
                string tableData = string.Empty;
                string mapImage = string.Empty;
                string narrative = (string)jObject["narrative"];
                int turnType=(int)jObject["turnType"];
                // get image based on turn type
                turnTypePath = GetTurnTypeImage(turnType);

                // get the start and end destination

                // get the sub template and bind data - only directions from JSON
                tableData = BindDataToRows(turnType,narrative);
                // bind the image after the directions
                strRows.Append(tableData);

            }
            StringBuilder rawData = new StringBuilder();
            rawData.Append(strRows.ToString());
            // get the final template and bind the sub template data to it 
            rawData.Append(GetMileageMapImage("MileageImageTest.png"));
            string template = GetMileageReportTemplate("MileageReportTemplate.html");
            template = template.Replace("[[dataRows]]", rawData.ToString());

            // call the method to convert the template to PDF and create the input stream 
            InputStream=GetPDFFileContent(template);

            return File(InputStream, "application/pdf;", "Mileage-Report-"+ DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf");
        }

        private string GetMileageMapImage(string imageFileName)
        {
            string fullImagePath = string.Empty;
            string repoPath = "SupportFiles/Img"; // pull this from config
            fullImagePath = Path.Combine(repoPath, imageFileName);
            StringBuilder strImage = new StringBuilder();
            strImage.Append("<tr>");
            strImage.Append("<td colspan=2><img src=" + fullImagePath + " /></td>");
            strImage.Append("</tr>");
            return strImage.ToString();
        }
        private byte[] GetPDFFileContent(string htmlFormBody)
        {
            byte[] fileContent = null;
            StringReader sr = new StringReader(htmlFormBody); // workable code uncomment after testing 
            //TextReader sr = new StringReader(htmlFormBody);
            //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            Document pdfDoc = new Document(PageSize.A4, 50, 50, 50, 50);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();

                htmlparser.Parse(sr);
                pdfDoc.Close();

                fileContent = memoryStream.ToArray();
                memoryStream.Close();
            }
            return fileContent;
        }

        private string GetMileageReportTemplate(string templateName)
        {
            string body = string.Empty;
            string filepath = Path.Combine("SupportFiles/DocumentTemplates", templateName);
            using (StreamReader reader = new StreamReader(Path.GetFullPath(filepath)))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }

        private string BindDataToRows(int turnType,string narrative)
        {
            string tableData = string.Empty;
            string imagePath = string.Empty;
            imagePath=GetTurnTypeImage(turnType);
            tableData = ConstructTableData(imagePath, narrative);
            return tableData;
        }

        private string ConstructTableData(string imagePath,string narrative)
        {
            StringBuilder strTableData = new StringBuilder();
            strTableData.Append("<tr>");
            strTableData.Append("<td><img src="+imagePath+" /></td>");
            strTableData.Append("<td>"+narrative+"</td>");
            strTableData.Append("</tr>");
            return strTableData.ToString();
        }
        private string GetTurnTypeImage(int turnType)
        {
            string imgPath = string.Empty;
            switch(turnType){
                case 0:
                    imgPath = "STRAIGHT.png";// straight
                    break;
                case 1:
                    imgPath = "SLIGHT_RIGHT.png";// slight right
                    break;
                case 2:
                    imgPath = "TURN_RIGHT.png";// right
                    break;
                case 3:
                    imgPath = "SHARP_RIGHT.png";// sharp right
                    break;
                case 6:
                    imgPath = "TURN_LEFT.png";// left
                    break;
                case 7:
                    imgPath = "SLIGHT_LEFT.png";// slight left
                    break;
                case 8:
                    imgPath = "U_TURN_RIGHT.png";// right U turn
                    break;
                case 9:
                    imgPath = "U_TURN_LEFT.png";// left U turn
                    break;
                case 10:
                    imgPath = "MERGE.png";// right merge
                    break;
                case 11:
                    imgPath = "MERGE.png";// left merge
                    break;
                case 12:
                    imgPath = "RAMP_RIGHT.png";// right on ramp
                    break;
                case 13:
                    imgPath = "RAMP_LEFT.png";// left on ramp
                    break;
                case 16:
                    imgPath = "FORK_RIGHT.png";// right fork
                    break;
                case 17:
                    imgPath = "FORK_LEFT.png";// left fork
                    break;
                default:
                    imgPath = "STRAIGHT.png";// straight
                    break;
            }
            string filepath = Path.Combine("SupportFiles/Img", imgPath);
            return filepath;
            
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
            //InputStream = Merge(DocumentName + ".pdf");
            return File(InputStream, "application/pdf;", DocumentName + ".pdf");
            
        }
        [HttpGet("GetPDFFromJSON")]
        public IActionResult GetPDFFromJSON(string JSON,string documentName)
        {
            Byte[] InputStream = null;
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            var workingFolderPath = Path.Combine(fileRepoPath, "WorkingFolder");
            var fileName = documentName + "_" + DateTime.Now.ToString("MMddyyyyHHmmss")+".pdf";
            // get the json , map it to the entities 
            TemplateStore<GraduateProgramRecommenderForm> store = new TemplateStore<GraduateProgramRecommenderForm>();
            GraduateProgramRecommenderForm obj = new GraduateProgramRecommenderForm();
            obj=store.GetDeserializedTemplate(JSON);
            // get the html template and fill in the values from the entities
            string recommenderFormTemplate = GetFormTemplate("GraduateApplicationRecommendationTemplate.html");
            recommenderFormTemplate = recommenderFormTemplate.Replace("[[STUDENT_NAME]]", obj.personalInfo.studentName)
                                                             .Replace("[[recommender_name]]", obj.personalInfo.recommenderName)
                                                             .Replace("[[occupation]]", obj.personalInfo.occupation)
                                                             .Replace("[[jobTitle]]", obj.personalInfo.jobTitle)
                                                             .Replace("[[organization]]", obj.personalInfo.organization)
                                                             .Replace("[[email]]", obj.personalInfo.email)
                                                             .Replace("[[phone]]", obj.personalInfo.email)
                                                             .Replace("[[applicantKnowTime]]", obj.relationshipToApplicant.applicationKnowTime)
                                                             .Replace("[[applicationKnowSource]]", obj.relationshipToApplicant.applicationKnowSource)
                                                             .Replace("[[communicationSkillsOral]]", obj.referrenceRatings.communicationSkillsOral)
                                                             .Replace("[[communicationSkillsWritten]]", obj.referrenceRatings.communicationSkillsWritten)
                                                             .Replace("[[TechnologySkills]]", obj.referrenceRatings.TechnologySkills)
                                                             .Replace("[[Initiative]]", obj.referrenceRatings.Initiative)
                                                             .Replace("[[Maturity]]", obj.referrenceRatings.Maturity)
                                                             .Replace("[[MotivationForTheProgramOfStudy]]", obj.referrenceRatings.MotivationForTheProgramOfStudy)
                                                             .Replace("[[Creativity]]", obj.referrenceRatings.Creativity)
                                                             .Replace("[[AbilityToWorkWithOthers]]", obj.referrenceRatings.AbilityToWorkWithOthers)
                                                             .Replace("[[IntellectualPotential]]", obj.referrenceRatings.IntellectualPotential)
                                                             .Replace("[[PresentAcademicPerformance]]", obj.referrenceRatings.PresentAcademicPerformance)
                                                             .Replace("[[PotentialForGraduateWork]]", obj.referrenceRatings.PotentialForGraduateWork)
                                                             .Replace("[[overAllRecommendation]]", obj.overAllRecommendation);

            // convert the html to pdf and save it to file system 
            InputStream = ConvertHTMLToPDF(recommenderFormTemplate,fileName, workingFolderPath);
            return File(InputStream, "application/pdf;", fileName + ".pdf");

        }
        private byte[] ConvertHTMLToPDF(string fileContent,string fileName, string destinationFilePath)
        {
            Byte[] fileStream = null;
            var physicalFilepath = Path.Combine(destinationFilePath,fileName);
            TextReader reader = new StringReader(fileContent);

            // step 1: creation of a document-object
            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 30, 30);

            // step 2:
            // we create a writer that listens to the document
            iTextSharp.text.pdf.PdfWriter.GetInstance(document, new FileStream(physicalFilepath, FileMode.Create));

            // step 3: we create a worker parse the document
            iTextSharp.text.html.simpleparser.HTMLWorker worker = new iTextSharp.text.html.simpleparser.HTMLWorker(document);

            // step 4: we open document and start the worker on the document
            document.Open();
            worker.StartDocument();

            // step 5: parse the html into the document
            worker.Parse(reader);
            //PdfContentByte cb=PdfWriter
            // step 6: close the document and the worker
            worker.EndDocument();
            worker.Close();
            document.Close();
            //--------------------read the saved file and return the byte array as filecontent
            
            System.IO.FileStream fs = new System.IO.FileStream(physicalFilepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            long byteLength = new System.IO.FileInfo(physicalFilepath).Length;
            fileStream = binaryReader.ReadBytes((Int32)byteLength);
            fs.Close();
            fs.Dispose();
            binaryReader.Close();

            return fileStream;
        }
        private string GetFormTemplate(string templateName)
        {
            string body = string.Empty;
            string filepath = Path.Combine("SupportFiles/DocumentTemplates", templateName);
            using (StreamReader reader = new StreamReader(Path.GetFullPath(filepath)))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }
    }
    public class GraduateProgramRecommenderForm
    {
        public personalInfo personalInfo { get; set; }
        public relationshipToApplicant relationshipToApplicant { get; set; }
        public referrenceRatings referrenceRatings { get; set; }
        public string overAllRecommendation { get; set; }
    }
    public class personalInfo
    {
        public string studentName { get; set; }
        public string recommenderName { get; set; }
        public string occupation { get; set; }
        public string jobTitle { get; set; }
        public string organization { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }
    public class relationshipToApplicant
    {
        public string applicationKnowTime { get; set; }
        public string applicationKnowSource { get; set; }
    }
    public class referrenceRatings
    {
        public string communicationSkillsOral { get; set; }
        public string communicationSkillsWritten { get; set; }
        public string TechnologySkills { get; set; }
        public string Initiative { get; set; }
        public string Maturity { get; set; }
        public string MotivationForTheProgramOfStudy { get; set; }
        public string Creativity { get; set; }
        public string AbilityToWorkWithOthers { get; set; }
        public string IntellectualPotential { get; set; }
        public string PresentAcademicPerformance { get; set; }
        public string PotentialForGraduateWork { get; set; }
    }
    public class MileageReportRequest
    {
        public string DirectionsJSON { get; set; }
        //public byte[] MapContent { get; set; }
    }
}
