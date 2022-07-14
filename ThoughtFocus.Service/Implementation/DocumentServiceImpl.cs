using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Service.Interfaces;
using ThoughtFocus.DocumentManager;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace ThoughtFocus.Service.Implementation
{
    public class DocumentServiceImpl : IDocumentService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly IFileConverter _fileConverter;
        static bool useMultiThread = false;
        public DocumentServiceImpl(ISqlDBUtility helper, IConfiguration configuration, IFileConverter fileConverter)
        {
            _helper = helper;
            _configuration = configuration;
            _fileConverter = fileConverter;
        }
        public byte[] GetDocument(int userId, int formAttachmentId)
        {
            byte[] inputStream = null;


            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@UserID", SqlDbType.Int, 50) { Value = userId },
                                          new SqlParameter("@FormID", SqlDbType.Int, 50) { Value = 0 },
                                          new SqlParameter("@FormAttachmentId", SqlDbType.Int, 50) { Value = formAttachmentId }
                                        };

            DataSet dsGetDocument = _helper.GetDataSet("[dbo].[GetDocuments]", parameters);
            if (dsGetDocument.Tables.Count > 0)
            {
               
                
            }

            return inputStream;
        }

        public byte[] GetMergedDocument(int userId, int formId)
        {
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            string fileServerPath = fileRepoPath.ToString();
            byte[] inputStream = null;

            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@UserID", SqlDbType.Int, 50) { Value = userId },
                                          new SqlParameter("@FormID", SqlDbType.Int, 50) { Value = formId },
                                          new SqlParameter("@FormAttachmentId", SqlDbType.Int, 50) { Value = 0 }
                                        };

            DataSet dsGetDocuments = _helper.GetDataSet("[dbo].[GetDocuments]", parameters);
            if (dsGetDocuments.Tables.Count > 0)
            {
                DataTable dtuserFolder = dsGetDocuments.Tables[0];
                DataTable dtDocuments = dsGetDocuments.Tables[1];
                string userFolderName = string.Empty; // pull this value from SP table[0] , once the upload is ready pull it from tables[1] directly 
                if (dtuserFolder.Rows.Count > 0)
                {
                    userFolderName = dtuserFolder.Rows[0]["foldername"].ToString();
                    if (dtDocuments.Rows.Count > 0)
                    {
                        for(int i = 0; i < dtDocuments.Rows.Count; i++)
                        {
                            string file = string.Empty;
                            string uploadedFileName = string.Empty;
                            string uploadedFileExtension = string.Empty;
                            uploadedFileName = dtDocuments.Rows[i]["FileName"].ToString();
                            uploadedFileExtension = dtDocuments.Rows[i]["FileExtn"].ToString();
                            file = ($"{uploadedFileName}.{uploadedFileExtension}");
                            string fullPath = Path.Combine(fileServerPath, userFolderName);
                            string fileName = Path.GetFileName(file);
                            using (FileStream stream = new FileStream(Path.Combine(fullPath, fileName), FileMode.Create))
                            {
                                FileInfo fi = new FileInfo(Path.Combine(fullPath, fileName));
                                
                               // fi.CopyTo(stream);
                                object oFile = fi.DirectoryName + "\\" + Path.GetFileNameWithoutExtension(fi.Name) + ".pdf";
                                if (!useMultiThread)
                                    this.ConvertFile(Path.Combine(fullPath, fileName));
                                else
                                    (new Thread(() => this.ConvertFile(Path.Combine(fullPath, fileName)))).Start();
                            }
                        }
                    }
                }


                FIleConverter fc = new FIleConverter();
            }

                return inputStream;
        }
        private void ConvertFile(string srcFileName)
        {
            FileInfo fi = new FileInfo(srcFileName);
            object oFile = string.Empty;
            if (fi.Exists)
            {
                //FIleConverter fc = new FIleConverter();
                oFile = fi.DirectoryName + "\\" + Path.GetFileNameWithoutExtension(fi.Name) + ".pdf";
                if (fi.Extension.ToString().ToUpper().Equals(".DOC") || fi.Extension.ToString().ToUpper().Equals(".DOCX"))
                {
                    _fileConverter.ConvertDocumentToPDF(fi.FullName, oFile.ToString());
                }
                else if (fi.Extension.ToString().ToUpper().Equals(".XLS") || fi.Extension.ToString().ToUpper().Equals(".XLSX"))
                {
                    _fileConverter.ConvertSpreadsheetToPDF(fi.FullName, oFile.ToString());
                }
                //else if (fi.Extension.ToString().ToUpper().Equals(".PPT") || fi.Extension.ToString().ToUpper().Equals(".PPTX"))
                //{
                //    fc.ConvertPPTToPDF(fi.FullName, oFile.ToString());
                //}
                else if (fi.Extension.ToString().ToUpper().Equals(".TXT"))
                {
                    _fileConverter.ConvertDocumentToPDF(fi.FullName, oFile.ToString());
                }
                else if (fi.Extension.ToString().ToUpper().Equals(".HTML") || fi.Extension.ToString().ToUpper().Equals(".HTM"))
                {
                    _fileConverter.ConvertHTMLToPDF(fi.FullName, oFile.ToString());
                }
                else if (fi.Extension.ToString().ToUpper().Equals(".JPEG") || fi.Extension.ToString().ToUpper().Equals(".JPG")
                    || fi.Extension.ToString().ToUpper().Equals(".PNG") || fi.Extension.ToString().ToUpper().Equals(".GIF")
                    || fi.Extension.ToString().ToUpper().Equals(".TIFF") || fi.Extension.ToString().ToUpper().Equals(".BMP"))
                {
                    _fileConverter.ConvertImageToPDF(fi.FullName, oFile.ToString());
                }
                else
                {
                    oFile = "";
                }
            }
        }
    }
}
