using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PasswordGenerator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Enumeration;
using ThoughtFocus.Domain.Request.FieldWork;
using ThoughtFocus.Domain.Request.InitialCredentialProgram;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;
using ThoughtFocus.Service.Interfaces;
using static ThoughtFocus.Domain.Request.GraduateProgram.DispositionMSCPFiledata;
using static ThoughtFocus.Domain.Response.FieldWork.FieldWorkActivityLogListResponse;

namespace ThoughtFocus.Service.Implementation
{
    public class FieldWorkServiceImpl : IFieldWorkService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        public ILogger<FieldWorkServiceImpl> _logger;
        public FieldWorkServiceImpl(ISqlDBUtility helper, IConfiguration configuration, ISendMail sendMail, ILogger<FieldWorkServiceImpl> logger)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
        }
        public FieldWorkDataResponse GetFieldWorkDetailsById(int userId, int fieldWorkId)
        {
            FieldWorkDataResponse obj = new FieldWorkDataResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserId", SqlDbType.Int, 50) { Value = userId },
                                          new SqlParameter("@FieldWorkId", SqlDbType.Int, 50) { Value = fieldWorkId }
                                        };

            DataSet dsFieldWorkData = _helper.GetDataSet("[dbo].[GetFieldWork]", parameters);
            try
            {
                if (dsFieldWorkData.Tables.Count > 0)
                {
                    obj.FieldWork = dsFieldWorkData.Tables[0].AsEnumerable().Select(row =>
                                              new FieldWorkResponse
                                              {
                                                  FieldWorkId = Convert.ToInt32(row["ID"]),
                                                  StudentName = Convert.ToString(row["StudentName"]),
                                                  CourseTitle = Convert.ToString(row["CourseTitle"]),
                                                  CSULBCourseID = Convert.ToString(row["Course"]),
                                                  College = Convert.ToString(row["College"]),
                                                  Section = Convert.ToString(row["Section"]),
                                                  Term = Convert.ToString(row["Term"]),
                                                  FieldWorkPrerequisiteStatus = Convert.ToInt32(row["FieldWorkPrerequisiteStatus"]),
                                                  UIHandler = Convert.ToString(row["UIHandler"])
                                              }).FirstOrDefault();

                    obj.FieldWorkRoles = dsFieldWorkData.Tables[1].AsEnumerable().Select(row =>
                                              new FieldWorkRoles
                                              {
                                                  ID = Convert.ToInt32(row["ID"]),
                                                  UserID = Convert.ToInt32(row["UserID"]),
                                                  FieldWorkID = Convert.ToInt32(row["FieldWorkID"]),
                                                  RoleID = Convert.ToInt32(row["RoleID"]),
                                                  Name = Convert.ToString(row["Name"]),
                                                  Description = Convert.ToString(row["Description"])
                                              }).ToList();

                    obj.FieldWorkAttachments = dsFieldWorkData.Tables[2].AsEnumerable().Select(row =>
                                              new FieldWorkProfileAttachments
                                              {
                                                  FieldWorkAttachmentID = Convert.ToInt32(row["ID"]),
                                                  UserID = Convert.ToInt32(row["UserID"]),
                                                  DocumentID = Convert.ToInt32(row["DocumentID"]),
                                                  DocumentName = Convert.ToString(row["DocumentName"]),
                                                  FileName = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                                  // FileExtn = Convert.ToString(row["FileExtn"]),
                                                  // FolderName = Convert.ToString(row["FolderName"]),
                                                  IsApproved = Convert.ToBoolean(row["IsApproved"] == DBNull.Value ? null : row["IsApproved"]),
                                                  ApprovedBy = Convert.ToString(row["ApprovedBy"] == DBNull.Value ? null : row["ApprovedBy"]),
                                                  ValidatedDate = Convert.ToDateTime(row["ValidatedDate"] == DBNull.Value ? null : row["ValidatedDate"]),
                                                  //CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                                                  //CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                                                  ValidTill = Convert.ToDateTime(row["ValidTill"] == DBNull.Value ? null : row["ValidTill"]),
                                                  //FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(Path.Combine(row["FolderName"].ToString(), "FieldWork"), Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"])),
                                                  RejectReason = Convert.ToString(row["RejectedReason"]),
                                                  Comments = Convert.ToString(row["Comments"]),
                                                  DocumentStatus = Convert.ToString(row["DocumentStatus"]),
                                                  DocumentInfo = Convert.ToString(row["DocumentInfo"]),
                                                  CanUpload = Convert.ToBoolean(row["CanUpload"]),
                                                  CanValidate = Convert.ToBoolean(row["CanValidate"])
                                              }).ToList();

                    obj.FieldWorkHours = dsFieldWorkData.Tables[3].AsEnumerable().Select(row =>
                                          new FieldWorkSummaryTabHours
                                          {
                                              ExpectedHours = Convert.ToDecimal(row["ExpectedHours"]),
                                              LoggedHours = Convert.ToDecimal(row["LoggedHours"]),
                                              SentforApproval = Convert.ToDecimal(row["SentforApproval"]),
                                              ApprovedHours = Convert.ToDecimal(row["ApprovedHours"]),
                                              Approved = Convert.ToDecimal(row["Approved"])

                                          }).FirstOrDefault();

                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

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

        public byte[] GetFileContent(string userFolderPath, string fileName)
        {
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            string filepath = Path.Combine(fileRepoPath, Path.Combine(userFolderPath, fileName));
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
        public FieldWorkListResponse GetFieldWorkList(int userId)
        {
            FieldWorkListResponse objList = new FieldWorkListResponse();
            List<FieldWorkResponse> obj = new List<FieldWorkResponse>();
            var jsonObj = JObject.Parse(File.ReadAllText(@"SupportFiles/MycedConfigurations/CSULBCEDConfig.json"));
            string csulbIDs = String.Empty;
            csulbIDs = jsonObj["EnableIntern2"]?.ToString();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserId", SqlDbType.Int, 50) { Value = userId },
                                          new SqlParameter("@CSULBIDs", SqlDbType.NVarChar, -1) { Value = csulbIDs }
                                        };
            var courseList = jsonObj["FieldWorkValidCourses"]?.ToString().Split(',').Select(id => id.Trim());
            var summer2025ValidCourses = jsonObj["FieldWorkValidCourses_Summer2025"]?.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(id => id.Trim()).ToList();

            DataTable dtFieldWorkList = _helper.GetDataTable("[dbo].[GetFieldWorkData]", parameters);
            try
            {
                if (dtFieldWorkList.Rows.Count > 0)
                {
                    obj = dtFieldWorkList.AsEnumerable().Where(row => courseList.Any(course => string.Equals(Convert.ToString(row["CourseName"]), course, StringComparison.OrdinalIgnoreCase))).Select(row =>
                                              new FieldWorkResponse
                                              {
                                                  FieldWorkId = Convert.ToInt32(row["ID"]),
                                                  StudentName = Convert.ToString(row["StudentName"]),
                                                  FirstName = Convert.ToString(row["FirstName"]),
                                                  LastName = Convert.ToString(row["LastName"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),
                                                  CourseTitle = Convert.ToString(row["CourseTitle"]),
                                                  CSULBCourseID = Convert.ToString(row["Course"]),
                                                  College = Convert.ToString(row["College"]),
                                                  Section = Convert.ToString(row["Section"]),
                                                  Term = Convert.ToString(row["Term"]),
                                                  FieldWorkPrerequisiteStatus = Convert.ToInt32(row["FieldWorkPrerequisiteStatus"]),
                                                  PrerequisiteStatus = Convert.ToString(row["PrerequisiteStatus"]),
                                                  LoggedHours = Convert.ToDecimal(row["LoggedHours"]),
                                                  ApprovedHours = Convert.ToDecimal(row["ApprovedHours"]),
                                                  CourseName = Convert.ToString(row["CourseName"])

                                              }).ToList();

                    objList.FieldWorkResponse = obj;
                    objList.IsSuccess = true;
                    objList.Message = "Data Retrieved Successfully";

                }
            }
            catch (Exception ex)
            {
                objList.IsSuccess = false;
                objList.Message = "Data Retrievel Failed";
                objList.StackTrace = ex.Message;
            }
            return objList;
        }

        public BaseResponse UpdateFieldWorkValidation(FieldWorkValidationRequest input)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                response.IsSuccess = true;
                string responseString = string.Empty;
                if (input.ApprovalStatus == true)
                {
                    // set the validtill value and empty the rejectreason value
                    input.RejectedReason = null;
                }
                else
                {
                    // set the rejectreason value  and empty the validtill value
                    input.ValidTill = null;
                }
                DateTime validatedDate = DateTime.Now;


                SqlParameter[] parameters =
                                            {
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt, 50) { Value = input.FieldWorkAttachmentId },
                                          new SqlParameter("@IsApproved", SqlDbType.Bit, 50) { Value = input.ApprovalStatus },
                                          new SqlParameter("@ApprovedBy", SqlDbType.BigInt, 50) { Value = input.ApproverUserId },
                                          new SqlParameter("@ValidatedDate", SqlDbType.DateTime, 50) { Value = validatedDate },
                                          new SqlParameter("@ValidTill", SqlDbType.DateTime, 50) { Value = (object)input.ValidTill??DBNull.Value },
                                          new SqlParameter("@RejectReason", SqlDbType.NVarChar, 255) { Value = (object)input.RejectedReason??DBNull.Value },
                                          new SqlParameter("@Comments", SqlDbType.NVarChar,-1) { Value = (object)input.Comments??DBNull.Value }
                                        };

                int identity = _helper.InsertTable("[dbo].[UpdateFieldWorkValidation]", parameters);

                response.Message = "Prerequisite review status updated successfully.";

                try
                {

                    // create the mail body / subject and pull the students email id 
                    StudentsDetails mailInfo = GetStudentDetailsFromFieldWorkId(input.FieldWorkID, input.FieldWorkAttachmentId);
                    ApproveRejectMailer approvedRejectMailer = GetEmailSubjectAndBody(input.ApprovalStatus, mailInfo.FileName, input.RejectedReason, input.Comments, mailInfo.DisplayName);
                    // send the approve / reject mail here 
                    _sendMail.SendEmail(mailInfo.Email, "", "COMMON", approvedRejectMailer.Subject, approvedRejectMailer.Body, "");
                }
                catch (Exception ee)
                {
                    response.IsSuccess = false;
                    response.Message = "Prerequisite submitted/not submitted status send mail failure.";
                    // _sendMail.SendEmail("asif.khan@thoughtfocus.com", "", "approved/Reject Mailer", ee.Message, "");
                }

                try
                {
                    EmailMessageModel emailModel = GetMessageBody(input.FieldWorkID,input.FieldWorkAttachmentId,input.Semester);

                    // check the prerequisite status and send mail to student 
                    if (!String.IsNullOrEmpty(emailModel.Body))
                    {
                        // fire the mail 
                        string toUser = emailModel.toEmail; // pull  this from SP 
                                                            // get the below body section from HTML
                                                            // string body = "This is the body section needs to be re-visited.";
                        string body = GetMailBodyTemplate("FinalApprovedTemplate.html");
                        string logoText = "cid:myImageID";
                        body = body.Replace("[[logoPath]]", logoText).Replace("[[ApplicantName]]", emailModel.ApplicantName);
                        string subject = "MyCED prerequisites review completed";
                        _sendMail.SendEmail(toUser, "", "COMMON", subject, body, emailModel.Body);
                        response.Message = "";
                        response.Message = "Fieldwork prerequisites reviewed and mail sent to " + emailModel.ApplicantName;
                    }
                }
                catch (Exception ee)
                {
                    response.IsSuccess = false;
                    response.Message = "Prerequisite status send mail failure.";
                    //_sendMail.SendEmail("asif.khan@thoughtfocus.com", "", "Prerequisite status send mail failure", ee.Message, "");
                }


            }
            catch (Exception ex)
            {
                if (response.IsSuccess == true)
                {
                    response.IsSuccess = false;
                    response.Message = "Fieldwork validation failed";
                }
            }

            return response;
        }
        private string GetMailBodyTemplate(string templateName)
        {
            string body = string.Empty;
            string filepath = Path.Combine("SupportFiles/EmailTemplates", templateName);
            using (StreamReader reader = new StreamReader(Path.GetFullPath(filepath)))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }
        private ApproveRejectMailer GetEmailSubjectAndBody(bool approvalStatus, string documentName, string rejectReason, string comments, string displayName)
        {
            ApproveRejectMailer model = new ApproveRejectMailer();
            string body = string.Empty;
            string logopath = Path.GetFullPath("SupportFiles/Img/logo.png");
            string logoText = "cid:myImageID";
            if (approvalStatus == true)
            {
                model.Subject = "MyCED prerequisites review approved";
                // get the body from email template
                body = GetMailBodyTemplate("ApprovedMailTemplate.html");
                body = body.Replace("[[ApplicantName]]", displayName).Replace("[[DocumentName]]", documentName).Replace("[[logoPath]]", logoText);
                //body = "<html><body><p>Your document "+documentName+" has been approved</p><p>Thank you,</br>CSULB College of Education  </p></body></html>";
                model.Body = body;
            }
            else if (approvalStatus == false)
            {
                model.Subject = "MyCED prerequisites review not approved";
                // get the body from email template
                body = GetMailBodyTemplate("NotApprovedMailTemplate.html");
                body = body.Replace("[[ApplicantName]]", displayName).Replace("[[DocumentName]]", documentName).Replace("[[logoPath]]", logoText).Replace("[[Reason]]", rejectReason).Replace("[[Comment]]", comments);
                //body = "<html><body> <p>Your document "+ documentName +" has not been approved</p><p>Reason  : "+rejectReason+"</p><p>Comment : "+comments+"</p><p>Please upload a new document</p><p>Thank you,</br>CSULB College of Education  </p></body></html>";
                model.Body = body;
            }
            return model;
        }
        private StudentsDetails GetStudentDetailsFromFieldWorkId(int fieldWorkId, int fieldWorkAttachmentId)
        {
            StudentsDetails model = new StudentsDetails();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@FieldWorkID", SqlDbType.Int, 50) { Value = fieldWorkId },
                                          new SqlParameter("@FieldWorkAttachmentID", SqlDbType.Int, 50) { Value = fieldWorkAttachmentId }
                                        };

            DataTable dtEmailData = _helper.GetDataTable("[dbo].[GetStudentInformationByFieldWorkId]", parameters);
            if (dtEmailData.Rows.Count > 0)
            {
                model = dtEmailData.AsEnumerable().Select(row =>
                                        new StudentsDetails
                                        {
                                            UserId = Convert.ToInt32(row["UserId"]),
                                            CSULBID = Convert.ToString(row["CSULBID"]),
                                            Email = Convert.ToString(row["Email"]),
                                            FirstName = Convert.ToString(row["FirstName"]),
                                            LastName = Convert.ToString(row["LastName"]),
                                            DisplayName = Convert.ToString(row["DisplayName"]),
                                            FileName = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"])
                                        }).FirstOrDefault();
            }
            return model;
        }

        private EmailMessageModel GetMessageBody(int fieldWorkID,int fieldWorkattachmentID,string semester)
        {
            string body = string.Empty;
            string ApplicantName = string.Empty;
            string CSULBID = string.Empty;
            string Semester = string.Empty;
            EmailMessageModel model = new EmailMessageModel();

            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@FieldWorkID", SqlDbType.Int, 50) { Value = fieldWorkID },
                                          new SqlParameter("@FieldWorkAttachmentID", SqlDbType.Int, 50) { Value = fieldWorkattachmentID }
                                        };

            DataTable dtEmailData = _helper.GetDataTable("[dbo].[GetPrerequisitesApprovalEmailConfirmation]", parameters);
            if (dtEmailData.Rows.Count > 0)
            {

                using (StreamReader reader = new StreamReader(Path.GetFullPath("SupportFiles/EmailTemplates/PrerequisiteApprovalMail.html")))
                {
                    body = reader.ReadToEnd();
                }
                string logopath = Path.GetFullPath("SupportFiles/Img/logo.png");
                ApplicantName = Convert.ToString(dtEmailData.Rows[0]["ApplicantName"]);
                CSULBID = Convert.ToString(dtEmailData.Rows[0]["CSULBID"]);
                Semester = Convert.ToString(dtEmailData.Rows[0]["Name"]);
                if(fieldWorkID == 0)
                {
                    body = body.Replace("[[ApplicantName]]", ApplicantName).Replace("[[CSULBID]]", CSULBID).Replace("[[Semester]]", semester).Replace("[[Date]]", DateTime.Now.ToString("MMM-dd-yyyy")).Replace("[[logopath]]", logopath);
                }
                else
                {
                    body = body.Replace("[[ApplicantName]]", ApplicantName).Replace("[[CSULBID]]", CSULBID).Replace("[[Semester]]", Semester).Replace("[[Date]]", DateTime.Now.ToString("MMM-dd-yyyy")).Replace("[[logopath]]", logopath);
                }
                model.toEmail = Convert.ToString(dtEmailData.Rows[0]["Email"]);
                model.CSULBID = CSULBID;
                model.ApplicantName = ApplicantName;
                model.Semester = Semester;
                model.Body = body;
            }
            return model;
        }

        private AttachmentFileDetails GetAttachedFileSplitValues(string filename)
        {
            AttachmentFileDetails fileObject = new AttachmentFileDetails();
            string uploadedFileName = filename;
            string[] splitter = uploadedFileName.Split('.');
            StringBuilder fileNameAppender = new StringBuilder();
            string fileExtension = uploadedFileName.Split('.').Last();
            int length = splitter.Length;
            for (int i = 0; i < splitter.Length; i++)
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
                    if (i != length - 1)
                        fileNameAppender.Append(splitter[i] + ".");
                }
            }
            fileObject.FileName = fileNameAppender.ToString();
            fileObject.FileExtension = fileExtension;
            return fileObject;
        }
        public BaseResponse UpdateFieldWorkDocumentValidation(FieldWorkUploadDocumentsRequest input)
        {
            BaseResponse response = new BaseResponse();
            string fileName = string.Empty;
            string fileExtension = string.Empty;
            string userFolderName = string.Empty;
            string savedFileName = string.Empty;
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            FormAttachmentFileNames fileNames = GetFormAttachmentFileName(input.FieldWorkAttachmentId, input.DocumentID);
            if (input.FileName != string.Empty)
            {
                //string[] fileSplit = input.FileName.Split('.');
                //fileName = fileSplit[0].ToString();
                //fileExtension = fileSplit[1].ToString();
                //savedFileName = input.FieldWorkAttachmentId + fileSplit[0].ToString() + DateTime.Now.ToString("MMddyyyyHHmmss");
                AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.FileName);
                // fileName = fileDetails.FileName;
                fileExtension = fileDetails.FileExtension;
                // savedFileName = input.FieldWorkAttachmentId+fileDetails.FileName.ToString()+DateTime.Now.ToString("MMddyyyyHHmmss");
            }
            // call the current file name from DB  and delete the file from the file system and then run the below 
            // first time upload , the filename will be null 
            // call the SP to save the save the file details in fieldwork.attachments table 
            SqlParameter[] parameters =
                                      {
                                          new SqlParameter("@UserId", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FieldWorkAttachmentID", SqlDbType.BigInt) { Value = input.FieldWorkAttachmentId },
                                          new SqlParameter("@FileName", SqlDbType.VarChar, 250) { Value = fileNames.FileName },
                                          new SqlParameter("@FileExtn", SqlDbType.VarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = fileNames.SavedFileName },
                                          new SqlParameter("@ValidTill", SqlDbType.DateTime) { Value = input.ValidTill },
                                          new SqlParameter("@Comments", SqlDbType.VarChar, -1) { Value = input.Comments },
                                          new SqlParameter("@UploadedDate", SqlDbType.DateTime) { Value = input.UploadedDate}
                                        };
            DataTable dtFWDoc = _helper.GetDataTable("[dbo].[UpdateFieldWorkRequiredDocuments]", parameters);
            if (dtFWDoc.Rows.Count > 0 && input.FileName != string.Empty)
            {
                // check if the userFolder exists and if it exists then check if if the FieldWork Folder exists
                string[] folderSplit = dtFWDoc.Rows[0]["FolderName"].ToString().Split('~');
                userFolderName = folderSplit[0].ToString();
                string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
                if (Directory.Exists(dirUserFolderPath))
                {
                    string dirFieldWork = Path.Combine(dirUserFolderPath, "Fieldwork");
                    if (Directory.Exists(dirFieldWork))
                    {
                        // copy the file here 
                        File.WriteAllBytes(Path.Combine(dirFieldWork, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                    }
                    else
                    {
                        Directory.CreateDirectory(dirFieldWork);
                        File.WriteAllBytes(Path.Combine(dirFieldWork, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                    }
                }
                else
                {
                    string dirFieldWork = Path.Combine(dirUserFolderPath, "Fieldwork");
                    DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                    DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirFieldWork);
                    DirectorySecurity dSecurity = dirFieldWorkFolder.GetAccessControl();
                    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    dirFieldWorkFolder.SetAccessControl(dSecurity);

                    File.WriteAllBytes(Path.Combine(dirFieldWork, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                }
                // now delete the old file based on the file name return from DB call above 
            }

            response.IsSuccess = true;
            response.Message = "Field Work Document Uploaded Successfully";
            return response;
        }
        private FormAttachmentFileNames GetFormAttachmentFileName(int formID, int documentID)
        {
            FormAttachmentFileNames fileNames = new FormAttachmentFileNames();
            string savedFileName = string.Empty;
            SqlParameter[] parameters =
                                    {

                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID },
                                          new SqlParameter("@DocumentID", SqlDbType.BigInt) { Value = documentID }
                                    };
            DataTable dtName = _helper.GetDataTable("[dbo].[GetFormAttachmentFileName]", parameters);

            if (dtName.Rows.Count > 0)
            {
                fileNames.SavedFileName = Convert.ToString(dtName.Rows[0]["SavedFileName"]);
                fileNames.FileName = Convert.ToString(dtName.Rows[0]["FileName"]);
                fileNames.UserFolder = Convert.ToString(dtName.Rows[0]["UserFolder"]);
            }

            return fileNames;
        }

        public FieldWorkProfileAttachments DownloadRequiredDocuments(int userId, int fieldworkAttachmentId)
        {
            FieldWorkProfileAttachments obj = new FieldWorkProfileAttachments();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userId },
                                          new SqlParameter("@FieldWorkAttachmentID", SqlDbType.BigInt) { Value = fieldworkAttachmentId }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[dbo].[DownloadFieldWorkRequiredDocument]", parameters);

            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new FieldWorkProfileAttachments
                                          {
                                              FieldWorkAttachmentID = Convert.ToInt32(row["ID"]),
                                              UserID = Convert.ToInt32(row["UserID"]),
                                              FileName = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                              IsApproved = Convert.ToBoolean(row["IsApproved"] == DBNull.Value ? null : row["IsApproved"]),
                                              ApprovedBy = Convert.ToString(row["ApprovedBy"] == DBNull.Value ? null : row["ApprovedBy"]),
                                              ValidatedDate = Convert.ToDateTime(row["ValidatedDate"] == DBNull.Value ? null : row["ValidatedDate"]),
                                              ValidTill = Convert.ToDateTime(row["ValidTill"] == DBNull.Value ? null : row["ValidTill"]),
                                              //FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(Path.Combine(row["FolderName"].ToString(), "FieldWork"), Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]))
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(Path.Combine(GetAttachmentsFolderName(row["FolderName"].ToString()), "FieldWork"), GetAttachmentsSavedFileName(row["FolderName"].ToString()) + "." + Convert.ToString(row["FileExtn"]))
                                          }).FirstOrDefault();

            return obj;
        }

        public FieldWorkActivityLogListResponse GetFieldWorkActivityLog(int userId, int fieldworkId)
        {
            FieldWorkActivityLogListResponse obj = new FieldWorkActivityLogListResponse();
            DownloadDetail objDD = new DownloadDetail();
            List<FieldWorkActivityLogResponse> objList = new List<FieldWorkActivityLogResponse>();

            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userId },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = fieldworkId }
                                     };
            //DataTable dtActivityLog = _helper.GetDataTable("[dbo].[GetFieldWorkActivityLog]", parameters);dbo.[GetFieldWorkActivityLogForFieldWorkUser]
            DataSet dtActivityLog = _helper.GetDataSet("[dbo].[GetFieldWorkActivityLogList]", parameters);
            if (dtActivityLog.Tables.Count > 0)
            {
                objList = dtActivityLog.Tables[0].AsEnumerable().Select(row =>
                                              new FieldWorkActivityLogResponse
                                              {
                                                  ActivityLogID = Convert.ToInt32(row["ID"]),
                                                  DisplayID = Convert.ToString(row["DisplayID"]),
                                                  FieldWorkID = Convert.ToInt32(row["FieldWorkID"]),
                                                  CommunitySiteID = Convert.ToInt32(row["CommunitySiteID"]),
                                                  SiteName = Convert.ToString(row["Site"]),
                                                  ActivityStartDate = Convert.ToDateTime(row["ActivityStartDate"]),
                                                  ActivityEndDate = Convert.ToDateTime(row["ActivityEndDate"]),
                                                  Hours = Convert.ToDecimal(row["Hours"]),
                                                  status = Convert.ToString(row["Status"]),
                                                  ShowCheckbox = Convert.ToBoolean(row["ShowCheckbox"])
                                              }).ToList();

                obj.activityLogHandler = dtActivityLog.Tables[1].AsEnumerable().Select(row =>
                                       new FieldWorkActivityLogHandler
                                       {
                                           ActivityLogHandler = Convert.ToString(row["AcitivityLogHandler"])
                                       }).FirstOrDefault();

                Summary objSummary = dtActivityLog.Tables[2].AsEnumerable().Select(row =>
                                                    new Summary
                                                    {
                                                        ExpectedHours = Convert.ToDecimal(row["ExpectedHours"]),
                                                        LoggedHours = Convert.ToDecimal(row["LoggedHours"]),
                                                        SentForApproval = Convert.ToDecimal(row["SentforApproval"]),
                                                        ApprovedHours = Convert.ToDecimal(row["ApprovedHours"])
                                                    }).FirstOrDefault();
                objDD.summary = objSummary;

                FieldWorkInformation objFI = dtActivityLog.Tables[3].AsEnumerable().Select(row =>
                                                new FieldWorkInformation
                                                {
                                                    StudentName = Convert.ToString(row["StudentName"]),
                                                    CourseName = Convert.ToString(row["CourseTitle"]),
                                                    Semester = Convert.ToString(row["Term"]),
                                                    Instructor = Convert.ToString(row["SupervisorName"]),
                                                    StudentID = Convert.ToString(row["StudentID"]),
                                                    CourseNumber = Convert.ToString(row["Course"]),
                                                    Section = Convert.ToInt32(row["Section"])
                                                }).FirstOrDefault();
                objDD.fieldWorkInformation = objFI;
                obj.downloadDetails = objDD;

                obj.fieldWorkList = objList;
                
                obj.IsSuccess = true;
                obj.Message = "Data Retrieved Successfully";
            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data";
            }

            return obj;
        }
        private DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public FieldWorkActivityLogByIDResponse UpdateFieldWorkActivityLog(FieldWorkActivityLogRequest input)
        {
            DataTable standardsTable = ToDataTable(input.standards);

            //BaseResponse obj = new BaseResponse();

            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FieldWorkActivityLogID", SqlDbType.BigInt) { Value = input.ActivityLogID },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = input.FieldWorkID },
                                          new SqlParameter("@CommunityDistrictID", SqlDbType.BigInt) { Value = input.CommunityDistrictID },
                                          new SqlParameter("@CommunitySchoolID", SqlDbType.BigInt) { Value = input.CommunitySchoolID },
                                          new SqlParameter("@CommunitySiteUsersID", SqlDbType.BigInt) { Value = input.CommunitySiteUserID },
                                          new SqlParameter("@CommunitySiteUserName", SqlDbType.NVarChar,200) { Value = string.IsNullOrEmpty(input.CommunitySiteUserName) ? string.Empty:input.CommunitySiteUserName },
                                          new SqlParameter("@CommunitySiteUserEmail", SqlDbType.NVarChar,200) { Value = string.IsNullOrEmpty(input.CommunitySiteUserEmail) ?string.Empty: input.CommunitySiteUserEmail},
                                          new SqlParameter("@ActivityStartDate", SqlDbType.DateTime) { Value = input.ActivityStartDate },
                                          new SqlParameter("@ActivityEndDate", SqlDbType.DateTime) { Value = input.ActivityEndDate },
                                          new SqlParameter("@Hours", SqlDbType.Decimal) { Value = input.Hours },
                                          new SqlParameter("@Status", SqlDbType.VarChar) { Value = input.status },
                                          new SqlParameter("@LogStandards", SqlDbType.Structured) { Value = standardsTable }
                                     };
            DataTable dtActivityLog = _helper.GetDataTable("[dbo].[SaveFieldWorkActivityLog]", parameters);

            int activityLogID = Convert.ToInt32(dtActivityLog.Rows[0]["FieldWorkActivityLogID"]);

            FieldWorkActivityLogByIDResponse obj = new FieldWorkActivityLogByIDResponse();

            obj = GetFielWorkActivityLogByID(input.UserID, activityLogID);
            if (dtActivityLog.Rows.Count > 0)
            {
                obj.ValidateCommunitySiteSupervisor = dtActivityLog.AsEnumerable().Select(row =>
                                                  new ValidateCommunitySiteSupervisorFieldWork
                                                  {
                                                      Status = Convert.ToInt32(row["Status"]),
                                                      Message = Convert.ToString(row["Message"])
                                                  }).FirstOrDefault();
            }


            //int ID = _helper.InsertTable("[dbo].[SaveFieldWorkActivityLog]", parameters);
            #region Old Codes 
            //if (dtActivityLog.Rows.Count > 0)
            //{
            //    obj = dtActivityLog.AsEnumerable().Select(row =>
            //                                  new FieldWorkActivityLogResponse
            //                                  {
            //                                      //FieldWorkId = Convert.ToInt32(row["FieldWorkID"]),
            //                                      //BaseSchema = Convert.ToString(row["BaseSchema"]),
            //                                      //ResponseSchema = Convert.ToString(row["ResponseSchema"])
            //                                  }).FirstOrDefault();
            //    obj.IsSuccess = true;
            //    obj.Message = "Data Saved Successfully";
            //}
            #endregion

            obj.IsSuccess = true;
            obj.Message = "Data Saved Successfully";
            return obj;
        }
        public FieldWorkActivityLogByIDResponse GetFielWorkActivityLogByID(int userID, int activityLogID)
        {
            FieldWorkActivityLogByIDResponse obj = new FieldWorkActivityLogByIDResponse();

            SqlParameter[] parameters =
                              {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userID },
                                         // new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = fieldWorkID },
                                          new SqlParameter("@ActivityLogID", SqlDbType.BigInt) { Value = activityLogID }
                                          //new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = startDate}

                                     };

            DataSet dtActivityLogByID = _helper.GetDataSet("[dbo].[GetFieldWorkActivityLogByID]", parameters);
            if (dtActivityLogByID.Tables.Count > 0)
            {
                obj.DataByID = dtActivityLogByID.Tables[0].AsEnumerable().Select(row =>
                                          new FieldWorkActivityLogByID
                                          {
                                              ActivityLogID = Convert.ToInt32(row["ID"]),
                                              FieldWorkID = Convert.ToInt32(row["FieldWorkID"]),
                                              CommunityDistrictID = Convert.ToInt32(row["CommunityDistrictID"]),
                                              CommunityDistrictName = Convert.ToString(row["CommunityDistrict"]),
                                              CommunitySchoolID = Convert.ToInt32(row["CommunitySchoolID"]),
                                              CommunitySchoolName = Convert.ToString(row["CommunitySchool"]),
                                              CommunitySiteUserID = Convert.ToInt32(row["CommunitySiteUsersID"]),
                                              CommunitySiteUserName = Convert.ToString(row["CommunitySiteUser"]),
                                              CommunitySiteUserEmail = Convert.ToString(row["CommunitySiteUserEmail"]),
                                              ActivityStartDate = Convert.ToDateTime(row["ActivityStartDate"]),
                                              ActivityEndDate = Convert.ToDateTime(row["ActivityEndDate"]),
                                              Hours = Convert.ToDecimal(row["Hours"]),
                                              Status = Convert.ToString(row["Status"]),
                                              ApprovedByUser = row["ApprovedByUser"] == DBNull.Value ? null : Convert.ToString(row["ApprovedByUser"]),
                                              ApprovedDateTime = row["ApprovedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ApprovedDateTime"])
                                          }).FirstOrDefault();

                obj.standardList = dtActivityLogByID.Tables[1].AsEnumerable().Select(row =>
                                        new FieldWorkActivityLogStandardList
                                        {
                                            FieldWorkActivityLogID = Convert.ToInt32(row["FieldWorkActivityLogID"]),
                                            FieldWorkCoursesCategoryStandardID = Convert.ToInt32(row["FieldWorkCoursesCategoryStandardID"]),
                                            FieldWorkCoursesCategoryStandard = Convert.ToString(row["FieldWorkCoursesCategoryStandard"]),
                                            FieldWorkCoursesCategorySchoolTypeID = Convert.ToInt32(row["FieldWorkCoursesCategorySchoolTypeID"]),
                                            FieldWorkCoursesCategorySchoolType = Convert.ToString(row["FieldWorkCoursesCategorySchoolType"]),
                                            Hours = Convert.ToDecimal(row["Hours"]),
                                            Details = Convert.ToString(row["Details"])
                                        }).ToList();

                obj.ActivityLogHandler = dtActivityLogByID.Tables[2].AsEnumerable().Select(row =>
                                          new FieldWorkActivityLogHandler
                                          {
                                              ActivityLogHandler = Convert.ToString(row["AcitivityLogHandler"])
                                          }).FirstOrDefault();

                obj.IsSuccess = true;
                obj.Message = "Data Retrieved Successfully.";

            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data .";
            }


            return obj;
        }

        public FieldWorkActivityLogByIDResponse GetFieldWorkActivityLogforAdd(int userID, int fieldWorkID)
        {
            FieldWorkActivityLogByIDResponse obj = new FieldWorkActivityLogByIDResponse();

            SqlParameter[] parameters =
                              {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userID },
                                         new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = fieldWorkID }

                                     };

            DataSet dtActivityLogByID = _helper.GetDataSet("[dbo].[GetFieldWorkActivityLogforAdd]", parameters);
            if (dtActivityLogByID.Tables.Count > 0)
            {
                obj.DataByID = dtActivityLogByID.Tables[0].AsEnumerable().Select(row =>
                                          new FieldWorkActivityLogByID
                                          {
                                              ActivityLogID = Convert.ToInt32(row["ID"]),
                                              FieldWorkID = Convert.ToInt32(row["FieldWorkID"]),
                                              CommunityDistrictID = Convert.ToInt32(row["CommunityDistrictID"]),
                                              CommunityDistrictName = Convert.ToString(row["CommunityDistrict"]),
                                              CommunitySchoolID = Convert.ToInt32(row["CommunitySchoolID"]),
                                              CommunitySchoolName = Convert.ToString(row["CommunitySchool"]),
                                              CommunitySiteUserID = Convert.ToInt32(row["CommunitySiteUsersID"]),
                                              CommunitySiteUserName = Convert.ToString(row["CommunitySiteUser"]),
                                              ActivityStartDate = Convert.ToDateTime(row["ActivityStartDate"]),
                                              ActivityEndDate = Convert.ToDateTime(row["ActivityEndDate"]),
                                              Hours = Convert.ToDecimal(row["Hours"]),
                                              Status = Convert.ToString(row["Status"])
                                          }).FirstOrDefault();

                obj.standardList = dtActivityLogByID.Tables[1].AsEnumerable().Select(row =>
                                        new FieldWorkActivityLogStandardList
                                        {
                                            FieldWorkActivityLogID = Convert.ToInt32(row["FieldWorkActivityLogID"]),
                                            FieldWorkCoursesCategoryStandardID = Convert.ToInt32(row["FieldWorkCoursesCategoryStandardID"]),
                                            FieldWorkCoursesCategoryStandard = Convert.ToString(row["FieldWorkCoursesCategoryStandard"]),
                                            FieldWorkCoursesCategorySchoolTypeID = Convert.ToInt32(row["FieldWorkCoursesCategorySchoolTypeID"]),
                                            FieldWorkCoursesCategorySchoolType = Convert.ToString(row["FieldWorkCoursesCategorySchoolType"]),
                                            Hours = Convert.ToDecimal(row["Hours"]),
                                            Details = Convert.ToString(row["Details"])
                                        }).ToList();

                obj.ActivityLogHandler = dtActivityLogByID.Tables[2].AsEnumerable().Select(row =>
                                          new FieldWorkActivityLogHandler
                                          {
                                              ActivityLogHandler = Convert.ToString(row["AcitivityLogHandler"])
                                          }).FirstOrDefault();

                obj.IsSuccess = true;
                obj.Message = "Data Retrieved Successfully.";

            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data .";
            }


            return obj;
        }

        public FieldWorkAttachmentsResponse UploadFieldWorkActivityDocuments(FieldWorkAttachmentsRequest input)
        {
            FieldWorkAttachmentsResponse response = new FieldWorkAttachmentsResponse();
            string fileName = string.Empty;
            string fileExtension = string.Empty;
            string savedFileName = string.Empty;
            string userFolderName = string.Empty;
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            if (input.FileName != string.Empty)
            {
                string[] fileSplit = input.FileName.Split('.');
                fileName = fileSplit[0].ToString();
                savedFileName = input.UserID + fileSplit[0].ToString() + DateTime.Now.ToString("MMddyyyyHHmmss");
                fileExtension = fileSplit[1].ToString();
            }
            // call the SP to save the save the file details in fieldwork.attachments table 
            //SqlParameter[] parameters =
            //                          {
            //                              new SqlParameter("@UserId", SqlDbType.BigInt) { Value = input.UserID },
            //                              new SqlParameter("@FileName", SqlDbType.VarChar, 250) { Value = fileName },
            //                              new SqlParameter("@FileExtn", SqlDbType.VarChar, 20) { Value = fileExtension },
            //                              new SqlParameter("@SavedFileName", SqlDbType.VarChar, 250) { Value = savedFileName },
            //                            };
            //DataTable dtFWDoc = _helper.GetDataTable("[dbo].[UploadFieldWorkActivityLogAttachments]", parameters);
            //if (dtFWDoc.Rows.Count > 0)
            //{
            // check if the userFolder exists and if it exists then check if if the FieldWork Folder exists
            //string[] folderSplit = dtFWDoc.Rows[0]["FolderName"].ToString().Split('~');
            // userFolderName = folderSplit[0].ToString(); -- userfoldername should be a combination of Userid/"FieldWork"/fieldworkId
            userFolderName = input.UserID.ToString();
            string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
            if (Directory.Exists(dirUserFolderPath))
            {
                string dirFieldWork = Path.Combine(dirUserFolderPath, "Fieldwork");
                string dirFieldWorkId = Path.Combine(dirFieldWork, input.FieldworkID.ToString());
                if (Directory.Exists(dirFieldWork))
                {
                    if (Directory.Exists(dirFieldWorkId))
                    {
                        File.WriteAllBytes(Path.Combine(dirFieldWorkId, savedFileName + "." + fileExtension), input.FileContent);
                    }
                    else
                    {
                        Directory.CreateDirectory(dirFieldWorkId);
                        File.WriteAllBytes(Path.Combine(dirFieldWorkId, savedFileName + "." + fileExtension), input.FileContent);
                    }
                }
                else
                {
                    Directory.CreateDirectory(dirFieldWork);
                    File.WriteAllBytes(Path.Combine(dirFieldWork, savedFileName + "." + fileExtension), input.FileContent);
                }
            }
            else
            {
                string dirFieldWork = Path.Combine(dirUserFolderPath, "Fieldwork");
                string dirFieldWorkId = Path.Combine(dirFieldWork, input.FieldworkID.ToString());
                DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirFieldWork);
                DirectoryInfo dirFieldWorkIdFolder = System.IO.Directory.CreateDirectory(dirFieldWorkId);
                DirectorySecurity dSecurity = dirFieldWorkIdFolder.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dirFieldWorkIdFolder.SetAccessControl(dSecurity);

                File.WriteAllBytes(Path.Combine(dirFieldWorkId, savedFileName + "." + fileExtension), input.FileContent);
            }
            Guid guidObj = Guid.NewGuid();
            response.AttachmentID = guidObj.ToString();
            response.FileDisplayName = fileName + "." + fileExtension;
            response.FileSavedName = savedFileName + "." + fileExtension;
            response.IsSuccess = true;
            response.Message = "Activity Log Attachment Uploaded Succesfully ";
            // response.AttachmentID = Convert.ToInt32(dtFWDoc.Rows[0]["ID"]);
            //}

            //response.IsSuccess = true;
            //response.Message = "Field Work Document Uploaded Successfully";
            return response;
        }



        public FieldWorkProfileAttachments DownloadActivityAttachments(int userId, int fieldworkId, string savedFileName)
        {
            FieldWorkProfileAttachments obj = new FieldWorkProfileAttachments();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userId },
                                          new SqlParameter("@FieldWorkAttachmentID", SqlDbType.BigInt) { Value = fieldworkId }
                                     };
            // DataTable dtAttachments = _helper.GetDataTable("[dbo].[DownloadFieldWorkRequiredDocument]", parameters);
            DataTable dtAttachments = null;

            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new FieldWorkProfileAttachments
                                          {
                                              FieldWorkAttachmentID = Convert.ToInt32(row["ID"]),
                                              UserID = Convert.ToInt32(row["UserID"]),
                                              FileName = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                              IsApproved = Convert.ToBoolean(row["IsApproved"] == DBNull.Value ? null : row["IsApproved"]),
                                              ApprovedBy = Convert.ToString(row["ApprovedBy"] == DBNull.Value ? null : row["ApprovedBy"]),
                                              ValidatedDate = Convert.ToDateTime(row["ValidatedDate"] == DBNull.Value ? null : row["ValidatedDate"]),
                                              ValidTill = Convert.ToDateTime(row["ValidTill"] == DBNull.Value ? null : row["ValidTill"]),
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(Path.Combine(GetAttachmentsFolderName(row["FolderName"].ToString()), "FieldWork"), GetAttachmentsSavedFileName(row["FolderName"].ToString()) + "." + Convert.ToString(row["FileExtn"]))
                                          }).FirstOrDefault();

            return obj;
        }
        private string GetAttachmentsFolderName(string combinedString)
        {
            string[] folderSplit = combinedString.ToString().Split('~');
            string userFolderName = folderSplit[0].ToString();
            return userFolderName;
        }
        private string GetAttachmentsSavedFileName(string combinedString)
        {
            string[] folderSplit = combinedString.ToString().Split('~');
            string savedFileName = folderSplit[1].ToString();
            return savedFileName;
        }

        public FieldWorkCommunitySitesResponse GetCommunitySites()
        {
            FieldWorkCommunitySitesResponse obj = new FieldWorkCommunitySitesResponse();
            List<FieldWorkCommunitySites> sites = new List<FieldWorkCommunitySites>();

            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@CommunitySiteId", SqlDbType.Int, 50) { Value = 0 }
                                        };

            DataTable dtSitesList = _helper.GetDataTable("[dbo].[GetFieldworkCommunitySitesAndCommunityUsers]", parameters);

            if (dtSitesList.Rows.Count > 0)
            {
                sites = dtSitesList.AsEnumerable().Select(row =>
                                          new FieldWorkCommunitySites
                                          {
                                              CommunitySiteID = Convert.ToInt32(row["ID"]),
                                              CommunitySite = Convert.ToString(row["CommunitySite"])

                                          }).ToList();
                obj.sites = sites;
                obj.IsSuccess = true;
                obj.Message = "Data retrieved successfully";
            }


            return obj;
        }

        public FieldWorkCommunitySiteUsersResponse GetCommunitySiteUsers(int communitySiteId)
        {
            FieldWorkCommunitySiteUsersResponse obj = new FieldWorkCommunitySiteUsersResponse();
            List<FieldWorkCommunitySiteUsers> _users = new List<FieldWorkCommunitySiteUsers>();
            SqlParameter[] parameters =
                                      {
                                          new SqlParameter("@CommunitySiteId", SqlDbType.Int, 50) { Value = communitySiteId }
                                        };

            DataTable dtUsersList = _helper.GetDataTable("[dbo].[GetFieldworkCommunitySitesAndCommunityUsers]", parameters);

            if (dtUsersList.Rows.Count > 0)
            {
                _users = dtUsersList.AsEnumerable().Select(row =>
                                          new FieldWorkCommunitySiteUsers
                                          {
                                              CommunitySiteUserID = Convert.ToInt32(row["ID"]),
                                              CommunitySiteUser = Convert.ToString(row["CommunitySiteUser"])

                                          }).ToList();
                obj.users = _users;
                obj.IsSuccess = true;
                obj.Message = "Data retrieved successfully";

            }
            return obj;
        }
        public FieldWorkStandardsResponse GetStandards(int userID, int fieldWorkID)
        {
            FieldWorkStandardsResponse obj = new FieldWorkStandardsResponse();
            List<FieldWorkStandards> _standards = new List<FieldWorkStandards>();
            SqlParameter[] parameters =
                                      {
                                          new SqlParameter("@UserID", SqlDbType.Int, 50) { Value = userID },
                                          new SqlParameter("@FieldWorkID", SqlDbType.Int, 50) { Value = fieldWorkID }
                                        };

            DataTable dtUsersList = _helper.GetDataTable("[dbo].[GetStandardsList]", parameters);

            if (dtUsersList.Rows.Count > 0)
            {
                _standards = dtUsersList.AsEnumerable().Select(row =>
                                          new FieldWorkStandards
                                          {
                                              StandardID = Convert.ToInt32(row["ID"]),
                                              standard = Convert.ToString(row["Standard"])

                                          }).ToList();
                obj.standards = _standards;
                obj.IsSuccess = true;
                obj.Message = "Data retrieved successfully";

            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data";
            }
            return obj;
        }

        public FieldWorkFnCSchemaResponse GetFnCSchema(int userID, int fieldWorkID, int schemaTypeId, int fieldWorkActivityLogID)
        {
            FieldWorkFnCSchemaResponse obj = new FieldWorkFnCSchemaResponse();
            string schemaType = getSchemaType(schemaTypeId);

            SqlParameter[] parameters =
                                {
                                          new SqlParameter("@fieldWorkID", SqlDbType.Int, 50) { Value = fieldWorkID },
                                          new SqlParameter("@schemaType", SqlDbType.Int, 50) { Value = schemaTypeId },
                                          new SqlParameter("@fieldWorkActivityLogID", SqlDbType.Int, 50) { Value = fieldWorkActivityLogID },
                                        };

            DataTable dtUsersList = _helper.GetDataTable("[dbo].[GetFieldWorkFnCSchema]", parameters);

            if (dtUsersList.Rows.Count > 0)
            {
                obj = dtUsersList.AsEnumerable().Select(row =>
                                          new FieldWorkFnCSchemaResponse
                                          {
                                              schema = Convert.ToString(row["schema"])

                                          }).FirstOrDefault();
                obj.IsSuccess = true;
                obj.Message = "Data retrieved successfully";

            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data";
            }

            return obj;
        }
        public BaseResponse UpdateFnCSchema(FieldWorkFnCSchemaUpdateRequest input)
        {
            BaseResponse obj = new BaseResponse();
            string schemaType = getSchemaType(input.schemaTypeID);

            SqlParameter[] parameters =
                                {
                                          new SqlParameter("@fieldWorkID", SqlDbType.BigInt, 50) { Value = input.fieldWorkID },
                                          new SqlParameter("@schemaType", SqlDbType.Int, 50) { Value = input.schemaTypeID },
                                          new SqlParameter("@schema", SqlDbType.NVarChar, 4000) { Value = input.schema },
                                          new SqlParameter("@FieldWorkActivityLogID", SqlDbType.BigInt, 50) { Value = input.FieldWorkActivityLogID }
                                        };

            int retVal = _helper.InsertTable("[dbo].[UpdateFnCSchema]", parameters);
            obj.IsSuccess = true;
            obj.Message = "Data Saved Successfully";

            return obj;
        }
        public BaseResponse UpdateFieldworkCommunityUsersforCreation()
        {
            BaseResponse obj = new BaseResponse();
            SqlParameter[] parameters =
                                { };

            DataTable dtNewPartners = _helper.GetDataTable("[dbo].[GetFieldworkCommunityUsersforCreation]", parameters);
            if (dtNewPartners.Rows.Count > 0)
            {
                for (int i = 0; i < dtNewPartners.Rows.Count; i++)
                {
                    string partnerName = string.Empty;
                    string userName = string.Empty;
                    string userPassword = string.Empty;
                    string partnerUserEmail = string.Empty;
                    int userID;
                    // get the autogenerated password here 
                    var pwd = new Password(includeLowercase: true, includeUppercase: true, includeNumeric: true, includeSpecial: true, passwordLength: 12);
                    userPassword = pwd.Next();
                    partnerName = Convert.ToString(dtNewPartners.Rows[i]["FirstName"]) + " " + Convert.ToString(dtNewPartners.Rows[i]["LastName"]);
                    partnerUserEmail = Convert.ToString(dtNewPartners.Rows[i]["Email"]);
                    userID = Convert.ToInt32(dtNewPartners.Rows[i]["ID"]);
                    // update user crentials 
                    bool isPartnerUserAdded = AddPartnerUserCredentials(userID, partnerUserEmail, userPassword);
                    // fire an email to community partneruser
                    if (isPartnerUserAdded)
                    {
                        string logoText = "cid:myImageID";
                        string body = GetMailBodyTemplate("PartnerUser_Cred.html");
                        string subject = "Credentials for MyCED Application";
                        body = body.Replace("[[logoPath]]", logoText)
                                   .Replace("[[PartnerUserName]]", partnerName)
                                   .Replace("[[UserName]]", partnerUserEmail)
                                   .Replace("[[Password]]", userPassword);
                        _sendMail.SendEmail(partnerUserEmail, "", "COMMON", subject, body, "");
                    }
                }
                obj.IsSuccess = true;
                obj.Message = "Community Partner User's added successfully";
            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No New Community Partner User's to be assigned";
            }

            return obj;
        }

        public PUNS_GetCommunitySiteSupervisorDemonstrationTeacherListResponse PUNS_GetCommunitySiteSupervisorDemonstrationTeacherList()
        {
            PUNS_GetCommunitySiteSupervisorDemonstrationTeacherListResponse obj = new PUNS_GetCommunitySiteSupervisorDemonstrationTeacherListResponse();

            SqlParameter[] parameters =
                                {
                                          };

            DataTable dtUsersList = _helper.GetDataTable("[FieldWork].[PUNS_GetCommunitySiteSupervisorDemonstrationTeacherList]", parameters);

            if (dtUsersList.Rows.Count > 0)
            {
                obj.listPartnerUser = dtUsersList.AsEnumerable().Select(row =>
                                          new PUNS_GetCommunitySiteSupervisorDemonstrationTeacherList
                                          {
                                              CSSDTID = Convert.ToInt32(row["CSSDTID"]),
                                              CommunitySiteUserName = Convert.ToString(row["CommunitySiteUserName"]),
                                              CommunitySiteUserEmail = Convert.ToString(row["CommunitySiteUserEmail"]),
                                              //EmailSentOn = Convert.ToDateTime(row["EmailSentOn"]),
                                              // = Convert.ToDateTime(row["EmailSentOn"] == DBNull.Value ? DateTime.MinValue : row["EmailSentOn"]),
                                              EmailSentOn = row["EmailSentOn"] == DBNull.Value ? "" : Convert.ToDateTime(row["EmailSentOn"]).ToString("MM/dd/yyyy hh:mm")

                                          }).ToList();
                obj.IsSuccess = true;
                obj.Message = "Data retrieved successfully";

            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data";
            }

            return obj;
        }

        private bool AddPartnerUserCredentials(int userID, string userName, string password)
        {
            bool isAdded = false;
            SqlParameter[] parameters =
                              {
                                          new SqlParameter("@UserID", SqlDbType.BigInt, 50) { Value = userID },
                                          new SqlParameter("@UserName", SqlDbType.NVarChar, 50) { Value = userName },
                                          new SqlParameter("@Password", SqlDbType.NVarChar, 50) { Value = password }
                                        };

            int retVal = _helper.InsertTable("[dbo].[UpdateFieldworkCommunityUsersforCreation]", parameters);
            isAdded = true;
            return isAdded;
        }

        private string getSchemaType(int schemaTypeId)
        {
            string schemaType = string.Empty;
            foreach (FnCSchema schema in Enum.GetValues(typeof(FnCSchema)))
            {
                if (schemaTypeId == (int)schema)
                {
                    schemaType = schema.ToString();
                    break;
                }
            }
            return schemaType;
        }

        public FieldWorkCommunityDistrictResponse GetFieldworkCommunityDistrict()
        {
            FieldWorkCommunityDistrictResponse obj = new FieldWorkCommunityDistrictResponse();
            List<FieldWorkCommunityDistrict> districts = new List<FieldWorkCommunityDistrict>();

            SqlParameter[] parameters =
                                        {
                                        };

            DataTable dtDistricts = _helper.GetDataTable("[dbo].[GetFieldworkCommunityDistrict]", parameters);

            if (dtDistricts.Rows.Count > 0)
            {
                districts = dtDistricts.AsEnumerable().Select(row =>
                                          new FieldWorkCommunityDistrict
                                          {
                                              CommunityDistrictID = Convert.ToInt32(row["CommunityDistrictID"]),
                                              CommunityDistrictName = Convert.ToString(row["CommunityDistrictName"])

                                          }).ToList();
                obj.districts = districts;
                obj.IsSuccess = true;
                obj.Message = "Data retrieved successfully";
            }


            return obj;
        }
        public GetFieldWorkCoursesCategorySchoolTypesResponse GetFieldWorkCoursesCategorySchoolTypes(int userID, int fieldWorkID)
        {
            GetFieldWorkCoursesCategorySchoolTypesResponse obj = new GetFieldWorkCoursesCategorySchoolTypesResponse();
            List<GetFieldWorkCoursesCategorySchoolTypes> schoolTypes = new List<GetFieldWorkCoursesCategorySchoolTypes>();

            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserID", SqlDbType.BigInt, 50) { Value = userID },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt, 50) { Value = fieldWorkID }
                                        };

            DataTable dtSchoolTypes = _helper.GetDataTable("[dbo].[SchoolTypesList]", parameters);

            if (dtSchoolTypes.Rows.Count > 0)
            {
                schoolTypes = dtSchoolTypes.AsEnumerable().Select(row =>
                                          new GetFieldWorkCoursesCategorySchoolTypes
                                          {
                                              SchoolTypeID = Convert.ToInt32(row["ID"]),
                                              SchoolType = Convert.ToString(row["SchoolType"])

                                          }).ToList();
                obj.schoolTypes = schoolTypes;
                obj.IsSuccess = true;
                obj.Message = "Data retrieved successfully";
            }


            return obj;
        }

        public GetFieldworkCommunitySchoolSiteUsersByDistrictResponse GetFieldworkCommunitySchoolSiteUsersByDistrict(int CommunityDistrictID)
        {
            GetFieldworkCommunitySchoolSiteUsersByDistrictResponse obj = new GetFieldworkCommunitySchoolSiteUsersByDistrictResponse();

            SqlParameter[] parameters =
                              {
                                          new SqlParameter("@CommunityDistrictID", SqlDbType.BigInt) { Value = CommunityDistrictID }

                                     };

            DataSet dtFieldWork = _helper.GetDataSet("[dbo].[GetFieldworkCommunitySchoolSiteUsersByDistrict]", parameters);
            if (dtFieldWork.Tables.Count > 0)
            {
                obj.schools = dtFieldWork.Tables[0].AsEnumerable().Select(row =>
                                          new GetFieldworkCommunitySchool
                                          {
                                              CommunitySchoolID = Convert.ToInt32(row["CommunitySchoolID"]),
                                              CommunitySchoolName = Convert.ToString(row["CommunitySchoolName"])
                                          }).ToList();

                obj.users = dtFieldWork.Tables[1].AsEnumerable().Select(row =>
                                        new GetFieldworkCommunitySiteUsers
                                        {
                                            CommunitySiteUserID = Convert.ToInt32(row["ID"]),
                                            CommunitySiteUser = Convert.ToString(row["CommunitySiteUser"])
                                        }).ToList();
                obj.IsSuccess = true;
                obj.Message = "Data Retrieved Successfully.";

            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data .";
            }


            return obj;
        }

        public BaseResponse UpdateFieldWorkActivityLogStatus(UpdateFieldWorkActivityLogStatusRequest input)
        {
            BaseResponse obj = new BaseResponse();

            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = input.FieldWorkID },
                                          new SqlParameter("@FieldWorkActivityLogID", SqlDbType.BigInt) { Value = input.FieldWorkActivityLogID },
                                          new SqlParameter("@Status", SqlDbType.VarChar) { Value = input.Status }
                                     };
            int ID = _helper.InsertTable("[dbo].[UpdateFieldWorkActivityLogStatus]", parameters);
            obj.IsSuccess = true;
            obj.Message = "Activity Log Status Updated Successfully";
            return obj;
        }

        public PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail(int CSSDTID)
        {
            PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail obj = new PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail();

            SqlParameter[] parameters =
                              {
                                          new SqlParameter("@CssdtID", SqlDbType.BigInt) { Value = CSSDTID }

                              };

            DataSet dtFieldWork = _helper.GetDataSet("[FieldWork].[PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail]", parameters);
            if (dtFieldWork.Tables.Count > 0)
            {
                obj = dtFieldWork.Tables[0].AsEnumerable().Select(row =>
                                          new PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail
                                          {
                                              CSSDTID = Convert.ToInt32(row["CSSDTID"]),
                                              CommunitySiteUserName = Convert.ToString(row["CommunitySiteUserName"]),
                                              CommunitySiteUserEmail = Convert.ToString(row["CommunitySiteUserEmail"]),
                                              CommunitySiteUserIdentifier = Convert.ToString(row["CommunitySiteUserIdentifier"])

                                          }).FirstOrDefault();

                obj.IsSuccess = true;
                obj.Message = "Data Retrieved Successfully.";
                try
                {
                    //please uncomment after testing
                    //string toUser = "asif.khan@thoughtfocus.com";
                    string toUser = obj.CommunitySiteUserEmail;
                    string link = _configuration["ApplicationKeys:PartnerUserBaseURL"] + obj.CommunitySiteUserIdentifier;
                    string body = GetMailBodyTemplate("PartnerUserMailTemplate.html");
                    string logoText = "cid:myImageID";
                    body = body.Replace("[[logoPath]]", logoText)
                              .Replace("[[link]]", link);
                    string subject = "Approve student hours for CSULB Clinical Practice";
                    _sendMail.SendEmail(toUser, "", "COMMON", subject, body, "");
                    obj.IsSuccess = true;
                    obj.Message = "Partner User Activation mail sent successfully.";
                }
                catch (Exception ee)
                {
                    obj.IsSuccess = false;
                    obj.Message = "Failure sending mail.";
                }
            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data .";
            }


            return obj;
        }

        public PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail PUNS_AutharizeCommunitySiteSupervisorDemonstrationTeacher(string CommunitySiteUserIdentifier, string CommunitySiteUserEmail)
        {
            PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail obj = new PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail();

            SqlParameter[] parameters =
                              {
                                          new SqlParameter("@CommunitySiteUserIdentifier", SqlDbType.NVarChar) { Value = CommunitySiteUserIdentifier },
                                          new SqlParameter("@CommunitySiteUserEmail", SqlDbType.NVarChar) { Value = Convert.ToString(CommunitySiteUserEmail) }

                              };

            DataSet dtFieldWork = _helper.GetDataSet("[FieldWork].[PUNS_AuthorizeCommunitySiteSupervisorDemonstrationTeacher]", parameters);
            if (dtFieldWork.Tables.Count > 0 && dtFieldWork != null)
            {
                obj = dtFieldWork.Tables[0].AsEnumerable().Select(row =>
                                          new PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail
                                          {
                                              CSSDTID = Convert.ToInt32(row["CSSDTID"]),
                                              CommunitySiteUserName = Convert.ToString(row["CommunitySiteUserName"]),
                                              CommunitySiteUserEmail = Convert.ToString(row["CommunitySiteUserEmail"]),
                                              CommunitySiteUserIdentifier = Convert.ToString(row["CommunitySiteUserIdentifier"])

                                          }).FirstOrDefault();

                obj.IsSuccess = true;
                obj.Message = "Data Retrieved Successfully.";

            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data .";
            }


            return obj;
        }

        public FieldWorkListResponse GetFieldWorkData(string CommunitySiteUserIdentifier)
        {
            FieldWorkListResponse objList = new FieldWorkListResponse();
            List<FieldWorkResponse> obj = new List<FieldWorkResponse>();
            SqlParameter[] parameters =
                              {
                                          new SqlParameter("@CommunitySiteUserIdentifier", SqlDbType.NVarChar) { Value = CommunitySiteUserIdentifier }

                              };

            DataTable dtFieldWork = _helper.GetDataTable("[FieldWork].[PUNS_AuthorizeCommunitySiteSupervisorDemonstrationTeacher]", parameters);
            if (dtFieldWork.Rows.Count > 0 && dtFieldWork != null)
            {
                int cssdtid = Convert.ToInt32(dtFieldWork.Rows[0]["CSSDTID"]);
                // pull fielldwork list 
                SqlParameter[] parameters1 =
                                            {
                                          new SqlParameter("@CssdtID", SqlDbType.Int, 50) { Value = cssdtid }
                                        };

                DataTable dtFieldWorkList = _helper.GetDataTable("[FieldWork].[PUNS_GetFieldWorkData]", parameters1);
                try
                {
                    if (dtFieldWorkList.Rows.Count > 0)
                    {
                        obj = dtFieldWorkList.AsEnumerable().Select(row =>
                                                  new FieldWorkResponse
                                                  {
                                                      FieldWorkId = Convert.ToInt32(row["ID"]),
                                                      StudentName = Convert.ToString(row["StudentName"]),
                                                      FirstName = Convert.ToString(row["FirstName"]),
                                                      LastName = Convert.ToString(row["LastName"]),
                                                      CSULBID = Convert.ToString(row["CSULBID"]),
                                                      CourseTitle = Convert.ToString(row["CourseTitle"]),
                                                      CSULBCourseID = Convert.ToString(row["Course"]),
                                                      College = Convert.ToString(row["College"]),
                                                      Section = Convert.ToString(row["Section"]),
                                                      Term = Convert.ToString(row["Term"]),
                                                      FieldWorkPrerequisiteStatus = Convert.ToInt32(row["FieldWorkPrerequisiteStatus"])

                                                  }).ToList();

                        objList.FieldWorkResponse = obj;
                        objList.IsSuccess = true;
                        objList.Message = "Data Retrieved Successfully";

                    }
                }
                catch (Exception ex)
                {
                    objList.IsSuccess = false;
                    objList.Message = "Data Retrievel Failed";
                    objList.StackTrace = ex.Message;
                }

            }
            else
            {
                objList.IsSuccess = false;
                objList.Message = "Invalid Link.";
            }


            return objList;
        }
        public PUFieldWorkActivityLogListResponse GetFieldWorkActivityLogList(string CommunitySiteUserIdentifier, int fieldworkId)
        {

            PUFieldWorkActivityLogListResponse obj = new PUFieldWorkActivityLogListResponse();
            List<FieldWorkActivityLogResponse> objList = new List<FieldWorkActivityLogResponse>();
            SqlParameter[] parameters =
                             {
                                          new SqlParameter("@CommunitySiteUserIdentifier", SqlDbType.NVarChar) { Value = CommunitySiteUserIdentifier }

                              };

            DataTable dtFieldWork = _helper.GetDataTable("[FieldWork].[PUNS_AuthorizeCommunitySiteSupervisorDemonstrationTeacher]", parameters);
            if (dtFieldWork.Rows.Count > 0 && dtFieldWork != null)
            {
                int cssdtid = Convert.ToInt32(dtFieldWork.Rows[0]["CSSDTID"]);

                SqlParameter[] parameters1 =
                                        {
                                          new SqlParameter("@CssdtID", SqlDbType.BigInt) { Value = cssdtid },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = fieldworkId }
                                     };
                DataSet dtActivityLog = _helper.GetDataSet("[FieldWork].[PUNS_GetFieldWorkActivityLogList]", parameters1);
                if (dtActivityLog.Tables.Count > 0)
                {
                    objList = dtActivityLog.Tables[0].AsEnumerable().Select(row =>
                                                  new FieldWorkActivityLogResponse
                                                  {
                                                      ActivityLogID = Convert.ToInt32(row["ID"]),
                                                      DisplayID = Convert.ToString(row["DisplayID"]),
                                                      FieldWorkID = Convert.ToInt32(row["FieldWorkID"]),
                                                      CommunitySiteID = Convert.ToInt32(row["CommunitySiteID"]),
                                                      SiteName = Convert.ToString(row["Site"]),
                                                      ActivityStartDate = Convert.ToDateTime(row["ActivityStartDate"]),
                                                      ActivityEndDate = Convert.ToDateTime(row["ActivityEndDate"]),
                                                      Hours = Convert.ToDecimal(row["Hours"]),
                                                      status = Convert.ToString(row["Status"]),
                                                      ShowCheckbox = Convert.ToBoolean(row["ShowCheckbox"])
                                                  }).ToList();

                    obj.activityLogHandler = dtActivityLog.Tables[1].AsEnumerable().Select(row =>
                                           new FieldWorkActivityLogHandler
                                           {
                                               ActivityLogHandler = Convert.ToString(row["AcitivityLogHandler"])
                                           }).FirstOrDefault();

                    obj.fieldWorkSummary = dtActivityLog.Tables[2].AsEnumerable().Select(row =>
                                           new FieldWorkSummary
                                           {
                                               fieldWorkSummary = Convert.ToString(row["FieldWorkSummary"])
                                           }).FirstOrDefault();

                    obj.fieldWorkList = objList;
                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";
                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data";
                }
            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "Invalid Link";
            }
            return obj;
        }

        public FieldWorkActivityLogByIDResponse PUNS_GetFieldWorkActivityLogByID(string CommunitySiteUserIdentifier, int ActivityLogID)
        {

            FieldWorkActivityLogByIDResponse obj = new FieldWorkActivityLogByIDResponse();
            SqlParameter[] parameters =
                         {
                                          new SqlParameter("@CommunitySiteUserIdentifier", SqlDbType.NVarChar) { Value = CommunitySiteUserIdentifier }

                              };

            DataTable dtFieldWork = _helper.GetDataTable("[FieldWork].[PUNS_AuthorizeCommunitySiteSupervisorDemonstrationTeacher]", parameters);
            if (dtFieldWork.Rows.Count > 0 && dtFieldWork != null)
            {
                int cssdtid = Convert.ToInt32(dtFieldWork.Rows[0]["CSSDTID"]);
                SqlParameter[] parameters1 =
                              {
                                          new SqlParameter("@CssdtID", SqlDbType.BigInt) { Value = cssdtid },
                                          new SqlParameter("@ActivityLogID", SqlDbType.BigInt) { Value = ActivityLogID }

                                     };

                DataSet dtActivityLogByID = _helper.GetDataSet("[FieldWork].[PUNS_GetFieldWorkActivityLogByID]", parameters1);
                if (dtActivityLogByID.Tables.Count > 0)
                {
                    obj.DataByID = dtActivityLogByID.Tables[0].AsEnumerable().Select(row =>
                                              new FieldWorkActivityLogByID
                                              {
                                                  ActivityLogID = Convert.ToInt32(row["ID"]),
                                                  FieldWorkID = Convert.ToInt32(row["FieldWorkID"]),
                                                  CommunityDistrictID = Convert.ToInt32(row["CommunityDistrictID"]),
                                                  CommunityDistrictName = Convert.ToString(row["CommunityDistrict"]),
                                                  CommunitySchoolID = Convert.ToInt32(row["CommunitySchoolID"]),
                                                  CommunitySchoolName = Convert.ToString(row["CommunitySchool"]),
                                                  CommunitySiteUserID = Convert.ToInt32(row["CommunitySiteUsersID"]),
                                                  CommunitySiteUserName = Convert.ToString(row["CommunitySiteUser"]),
                                                  CommunitySiteUserEmail = Convert.ToString(row["CommunitySiteUserEmail"]),
                                                  ActivityStartDate = Convert.ToDateTime(row["ActivityStartDate"]),
                                                  ActivityEndDate = Convert.ToDateTime(row["ActivityEndDate"]),
                                                  Hours = Convert.ToDecimal(row["Hours"]),
                                                  Status = Convert.ToString(row["Status"]),
                                                  ApprovedByUser = row["ApprovedByUser"] == DBNull.Value ? null : Convert.ToString(row["ApprovedByUser"]),
                                                  ApprovedDateTime = row["ApprovedDateTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ApprovedDateTime"])
                                              }).FirstOrDefault();

                    obj.standardList = dtActivityLogByID.Tables[1].AsEnumerable().Select(row =>
                                            new FieldWorkActivityLogStandardList
                                            {
                                                FieldWorkActivityLogID = Convert.ToInt32(row["FieldWorkActivityLogID"]),
                                                FieldWorkCoursesCategoryStandardID = Convert.ToInt32(row["FieldWorkCoursesCategoryStandardID"]),
                                                FieldWorkCoursesCategoryStandard = Convert.ToString(row["FieldWorkCoursesCategoryStandard"]),
                                                FieldWorkCoursesCategorySchoolTypeID = Convert.ToInt32(row["FieldWorkCoursesCategorySchoolTypeID"]),
                                                FieldWorkCoursesCategorySchoolType = Convert.ToString(row["FieldWorkCoursesCategorySchoolType"]),
                                                Hours = Convert.ToDecimal(row["Hours"]),
                                                Details = Convert.ToString(row["Details"])
                                            }).ToList();

                    obj.ActivityLogHandler = dtActivityLogByID.Tables[2].AsEnumerable().Select(row =>
                                              new FieldWorkActivityLogHandler
                                              {
                                                  ActivityLogHandler = Convert.ToString(row["AcitivityLogHandler"])
                                              }).FirstOrDefault();

                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully.";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data .";
                }

            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "Invalid Link !!!";
            }
            return obj;
        }

        public BaseResponse PUNS_UpdateFieldWorkActivityLogStatus(PUUpdateFieldWorkActivityLogStatusRequest input)
        {
            BaseResponse obj = new BaseResponse();
            SqlParameter[] parameters =
                        {
                                          new SqlParameter("@CommunitySiteUserIdentifier", SqlDbType.NVarChar) { Value = input.CommunitySiteUserIdentifier }

                              };

            DataTable dtFieldWork = _helper.GetDataTable("[FieldWork].[PUNS_AuthorizeCommunitySiteSupervisorDemonstrationTeacher]", parameters);
            if (dtFieldWork.Rows.Count > 0 && dtFieldWork != null)
            {
                int cssdtid = Convert.ToInt32(dtFieldWork.Rows[0]["CSSDTID"]);
                SqlParameter[] parameters1 =
                                        {
                                          new SqlParameter("@CssdtID", SqlDbType.BigInt) { Value = cssdtid },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = input.fieldWorkID },
                                          new SqlParameter("@FieldWorkActivityLogID", SqlDbType.BigInt) { Value = input.fieldWorkActivityLogID },
                                          new SqlParameter("@Status", SqlDbType.VarChar) { Value = input.status }
                                     };
                int ID = _helper.InsertTable("[FieldWork].[PUNS_UpdateFieldWorkActivityLogStatus]", parameters1);
                obj.IsSuccess = true;
                obj.Message = "Activity Log Status Updated Successfully";
            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "Invalid Link !!!";
            }
            return obj;
        }

        public FieldWorkEvaluationByIDResponse GetEvaluationByFieldWorkID(int UserID, int FieldWorkID, int ProgramID, string TermCode)
        {
            FieldWorkEvaluationByIDResponse obj = new FieldWorkEvaluationByIDResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = FieldWorkID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar,10) { Value = TermCode }
                                     };
            DataTable evaluationDetails = _helper.GetDataTable("[FieldWork].[GetEvaluationByFieldWorkID]", parameters);
            try
            {
                if(evaluationDetails != null)
                {
                    if (evaluationDetails.Rows.Count > 0)
                    {
                        obj.FieldWorkEvaluationByID = evaluationDetails.AsEnumerable().Select(row =>
                                                  new FieldWorkEvaluationByID
                                                  {
                                                      EvaluationID = Convert.ToInt32(row["EvaluationID"]),
                                                      FieldWorkId = Convert.ToInt32(row["FieldWorkId"]),
                                                      EvaluatorName = Convert.ToString(row["EvaluatorName"]),
                                                      EvaluatorEmail = Convert.ToString(row["EvaluatorEmail"]),
                                                      CreatedBy = Convert.ToInt16(row["CreatedBY"]),
                                                      CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                                                      EvaluationURL = Convert.ToString(row["EvaluationURL"]),
                                                      EvaluationURLValidTill = Convert.ToDateTime(row["EvaluationURLValidTill"] == DBNull.Value ? null : row["EvaluationURLValidTill"]),
                                                      EvaluationIdentifier = Convert.ToString(row["EvaluationIdentifier"]),
                                                      isMailSent = Convert.ToBoolean(row["isMailSent"]),
                                                      EvaluationJSON = Convert.ToString(row["EvaluationJSON"] == DBNull.Value ? null : row["EvaluationJSON"]),
                                                      CanView = Convert.ToBoolean(row["CanView"]),
                                                      FileLink = Convert.ToString(row["FileLink"]),
                                                      ApplicationType = Convert.ToString(row["ApplicationType"])
                                                  }).ToList();

                    }
                    else
                    {
                        List<FieldWorkEvaluationByID> lstEvl = new List<FieldWorkEvaluationByID>();
                        FieldWorkEvaluationByID objEvl = new FieldWorkEvaluationByID();
                        objEvl.EvaluationID = 0;
                        objEvl.FieldWorkId = FieldWorkID;
                        objEvl.EvaluatorName = string.Empty;
                        objEvl.EvaluatorEmail = string.Empty;
                        lstEvl.Add(objEvl);
                        obj.FieldWorkEvaluationByID= lstEvl;
                    }
                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

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
        public BaseResponse UpsertEvaluation(UpsertEvaluationRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@EvaluationID", SqlDbType.BigInt) { Value = input.EvaluationID },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = input.FieldWorkID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@EvaluatorName", SqlDbType.VarChar,200) { Value = input.EvaluatorName },
                                          new SqlParameter("@EvaluatorEmail", SqlDbType.VarChar,200) { Value = input.EvaluatorEmail }
                                        };

            DataSet evaluationDetails = _helper.GetDataSet("[FieldWork].[UpsertEvaluation]", parameters);
            string logoText = "cid:myImageID";
            string evaluatorName = string.Empty;
            string evaluatorEmail = string.Empty;
            string evaluationURL = string.Empty;
            string evaluationIdentifier = string.Empty;
            string applicantName = string.Empty;
            string body = string.Empty;
            string link = string.Empty;
            bool isMailSent = false;
            try
            {
                if (evaluationDetails.Tables[1].Rows.Count > 0)
                {
                    if (Convert.ToString(evaluationDetails.Tables[1].Rows[0]["Status"]) == "FAILURE")
                    {
                        response.Message = Convert.ToString(evaluationDetails.Tables[1].Rows[0]["Message"]);
                        response.IsSuccess = false;
                    }
                    else
                    {
                        if (evaluationDetails.Tables[0].Rows.Count > 0)
                        {
                            // send mail to the evaluator with the URL link  
                            evaluatorName = Convert.ToString(evaluationDetails.Tables[0].Rows[0]["EvaluatorName"]);
                            evaluatorEmail = Convert.ToString(evaluationDetails.Tables[0].Rows[0]["EvaluatorEmail"]);
                            evaluationURL = Convert.ToString(evaluationDetails.Tables[0].Rows[0]["EvaluationURL"]);
                            evaluationIdentifier = Convert.ToString(evaluationDetails.Tables[0].Rows[0]["EvaluationIdentifier"]);
                            applicantName = Convert.ToString(evaluationDetails.Tables[0].Rows[0]["StudentName"]);
                            link = evaluationURL + evaluationIdentifier;
                            //get evaluation mail body
                            SqlParameter[] parameters1 ={
                                            new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt) { Value = 1 },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = 2 },
                                            new SqlParameter("@Identifier", SqlDbType.NVarChar,50) { Value = "Evaluation Mail"}
                                       };
                            DataTable dtEval = _helper.GetDataTable("[Application].[GetEvaluatorEmail]", parameters1);
                            if (dtEval.Rows.Count > 0)
                            {
                                if (dtEval.Rows[0]["EvaluatorMailBody"] != DBNull.Value)
                                {
                                    body = Convert.ToString(dtEval.Rows[0]["EvaluatorMailBody"]);

                                    string beforeBody = string.Empty;
                                    string afterBody = string.Empty;
                                    beforeBody = "<html><body><div><img alt=\"logo\" src=[[logoPath]] width=\"200\" height=\"61\" /></div>";
                                    afterBody = "</body></html>";
                                    body = $"{beforeBody}{body}{afterBody}";
                                    //body = GetMailBodyTemplate("FieldWork_Clinical_Practice_Evaluation_Form.html");
                                    body = body.Replace("[[logoPath]]", logoText)
                                        .Replace("[[applicantname]]", applicantName)
                                        .Replace("[[link]]", link);

                                    string subject = "CSULB MSCP Clinical Practice Evaluation Form";
                                    _sendMail.SendEmail(evaluatorEmail, "", "COMMON", subject, body, "");
                                    isMailSent = true;
                                    SqlParameter[] parmeter1 =
                                    {
                                        new SqlParameter("@EvaluationIdentifier", SqlDbType.UniqueIdentifier) { Value = new Guid(evaluationIdentifier) }
                                    };
                                    DataTable evalDetails = _helper.GetDataTable("[FieldWork].[GetEvaluationByEvaluationIdentifier]", parmeter1);
                                    string id = evalDetails.Rows[0]["EvaluationID"].ToString();
                                    UpdateEvaluationMailSent(input, isMailSent, id);
                                }
                            }
                        }
                        response.Message = "Evaluation added and mail sent successfully";
                        response.IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Evaluation mail send fail.";
            }
            return response;
        }
        private void UpdateEvaluationMailSent(UpsertEvaluationRequest input, bool isMailSent,string id)
        {
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@EvaluationID", SqlDbType.BigInt) { Value = id},
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = input.FieldWorkID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@IsMailSent", SqlDbType.Bit) { Value = isMailSent}
                                        };

            int ID = _helper.InsertTable("[FieldWork].[UpdateEvaluationMailSent]", parameters);

        }
        public EvaluationByEvaluationIdentifierResponse GetEvaluationByEvaluationIdentifier(string evaluationIdentifier)
        {
            EvaluationByEvaluationIdentifierResponse obj = new EvaluationByEvaluationIdentifierResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@EvaluationIdentifier", SqlDbType.UniqueIdentifier) { Value = new Guid(evaluationIdentifier) }
                                     };
            DataTable evaluationDetails = _helper.GetDataTable("[FieldWork].[GetEvaluationByEvaluationIdentifier]", parameters);
                if (evaluationDetails.Rows.Count > 0)
                {


                    obj.evaluationByEvaluationIdentifier = evaluationDetails.AsEnumerable().Select(row =>
                                              new EvaluationByEvaluationIdentifier
                                              {
                                                  EvaluationID = Convert.ToInt32(row["EvaluationID"]),
                                                  FieldWorkID = Convert.ToInt32(row["FieldWorkID"]),
                                                  EvaluatorName = Convert.ToString(row["EvaluatorName"]),
                                                  EvaluationJSON = Convert.ToString(row["EvaluationJSON"] == DBNull.Value ? null : row["EvaluationJSON"]),
                                                  StudentName = Convert.ToString(row["StudentName"]),
                                                  StudentFirstName = Convert.ToString(row["StudentFirstName"]),
                                                  StudentLastName = Convert.ToString(row["StudentLastName"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),
                                                  StudentEmail = Convert.ToString(row["StudentEmail"]),
                                                  CourseTitle = Convert.ToString(row["CourseTitle"]),
                                                  TermName = Convert.ToString(row["TermName"])
                                              }).FirstOrDefault();


                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "The page you are trying to reach has either expired or is not valid.";
                }
            return obj;

        }
        public BaseResponse UpdateEvaluationJSON(UpdateEvaluationJSONRequest input)
        {
            BaseResponse response = new BaseResponse();
            string logoText = "cid:myImageID";
            string evaluatorEmail = string.Empty;
            string applicantName = string.Empty;
            string body = string.Empty;
            bool isMailSent = false;
            string subject = string.Empty;
            string beforeBody = string.Empty;
            string afterBody = string.Empty;
            string studentEmail = string.Empty;
            UpsertEvaluationRequest upsertEvaluationRequest = new UpsertEvaluationRequest();
            //input.EvaluationJSON = "{\"personalInfo\":{\"date\":\"03/01/2025\",\"gradeLevelTaught\":\"Grade \",\"schoolDistrict\":\"School\",\"schoolName\":\"Name\",\"disposition\":[{\"Criteria\":\"Promptness: Timeliness in first contact; Punctuality in attendance\",\"value\":\"Met (Performance Expectations)\"},{\"Criteria\":\"Responsibility: Consistency in schedule and work\",\"value\":\"Met (Performance Expectations)\"},{\"Criteria\":\"Honoring school setting: Compliance with school policies; Displays legal and ethical conduct; and, observing confidentiality at all times\",\"value\":\"Met (Performance Expectations)\"},{\"Criteria\":\"Representing the university: Respectful in professional language, behavior, and appearance. No use of social media in the schooling context at any time.\",\"value\":\"Met (Performance Expectations)\"},{\"Criteria\":\"Communication Skills: University-level language in email, phone contact, and in person\",\"value\":\"Met (Performance Expectations)\"},{\"Criteria\":\"Working with Diverse Populations: Respect and demonstrates insightfulness for all students, various backgrounds, abilities, and orientations\",\"value\":\"Met (Performance Expectations)\"},{\"Criteria\":\"Collaboration: Willing contribution to classroom environment and learning opportunities\",\"value\":\"Met (Performance Expectations)\"},{\"Criteria\":\"Knowledge: Application of course content and best practices; reflection on learning\",\"value\":\"Met (Performance Expectations)\"},{\"Criteria\":\"OVERALL FINAL EVAULATION\",\"value\":\"Met (Performance Expectations)\"}]},\"comments\":\"additional\",\"teacherName\":\"Chandana1\"}";

            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@EvaluationID", SqlDbType.BigInt) { Value = input.EvaluationID },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = input.FieldWorkID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@EvaluationJSON", SqlDbType.VarChar, -1) { Value = input.EvaluationJSON }
                                        };

            DataSet dtDLLOR = _helper.GetDataSet("[FieldWork].[UpdateEvaluationJSON]", parameters);

            if (dtDLLOR.Tables[0].Rows.Count > 0)
            {
                evaluatorEmail = dtDLLOR.Tables[0].Rows[0]["EvaluatorEmail"] != DBNull.Value ? Convert.ToString(dtDLLOR.Tables[0].Rows[0]["EvaluatorEmail"]) : "";
                applicantName = dtDLLOR.Tables[0].Rows[0]["ApplicantName"] != DBNull.Value ? Convert.ToString(dtDLLOR.Tables[0].Rows[0]["ApplicantName"]) : "";
                studentEmail = dtDLLOR.Tables[0].Rows[0]["StudentEmail"] != DBNull.Value ? Convert.ToString(dtDLLOR.Tables[0].Rows[0]["StudentEmail"]) : "";
            }

            upsertEvaluationRequest.EvaluationID = input.EvaluationID;
            upsertEvaluationRequest.UserID = input.UserID;
            upsertEvaluationRequest.ProgramID = input.ProgramID;
            upsertEvaluationRequest.TermCode = input.TermCode;
            upsertEvaluationRequest.FieldWorkID = input.FieldWorkID;

            SqlParameter[] parameters1 ={
                                            new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt, 10) { Value = 1 },
                                            new SqlParameter("@ProgramId", SqlDbType.BigInt) { Value = 2 },
                                            new SqlParameter("@Identifier", SqlDbType.NVarChar) { Value = "Student Confirmation Mail" },

                                       };
            DataSet dtDL = _helper.GetDataSet("[Application].[GetEvaluatorEmail]", parameters1);
            if (dtDL.Tables[0].Rows.Count > 0)
            {
                if (dtDL.Tables[0].Rows[0]["EvaluatorMailBody"] != DBNull.Value)
                {
                    body = Convert.ToString(dtDL.Tables[0].Rows[0]["EvaluatorMailBody"]);
                    beforeBody = "<html><body><div><img alt=\"logo\" src=[[logoPath]] width=\"200\" height=\"61\" /></div>";
                    afterBody = "</body></html>";
                    body = $"{beforeBody}{body}{afterBody}";
                    body = body.Replace("[[logoPath]]", logoText)
                            .Replace("[[applicantname]]", applicantName);
                    subject = "CSULB MSCP Clinical Practice Evaluation Submitted";
                    _sendMail.SendEmail(studentEmail, evaluatorEmail, "COMMON", subject, body, "");
                    isMailSent = true;
                    UpdateEvaluationMailSent(upsertEvaluationRequest, isMailSent, Convert.ToString(input.EvaluationID));
                }
            }
            response.Message = "Data updated successfully";
            response.IsSuccess = true;
            return response;
        }
        public FieldWorkAttachmentsRequest DownloadAttachment(DownloadAttachment input)
        {
            FieldWorkAttachmentsRequest obj = new FieldWorkAttachmentsRequest();
            byte[] fileContentJSONToPDF = GetPDFFromJSON(input.evaluationjson);
            obj.FileName = "FieldWorkEvaluationForm" + "_" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf";
            obj.FileContent = fileContentJSONToPDF;
            return obj;
        }
        private byte[] GetPDFFromJSON(string jsonString)
        {
            byte[] pdfFileContent = null;
            string evaluationTemplateBody = string.Empty;
            string logoPath = Path.GetFullPath("SupportFiles/Img/logo.jpg");
            jsonString = jsonString.Replace("+", " ");
            JObject schema = JObject.Parse(jsonString);

            string date = string.Empty;
            string gradeLevelTaught = string.Empty;
            string schoolDistrict = string.Empty;
            string schoolName = string.Empty;

            string promptness = string.Empty;
            string responsibility = string.Empty;
            string honor = string.Empty;
            string representUniversity = string.Empty;
            string communicationSkills = string.Empty;
            string diversePopulations = string.Empty;
            string collaboration = string.Empty;
            string knowledge = string.Empty;
            string finalEvaluation = string.Empty;

            //string teacherSignature = string.Empty;
            string teacherName = string.Empty;
            string comment = string.Empty;


            JObject personalInfo = (JObject)schema["personalInfo"];
            JArray disposition = (JArray)personalInfo["disposition"];

            date = Convert.ToString(personalInfo["date"]);
            gradeLevelTaught = Convert.ToString(personalInfo.GetValue("gradeLevelTaught"));
            schoolDistrict = Convert.ToString(personalInfo.GetValue("schoolDistrict"));
            schoolName = Convert.ToString(personalInfo.GetValue("schoolName"));
            //teacherSignature = Convert.ToString(schema.GetValue("teacherSignature"));
            teacherName = Convert.ToString(schema.GetValue("teacherName"));
            comment = Convert.ToString(schema.GetValue("comments"));

            //Disposition Criteria
            foreach (JObject content in disposition.Children<JObject>())
            {
                if (content["Criteria"].ToString() == "Promptness: Timeliness in first contact; Punctuality in attendance")
                {
                    promptness = Convert.ToString(content.GetValue("value"));
                }
                if (content["Criteria"].ToString() == "Responsibility: Consistency in schedule and work")
                {
                    responsibility = Convert.ToString(content.GetValue("value"));
                }
                if (content["Criteria"].ToString() == "Honoring school setting: Compliance with school policies; Displays legal and ethical conduct; and, observing confidentiality at all times")
                {
                    honor = Convert.ToString(content.GetValue("value"));
                }
                if (content["Criteria"].ToString() == "Representing the university: Respectful in professional language, behavior, and appearance. No use of social media in the schooling context at any time.")
                {
                    representUniversity = Convert.ToString(content.GetValue("value"));
                }
                if (content["Criteria"].ToString() == "Communication Skills: University-level language in email, phone contact, and in person")
                {
                    communicationSkills = Convert.ToString(content.GetValue("value"));
                }
                if (content["Criteria"].ToString() == "Working with Diverse Populations: Respect and demonstrates insightfulness for all students, various backgrounds, abilities, and orientations")
                {
                    diversePopulations = Convert.ToString(content.GetValue("value"));
                }
                if (content["Criteria"].ToString() == "Collaboration: Willing contribution to classroom environment and learning opportunities")
                {
                    collaboration = Convert.ToString(content.GetValue("value"));
                }
                if (content["Criteria"].ToString() == "Knowledge: Application of course content and best practices; reflection on learning")
                {
                    knowledge = Convert.ToString(content.GetValue("value"));
                }
                if (content["Criteria"].ToString() == "OVERALL FINAL EVAULATION")
                {
                    finalEvaluation = Convert.ToString(content.GetValue("value"));
                }
            }

            evaluationTemplateBody = GetDocumentBodyTemplate("FieldWorkEvaluationFormTemplate.html");
            // replace the values in the template 
            evaluationTemplateBody = evaluationTemplateBody.Replace("[[logoPath]]", logoPath)
                                                                     .Replace("[[Date]]", date)
                                                                     .Replace("[[GradeLevelTaught]]", gradeLevelTaught)
                                                                     .Replace("[[SchoolDistrictName]]", schoolDistrict)
                                                                     .Replace("[[SchoolName]]", schoolName)
                                                                     .Replace("[[Promptness]]", promptness)
                                                                     .Replace("[[Responsibility]]", responsibility)
                                                                     .Replace("[[HonoringSchoolSetting]]", honor)
                                                                     .Replace("[[RepresentingUniversity]]", representUniversity)
                                                                     .Replace("[[CommunicationSkills]]", communicationSkills)
                                                                     .Replace("[[WorkingDiversePopulations]]", diversePopulations)
                                                                     .Replace("[[Collaboration]]", collaboration)
                                                                     .Replace("[[Knowledge]]", knowledge)
                                                                     .Replace("[[FinalEvaluation]]", finalEvaluation)
                                                                     .Replace("[[AdditinalComments]]", comment)
                                                                     //.Replace("[[CooperatingTeacherSignature]]", teacherSignature)
                                                                     .Replace("[[CooperatingTeacherName]]", teacherName);
            // get the filecontent
            pdfFileContent = GetPDFFileContent(evaluationTemplateBody);

            return pdfFileContent;
        }
        public PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents(int communitySiteUsersID, string communitySiteUserName, string communitySiteUserEmail,int activityLogID)
        {
            PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents obj = new PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents();

            SqlParameter[] parameters =
                              {
                                          new SqlParameter("@CommunitySiteUsersID", SqlDbType.BigInt) { Value = communitySiteUsersID },
                                          new SqlParameter("@CommunitySiteUserName", SqlDbType.NVarChar,200) { Value = communitySiteUserName },
                                          new SqlParameter("@CommunitySiteUserEmail", SqlDbType.NVarChar,200) { Value = communitySiteUserEmail },
                                          new SqlParameter("@FieldWorkActivityLogId", SqlDbType.BigInt) { Value =  activityLogID }

                              };

            DataSet dtFieldWork = _helper.GetDataSet("[FieldWork].[PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents]", parameters);
            if (dtFieldWork.Tables.Count > 0)
            {
                obj = dtFieldWork.Tables[0].AsEnumerable().Select(row =>
                                          new PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail_ForStudents
                                          {
                                              CSSDTID = Convert.ToInt32(row["CSSDTID"]),
                                              CommunitySiteUserName = Convert.ToString(row["CommunitySiteUserName"]),
                                              CommunitySiteUserEmail = Convert.ToString(row["CommunitySiteUserEmail"]),
                                              CommunitySiteUserIdentifier = Convert.ToString(row["CommunitySiteUserIdentifier"])

                                          }).FirstOrDefault();

                obj.IsSuccess = true;
                obj.Message = "Data Retrieved Successfully.";
                try
                {
                    //please uncomment after testing
                    //string toUser = "asif.khan@thoughtfocus.com";
                    string toUser = obj.CommunitySiteUserEmail;
                    string link = _configuration["ApplicationKeys:PartnerUserBaseURL"] + obj.CommunitySiteUserIdentifier;
                    string body = GetMailBodyTemplate("PartnerUserMailTemplate.html");
                    string logoText = "cid:myImageID";
                    body = body.Replace("[[logoPath]]", logoText)
                              .Replace("[[link]]", link);
                    string subject = "Approve student hours for CSULB Clinical Practice";
                    _sendMail.SendEmail(toUser, "", "COMMON", subject, body, "");
                    obj.IsSuccess = true;
                    obj.Message = "Partner User Activation mail sent successfully.";
                }
                catch (Exception ee)
                {
                    obj.IsSuccess = false;
                    obj.Message = "Failure sending mail.";
                }
            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data .";
            }


            return obj;
        }
        public FieldWorkActivityLogsAttachmentResponse DownloadActivityLogs(FieldWorkActivityLogsAttachmentRequest input)
        {
            FieldWorkActivityLogsAttachmentResponse obj = new FieldWorkActivityLogsAttachmentResponse();

            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = input.FieldWorkID }
                                     };
            DataSet dsFieldWorkData = _helper.GetDataSet("[dbo].[GetFieldWorkActivityLogDataForDownload]", parameters);
            obj.FileName = "ActivityLogs-Report-" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf";
            obj.FileContent = DownloadActivityLogsContent(dsFieldWorkData);
            return obj;
        }
        public BaseResponse DeleteFieldWorkActivityLog(DeleteActivityLogRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@ActiviyLogId", SqlDbType.BigInt) { Value = input.ActivityLogID },
                                          new SqlParameter("@FieldWorkId", SqlDbType.BigInt) { Value = input.FieldWorkID },
                                          new SqlParameter("@UserId", SqlDbType.BigInt) { Value = input.UserID }
                                    };
            int id = _helper.InsertTable("[FieldWork].[DeleteFieldWorkActivityLog]", parameters);
            response.Message = "Activity Log Deleted Successfully";
            response.IsSuccess = true;
            return response;
        }
        public UpdateCommunitySiteSupervisorDemonstrationTeacherListResponse PUNS_UpdateCommunitySiteSupervisorDemonstrationTeacherList(UpdateCommunitySiteSupervisorDemonstrationTeacherListRequest input)
        {
            UpdateCommunitySiteSupervisorDemonstrationTeacherListResponse obj = new UpdateCommunitySiteSupervisorDemonstrationTeacherListResponse();

            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@CssdtID", SqlDbType.BigInt) { Value = input.cssdtID},
                                          new SqlParameter("@CommunitySiteUserEmail", SqlDbType.NVarChar, 200) { Value = input.communitySiteUserEmail }
                                     };
            DataTable dtCommunityData = _helper.GetDataTable("[FieldWork].[PUNS_UpdateCommunitySiteSupervisorDemonstrationTeacherList]", parameters);
            if (dtCommunityData.Rows.Count > 0)
            {
                obj = dtCommunityData.AsEnumerable().Select(row =>
                                                  new UpdateCommunitySiteSupervisorDemonstrationTeacherListResponse
                                                  {
                                                      CssdtID = Convert.ToInt32(row["CssdtID"]),
                                                      status = Convert.ToBoolean(row["Status"]),
                                                      message = Convert.ToString(row["Message"]).Replace("  ", "").Trim()
                                                  }).FirstOrDefault();
            }
            obj.IsSuccess = true;
            //obj.Message = "Community User Data Updated Successfully";
            return obj;
        }
        private byte[] DownloadActivityLogsContent(DataSet dsFieldWorkData)
        {
            byte[] inputStream = null;
            StringBuilder sbLogData = new StringBuilder();
            string logoPath = Path.GetFullPath("SupportFiles/Img/logo.jpg");
            string studentName = string.Empty;
            string courseTitle = string.Empty;
            string course = string.Empty;
            string instructor = string.Empty;
            string term = string.Empty;
            string studentID = string.Empty;
            string section = string.Empty;
            decimal excpectedHours = 0;
            decimal loggedHours = 0;
            decimal sentForApproval = 0;
            decimal approvedHours = 0;
            //decimal approve = 0;

            //student details
            if (dsFieldWorkData.Tables[2].Rows.Count > 0)
            {
                studentName = Convert.ToString(dsFieldWorkData.Tables[2].Rows[0]["StudentName"]);
                courseTitle = Convert.ToString(dsFieldWorkData.Tables[2].Rows[0]["CourseTitle"]);
                course = Convert.ToString(dsFieldWorkData.Tables[2].Rows[0]["Course"]);
                instructor = Convert.ToString(dsFieldWorkData.Tables[2].Rows[0]["SupervisorName"]);
                term = Convert.ToString(dsFieldWorkData.Tables[2].Rows[0]["Term"]);
                studentID = Convert.ToString(dsFieldWorkData.Tables[2].Rows[0]["StudentID"]);
                section = Convert.ToString(dsFieldWorkData.Tables[2].Rows[0]["Section"]);
            }
            //summary data
            if (dsFieldWorkData.Tables[1].Rows.Count > 0)
            {
                excpectedHours = Convert.ToDecimal(dsFieldWorkData.Tables[1].Rows[0]["ExpectedHours"]);
                loggedHours = Convert.ToDecimal(dsFieldWorkData.Tables[1].Rows[0]["LoggedHours"]);
                sentForApproval = Convert.ToDecimal(dsFieldWorkData.Tables[1].Rows[0]["SentforApproval"]);
                approvedHours = Convert.ToDecimal(dsFieldWorkData.Tables[1].Rows[0]["ApprovedHours"]);
                //approve = Math.Round(Convert.ToDecimal(dsFieldWorkData.Tables[1].Rows[0]["Approved"]),2);
            }

            // loop through activity logs
            for (int y = 0; y < dsFieldWorkData.Tables[0].Rows.Count; y++)
            {
                string activityStartDate = string.Empty;
                string activityEndDate = string.Empty;
                string site = string.Empty;
                decimal hours = 0;
                string schoolDistrict = string.Empty;
                string supervisorName = string.Empty;
                string dropdown1 = string.Empty;
                string dropdown2 = string.Empty;
                string description = string.Empty;
                string status = string.Empty;

                activityStartDate = Convert.ToDateTime(dsFieldWorkData.Tables[0].Rows[y]["ActivityStartDate"]).ToString("MMM-dd-yyyy");
                activityEndDate = Convert.ToDateTime(dsFieldWorkData.Tables[0].Rows[y]["ActivityEndDate"]).ToString("MMM-dd-yyyy");
                site = Convert.ToString(dsFieldWorkData.Tables[0].Rows[y]["Site"]);
                hours = Convert.ToDecimal(dsFieldWorkData.Tables[0].Rows[y]["Hours"]);
                schoolDistrict = Convert.ToString(dsFieldWorkData.Tables[0].Rows[y]["District"]);
                supervisorName = Convert.ToString(dsFieldWorkData.Tables[0].Rows[y]["ParnetUserName"]);
                dropdown1 = Convert.ToString(dsFieldWorkData.Tables[0].Rows[y]["CategoryStandard"]);
                dropdown2 = Convert.ToString(dsFieldWorkData.Tables[0].Rows[y]["SchoolType"]);
                description = Convert.ToString(dsFieldWorkData.Tables[0].Rows[y]["Details"]);
                status = Convert.ToString(dsFieldWorkData.Tables[0].Rows[y]["Status"]);
                string strLogs = ConstructActivityLogRows(activityStartDate,activityEndDate,site,hours.ToString(),schoolDistrict,supervisorName,dropdown1,dropdown2,description,status);
                sbLogData.Append(strLogs);
            }
            // push the data into template 
            string template = GetDocumentBodyTemplate("FieldWorkReport.html");
            template = template.Replace("[[dtFieldWorkDetails]]", sbLogData.ToString())
                               .Replace("[[logoPath]]", logoPath)
                               .Replace("[[studentName]]", studentName)
                               .Replace("[[courseTitle]]", courseTitle)
                               .Replace("[[course]]", course)
                               .Replace("[[instructor]]", instructor)
                               .Replace("[[term]]",term)
                               .Replace("[[section]]",section)
                               .Replace("[[studentID]]",studentID)
                               .Replace("[[excpectedHours]]", excpectedHours.ToString())
                               .Replace("[[loggedHours]]", loggedHours.ToString())
                               .Replace("[[sentForApproval]]", sentForApproval.ToString())
                               .Replace("[[approvedHours]]", approvedHours.ToString());
            inputStream = GetPDFFileContentAsLandscape(template);
            return inputStream;
        }
        public AdhocMailLogResponse PrerequisiteExpired_Sendmail_To_Students(PrerequisiteExpiredRequest input)
        {
            AdhocMailLogResponse obj = new AdhocMailLogResponse();
            DataTable dtPreqList = _helper.GetDataTable("[FieldWork].[GetExpiredPrerequisiteStudentList]", null);
            StringBuilder sbLogData = new StringBuilder();
            sbLogData.Append("<ol>"); 
            int totalFailure = 0;
            if (dtPreqList.Rows.Count > 0)
            {
                string subject = "MyCED prerequisites expired";
                string logoText = "cid:myImageID";
                int count = 0;
                for (int i = 0; i < dtPreqList.Rows.Count; i++)
                {
                    DataRow row = dtPreqList.Rows[i];
                    string applicantName = Convert.ToString(row["ApplicantName"]);
                    string applicantEmail = Convert.ToString(row["Email"]);
                    string CSULBID = Convert.ToString(row["CSULBID"]);
                    try
                    {
                        string body = GetMailBodyTemplate("Prerequisite_Expired_Mail.html");
                        body = body.Replace("[[logoPath]]", logoText)
                                  .Replace("[[ApplicantName]]", applicantName);
                        _sendMail.SendEmail(applicantEmail, "", "COMMON", subject, body, "");
                        count++;
                        string logSummary = $"{applicantName} {CSULBID} mail sent to {applicantEmail} successfully.";
                        sbLogData.Append($"<li>{logSummary}</li>");

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"{input.Type} {input.Identifier} : {ex.Message}.");
                        continue;
                    }
                }
                sbLogData.Append("</ol>");
                //initiate logging
                totalFailure = dtPreqList.Rows.Count - count;
                obj=GetAdocMailLogDetails(input.Type, input.Identifier, sbLogData.ToString(), count, totalFailure, input.UserID);
                obj.IsSuccess = true;
                //obj.Message = "Prerequisites Expired mail sent successfully";

            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data Present.";
            }


            return obj;
        }
        public AdhocMailLogResponse StudentsEnrolled_ApprovedDocuments_BulkEmail(PrerequisiteApprovedRequest input)
        {
                AdhocMailLogResponse obj = new AdhocMailLogResponse(); 
                SqlParameter[] parameters = {
                                                new SqlParameter("@TermCode", SqlDbType.NVarChar, 255) { Value = (object)input.TermCode ?? DBNull.Value }
                                            };

                DataTable dtStudentsInfo = _helper.GetDataTable("[FieldWork].[GetStudentsEnrolled_ApprovedDocuments_BulkEmail]", parameters);
                StringBuilder sbLogData = new StringBuilder();
                sbLogData.Append("<ol>");
                int count = 0;
                int totalFailure = 0;
                if (dtStudentsInfo.Rows.Count > 0 && dtStudentsInfo != null)
                {
                    for (int i = 0; i < dtStudentsInfo.Rows.Count; i++)
                    {
                        try
                        {
                            int FieldWorkID = Convert.ToInt32(dtStudentsInfo.Rows[i]["FieldWorkID"]);
                            EmailMessageModel emailModel = GetMessageBody(FieldWorkID,0,"");
                            // check the prerequisite status and send mail to student 
                            if (!String.IsNullOrEmpty(emailModel.Body))
                            {
                                string toUser = emailModel.toEmail;
                                string body = GetMailBodyTemplate("FinalApprovedTemplate.html");
                                string logoText = "cid:myImageID";
                                body = body.Replace("[[logoPath]]", logoText).Replace("[[ApplicantName]]", emailModel.ApplicantName);
                                string subject = "MyCED prerequisites review completed";
                                _sendMail.SendEmail(toUser, "", "COMMON", subject, body, emailModel.Body);
                                count++;
                                string logSummary = $"{emailModel.ApplicantName} {emailModel.CSULBID} mail sent to {toUser} successfully.";
                                sbLogData.Append($"<li>{logSummary}</li>");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"{input.Type} {input.Identifier} : {ex.Message}.");
                            continue;
                        }
                    }
                    sbLogData.Append("</ol>");
                    //initiate logging
                    totalFailure = dtStudentsInfo.Rows.Count - count;
                    obj = GetAdocMailLogDetails(input.Type, input.Identifier, sbLogData.ToString(), count, totalFailure, input.UserID);
                    obj.IsSuccess = true;
                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "No Data Present.";
                }
            return obj;
        }
        public AdhocMailLogResponse SendNotificationforUnapprovedPartnerUser(UnapprovedPartnerUserMailRequest input)
        {
            AdhocMailLogResponse obj = new AdhocMailLogResponse();
            PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail response = new PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail();
            SqlParameter[] parameters =
                                       {
                                           new SqlParameter("@TermCode", SqlDbType.VarChar, 50) { Value = (object)input.TermCode ?? DBNull.Value }
                                       };

            DataTable dtPartnerUserDetails = _helper.GetDataTable("[FieldWork].[GetPartnerUsers_UnApprovedFieldWorkHours_BulkEmail]", parameters);
            StringBuilder sbLogData = new StringBuilder();
            int totalFailure = 0;
            int count = 0;
            sbLogData.Append("<ol>");
            for (int i = 0; i < dtPartnerUserDetails.Rows.Count; i++)
            {
                try
                { 
                    response = PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail(Convert.ToInt32(dtPartnerUserDetails.Rows[i]["CSSDTID"]));
                    count++;
                    string logSummary = $"{response.CommunitySiteUserName} mail sent to {response.CommunitySiteUserEmail} successfully.";
                    sbLogData.Append($"<li>{logSummary}</li>");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{input.Type} {input.Identifier} : {ex.Message}.");
                    continue;
                }
            }
            sbLogData.Append("</ol>");
            //initiate logging
            totalFailure = dtPartnerUserDetails.Rows.Count - count;
            obj = GetAdocMailLogDetails(input.Type, input.Identifier, sbLogData.ToString(), count, totalFailure, input.UserID);
            obj.IsSuccess = true;
            return obj;
        }
        public FieldWorkSemesterList GetFieldWorkTerms()
        {
            FieldWorkSemesterList obj = new FieldWorkSemesterList();
            SqlParameter[] parameters = { };
            DataTable dtSemesters = _helper.GetDataTable("[FieldWork].[GetFieldWorkTerms]", parameters);
            try
            {
                if (dtSemesters.Rows.Count > 0)
                {
                    obj.Semesters = dtSemesters.AsEnumerable().Select(row =>
                                              new FieldWorkSemester
                                              {
                                                  Value = Convert.ToString(row["TermCode"]),
                                                  Label = Convert.ToString(row["Description"])
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

        public FieldWorkCourseList GetFieldWorkCourses(string termCode)
        {
            FieldWorkCourseList obj = new FieldWorkCourseList();
           // SqlParameter[] parameters = { };
            SqlParameter[] parameters =
                                     {
                                           new SqlParameter("@TermCode", SqlDbType.VarChar, 50) { Value = (object)termCode ?? DBNull.Value }
                                       };

            DataTable dtCourses = _helper.GetDataTable("[FieldWork].[GetFieldWorkCourses]", parameters);
            try
            {
                if (dtCourses.Rows.Count > 0)
                {
                    obj.Courses = dtCourses.AsEnumerable().Select(row =>
                                              new FieldWorkCourse
                                              {
                                                  Value = Convert.ToInt32(row["Id"]),
                                                  Label = Convert.ToString(row["Name"])
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

        public FieldWorkCourseConfiguration GetFieldWorkCourseConfiguration(int courseId)
        {
            FieldWorkCourseConfiguration obj = new FieldWorkCourseConfiguration();
            // SqlParameter[] parameters = { };
            SqlParameter[] parameters =
                                       {
                                           new SqlParameter("@CourseId", SqlDbType.Int) { Value = (object)courseId ?? DBNull.Value }
                                       };

            DataSet dtCourseConfiguration = _helper.GetDataSet("[FieldWork].[GetFieldWorkCourseConfiguration]", parameters);
            try
            {
                if (dtCourseConfiguration.Tables.Count > 0)
                {
                    DataTable dtCourseConfig = dtCourseConfiguration.Tables[0];

                    obj.Name = Convert.ToString(dtCourseConfig.Rows[0]["Name"]);
                    obj.Subject = Convert.ToString(dtCourseConfig.Rows[0]["Subject"]);
                    obj.CourseNumber = Convert.ToString(dtCourseConfig.Rows[0]["CourseNumber"]);
                    obj.ClassSection = Convert.ToString(dtCourseConfig.Rows[0]["ClassSection"]);
                    var activityLogConfig = new FieldWorkActivityConfig();

                    activityLogConfig.EnableActivityLog = Convert.ToBoolean(dtCourseConfig.Rows[0]["EnableActivityLog"]);
                    activityLogConfig.AutoCompute = Convert.ToBoolean(dtCourseConfig.Rows[0]["AutoCompute"]);
                    activityLogConfig.FieldWorkHours = Convert.ToString(dtCourseConfig.Rows[0]["FieldWorkHours"]);
                    activityLogConfig.CategoryID = Convert.ToString(dtCourseConfig.Rows[0]["CategoryID"]) == "0" ? "" : Convert.ToString(dtCourseConfig.Rows[0]["CategoryID"]);
                    activityLogConfig.RecordByDate = Convert.ToBoolean(dtCourseConfig.Rows[0]["RecordByDate"]);


                    var documentsConfig = dtCourseConfiguration.Tables[1].AsEnumerable().Select(row =>
                                              new FieldWorkDocuments
                                              {
                                                  DocumentName = Convert.ToString(row["DocumentName"]),
                                                  DocumentId = Convert.ToInt32(row["DocumentID"]),
                                                  IsRestricted = Convert.ToBoolean(row["IsRestricted"]),
                                                  isRequiredPrerequisite = Convert.ToBoolean(row["isRequiredPrerequisite"])
                                              }).ToList();

                    activityLogConfig.DocumentsConfig = documentsConfig;

                    obj.Configs = activityLogConfig;
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

        public FieldWorkCoursesCategoryList GetFieldWorkCoursesCategories()
        {
            FieldWorkCoursesCategoryList obj = new FieldWorkCoursesCategoryList();
            SqlParameter[] parameters = { };
            DataTable dtCategories = _helper.GetDataTable("[FieldWork].[GetFieldWorkCoursesCategory]", parameters);
            try
            {
                if (dtCategories.Rows.Count > 0)
                {
                    obj.Categories = dtCategories.AsEnumerable().Select(row =>
                                              new FieldWorkCoursesCategory
                                              {
                                                  Value = Convert.ToString(row["ID"]),
                                                  Label = Convert.ToString(row["Category"])
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

        public BaseResponse UpdateFieldWorkCourseConfiguration(FieldWorkCourseConfigurationRequest input)
        {
            BaseResponse obj = new BaseResponse();
            try
            {
                if (input.DocumentsConfig != null)
                {
                    foreach (var item in input.DocumentsConfig)
                    {
                        item.CourseId = input.CourseId;
                    }
                }

                DataTable documentConfigTable = ToDataTable(input.DocumentsConfig);

                SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@CourseId", SqlDbType.Int) { Value = input.CourseId },
                                          new SqlParameter("@EnableActivityLog", SqlDbType.Bit) { Value = input.EnableActivityLog },
                                          new SqlParameter("@AutoCompute", SqlDbType.Bit) { Value = input.AutoCompute },
                                          new SqlParameter("@CategoryID", SqlDbType.Int) { Value = Convert.ToInt16(input.CategoryID) },
                                          new SqlParameter("@RecordByDate", SqlDbType.Bit) { Value = input.RecordByDate },
                                          new SqlParameter("@FieldWorkHours", SqlDbType.Int) { Value = input.FieldWorkHours },
                                          new SqlParameter("@DocumentConfig", SqlDbType.Structured) { Value = documentConfigTable }
                                     };

                DataTable dtResult = _helper.GetDataTable("[FieldWork].[UpSertFieldWorkCourseConfiguration]", parameters);

                if (dtResult != null && dtResult.Rows.Count > 0)
                {

                    obj = dtResult.AsEnumerable().Select(row =>
                                                     new BaseResponse
                                                     {
                                                         IsSuccess = Convert.ToBoolean(row["IsSuccess"]),
                                                         Message = Convert.ToString(row["Message"]),

                                                     }).FirstOrDefault();

                }
                else
                {

                    obj.IsSuccess = false;
                    obj.Message = "Course Configuration Update Error";


                }
            }
            catch (Exception ex)
            {

                obj.IsSuccess = false;
                obj.Message = "Data Updated Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }

            return obj;
        }
        public StandardsAndSchoolTypeDropdownList GetStandardsAndSchoolTypeDropdown(int categoryID, string dropdownType)
        {
            StandardsAndSchoolTypeDropdownList obj = new StandardsAndSchoolTypeDropdownList();
            SqlParameter[] parameters = {
                                           new SqlParameter("@CategoryID", SqlDbType.BigInt) { Value = categoryID },
                                           new SqlParameter("@DropdownType", SqlDbType.VarChar,20) { Value = dropdownType },
                                        };
            DataTable dtDropDownList = _helper.GetDataTable("[FieldWork].[GetStandardsAndSchoolTypeDropdown]", parameters);
            try
            {
                if (dtDropDownList.Rows.Count > 0)
                {
                    obj.dropDowns = dtDropDownList.AsEnumerable().Select(row =>
                                              new StandardsAndSchoolTypeDropdown
                                              {
                                                  DropdownId = Convert.ToInt32(row["ID"]),
                                                  ControlLabel = Convert.ToString(row["NAME"]),
                                                  ControlValue = Convert.ToString(row["DESCRIPTION"]),
                                                  Active = Convert.ToBoolean(row["IsActive"])
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
        public BaseResponse UpsertDropDown(UpdateDropDownRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@ID", SqlDbType.BigInt) { Value = input.DropdownId },
                                          new SqlParameter("@CategoryID", SqlDbType.BigInt) { Value = input.CategoryID },
                                          new SqlParameter("@DropdownType", SqlDbType.VarChar,20) { Value = input.DropdownType},
                                          new SqlParameter("@ActionFlag", SqlDbType.BigInt) { Value = input.Action },
                                          new SqlParameter("@Name", SqlDbType.VarChar,250) { Value = input.ControlLabel },
                                          new SqlParameter("@Description", SqlDbType.VarChar, -1) { Value = input.ControlValue},
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID}
                                        };
            DataTable dtResponse = _helper.GetDataTable("[FieldWork].[AddEditDeleteDropdown]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                if (Convert.ToString(dtResponse.Rows[0]["Message"]) == "SUCCESS")
                {
                    response.Message = Convert.ToString(dtResponse.Rows[0]["SuccessMessage"]);
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtResponse.Rows[0]["Message"]) == "FAILURE")
                {
                    response.Message = Convert.ToString(dtResponse.Rows[0]["SuccessMessage"]);
                    response.IsSuccess = false;
                }
            }
            return response;
        }

        public AdhocMailLogResponse GetAdocMailLogDetails(string type, string identifier, string sbLogData, int count, int totalFailure, int userID)
        {
            AdhocMailLogResponse obj = new AdhocMailLogResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@Type", SqlDbType.NVarChar,100) { Value = type},
                                          new SqlParameter("@Identifier", SqlDbType.NVarChar, 50) { Value = identifier },
                                          new SqlParameter("@LogSummary", SqlDbType.NVarChar, -1) { Value = sbLogData.ToString() },
                                          new SqlParameter("@TotalSent", SqlDbType.BigInt) { Value = count },
                                          new SqlParameter("@TotalFailure", SqlDbType.BigInt) { Value = totalFailure },
                                          new SqlParameter("@TriggeredBy", SqlDbType.BigInt) { Value = userID }
                                     };
            DataTable dtMailDetails = _helper.GetDataTable("[dbo].[Upsert_Adhoc_Mail_Notification_Log]", parameters);
            if (dtMailDetails.Rows.Count > 0)
            {
                obj = dtMailDetails.AsEnumerable().Select(row =>
                                                  new AdhocMailLogResponse
                                                  {
                                                      ID = Convert.ToInt32(row["ID"]),
                                                      Type = Convert.ToString(row["Type"]),
                                                      Identifier = Convert.ToString(row["Identifier"]),
                                                      LogSummary = Convert.ToString(row["LogSummary"]),
                                                      TotalSent = Convert.ToInt32(row["TotalSent"]),
                                                      TotalFailure = Convert.ToInt32(row["TotalFailure"]),
                                                      TriggeredBy = Convert.ToInt32(row["TriggeredBy"]),
                                                      TriggeredDate = Convert.ToDateTime(row["TriggeredDate"])
                                                  }).FirstOrDefault();


            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data Present in Adhoc_Mail_Notification_Log table .";
            }
            return obj;
        }
        private string ConstructActivityLogRows(string activityStartDate, string activityEndDate,string site, string hours,string schoolDistrict,string supervisorName,string dropdown1,string dropdown2,string description,string status)
        {
            StringBuilder sbRows = new StringBuilder();
            sbRows.Append("<tr>");
            sbRows.Append("<td width='12%' style='font-size: 10px; text-align:center;'>" + activityStartDate + "</td>");
            sbRows.Append("<td width='12%' style='font-size: 10px; text-align:center;'>" + activityEndDate + "</td>");
            sbRows.Append("<td width='18%' style='font-size: 10px; text-align:center;'>" + schoolDistrict + "</td>");
            sbRows.Append("<td width='23%' style='font-size: 10px; text-align:center;'>" + site + "</td>");
            sbRows.Append("<td width='15%' style='font-size: 10px; text-align:center;'>" + supervisorName + "</td>");
            sbRows.Append("<td width='15%' style='font-size: 10px; text-align:center;'>" + dropdown1 + "</td>");
            sbRows.Append("<td width='15%' style='font-size: 10px; text-align:center;'>" + dropdown2 + "</td>");
            sbRows.Append("<td width='25%' style='font-size: 10px; text-align:center;'>" + description + "</td>");
            sbRows.Append("<td width='8%' style='font-size: 10px; text-align:center;'>" + hours + "</td>");
            sbRows.Append("<td width='10%' style='font-size: 10px; text-align:center;'>" + status + "</td>");
            sbRows.Append("</tr>");
            return sbRows.ToString();
        }
        private byte[] GetPDFFileContentAsLandscape(string htmlFormBody)
        {
            byte[] fileContent = null;
            StringReader sr = new StringReader(htmlFormBody); // workable code uncomment after testing 
            //iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50); //portrait mode
            iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 25, 25, 16, 16);//landscape mode

            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                iTextSharp.text.pdf.PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();

                htmlparser.Parse(sr);
                pdfDoc.Close();

                fileContent = memoryStream.ToArray();
                memoryStream.Close();
            }
            return fileContent;
        }
        private byte[] GetPDFFileContent(string htmlFormBody)
        {
            byte[] fileContent = null;
            StringReader sr = new StringReader(htmlFormBody); // workable code uncomment after testing 
            //TextReader sr = new StringReader(htmlFormBody);
            //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50);
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
        private string GetDocumentBodyTemplate(string templateName)
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



    public class EmailMessageModel
    {
        public string toEmail { get; set; }
        public string ApplicantName { get; set; }
        public string CSULBID { get; set; }
        public string Semester { get; set; }
        public string Body { get; set; }
    }
    public class StudentsDetails
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string CSULBID { get; set; }
        public string FileName { get; set; }
    }
    public class ApproveRejectMailer
    {
        public string Body { get; set; }
        public string Subject { get; set; }
    }
    public class AttachmentFileDetails
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }
    
}
