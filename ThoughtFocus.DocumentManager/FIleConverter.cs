using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
//using Microsoft.Office.Interop.Excel;
//using Microsoft.Office.Interop.PowerPoint;
using iTextSharp.text;
using iTextSharp.text.pdf;
//using ppt = Microsoft.Office.Interop.PowerPoint;
using System.IO;

namespace ThoughtFocus.DocumentManager
{
    public class FIleConverter:IFileConverter
    {
        public void ConvertDocumentToPDF(string srcFilename, string dstFilename)  
        {
            ////Console.WriteLine("Converting document: {0} to {1}", srcFilename, dstFilename);

            Microsoft.Office.Interop.Word.ApplicationClass MSWordDoc;
            object UnknownType = Type.Missing;
            object readOnly = true;
            object InputLocation = srcFilename;
            object OutputLocation = dstFilename;

            MSWordDoc = new Microsoft.Office.Interop.Word.ApplicationClass();

            try
            {
                MSWordDoc.Visible = false;
                //To Open the Word Document
                MSWordDoc.Documents.Open(ref InputLocation,    //Input File Name Location
                    ref UnknownType,    // Conversion Conformation
                    ref readOnly,       // Set ReadOnly Property
                    ref UnknownType,    // Add to the Recent Files
                    ref UnknownType,    // Document Password Setting
                    ref UnknownType,    // Password Templete
                    ref UnknownType,    // Revert
                    ref UnknownType,    // Write Password to Document
                    ref UnknownType,    // Write Password Template
                    ref UnknownType,    // File Format
                    ref UnknownType,    // Encoding File
                    ref UnknownType,    // Visibility
                    ref UnknownType,    // To Open or Repair
                    ref UnknownType,    // Document Direction
                    ref UnknownType,    // Encoding Dialog
                    ref UnknownType);   // XML Text Transform

                //To Get Document in PDF Format
                object SavePDFFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;

                MSWordDoc.Visible = false;
                MSWordDoc.DisplayAlerts = Microsoft.Office.Interop.Word.WdAlertLevel.wdAlertsNone;
                MSWordDoc.Options.SavePropertiesPrompt = false;
                MSWordDoc.Options.SaveNormalPrompt = false;

                //To SaveAs the Document
                MSWordDoc.ActiveDocument.SaveAs(ref OutputLocation, //Output File Location
                ref SavePDFFormat,    // File Format
                ref UnknownType,    // Comment to PDF File
                ref UnknownType,    // Password
                ref UnknownType,    // Add to Recent File
                ref UnknownType,    // Write Password
                ref UnknownType,    // ReadOnly Propert
                ref UnknownType,    // Original Font Embeding
                ref UnknownType,    // Save Picture
                ref UnknownType,    // Saving Form Datas
                ref UnknownType,    // Save as AOVE Letter
                ref UnknownType,    // Encoding
                ref UnknownType,    // Inserting Line Breakes
                ref UnknownType,    // Allow Substitution
                ref UnknownType,    // Line Ending
                ref UnknownType);   // Add BiDi Marks

                //To Close the Document File
                MSWordDoc.Documents.Close(ref UnknownType, ref UnknownType, ref UnknownType);

                //To Exit the Word Application
                MSWordDoc.Quit(ref UnknownType, ref UnknownType, ref UnknownType);
                ////Console.WriteLine("Document converstion completed");
            }
            catch (Exception e)
            {
                ////Console.WriteLine("{0} Exception caught.", e);
                throw e;
            }
        }

        //public void ConvertSpreadsheetToPDF(string srcFilename, string dstFilename)
        //{
        //    ////Console.WriteLine("Converting spreadsheet: {0} to {1}", srcFilename, dstFilename);
        //    Microsoft.Office.Interop.Excel.ApplicationClass MSExcelDoc;
        //    MSExcelDoc = new Microsoft.Office.Interop.Excel.ApplicationClass();

        //    try
        //    {
        //        Microsoft.Office.Interop.Excel.Workbook wkb = MSExcelDoc.Workbooks.Open(srcFilename);
        //        Microsoft.Office.Interop.Excel.Worksheet myWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)wkb.Worksheets.get_Item(1);
        //        XlFixedFormatType paramExportFormat = XlFixedFormatType.xlTypePDF;
        //        XlFixedFormatQuality paramExportQuality = XlFixedFormatQuality.xlQualityStandard;
        //        bool paramOpenAfterPublish = false, paramIncludeDocProps = true;
        //        bool paramIgnorePrintAreas = true; //To Ignore PrintArea if set in Excel
        //        object paramFromPage = Type.Missing;
        //        object paramToPage = Type.Missing;

        //        //wkb.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, dstFilename)
        //        wkb.ExportAsFixedFormat(paramExportFormat, dstFilename, paramExportQuality, paramIncludeDocProps, paramIgnorePrintAreas, paramFromPage, paramToPage, paramOpenAfterPublish);
        //        //Console.WriteLine("Spredsheet converstion completed");
        //    }
        //    catch (Exception e)
        //    {
        //        //Console.WriteLine("{0} Exception caught.", e);
        //    }
        //    finally
        //    {
        //        MSExcelDoc.Quit();
        //    }
        //}
        #region Convert PPT TO PDF
        //public void ConvertPPTToPDF(string srcFilename, string dstFilename)
        //{
        //    ppt.Application powerpointApp = new ppt.Application();

        //    ppt.Presentation presentation = powerpointApp.Presentations.Open(srcFilename,
        //        MsoTriState.msoTrue, //ReadOnly
        //        Microsoft.Office.Core.MsoTriState.msoFalse, //Untitled
        //        Microsoft.Office.Core.MsoTriState.msoFalse); //Window not visible during converting

        //    presentation.ExportAsFixedFormat(dstFilename,
        //                    ppt.PpFixedFormatType.ppFixedFormatTypePDF);

        //    presentation.Close(); //Close document
        //    powerpointApp.Quit();
        //}
        # endregion
        public void ConvertImageToPDF(string srcFilename, string dstFilename)
        {
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            iTextSharp.text.pdf.PdfWriter.GetInstance(document, new FileStream(dstFilename, FileMode.Create));
            document.Open();
            System.Uri uri = new Uri(srcFilename);
            iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(uri);

            //Scaling the image
            if (pic.Height > pic.Width)
            {
                float percentage = 0.0f;
                percentage = 700 / pic.Height;
                pic.ScalePercent(percentage * 100);
            }
            else
            {
                float percentage = 0.0f;
                percentage = 540 / pic.Width;
                pic.ScalePercent(percentage * 100);
            }

            document.Add(pic);
            document.Close();
        }

        public void ConvertHTMLToPDF(string srcFilename, string dstFilename)
        {
            string st = System.IO.File.ReadAllText(srcFilename);
            TextReader reader = new StringReader(st);

            // step 1: creation of a document-object
            iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 30, 30, 30, 30);

            // step 2:
            // we create a writer that listens to the document
            iTextSharp.text.pdf.PdfWriter.GetInstance(document, new FileStream(dstFilename, FileMode.Create));

            // step 3: we create a worker parse the document
            iTextSharp.text.html.simpleparser.HTMLWorker worker = new iTextSharp.text.html.simpleparser.HTMLWorker(document);

            // step 4: we open document and start the worker on the document
            document.Open();
            worker.StartDocument();

            // step 5: parse the html into the document
            worker.Parse(reader);

            // step 6: close the document and the worker
            worker.EndDocument();
            worker.Close();
            document.Close();
        }

        public void Merge(String OutFile, string[] mergelist)
        {
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
                    iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                    copyProvider.AddDocument(pdfReader);
                    pdfReader.Close();
                }
            }
            document.Close();
        }

        public void Merge(string OutFile, MemoryStream[] msList)
        {
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

            foreach (MemoryStream ms in msList)
            {
                iTextSharp.text.Document fsDoc = new iTextSharp.text.Document();
                PdfReader _reader = new PdfReader((byte[])ms.ToArray());
                copyProvider.AddDocument(_reader);
                _reader.Close();
            }
            document.Close();
        }

        //public void ConvertWorksheetRangeToPDF()
        //{
        //    Microsoft.Office.Interop.Excel.Application xlsApp = new Microsoft.Office.Interop.Excel.Application();
        //    xlsApp.ScreenUpdating = false;
        //    Microsoft.Office.Interop.Excel.Workbook xlsBook;
        //    XlFixedFormatType paramExportFormat = XlFixedFormatType.xlTypePDF;
        //    XlFixedFormatQuality paramExportQuality = XlFixedFormatQuality.xlQualityStandard;
        //    bool paramOpenAfterPublish = false, paramIncludeDocProps = true, paramIgnorePrintAreas = true;
        //    object paramFromPage = Type.Missing;
        //    object paramToPage = Type.Missing;
        //    xlsBook = xlsApp.Workbooks.Open(@"D:\Projects\CSLUB\MergerPOC\Files\X02.xls", false, false);
        //    Microsoft.Office.Interop.Excel.Worksheet srcWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlsBook.Worksheets.get_Item(1); ;
        //    var range = srcWorksheet.Range["C10"];
        //    range.ExportAsFixedFormat(paramExportFormat, @"D:\Projects\CSLUB\MergerPOC\Files\bb.pdf", paramExportQuality, paramIncludeDocProps, paramIgnorePrintAreas, paramFromPage, paramToPage, paramOpenAfterPublish);
        //    xlsBook.Close(false);
        //    xlsApp.Quit();

        //}

        //public void ConvertWorksheetToPDF(string srcFileName, string sheetName)
        //{
        //    Microsoft.Office.Interop.Excel.Application xlsApp = new Microsoft.Office.Interop.Excel.Application();
        //    xlsApp.ScreenUpdating = false;
        //    Microsoft.Office.Interop.Excel.Workbook xlsBook;
        //    XlFixedFormatType paramExportFormat = XlFixedFormatType.xlTypePDF;
        //    XlFixedFormatQuality paramExportQuality = XlFixedFormatQuality.xlQualityStandard;
        //    bool paramOpenAfterPublish = false, paramIncludeDocProps = true, paramIgnorePrintAreas = true;
        //    object paramFromPage = Type.Missing;
        //    object paramToPage = Type.Missing;
        //    xlsBook = xlsApp.Workbooks.Open(srcFileName, false, false);
        //    Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlsBook.Worksheets[sheetName];
        //    worksheet.ExportAsFixedFormat(paramExportFormat, @"D:\Projects\CSLUB\MergerPOC\Files\aa.pdf", paramExportQuality, paramIncludeDocProps, paramIgnorePrintAreas, paramFromPage, paramToPage, paramOpenAfterPublish);
        //    xlsBook.Close(false);
        //    xlsApp.Quit();
        //}

     
    }
}
