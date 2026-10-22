using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Domain.Request.StudentProfile;
using ThoughtFocus.Domain.Request.StudentTeachingMilestone;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.StudentProfile;
using ThoughtFocus.Domain.Response.StudentTeachingMilestone;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class StudentTeachingMilestoneServiceImpl : IStudentTeachingMilestoneService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        public ILogger<StudentTeachingMilestoneServiceImpl> _logger;
        private readonly ICommonUtils _utils;
        public StudentTeachingMilestoneServiceImpl(ISqlDBUtility helper
                                         , IConfiguration configuration
                                         , ISendMail sendMail
                                         , ILogger<StudentTeachingMilestoneServiceImpl> logger
                                         , ICommonUtils utils)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
            _utils = utils;
        }
        public GetLatestFormResponse GetLatestForm(string csulbid)
        {
            GetLatestFormResponse obj = new GetLatestFormResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@CSULBID", SqlDbType.VarChar,9) { Value = csulbid }
                                        };

            DataTable dtFormDetails = _helper.GetDataTable("[Milestone].[GetLatestForm]", parameters);
            try
            {
                if (dtFormDetails.Rows.Count > 0)
                {
                    obj.getLatestFormDetails = dtFormDetails.AsEnumerable().Select(row =>
                                              new GetLatestFormDetails
                                              {
                                                  ProgramID = Convert.ToInt32(row["ProgramID"]),
                                                  FormId = Convert.ToInt32(row["FormId"]),
                                                  TermCode = Convert.ToString(row["TermCode"])
                                              }).FirstOrDefault();
                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";
                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public StudentTeachingMilestonesResponse GetStudentTeachingMilestoneDetails(string CsuldId, int UserID, int formID, int roleID)
        {
            StudentTeachingMilestonesResponse obj = new StudentTeachingMilestonesResponse();
            
            SqlParameter[] parameters = {
                                          new SqlParameter("@csulbid", SqlDbType.VarChar, 50) { Value = CsuldId },
                                          new SqlParameter("@UserID", SqlDbType.VarChar, 50) { Value = UserID },
                                          new SqlParameter("@FormID", SqlDbType.NVarChar, -1) { Value = formID },
                                          new SqlParameter("@LoggedInRole", SqlDbType.NVarChar, -1) { Value = roleID }
                                        };

            DataSet dtStudentTeaching = _helper.GetDataSet("[Milestone].[GetStudentTeachingData]", parameters);
            try
            {
                if (dtStudentTeaching.Tables.Count > 0)
                {
                    obj.studentTeachingMilestones = dtStudentTeaching.Tables[0].AsEnumerable().Select(row => new StudentTeachingMilestones
                    {
                        CSULBID = Convert.ToString(row["CSULBID"]),
                        FirstName = Convert.ToString(row["FirstName"]),
                        MiddleName = Convert.ToString(row["MiddleName"]),
                        LastName = Convert.ToString(row["LastName"]),
                        PreferredName = Convert.ToString(row["PreferredName"]),
                        AlternateName = Convert.ToString(row["AlternateName"]),
                        MailingAddress1 = Convert.ToString(row["MailingAddress1"]),
                        MailingAddress2 = Convert.ToString(row["MailingAddress2"]),
                        MailingAddress3 = Convert.ToString(row["MailingAddress3"]),
                        MailingAddress4 = Convert.ToString(row["MailingAddress4"]),
                        MailingCity = Convert.ToString(row["MailingCity"]),
                        MailingState = Convert.ToString(row["MailingState"]),
                        MailingPostal = Convert.ToString(row["MailingPostal"]),
                        Phone = Regex.Replace(Convert.ToString(row["Phone"]).Replace("/", "").Replace("-", ""), @"(\d{3})(\d{3})(\d{0,4})", "($1)-$2-$3"),
                        csulbemail = Convert.ToString(row["CSULBEmail"]),
                        AlternateEmail = Convert.ToString(row["AlternateEmail"]),
                        ConsolidatedAddress = Convert.ToString(row["ConsolidatedAddress"]),
                        UserID = Convert.ToInt32(row["UserID"]),
                        CredentialProgram = Convert.ToString(row["CredentialProgram"]),
                        CredentialPathway = Convert.ToString(row["CredentialPathway"]),
                        TermCode = Convert.ToString(row["TermCode"]),
                        ProgramStatusDesc = Convert.ToString(row["ProgramStatusDesc"])

                    }).FirstOrDefault();
                    obj.studentTeachingAcknowledgment = dtStudentTeaching.Tables[1].AsEnumerable().Select(row => new StudentTeachingAcknowledgment
                    {
                        AcknowledgmentJSON = Convert.ToString(row["AcknowledgementsJSON"])
                    }).FirstOrDefault();
                    obj.AttachmentsInformation = dtStudentTeaching.Tables[3].AsEnumerable().Select(row => new AttachmentsInformation
                    {
                        ID = row["ID"] == DBNull.Value ? 0: Convert.ToInt32(row["ID"]),
                        DocumentID = Convert.ToInt32(row["DocumentID"]),
                        DocumentName = Convert.ToString(row["DocumentName"]),
                        FileName = Convert.ToString(row["FileName"]),
                        FileExtn = Convert.ToString(row["FileExtn"]),
                        UniqueID = row["UniqueID"] == DBNull.Value ? Guid.Empty: Guid.Parse(Convert.ToString(row["UniqueID"])),
                        FolderName = Convert.ToString(row["FolderName"])
                    }).ToList();

                    obj.IsSuccess = true;
                    obj.Message = "Data retrieved succesfully ";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }

            return obj;
        }
        public DistrictListResponse GetDistrictsByProgramID(int programId)
        {
            DistrictListResponse obj = new DistrictListResponse();
            SqlParameter[] parameters ={
                                            new SqlParameter("@ProgramId", SqlDbType.BigInt) { Value = programId }
                                       };

            DataTable dtDD = _helper.GetDataTable("[Milestone].[GetDistrictsByProgramID]", parameters);
            try
            {
                if (dtDD.Rows.Count > 0)
                {
                    
                        obj.Districts = dtDD.AsEnumerable().Select(row =>
                                              new Districts
                                              {
                                                  ID = Convert.ToInt32(row["ID"]),
                                                  ProgramID = Convert.ToInt32(row["ProgramID"]),
                                                  Name = Convert.ToString(row["Name"]),
                                              }).ToList();
                    
                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public CredentialAreaListResponse GetCredentialPathwayAreaList()
        {
            CredentialAreaListResponse obj = new CredentialAreaListResponse();
            SqlParameter[] parameters = { };

            DataTable dtDD = _helper.GetDataTable("[Milestone].[GetCredentialAreas]", parameters);
            try
            {
                if (dtDD.Rows.Count > 0)
                {

                    obj.CredentialArea = dtDD.AsEnumerable().Select(row =>
                                          new CredentialArea
                                          {
                                              ID = Convert.ToInt32(row["ID"]),
                                              Name = Convert.ToString(row["Name"])
                                          }).ToList();

                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }
        public AttachmentResponse UpsertAttachement(UpsertAttachments input)
        {
            AttachmentResponse response = new AttachmentResponse();
            SqlParameter[] parameters =
                          {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@DocumentID", SqlDbType.BigInt) { Value = input.DocumentID },
                                          new SqlParameter("@UniqueID", SqlDbType.UniqueIdentifier) { Value = input.UniqueID },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 200) { Value = input.FileName },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = input.FileExtn },
                                          new SqlParameter("@CreatedBy", SqlDbType.BigInt) { Value = input.CreatedBy }
                                        };
            DataTable dtFormAttachment = _helper.GetDataTable("[STMilestone].[UpsertAttachements]", parameters);
            if (input.FileContent != null && input.FileContent.Length > 0)
            {
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                string fileExtensionWord = string.Empty;
                string userFolderName = string.Empty;
                string savedFileName = string.Empty;
                string subSectionName = string.Empty;
                bool isNotPDFExtension = false;
                var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];

                var workingFolderPath = Path.Combine(fileRepoPath, "WorkingFolder");
                if (input.FileName != string.Empty)
                {
                    //AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.FileName);
                    //fileExtension = fileDetails.FileExtension;
                    //fileName = input.FileName;
                    if (fileExtension.ToUpper() == "PNG" || fileExtension.ToUpper() == "JPG" || fileExtension.ToUpper() == "JPEG")
                    {
                        // isNotPDFExtension = true;
                        // logic to convert png to pdf 
                        byte[] imageContent = null;
                        imageContent = GetImageFilecontent(input.FileContent);
                        input.FileContent = null;
                        input.FileContent = imageContent;
                        fileExtension = "pdf";
                    }
                }
                if (dtFormAttachment.Rows.Count > 0 && input.FileName != string.Empty)
                {
                    string[] folderSplit = dtFormAttachment.Rows[0]["FolderName"].ToString().Split('~');
                    userFolderName = folderSplit[0].ToString();
                    string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
                    fileName = input.FormID + "_" + input.FileName;
                    if (Directory.Exists(dirUserFolderPath))
                    {
                        string dirForm = Path.Combine(dirUserFolderPath, "Form");
                        if (Directory.Exists(dirForm))
                        {
                            File.WriteAllBytes(Path.Combine(dirForm, fileName + "." + input.FileExtn), input.FileContent);

                        }
                        else
                        {
                            Directory.CreateDirectory(dirForm);
                            File.WriteAllBytes(Path.Combine(dirForm, fileName + "." + input.FileExtn), input.FileContent);

                        }
                    }
                    else
                    {
                        string dirForm = Path.Combine(dirUserFolderPath, "Form");
                        DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                        DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirForm);
                        DirectorySecurity dSecurity = dirFieldWorkFolder.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dirFieldWorkFolder.SetAccessControl(dSecurity);
                        File.WriteAllBytes(Path.Combine(dirForm, fileName + "." + input.FileExtn), input.FileContent);

                    }
                    response.FileName = Convert.ToString(dtFormAttachment.Rows[0]["FileName"]);
                    response.FileExtn = Convert.ToString(dtFormAttachment.Rows[0]["FileExtn"]);
                    response.UniqueID = (Guid)(dtFormAttachment.Rows[0]["UniqueID"]);
                    response.AttachementID = Convert.ToInt32(dtFormAttachment.Rows[0]["AttachementID"]);
                    response.IsSuccess = true;
                    response.Message = "Attachment Uploaded Successfully";
                }
                return response;
            }
            else
            {
                response.IsSuccess = true;
                response.Message = "No Attachment to upload";
                return response;
            }
        }
        public DownloadAttachementResponse DownloadAttachement(Guid UniqueID)
        {
            DownloadAttachementResponse obj = new DownloadAttachementResponse();

            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@UniqueID", SqlDbType.UniqueIdentifier) { Value = UniqueID }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[STMilestone].[GetAttachement]", parameters);
            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new DownloadAttachementResponse
                                          {
                                              ID = Convert.ToInt32(row["ID"]),
                                              UniqueID = Guid.Parse(row["UniqueID"].ToString()),
                                              FileName = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                              FolderName = Convert.ToString(row["FolderName"]),
                                              CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                                              CreatedDate = Convert.ToDateTime(row["CreatedDate"] == DBNull.Value ? null : row["CreatedDate"]),
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetProfileAttachmentFileContent(_utils.GetAttachmentsFolderName(row["FolderName"].ToString()), _utils.GetAttachmentsSavedFileName(row["FolderName"].ToString()) + "." + Convert.ToString(row["FileExtn"]))
                                          }).FirstOrDefault();
            obj.IsSuccess = true;
            obj.Message = "Attachment retrieved Successfully";

            return obj;
        }
        public BaseResponse DeleteAttachement(DeleteAttachementRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                    {
                                           new SqlParameter("@UniqueID", SqlDbType.UniqueIdentifier) { Value = input.UniqueID }
                                     };
            try
            {
                DataTable dtAttachment = _helper.GetDataTable("[STMilestone].[DeleteAttachement]", parameters);
                if (dtAttachment.Rows.Count > 0)
                {
                    if (Convert.ToString(dtAttachment.Rows[0]["RESULT"]) == "SUCCESS")
                    {
                        response.Message = "Attachment Deleted Successfully";
                        response.IsSuccess = true;
                    }
                    else if (Convert.ToString(dtAttachment.Rows[0]["RESULT"]) == "FAILURE")
                    {
                        response.Message = "Failed to Delete Attachment";
                        response.IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Data Retrieval Failed , Please contact site admin ";
                response.StackTrace = ex.Message;
            }
            return response;
        }
        private byte[] GetImageFilecontent(byte[] fileContent)
        {
            byte[] inputStream = null;
            string documentName = string.Empty;
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                //Initialize the PDF document object.
                using (iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 10f, 10f, 10f, 10f))
                {
                    PdfWriter.GetInstance(pdfDoc, stream).SetFullCompression();
                    pdfDoc.Open();

                    //Add the Image file to the PDF document object.
                    iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(fileContent);

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
                    pdfDoc.Add(pic);
                    pdfDoc.Close();
                    inputStream = stream.ToArray();
                }
            }
            return inputStream;
        }
        public byte[] GetProfileAttachmentFileContent(string userFolderPath, string fileName)
        {
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            string userPath = Path.Combine(fileRepoPath, Path.Combine(userFolderPath, "Form"));
            string filepath = Path.Combine(userPath, fileName);
            byte[] fileContent = null;
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            long byteLength = new System.IO.FileInfo(filepath).Length;
            fileContent = binaryReader.ReadBytes((Int32)byteLength);
            fs.Close();
            fs.Dispose();
            binaryReader.Close();
            return fileContent;
        }
    }
}
