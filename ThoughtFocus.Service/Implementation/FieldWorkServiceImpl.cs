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
                                                  UIHandler = Convert.ToString(row["UIHandler"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"])
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
                                                  CanValidate = Convert.ToBoolean(row["CanValidate"]),
                                                  DocumentNumber = Convert.ToInt32(row["DocumentNumber"] == DBNull.Value ? null : row["DocumentNumber"])
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


            //                            };
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
                                                  CourseName = Convert.ToString(row["CourseName"]),
                                                  Email = Convert.ToString(row["Email"]),
                                                  DocumentNumber = Convert.ToString(row["DocumentNumber"])
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
                                                            //string body = GetMailBodyTemplate("FinalApprovedTemplate.html");
                        string body = string.Empty;
                        SqlParameter[] parameters1 ={
                                 new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = 0 },
                                 new SqlParameter("@SectionName", SqlDbType.VarChar,10) { Value = ""},
                                 new SqlParameter("@CategoryName", SqlDbType.VarChar,20) { Value = "" },
                                 new SqlParameter("@Identifier", SqlDbType.VarChar,20) { Value = "FieldWork Mail"},
                                 new SqlParameter("@Name", SqlDbType.VarChar,50) { Value = "BothPrerequisiteApproved" }
                            };
                        DataTable dtMailBody = _helper.GetDataTable("[dbo].[GetAdmissionRequirementsMailBody]", parameters1);
                        if (dtMailBody.Rows.Count > 0)
                        {
                            if (dtMailBody.Rows[0]["EmailBody"] != DBNull.Value)
                            {
                                body = Convert.ToString(dtMailBody.Rows[0]["EmailBody"]);

                                string beforeBody = string.Empty;
                                string afterBody = string.Empty;
                                beforeBody = "<body>\r\n<div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                                afterBody = "</body>\r\n</html>";
                                body = $"{beforeBody}{body}{afterBody}";
                            }
                        }
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
            string logopath = Path.GetFullPath("SupportFiles/Img/logo.jpeg");
            string logoText = "cid:myImageID";
            string name = string.Empty;
            if (approvalStatus == true)
            {
                model.Subject = "MyCED prerequisites review approved";
                name = "PrerequisiteApproved";
                // get the body from email template
                //body = GetMailBodyTemplate("ApprovedMailTemplate.html");
            }
            else if (approvalStatus == false)
            {
                model.Subject = "MyCED prerequisites review not approved";
                name = "PrerequisiteNotApproved";
                // get the body from email template
                //body = GetMailBodyTemplate("NotApprovedMailTemplate.html");
            }
            SqlParameter[] parameters ={
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = 0 },
                                            new SqlParameter("@SectionName", SqlDbType.VarChar,10) { Value = ""},
                                            new SqlParameter("@CategoryName", SqlDbType.VarChar,20) { Value = "" },
                                            new SqlParameter("@Identifier", SqlDbType.VarChar,20) { Value = "FieldWork Mail"},
                                            new SqlParameter("@Name", SqlDbType.VarChar,50) { Value = name }
                                       };
            DataTable dtMailBody = _helper.GetDataTable("[dbo].[GetAdmissionRequirementsMailBody]", parameters);
            if (dtMailBody.Rows.Count > 0)
            {
                if (dtMailBody.Rows[0]["EmailBody"] != DBNull.Value)
                {
                    body = Convert.ToString(dtMailBody.Rows[0]["EmailBody"]);

                    string beforeBody = string.Empty;
                    string afterBody = string.Empty;
                    beforeBody = "<body>\r\n<div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                    afterBody = "</body>\r\n</html>";
                    body = $"{beforeBody}{body}{afterBody}";
                }
            }
            body = body.Replace("[[ApplicantName]]", displayName).Replace("[[DocumentName]]", documentName).Replace("[[logoPath]]", logoText).Replace("[[Reason]]", rejectReason).Replace("[[Comment]]", comments);
            //body = "<html><body><p>Your document "+documentName+" has been approved</p><p>Thank you,</br>CSULB College of Education  </p></body></html>";
            model.Body = body;
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
                string logopath = Path.GetFullPath("SupportFiles/Img/logo.jpeg");
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
                                          new SqlParameter("@UploadedDate", SqlDbType.DateTime) { Value = input.UploadedDate},
                                          new SqlParameter("@DocumentNumber", SqlDbType.BigInt) { Value = input.DocumentNumber}
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
                                              Status = Convert.ToString(row["Status"]),
                                              CommunitySiteUserEmail = Convert.ToString(row["CommunitySiteUserEmail"])

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
                    string URL = _configuration["ApplicationKeys:PartnerUserBaseURL"] + obj.CommunitySiteUserIdentifier;
                    string body = string.Empty;
                    //string body = GetMailBodyTemplate("PartnerUserMailTemplate.html");
                    string logoText = "cid:myImageID";
                    SqlParameter[] parameters1 ={
                                 new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = 0 },
                                 new SqlParameter("@SectionName", SqlDbType.VarChar,10) { Value = ""},
                                 new SqlParameter("@CategoryName", SqlDbType.VarChar,20) { Value = "" },
                                 new SqlParameter("@Identifier", SqlDbType.VarChar,20) { Value = "FieldWork Mail"},
                                 new SqlParameter("@Name", SqlDbType.VarChar,50) { Value = "PartnerUser" }
                            };
                    DataTable dtMailBody = _helper.GetDataTable("[dbo].[GetAdmissionRequirementsMailBody]", parameters1);
                    if (dtMailBody.Rows.Count > 0)
                    {
                        if (dtMailBody.Rows[0]["EmailBody"] != DBNull.Value)
                        {
                            body = Convert.ToString(dtMailBody.Rows[0]["EmailBody"]);

                            string beforeBody = string.Empty;
                            string afterBody = string.Empty;
                            beforeBody = "<html>\r\n<head>\r\n<style>\r\n\r\n#link {\r\ncolor: #0563C1;\r\n}\r\n.custom-link {\r\ncolor: #0563C1;\r\n}\r\n</style>\r\n</head>\r\n<body>\r\n<div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\"/></div>";
                            afterBody = "</body>\r\n</html>";
                            body = $"{beforeBody}{body}{afterBody}";
                        }
                    }
                    string link = @"<a href ='" + URL + "' target='_blank' class='custom-link'>Click here</a>";
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
                obj.Message = "No Data.";
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
            int programID = 0;
            string subject = string.Empty;
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
                            if(input.ApplicationType == "FieldWork-MSCP")
                            {
                                programID = 2;
                                subject = "CSULB MSCP Clinical Practice Evaluation Form";
                            }
                            else if(input.ApplicationType == "FieldWork-PK3")
                            {
                                programID = 3;
                                subject = "CSULB PK3 Clinical Practice Evaluation Form";
                            }
                            else if (input.ApplicationType == "FieldWork-SSCP")
                            {
                                programID = 4;
                                subject = "CSULB SSCP Clinical Practice Evaluation Form";
                            }
                            //get evaluation mail body
                            SqlParameter[] parameters1 ={
                                            new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt) { Value = 1 },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
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
                                    beforeBody = "<html><body><div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                                    afterBody = "</body></html>";
                                    body = $"{beforeBody}{body}{afterBody}";
                                    //body = GetMailBodyTemplate("FieldWork_Clinical_Practice_Evaluation_Form.html");
                                    body = body.Replace("[[logoPath]]", logoText)
                                        .Replace("[[applicantname]]", applicantName)
                                        .Replace("[[link]]", link);
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
                                                  TermName = Convert.ToString(row["TermName"]),
                                                  ApplicationType = Convert.ToString(row["ApplicationType"]),
                                                  CourseNameandNumber = Convert.ToString(row["CourseNameandNumber"])
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
            int programID = 0;
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
            if(input.ApplicationType == "FieldWork-MSCP")
            {
                programID = 2;
                subject = "CSULB MSCP Clinical Practice Evaluation Submitted";
            }
            else if (input.ApplicationType == "FieldWork-PK3")
            {
                programID = 3;
                subject = "CSULB PK3 Clinical Practice Evaluation Submitted";
            }
            else if (input.ApplicationType == "FieldWork-SSCP")
            {
                programID = 4;
                subject = "CSULB SSCP Clinical Practice Evaluation Submitted";
            }

            SqlParameter[] parameters1 ={
                                            new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt, 10) { Value = 1 },
                                            new SqlParameter("@ProgramId", SqlDbType.BigInt) { Value = programID },
                                            new SqlParameter("@Identifier", SqlDbType.NVarChar) { Value = "Student Confirmation Mail" },

                                       };
            DataSet dtDL = _helper.GetDataSet("[Application].[GetEvaluatorEmail]", parameters1);
            if (dtDL.Tables[0].Rows.Count > 0)
            {
                if (dtDL.Tables[0].Rows[0]["EvaluatorMailBody"] != DBNull.Value)
                {
                    body = Convert.ToString(dtDL.Tables[0].Rows[0]["EvaluatorMailBody"]);
                    beforeBody = "<html><body><div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                    afterBody = "</body></html>";
                    body = $"{beforeBody}{body}{afterBody}";
                    body = body.Replace("[[logoPath]]", logoText)
                            .Replace("[[applicantname]]", applicantName);
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
            byte[] fileContentJSONToPDF = GetPDFFromJSON(input);
            obj.FileName = "FieldWorkEvaluationForm" + "_" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf";
            obj.FileContent = fileContentJSONToPDF;
            return obj;
        }
        private byte[] GetPDFFromJSON(DownloadAttachment input)
        {
            byte[] pdfFileContent = null;
            string evaluationTemplateBody = string.Empty;
            string logoPath = Path.GetFullPath("SupportFiles/Img/logo.jpeg");
            //input.evaluationjson = "{\r\n  \"sections\": [\r\n    {\r\n      \"title\": \"A. Engaging and Supporting All Students in Learning (TPE 1)\",\r\n      \"subtitle\": \"Building & Maintaining Student Engagement\",\r\n      \"indicators\": [\r\n        {\r\n          \"label\": \"Establishes clear and appropriate learning goals and expected outcomes for all students and uses effective activities to meet them\",\r\n          \"rating\": \"E\"\r\n        },\r\n        {\r\n          \"label\": \"Connects subject matter to real-life contexts through active learning experiences by applying knowledge of students’ cultural, linguistic, and prior experiences to make instruction relevant\",\r\n          \"rating\": \"E\"\r\n        },\r\n        {\r\n          \"label\": \"Promotes students’ critical and creative thinking through instructional strategies that support access to the curriculum for all students\",\r\n          \"rating\": \"E\"\r\n        },\r\n        {\r\n          \"label\": \"Models instruction, provides clear directions, and checks for understanding to promote reflective self-directed learners who can work independently and collaboratively\",\r\n          \"rating\": \"E\"\r\n        }\r\n      ]\r\n    },\r\n    {\r\n      \"title\": \"B. Creating and Maintaining Effective Environments for Student Learning (TPE 2)\",\r\n      \"subtitle\": \"Student Behavior and Classroom Management\",\r\n      \"indicators\": [\r\n        {\r\n          \"label\": \"Designs and maintains a fair and appropriate system of classroom management that fosters an affirming learning community which incorporates student input\",\r\n          \"rating\": \"P\"\r\n        },\r\n        {\r\n          \"label\": \"Creates an effective learning environment that encourages positive interactions among students, reflects diverse perspectives, and is culturally responsive\",\r\n          \"rating\": \"P\"\r\n        },\r\n        {\r\n          \"label\": \"Establishes, maintains, and monitors high expectations for learning and student behavior utilizing procedures and routines that include a variety of strategies to support the full range of students in classroom\",\r\n          \"rating\": \"P\"\r\n        },\r\n        {\r\n          \"label\": \"Supports students assuming responsibility for learning; encourages important behaviors such as being on time, completing assignments, and remaining engaged throughout the lesson\",\r\n          \"rating\": \"P\"\r\n        }\r\n      ]\r\n    },\r\n    {\r\n      \"title\": \"C. Understanding & Organizing Subject Matter for Student Learning (TPE 3&7)\",\r\n      \"subtitle\": \"C-1 Subject Matter Knowledge Content Specific Pedagogy\",\r\n      \"indicators\": [\r\n        {\r\n          \"label\": \"Demonstrates subject matter knowledge to deliver evidence-based literacy instruction that integrates content and literacy strategies aligned with California State Standards, English Language Development Standards, curriculum frameworks, educational technology standards, and CA Dyslexia Guidelines\",\r\n          \"rating\": \"D\"\r\n        },\r\n        {\r\n          \"label\": \"Uses and adapts resources, standards-aligned instructional materials, and a range of technology to facilitate students’ equitable access to the curriculum\",\r\n          \"rating\": \"D\"\r\n        },\r\n        {\r\n          \"label\": \"Incorporates asset-based pedagogies, inclusive approaches, and culturally and linguistically affirming and sustaining practices in literacy instruction\",\r\n          \"rating\": \"D\"\r\n        },\r\n        {\r\n          \"label\": \"Demonstrates the ability to use subject-matter knowledge and pedagogical skills to design day-to-day cohesive instruction based on long-term learning goals\",\r\n          \"rating\": \"D\"\r\n        }\r\n      ]\r\n    },\r\n    {\r\n      \"title\": \"C. Understanding & Organizing Subject Matter for Student Learning (TPE 3&7)\",\r\n      \"subtitle\": \"C-2 Effective Literacy Instruction for All Students\",\r\n      \"indicators\": [\r\n        {\r\n          \"label\": \"Develops and advances students’ progress in the elements of foundational literacy skills, language, and cognitive skills that support them as they read and write increasingly complex disciplinary texts\",\r\n          \"rating\": \"NC\"\r\n        },\r\n        {\r\n          \"label\": \"Engage students in meaning making by building on prior knowledge and using complex literary and informational texts (print, digital, and oral), to develop students’ literal and inferential comprehension\",\r\n          \"rating\": \"NC\"\r\n        },\r\n        {\r\n          \"label\": \"Develop students’ oral and written language development by attending to vocabulary knowledge, use of grammatical structures, and effective expression as they write, discuss, present, and use language for a variety of purposes, audiences, and contexts\",\r\n          \"rating\": \"NC\"\r\n        },\r\n        {\r\n          \"label\": \"Promote students’ knowledge in content-area by engaging students in literacy instruction that integrates reading, writing, listening, and speaking through printed and digital texts and multimedia, discussions, experimentation, hands-on explorations, and wide and independent reading\",\r\n          \"rating\": \"NC\"\r\n        }\r\n      ]\r\n    },\r\n    {\r\n      \"title\": \"D. Planning Instruction and Designing Learning Experiences for All Students (TPE 4)\",\r\n      \"subtitle\": \"Lesson Plans and Delivery\",\r\n      \"indicators\": [\r\n        {\r\n          \"label\": \"Uses research-based practices and information about students to design effective unit and lesson plans, reflecting short-term and long-term goals, in collaboration with school colleagues\",\r\n          \"rating\": \"N/O\"\r\n        },\r\n        {\r\n          \"label\": \"Plans, implements, and differentiates instruction, making effective use of instructional time to maximize learning opportunities and provide access to the curriculum\",\r\n          \"rating\": \"N/O\"\r\n        },\r\n        {\r\n          \"label\": \"Plans and utilizes a variety of effective instructional approaches and modifies activities and materials to maximize learning\",\r\n          \"rating\": \"D\"\r\n        },\r\n        {\r\n          \"label\": \"Uses digital tools and learning technologies to create new content and provide integrated technology-rich lessons to develop digital literacy, promote digital citizenship, and offer students multiple means to demonstrate their learning\",\r\n          \"rating\": \"D\"\r\n        }\r\n      ]\r\n    },\r\n    {\r\n      \"title\": \"E. Assessing Student Learning (TPE 5)\",\r\n      \"subtitle\": \"Assessment\",\r\n      \"indicators\": [\r\n        {\r\n          \"label\": \"Applies knowledge of the purposes and appropriate uses of different informal, formal, and student self-assessments to provide actionable feedback, and when appropriate use scoring rubrics\",\r\n          \"rating\": \"D\"\r\n        },\r\n        {\r\n          \"label\": \"Monitor students’ progress in literacy development using formative assessment practices, ongoing progress monitoring, and diagnostic techniques to inform instructional decision making\",\r\n          \"rating\": \"D\"\r\n        },\r\n        {\r\n          \"label\": \"Communicates assessment results promptly to help students and families understand progress toward meeting learning goals\",\r\n          \"rating\": \"D\"\r\n        },\r\n        {\r\n          \"label\": \"Uses assessment data and student learning plans to set learning goals, differentiate instruction, make accommodations, and/or modify instruction to meet the needs of all students\",\r\n          \"rating\": \"D\"\r\n        }\r\n      ]\r\n    },\r\n    {\r\n      \"title\": \"F. Developing as a Professional Educator (TPE 6)\",\r\n      \"subtitle\": \"Professionalism\",\r\n      \"indicators\": [\r\n        {\r\n          \"label\": \"Reflects on their own teaching practice and content knowledge, using SMART goals and regular collaboration with colleagues to improve student\",\r\n          \"rating\": \"NC\"\r\n        },\r\n        {\r\n          \"label\": \"Models ethical behaviors and demonstrates care, support, acceptance, and fairness towards all students, school personnel, and members of the larger school community\",\r\n          \"rating\": \"NC\"\r\n        },\r\n        {\r\n          \"label\": \"Demonstrates enthusiasm and professional demeanor through competence in oral and written communication, on-time arrival, preparedness, professional appearance, adherence to deadlines, accurately maintained records, and involving other professionals to support student learning when appropriate\",\r\n          \"rating\": \"NC\"\r\n        },\r\n        {\r\n          \"label\": \"Understands and adheres to state and federal laws and procedures pertaining to the education of all students; enacts professional roles and responsibilities as mandated reporters and complies with laws pertaining to the use of social media and other digital platforms both inside and outside of the classroom\",\r\n          \"rating\": \"NC\"\r\n        }\r\n      ]\r\n    },\r\n    {\r\n      \"title\": \"G. Overall Teaching Effectiveness Assessment\",\r\n      \"subtitle\": \"Holistic Assessment of Performance\",\r\n      \"indicators\": [\r\n        {\r\n          \"label\": \"This rating is a holistic assessment of the student teacher’s performance.It is not an average score of categories A-F. <strong>A rating of “not consistent with standard expectations for beginning practice (NC)” in Category “G” on the Final Evaluation will result in no credit received for student teaching.</strong> The candidate will not be recommended for the credential.\",\r\n          \"rating\": \"P\"\r\n        }\r\n      ]\r\n    }\r\n  ],\r\n  \"strengths\": \"Testing\",\r\n  \"growthAreas\": \"Test2\",\r\n  \"info\": {\r\n    \"date\": \"August 13, 2026\",\r\n    \"studentName\": \"Selena Lopez\",\r\n    \"universityMentorName\": \"Test\"\r\n  }\r\n}";
            //input.evaluationjson = "{\r\n\"evaluationJSON\": {\r\n    \"sections\": [\r\n      {\r\n        \"title\": \"A. Engaging and Supporting All Students in Learning (TPE 1)\",\r\n        \"subtitle\": \"Building & Maintaining Student Engagement\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Establishes clear and appropriate learning goals and expected outcomes for all students and uses effective activities to meet them\",\r\n            \"rating\": \"E\"\r\n          },\r\n          {\r\n            \"label\": \"Connects subject matter to real-life contexts through active learning experiences by applying knowledge of students’ cultural, linguistic, and prior experiences to make instruction relevant\",\r\n            \"rating\": \"E\"\r\n          },\r\n          {\r\n            \"label\": \"Promotes students’ critical and creative thinking through instructional strategies that support access to the curriculum for all students\",\r\n            \"rating\": \"E\"\r\n          },\r\n          {\r\n            \"label\": \"Models instruction, provides clear directions, and checks for understanding to promote reflective self-directed learners who can work independently and collaboratively\",\r\n            \"rating\": \"E\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"B. Creating and Maintaining Effective Environments for Student Learning (TPE 2)\",\r\n        \"subtitle\": \"Student Behavior and Classroom Management\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Designs and maintains a fair and appropriate system of classroom management that fosters an affirming learning community which incorporates student input\",\r\n            \"rating\": \"P\"\r\n          },\r\n          {\r\n            \"label\": \"Creates an effective learning environment that encourages positive interactions among students, reflects diverse perspectives, and is culturally responsive\",\r\n            \"rating\": \"P\"\r\n          },\r\n          {\r\n            \"label\": \"Establishes, maintains, and monitors high expectations for learning and student behavior utilizing procedures and routines that include a variety of strategies to support the full range of students in classroom\",\r\n            \"rating\": \"P\"\r\n          },\r\n          {\r\n            \"label\": \"Supports students assuming responsibility for learning; encourages important behaviors such as being on time, completing assignments, and remaining engaged throughout the lesson\",\r\n            \"rating\": \"P\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"C. Understanding & Organizing Subject Matter for Student Learning (TPE 3&7)\",\r\n        \"subtitle\": \"C-1 Subject Matter Knowledge Content Specific Pedagogy\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Demonstrates subject matter knowledge to deliver evidence-based literacy instruction that integrates content and literacy strategies aligned with California State Standards, English Language Development Standards, curriculum frameworks, educational technology standards, and CA Dyslexia Guidelines\",\r\n            \"rating\": \"E\"\r\n          },\r\n          {\r\n            \"label\": \"Uses and adapts resources, standards-aligned instructional materials, and a range of technology to facilitate students’ equitable access to the curriculum\",\r\n            \"rating\": \"E\"\r\n          },\r\n          {\r\n            \"label\": \"Incorporates asset-based pedagogies, inclusive approaches, and culturally and linguistically affirming and sustaining practices in literacy instruction\",\r\n            \"rating\": \"E\"\r\n          },\r\n          {\r\n            \"label\": \"Demonstrates the ability to use subject-matter knowledge and pedagogical skills to design day-to-day cohesive instruction based on long-term learning goals\",\r\n            \"rating\": \"E\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"C. Understanding & Organizing Subject Matter for Student Learning (TPE 3&7)\",\r\n        \"subtitle\": \"C-2 Effective Literacy Instruction for All Students\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Develops and advances students’ progress in the elements of foundational literacy skills, language, and cognitive skills that support them as they read and write increasingly complex disciplinary texts\",\r\n            \"rating\": \"P\"\r\n          },\r\n          {\r\n            \"label\": \"Engage students in meaning making by building on prior knowledge and using complex literary and informational texts (print, digital, and oral), to develop students’ literal and inferential comprehension\",\r\n            \"rating\": \"P\"\r\n          },\r\n          {\r\n            \"label\": \"Develop students’ oral and written language development by attending to vocabulary knowledge, use of grammatical structures, and effective expression as they write, discuss, present, and use language for a variety of purposes, audiences, and contexts\",\r\n            \"rating\": \"P\"\r\n          },\r\n          {\r\n            \"label\": \"Promote students’ knowledge in content-area by engaging students in literacy instruction that integrates reading, writing, listening, and speaking through printed and digital texts and multimedia, discussions, experimentation, hands-on explorations, and wide and independent reading\",\r\n            \"rating\": \"P\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"D. Planning Instruction and Designing Learning Experiences for All Students (TPE 4)\",\r\n        \"subtitle\": \"Lesson Plans and Delivery\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Uses research-based practices and information about students to design effective unit and lesson plans, reflecting short-term and long-term goals, in collaboration with school colleagues\",\r\n            \"rating\": \"D\"\r\n          },\r\n          {\r\n            \"label\": \"Plans, implements, and differentiates instruction, making effective use of instructional time to maximize learning opportunities and provide access to the curriculum\",\r\n            \"rating\": \"D\"\r\n          },\r\n          {\r\n            \"label\": \"Plans and utilizes a variety of effective instructional approaches and modifies activities and materials to maximize learning\",\r\n            \"rating\": \"D\"\r\n          },\r\n          {\r\n            \"label\": \"Uses digital tools and learning technologies to create new content and provide integrated technology-rich lessons to develop digital literacy, promote digital citizenship, and offer students multiple means to demonstrate their learning\",\r\n            \"rating\": \"D\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"E. Assessing Student Learning (TPE 5)\",\r\n        \"subtitle\": \"Assessment\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Applies knowledge of the purposes and appropriate uses of different informal, formal, and student self-assessments to provide actionable feedback, and when appropriate use scoring rubrics\",\r\n            \"rating\": \"NC\"\r\n          },\r\n          {\r\n            \"label\": \"Monitor students’ progress in literacy development using formative assessment practices, ongoing progress monitoring, and diagnostic techniques to inform instructional decision making\",\r\n            \"rating\": \"NC\"\r\n          },\r\n          {\r\n            \"label\": \"Communicates assessment results promptly to help students and families understand progress toward meeting learning goals\",\r\n            \"rating\": \"NC\"\r\n          },\r\n          {\r\n            \"label\": \"Uses assessment data and student learning plans to set learning goals, differentiate instruction, make accommodations, and/or modify instruction to meet the needs of all students\",\r\n            \"rating\": \"NC\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"F. Developing as a Professional Educator (TPE 6)\",\r\n        \"subtitle\": \"Professionalism\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Reflects on their own teaching practice and content knowledge, using SMART goals and regular collaboration with colleagues to improve student\",\r\n            \"rating\": \"NC\"\r\n          },\r\n          {\r\n            \"label\": \"Models ethical behaviors and demonstrates care, support, acceptance, and fairness towards all students, school personnel, and members of the larger school community\",\r\n            \"rating\": \"NC\"\r\n          },\r\n          {\r\n            \"label\": \"Demonstrates enthusiasm and professional demeanor through competence in oral and written communication, on-time arrival, preparedness, professional appearance, adherence to deadlines, accurately maintained records, and involving other professionals to support student learning when appropriate\",\r\n            \"rating\": \"NC\"\r\n          },\r\n          {\r\n            \"label\": \"Understands and adheres to state and federal laws and procedures pertaining to the education of all students; enacts professional roles and responsibilities as mandated reporters and complies with laws pertaining to the use of social media and other digital platforms both inside and outside of the classroom\",\r\n            \"rating\": \"NC\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"G. Overall Teaching Effectiveness Assessment\",\r\n        \"subtitle\": \"Holistic Assessment of Performance\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"This rating is a holistic assessment of the student teacher’s performance. It is not an average score of categories A-F.\",\r\n            \"rating\": \"D\"\r\n          },\r\n          {\r\n            \"label\": \"A rating of “Not Consistent with Standard Expectations (NC)” in category “G” on the Midterm Evaluation denotes a critical need for improvement and may require a Student Success Action Plan to continue student teaching.\",\r\n            \"rating\": \"D\"\r\n          },\r\n          {\r\n            \"label\": \"A rating of “not consistent with standard expectations for beginning practice (NC)” in Category “G” on the Final Evaluation will result in no credit received for student teaching. The candidate will not be recommended for the credential.\",\r\n            \"rating\": \"D\"\r\n          }\r\n        ]\r\n      }\r\n    ],\r\n    \"strengths\": \"Test\",\r\n    \"growthAreas\": \"Test2\"\r\n  },\r\n  \"applicationType\": \"Fieldwork-Teaching-Midterm\",\r\n  \"universityMentorJSON\": {\r\n    \"sections\": [\r\n      {\r\n        \"title\": \"A. Engaging and Supporting All Students in Learning (TPE 1)\",\r\n        \"subtitle\": \"Building & Maintaining Student Engagement\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Establishes clear and appropriate learning goals and expected outcomes for all students and uses effective activities to meet them\",\r\n            \"rating\": \"P\"\r\n          },\r\n          {\r\n            \"label\": \"Connects subject matter to real-life contexts through active learning experiences by applying knowledge of students’ cultural, linguistic, and prior experiences to make instruction relevant\",\r\n            \"rating\": \"P\"\r\n          },\r\n          {\r\n            \"label\": \"Promotes students’ critical and creative thinking through instructional strategies that support access to the curriculum for all students\",\r\n            \"rating\": \"P\"\r\n          },\r\n          {\r\n            \"label\": \"Models instruction, provides clear directions, and checks for understanding to promote reflective self-directed learners who can work independently and collaboratively\",\r\n            \"rating\": \"P\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"B. Creating and Maintaining Effective Environments for Student Learning (TPE 2)\",\r\n        \"subtitle\": \"Student Behavior and Classroom Management\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Designs and maintains a fair and appropriate system of classroom management that fosters an affirming learning community which incorporates student input\",\r\n            \"rating\": \"D\"\r\n          },\r\n          {\r\n            \"label\": \"Creates an effective learning environment that encourages positive interactions among students, reflects diverse perspectives, and is culturally responsive\",\r\n            \"rating\": \"D\"\r\n          },\r\n          {\r\n            \"label\": \"Establishes, maintains, and monitors high expectations for learning and student behavior utilizing procedures and routines that include a variety of strategies to support the full range of students in classroom\",\r\n            \"rating\": \"D\"\r\n          },\r\n          {\r\n            \"label\": \"Supports students assuming responsibility for learning; encourages important behaviors such as being on time, completing assignments, and remaining engaged throughout the lesson\",\r\n            \"rating\": \"D\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"C. Understanding & Organizing Subject Matter for Student Learning (TPE 3&7)\",\r\n        \"subtitle\": \"C-1 Subject Matter Knowledge Content Specific Pedagogy\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Demonstrates subject matter knowledge to deliver evidence-based literacy instruction that integrates content and literacy strategies aligned with California State Standards, English Language Development Standards, curriculum frameworks, educational technology standards, and CA Dyslexia Guidelines\",\r\n            \"rating\": \"P\"\r\n          },\r\n          {\r\n            \"label\": \"Uses and adapts resources, standards-aligned instructional materials, and a range of technology to facilitate students’ equitable access to the curriculum\",\r\n            \"rating\": \"P\"\r\n          },\r\n          {\r\n            \"label\": \"Incorporates asset-based pedagogies, inclusive approaches, and culturally and linguistically affirming and sustaining practices in literacy instruction\",\r\n            \"rating\": \"P\"\r\n          },\r\n          {\r\n            \"label\": \"Demonstrates the ability to use subject-matter knowledge and pedagogical skills to design day-to-day cohesive instruction based on long-term learning goals\",\r\n            \"rating\": \"P\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"C. Understanding & Organizing Subject Matter for Student Learning (TPE 3&7)\",\r\n        \"subtitle\": \"C-2 Effective Literacy Instruction for All Students\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Develops and advances students’ progress in the elements of foundational literacy skills, language, and cognitive skills that support them as they read and write increasingly complex disciplinary texts\",\r\n            \"rating\": \"NC\"\r\n          },\r\n          {\r\n            \"label\": \"Engage students in meaning making by building on prior knowledge and using complex literary and informational texts (print, digital, and oral), to develop students’ literal and inferential comprehension\",\r\n            \"rating\": \"NC\"\r\n          },\r\n          {\r\n            \"label\": \"Develop students’ oral and written language development by attending to vocabulary knowledge, use of grammatical structures, and effective expression as they write, discuss, present, and use language for a variety of purposes, audiences, and contexts\",\r\n            \"rating\": \"NC\"\r\n          },\r\n          {\r\n            \"label\": \"Promote students’ knowledge in content-area by engaging students in literacy instruction that integrates reading, writing, listening, and speaking through printed and digital texts and multimedia, discussions, experimentation, hands-on explorations, and wide and independent reading\",\r\n            \"rating\": \"NC\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"D. Planning Instruction and Designing Learning Experiences for All Students (TPE 4)\",\r\n        \"subtitle\": \"Lesson Plans and Delivery\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Uses research-based practices and information about students to design effective unit and lesson plans, reflecting short-term and long-term goals, in collaboration with school colleagues\",\r\n            \"rating\": \"D\"\r\n          },\r\n          {\r\n            \"label\": \"Plans, implements, and differentiates instruction, making effective use of instructional time to maximize learning opportunities and provide access to the curriculum\",\r\n            \"rating\": \"D\"\r\n          },\r\n          {\r\n            \"label\": \"Plans and utilizes a variety of effective instructional approaches and modifies activities and materials to maximize learning\",\r\n            \"rating\": \"D\"\r\n          },\r\n          {\r\n            \"label\": \"Uses digital tools and learning technologies to create new content and provide integrated technology-rich lessons to develop digital literacy, promote digital citizenship, and offer students multiple means to demonstrate their learning\",\r\n            \"rating\": \"D\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"E. Assessing Student Learning (TPE 5)\",\r\n        \"subtitle\": \"Assessment\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Applies knowledge of the purposes and appropriate uses of different informal, formal, and student self-assessments to provide actionable feedback, and when appropriate use scoring rubrics\",\r\n            \"rating\": \"N/O\"\r\n          },\r\n          {\r\n            \"label\": \"Monitor students’ progress in literacy development using formative assessment practices, ongoing progress monitoring, and diagnostic techniques to inform instructional decision making\",\r\n            \"rating\": \"N/O\"\r\n          },\r\n          {\r\n            \"label\": \"Communicates assessment results promptly to help students and families understand progress toward meeting learning goals\",\r\n            \"rating\": \"N/O\"\r\n          },\r\n          {\r\n            \"label\": \"Uses assessment data and student learning plans to set learning goals, differentiate instruction, make accommodations, and/or modify instruction to meet the needs of all students\",\r\n            \"rating\": \"N/O\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"F. Developing as a Professional Educator (TPE 6)\",\r\n        \"subtitle\": \"Professionalism\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"Reflects on their own teaching practice and content knowledge, using SMART goals and regular collaboration with colleagues to improve student\",\r\n            \"rating\": \"N/O\"\r\n          },\r\n          {\r\n            \"label\": \"Models ethical behaviors and demonstrates care, support, acceptance, and fairness towards all students, school personnel, and members of the larger school community\",\r\n            \"rating\": \"N/O\"\r\n          },\r\n          {\r\n            \"label\": \"Demonstrates enthusiasm and professional demeanor through competence in oral and written communication, on-time arrival, preparedness, professional appearance, adherence to deadlines, accurately maintained records, and involving other professionals to support student learning when appropriate\",\r\n            \"rating\": \"N/O\"\r\n          },\r\n          {\r\n            \"label\": \"Understands and adheres to state and federal laws and procedures pertaining to the education of all students; enacts professional roles and responsibilities as mandated reporters and complies with laws pertaining to the use of social media and other digital platforms both inside and outside of the classroom\",\r\n            \"rating\": \"N/O\"\r\n          }\r\n        ]\r\n      },\r\n      {\r\n        \"title\": \"G. Overall Teaching Effectiveness Assessment\",\r\n        \"subtitle\": \"Holistic Assessment of Performance\",\r\n        \"indicators\": [\r\n          {\r\n            \"label\": \"This rating is a holistic assessment of the student teacher’s performance. It is not an average score of categories A-F.\",\r\n            \"rating\": \"E\"\r\n          },\r\n          {\r\n            \"label\": \"A rating of “Not Consistent with Standard Expectations (NC)” in category “G” on the Midterm Evaluation denotes a critical need for improvement and may require a Student Success Action Plan to continue student teaching.\",\r\n            \"rating\": \"E\"\r\n          },\r\n          {\r\n            \"label\": \"A rating of “not consistent with standard expectations for beginning practice (NC)” in Category “G” on the Final Evaluation will result in no credit received for student teaching. The candidate will not be recommended for the credential.\",\r\n            \"rating\": \"E\"\r\n          }\r\n        ]\r\n      }\r\n    ],\r\n    \"strengths\": \"Test1\",\r\n    \"growthAreas\": \"Test12\",\r\n    \"info\": {\r\n      \"date\": \"August 13, 2026\",\r\n      \"studentName\": \"Gabriela Kochanowski\",\r\n      \"universityMentorName\": \"Test1\"\r\n    }\r\n  }\r\n}";
            //input.evaluationjson = "{\r\n  \"evaluationJSON\": {\r\n    \"reviewType\": {\r\n      \"formative\": \"Formative Review123\",\r\n      \"summative\": \"Summative Reviewtttt\"\r\n    },\r\n    \"meta\": {\r\n      \"teacherCandidate\": \"Teacher Candidate\",\r\n      \"semester\": \"Student Teaching Semester\",\r\n      \"courseEnrolled\": \"Course Enrolled\",\r\n      \"cooperatingTeacher\": \"Cooperating Teacher\",\r\n      \"universitySupervisor\": \"University Supervisor\",\r\n      \"schoolSite\": \"School Site/District\"\r\n    },\r\n    \"tpeSections\": [\r\n      {\r\n        \"keyTitle\": \"TPE 1\",\r\n        \"title\": \"TPE 1: Engaging and Supporting All Students\",\r\n        \"indicators\": [\r\n          {\r\n            \"title\": \"Universal TPE 1 & CRSP Tenets\",\r\n            \"description\": \"Beginning teachers know and care about their students to engage them in learning. They connect learning to students’ prior knowledge, backgrounds, life experiences, and interests. They connect subject matter to meaningful, real-life contexts. Teachers use various instructional strategies, resources, and technologies to meet the diverse learning needs of students. They promote critical thinking through inquiry, problem solving, and reflection. They monitor student learning and adjust instruction while teaching. Beginning teachers foster interpersonal dialogue, relationships of trust and sense of community among students and adults. They use appropriate cultural referents and ability identities to co-construct knowledge and foster the development of the whole child/youth. They promote non-essentializing conceptualizations of culture, including multiple cultural dimensions (e.g., racial, ethnic, class, religious, ability, & gender).\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 1 Mild/Moderate Support Needs\",\r\n            \"description\": \"Beginning special education teachers demonstrate student TPEs to support students with Mild Moderate Support Needs. They establish a consistent, organized, and respectful learning environment. They provide positive and constructive feedback to guide students’ learning and behavior.\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 1 Extensive Support Needs\",\r\n            \"description\": \"Beginning special education teachers demonstrate TPEs to support students with Extensive Support Needs. They establish a consistent, organized, and respectful learning environment. They provide positive and constructive feedback to guide students’ learning and behavior.\"\r\n          }\r\n        ],\r\n        \"evidence\": {\r\n          \"interview\": false,\r\n          \"journalReflection\": true,\r\n          \"sampleExample\": false,\r\n          \"directObservation\": false,\r\n          \"other\": false,\r\n          \"otherText\": \"\"\r\n        },\r\n        \"strengths\": \"Areas of strength in TPE: 1\",\r\n        \"growthAreas\": \"Any areas needed for further development and supporting evidence. Required for any “Not a passing score”: 1\"\r\n      },\r\n      {\r\n        \"keyTitle\": \"TPE 2\",\r\n        \"title\": \"TPE 2: Creating and Maintaining Effective Environments\",\r\n        \"indicators\": [\r\n          {\r\n            \"title\": \"Universal TPE 2 & CRSP Tenets\",\r\n            \"description\": \"Beginning teachers promote social development and responsibility within a caring community where each student is treated fairly and respectfully. They create physical or virtual learning environments that promote student learning, reflect diversity, and encourage constructive and productive interactions among students. They establish and maintain learning environments that are physically, intellectually, and emotionally safe. Teachers create a rigorous learning environment with high expectations and appropriate support for all students. Teachers develop, communicate, and maintain high standards for individual and group behavior. They employ classroom routines, procedures, norms, and support for positive behavior to ensure a climate in which all students can learn. They use instructional time to optimize learning. Beginning teachers recognize the positionality of the instructor and acknowledge power dynamics in the classroom and with families. They communicate high expectations and provide appropriate contextualized support to reach these.\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 2 Mild Moderate Support Needs TPEs\",\r\n            \"description\": \"Beginning special education teachers demonstrate TPEs that support creating a positive environment for students with Mild Moderate Support Needs. They teach social behaviors for student success. They conduct functional behavioral assessments to develop individual student behavior support plans.\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 2 Extensive Support Needs TPEs\",\r\n            \"description\": \"Beginning special education teachers demonstrate TPEs that support creating a positive environment for students with Extensive Support Needs. They teach social behaviors for student success. They conduct functional behavioral assessments to develop individual student behavior support plans.\"\r\n          }\r\n        ],\r\n        \"evidence\": {\r\n          \"interview\": false,\r\n          \"journalReflection\": false,\r\n          \"sampleExample\": true,\r\n          \"directObservation\": false,\r\n          \"other\": false,\r\n          \"otherText\": \"\"\r\n        },\r\n        \"strengths\": \"Areas of strength in TPE: 2\",\r\n        \"growthAreas\": \"Any areas needed for further development and supporting evidence. Required for any “Not a passing score”: 2\"\r\n      },\r\n      {\r\n        \"keyTitle\": \"TPE 3\",\r\n        \"title\": \"TPE 3: Understanding and Organizing Subject Matter Application of Content\",\r\n        \"indicators\": [\r\n          {\r\n            \"title\": \"Universal TPE 3 & CRSP Tenets\",\r\n            \"description\": \"Beginning teachers exhibit in-depth working knowledge of subject matter, academic content standards, and curriculum frameworks. They apply knowledge of student development and proficiencies to ensure student understanding of content. They organize curriculum to facilitate students' understanding of the subject matter. Teachers utilize instructional strategies that are appropriate to the subject matter. They use and adapt resources, technologies, and standards-aligned instructional materials, including adopted materials, to make subject matter accessible to all students. They address the needs of English learners and students with special needs to provide equitable access to the content. Beginning teachers challenge and modify the ‘core curriculum’ by recognizing the legitimacy of cultural heritages of multiple racial, ethnic, class, religious, ability, and gender groups as worthy content to be taught in the formal curriculum. They enact anti-bias pedagogy and assets-based perspectives of families and communities and identify counternarratives. They promote student development of skills to become social critics and decision-makers. They actively engage in critical self-work to identify and address aspects of privilege and marginalization in one’s own multiple social identities.\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 3 Mild Moderate Support Needs TPEs\",\r\n            \"description\": \"Beginning special education teachers demonstrate content TPEs to support students with Mild Moderate Support Needs. They identify and prioritize long- and short-term learning goals. They adapt curriculum tasks and materials for specific learning goals. They teach students to maintain and generalize new learning across time and settings. They provide positive and constructive feedback to guide students’ learning and behavior.\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 3 Extensive Support Needs TPEs\",\r\n            \"description\": \"Beginning special education teachers demonstrate content TPEs to support students with Extensive Support Needs. They identify and prioritize long- and short-term learning goals. They adapt curriculum tasks and materials for specific learning goals. They teach students to maintain and generalize new learning across time and settings. They provide positive and constructive feedback to guide students’ learning and behavior.\"\r\n          }\r\n        ],\r\n        \"evidence\": {\r\n          \"interview\": true,\r\n          \"journalReflection\": false,\r\n          \"sampleExample\": false,\r\n          \"directObservation\": false,\r\n          \"other\": false,\r\n          \"otherText\": \"\"\r\n        },\r\n        \"strengths\": \"Areas of strength in TPE: 3\",\r\n        \"growthAreas\": \"Any areas needed for further development and supporting evidence. Required for any “Not a passing score”: 3\"\r\n      },\r\n      {\r\n        \"keyTitle\": \"TPE 4\",\r\n        \"title\": \"TPE 4: Planning Instruction and Designing Lesson Experiences for All Students\",\r\n        \"indicators\": [\r\n          {\r\n            \"title\": \"Universal TPE 4 & CRSP Tenets\",\r\n            \"description\": \"Beginning teachers apply knowledge of the purposes, characteristics, and uses of different types of assessments. They collect and analyze assessment data from a variety of sources and use those data to inform instruction. They review data, both individually and with colleagues, to monitor student learning. Teachers use assessment data to establish learning goals and to plan, differentiate, and modify instruction. They involve all students in self-assessment, goal setting and monitoring progress. Teachers use available technologies to assist in assessment, analysis, and communication of student learning. They use assessment information to share timely and comprehensible feedback with students and their families. Beginning teachers use instructional strategies connected to diverse ways of learning and students’ interests and backgrounds. They create curricular content that invites students to explore complex identities and discuss histories critically through analysis of power, opportunity, denial, and privilege.\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 4 Mild Moderate Support Needs TPEs\",\r\n            \"description\": \"Beginning special education teachers demonstrate instructional TPEs to support students with Mild Moderate Support Needs. They systematically design instruction toward a specific goal. They teach cognitive and metacognitive strategies to support learning and independence. They provide scaffolded supports, use explicit instruction, and use flexible grouping. They use strategies to promote active student engagement and assistive and instructional technologies. They provide intensive instruction and positive and constructive feedback to guide students’ learning and behavior.\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 4 Extensive Support Needs TPEs\",\r\n            \"description\": \"Beginning special education teachers demonstrate instructional TPEs to support students with Extensive Support Needs. They systematically design instruction toward a specific goal. They teach cognitive and metacognitive strategies to support learning and independence. They provide scaffolded supports, use explicit instruction, and use flexible grouping. They use strategies to promote active student engagement and assistive and instructional technologies. They provide intensive instruction and positive and constructive feedback to guide students’ learning and behavior.\"\r\n          }\r\n        ],\r\n        \"evidence\": {\r\n          \"interview\": true,\r\n          \"journalReflection\": false,\r\n          \"sampleExample\": false,\r\n          \"directObservation\": false,\r\n          \"other\": false,\r\n          \"otherText\": \"\"\r\n        },\r\n        \"strengths\": \"Areas of strength in TPE: 4\",\r\n        \"growthAreas\": \"Any areas needed for further development and supporting evidence. Required for any “Not a passing score”: 4\"\r\n      },\r\n      {\r\n        \"keyTitle\": \"TPE 5\",\r\n        \"title\": \"TPE 5: Assessing Student Learning\",\r\n        \"indicators\": [\r\n          {\r\n            \"title\": \"Universal TPE 5 & CRSP Tenets\",\r\n            \"description\": \"Beginning teachers use knowledge of students' academic readiness, language proficiency, cultural background, and individual development to plan instruction. They establish and articulate goals for student learning. They develop and sequence long-term and short-term instructional plans to support student learning. Teachers plan instruction that incorporates appropriate strategies to meet the diverse learning needs of all students. They modify and adapt instructional plans to meet the assessed learning needs of all students. Beginning teachers use intersectional knowledge of the students to make learning more appropriate and effective. They build bridges of meaningfulness between student’s lived socio-cultural realities, academic content, and educator expectations. They promote cultural identity and heritage through affirmation of student language acquisition experiences (language experiences in classroom to include non-standard English, sign language, augmentative/alternative communication, languages other than English etc.).\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 5 Mild Moderate Support Needs TPEs\",\r\n            \"description\": \"Beginning special education teachers demonstrate assessing TPEs to support students with Mild Moderate Support Needs. They use multiple sources of information to develop a comprehensive understanding of a student’s strengths and needs. They interpret and communicate assessment information with stakeholders to collaboratively design and implement educational programs. They use student assessments, analyze instructional practices, and make necessary adjustments that improve student outcomes.\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 5 Extensive Support Needs TPEs\",\r\n            \"description\": \"Beginning special education teachers demonstrate assessing TPEs to support students with Extensive Support Needs. They use multiple sources of information to develop a comprehensive understanding of a student’s strengths and needs. They interpret and communicate assessment information with stakeholders to collaboratively design and implement educational programs. They use student assessments, analyze instructional practices, and make necessary adjustments that improve student outcomes.\"\r\n          }\r\n        ],\r\n        \"evidence\": {\r\n          \"interview\": false,\r\n          \"journalReflection\": false,\r\n          \"sampleExample\": true,\r\n          \"directObservation\": false,\r\n          \"other\": false,\r\n          \"otherText\": \"\"\r\n        },\r\n        \"strengths\": \"Areas of strength in TPE: 5\",\r\n        \"growthAreas\": \"Any areas needed for further development and supporting evidence. Required for any “Not a passing score”: 5\"\r\n      },\r\n      {\r\n        \"keyTitle\": \"TPE 6\",\r\n        \"title\": \"TPE 6: Developing as a Professional Educator (Collaboration)\",\r\n        \"indicators\": [\r\n          {\r\n            \"title\": \"Universal TPE 6 & CRSP Tenets\",\r\n            \"description\": \"Beginning teachers reflect on their teaching practice to support student learning. They establish professional goals and engage in continuous and purposeful professional growth and development. They collaborate with colleagues and engage in the broader professional community to support teacher and student learning. Teachers learn about and work with families to support student learning. They engage local communities in support of the instructional program. They manage professional responsibilities to maintain motivation and commitment to all students. Teachers demonstrate professional responsibility, integrity, and ethical conduct. Beginning teachers foster interpersonal dialogue, relationships of trust and sense of community among students and adults. They recognize the positionality of the instructor and acknowledge power dynamics in the classroom and with families. They actively promote allyship to advance the culture of inclusion through intentional, positive, and conscious efforts that benefit people. They enact anti-bias pedagogy and assets-based perspectives of families and communities and identify counternarratives. They promote student development of skills to advance systemic reforms (e.g., change in policies, laws, and institutional practices). They actively engage in critical self-work to identify and address aspects of privilege and marginalization in one’s own multiple social identities.\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 6 Mild Moderate Support Needs TPEs\",\r\n            \"description\": \"Beginning special education teachers demonstrate professional TPEs to support students with Mild Moderate Support Needs. They collaborate with professionals to increase student success. They organize and facilitate effective meetings with professionals and families. They collaborate with families to support student learning and secure needed services.\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 6 Extensive Support Needs TPEs\",\r\n            \"description\": \"Beginning special education teachers demonstrate professional TPEs to support students with Extensive Support Needs. They collaborate with professionals to increase student success. They organize and facilitate effective meetings with professionals and families. They collaborate with families to support student learning and secure needed services.\"\r\n          }\r\n        ],\r\n        \"evidence\": {\r\n          \"interview\": false,\r\n          \"journalReflection\": false,\r\n          \"sampleExample\": false,\r\n          \"directObservation\": true,\r\n          \"other\": false,\r\n          \"otherText\": \"\"\r\n        },\r\n        \"strengths\": \"Areas of strength in TPE: 6\",\r\n        \"growthAreas\": \"Any areas needed for further development and supporting evidence. Required for any “Not a passing score”: 6\"\r\n      },\r\n      {\r\n        \"keyTitle\": \"TPE 7\",\r\n        \"title\": \"TPE 7: Effective Literacy Instruction for Students with Disabilities\",\r\n        \"indicators\": [\r\n          {\r\n            \"title\": \"Universal TPE 7 & CRSP Tenets\",\r\n            \"description\": \"Beginning teachers plan and implement evidence-based literacy instruction grounded in academic standards, Universal Design for Learning, MTSS and the California Dyslexia Guidelines. They teach foundational skills as needed in print concepts, phonological and phonemic awareness, phonics, decoding and encoding, and fluency through instruction that is structured, explicit and systematic to advance students’ progress in reading and writing increasingly complex texts. They promote oral and written academic language development and provide instruction in ELD. Beginning teachers also monitor student progress in literacy development, including screening for reading/writing difficulties and risk for dyslexia, and they implement appropriate interventions in literacy. Beginning teachers use intersectional knowledge of the students to make learning more appropriate and effective. They build bridges of meaningfulness between student’s lived socio-cultural realities, academic content, and educator expectations. They promote cultural identity and heritage through affirmation of student language acquisition experiences (language experiences in classroom to include non-standard English, sign language, augmentative/alternative communication, languages other than English etc.).\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 7 Mild Moderate Support Needs TPEs\",\r\n            \"description\": \"Beginning special education teachers demonstrate literacy TPEs to support students with Mild Moderate Support Needs. They apply their knowledge of students’ assets and learning needs and use the results of screening and assessment data to plan and implement Tier 2 and Tier 3 interventions. They collaborate with multidisciplinary teams, including families and other service providers, when determining eligibility for special education services, interpreting assessment results, and planning necessary adaptations and supplemental instruction for students with dyslexia and other disabilities that impact literacy development. Beginning teachers plan lessons that ensure access to grade-level literacy activities and use assistive technology and Augmentative and Alternative Communication as needed to support literacy.\"\r\n          },\r\n          {\r\n            \"title\": \"TPE 7 Extensive Support Needs TPEs\",\r\n            \"description\": \"Beginning special education teachers demonstrate literacy TPEs to support students with Extensive Support Needs. They apply their knowledge of students’ assets and learning needs and use the results of screening and assessment data to plan and implement Tier 2 and Tier 3 literacy interventions. They collaborate with multidisciplinary teams, including families and other service providers such as occupational therapists, physical therapists, DHH and VI teachers when determining eligibility for special education services, interpreting assessment results, providing day-to-day supplemental instruction in literacy. They collaborate with specialists to address multiple means of communication (e.g., PECS [Picture Exchange Communication System], voice output devices) and facilitate the use of multiple communication strategies to support the teaching of literacy, including American Sign Language, assistive technology, Augmentative and Alternative Communication (AAC), signed terms, eye gaze, vocalizations, or other modes as appropriate.\"\r\n          }\r\n        ],\r\n        \"evidence\": {\r\n          \"interview\": false,\r\n          \"journalReflection\": false,\r\n          \"sampleExample\": true,\r\n          \"directObservation\": false,\r\n          \"other\": false,\r\n          \"otherText\": \"\"\r\n        },\r\n        \"strengths\": \"Areas of strength in TPE: 7\",\r\n        \"growthAreas\": \"Any areas needed for further development and supporting evidence. Required for any “Not a passing score”: 7\"\r\n      }\r\n    ],\r\n    \"dispositions\": [\r\n      {\r\n        \"label\": \"Demonstrates the belief that all children can learn. (U 4.5, MM 3.2, ESN 3.4)\",\r\n        \"score\": \"1\"\r\n      },\r\n      {\r\n        \"label\": \"Values equity and fairness. (U 3.1, 6.7)\",\r\n        \"score\": \"2\"\r\n      },\r\n      {\r\n        \"label\": \"Values diversity and considers all points of view. (MM 6.2, ESN 6.3)\",\r\n        \"score\": \"2\"\r\n      },\r\n      {\r\n        \"label\": \"Exhibits dependability, initiative, enthusiasm, and follow-through. (U 6.1, 6.3)\",\r\n        \"score\": \"3\"\r\n      },\r\n      {\r\n        \"label\": \"Demonstrates appropriate self-esteem, flexibility, resourcefulness, and positive response to constructive feedback. (U 6.3)\",\r\n        \"score\": \"4\"\r\n      },\r\n      {\r\n        \"label\": \"Engages in socially appropriate and professionally ethical behavior. (U 6.5, 6.6)\",\r\n        \"score\": \"3\"\r\n      },\r\n      {\r\n        \"label\": \"Collaborates and interacts professionally with colleagues, parents, staff, and the community. (MM 2.4, 4.6 ESN 2.4, 4.7)\",\r\n        \"score\": \"2\"\r\n      },\r\n      {\r\n        \"label\": \"Improves professional practice through continuous reflection. (U 6.1, MM 3.1)\",\r\n        \"score\": \"1\"\r\n      },\r\n      {\r\n        \"label\": \"Pursues opportunities to contribute and grow professionally. (U 6.1, 6.2, 6.3)\",\r\n        \"score\": \"2\"\r\n      },\r\n      {\r\n        \"label\": \"Honors legal/professional obligations and follows regulations. (U 6.7, MM 6.3, 6.6, ESN 6.4)\",\r\n        \"score\": \"3\"\r\n      },\r\n      {\r\n        \"label\": \"Reflects on how teacher biases and student individual culture and other identities impact behavior and teacher interpretation of behavior. (U 6.7, MM 6.3, ESN 6.4)\",\r\n        \"score\": \"4\"\r\n      },\r\n      {\r\n        \"label\": \"Considers students’ culture as an asset to learning, uses an asset-based perspective of students’ families. (MM 5.3, ESN 5.4)\",\r\n        \"score\": \"3\"\r\n      },\r\n      {\r\n        \"label\": \"Recognizes the positionality of the teacher and acknowledges power dynamics in the classroom and with families. (U 1.1, 1.2, 1.5, 1.6)\",\r\n        \"score\": \"2\"\r\n      }\r\n    ],\r\n    \"signatures\": {\r\n      \"candidate\": \"Teacher Candidate Signature/Name 1\",\r\n      \"candidateDate\": \"08/18/2026\",\r\n      \"teacher\": \"Teacher Candidate Signature/Name 2\",\r\n      \"teacherDate\": \"08/25/2026\",\r\n      \"supervisor\": \"Teacher Candidate Signature/Name 3\",\r\n      \"supervisorDate\": \"08/27/2026\"\r\n    }\r\n  }\r\n}\r\n ";
            //input.evaluationjson = "{\r\n  \"reviewType\": {\r\n    \"formative\": \"Test\",\r\n    \"summative\": \"Test\"\r\n  },\r\n  \"meta\": {\r\n    \"teacherCandidate\": \"Test\",\r\n    \"semester\": \"Test\",\r\n    \"courseEnrolled\": \"Test\",\r\n    \"cooperatingTeacher\": \"Test\",\r\n    \"universitySupervisor\": \"Test\",\r\n    \"schoolSite\": \"Test\"\r\n  },\r\n  \"tpeSections\": [\r\n    {\r\n      \"keyTitle\": \"TPE 1\",\r\n      \"title\": \"TPE 1: Engaging and Supporting All Students\",\r\n      \"indicators\": [\r\n        {\r\n          \"title\": \"Universal TPE 1 & CRSP Tenets\",\r\n          \"description\": \"Beginning teachers know and care about their students to engage them in learning. They connect learning to students’ prior knowledge, backgrounds, life experiences, and interests. They connect subject matter to meaningful, real-life contexts. Teachers use various instructional strategies, resources, and technologies to meet the diverse learning needs of students.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 1 Mild/Moderate Support Needs\",\r\n          \"description\": \"Beginning special education teachers demonstrate student TPEs to support students with Mild Moderate Support Needs. They establish a consistent, organized, and respectful learning environment. They provide positive and constructive feedback to guide students’ learning and behavior.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 1 Extensive Support Needs\",\r\n          \"description\": \"Beginning special education teachers demonstrate TPEs to support students with Extensive Support Needs. They establish a consistent, organized, and respectful learning environment. They provide positive and constructive feedback to guide students’ learning and behavior.\"\r\n        }\r\n      ],\r\n      \"evidence\": {\r\n        \"interview\": false,\r\n        \"journalReflection\": true,\r\n        \"sampleExample\": false,\r\n        \"directObservation\": false,\r\n        \"other\": false,\r\n        \"otherText\": \"\"\r\n      },\r\n      \"strengths\": \"Test\",\r\n      \"growthAreas\": \"Test\"\r\n    },\r\n    {\r\n      \"keyTitle\": \"TPE 2\",\r\n      \"title\": \"TPE 2: Creating and Maintaining Effective Environments\",\r\n      \"indicators\": [\r\n        {\r\n          \"title\": \"Universal TPE 2 & CRSP Tenets\",\r\n          \"description\": \"Beginning teachers promote social development and responsibility within a caring community where each student is treated fairly and respectfully.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 2 Mild Moderate Support Needs TPEs\",\r\n          \"description\": \"Beginning special education teachers demonstrate TPEs that support creating a positive environment for students with Mild Moderate Support Needs.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 2 Extensive Support Needs TPEs\",\r\n          \"description\": \"Beginning special education teachers demonstrate TPEs that support creating a positive environment for students with Extensive Support Needs.\"\r\n        }\r\n      ],\r\n      \"evidence\": {\r\n        \"interview\": true,\r\n        \"journalReflection\": false,\r\n        \"sampleExample\": false,\r\n        \"directObservation\": false,\r\n        \"other\": false,\r\n        \"otherText\": \"\"\r\n      },\r\n      \"strengths\": \"Test\",\r\n      \"growthAreas\": \"Test\"\r\n    },\r\n    {\r\n      \"keyTitle\": \"TPE 3\",\r\n      \"title\": \"TPE 3: Understanding and Organizing Subject Matter Application of Content\",\r\n      \"indicators\": [\r\n        {\r\n          \"title\": \"Universal TPE 3 & CRSP Tenets\",\r\n          \"description\": \"Beginning teachers exhibit in-depth working knowledge of subject matter, academic content standards, and curriculum frameworks.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 3 Mild Moderate Support Needs TPEs\",\r\n          \"description\": \"Beginning special education teachers demonstrate content TPEs to support students with Mild Moderate Support Needs.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 3 Extensive Support Needs TPEs\",\r\n          \"description\": \"Beginning special education teachers demonstrate content TPEs to support students with Extensive Support Needs.\"\r\n        }\r\n      ],\r\n      \"evidence\": {\r\n        \"interview\": false,\r\n        \"journalReflection\": false,\r\n        \"sampleExample\": true,\r\n        \"directObservation\": false,\r\n        \"other\": false,\r\n        \"otherText\": \"\"\r\n      },\r\n      \"strengths\": \"Test\",\r\n      \"growthAreas\": \"Test\"\r\n    },\r\n    {\r\n      \"keyTitle\": \"TPE 4\",\r\n      \"title\": \"TPE 4: Planning Instruction and Designing Lesson Experiences for All Students\",\r\n      \"indicators\": [\r\n        {\r\n          \"title\": \"Universal TPE 4 & CRSP Tenets\",\r\n          \"description\": \"Beginning teachers apply knowledge of the purposes, characteristics, and uses of different types of assessments.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 4 Mild Moderate Support Needs TPEs\",\r\n          \"description\": \"Beginning special education teachers demonstrate instructional TPEs to support students with Mild Moderate Support Needs.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 4 Extensive Support Needs TPEs\",\r\n          \"description\": \"Beginning special education teachers demonstrate instructional TPEs to support students with Extensive Support Needs.\"\r\n        }\r\n      ],\r\n      \"evidence\": {\r\n        \"interview\": true,\r\n        \"journalReflection\": false,\r\n        \"sampleExample\": false,\r\n        \"directObservation\": false,\r\n        \"other\": false,\r\n        \"otherText\": \"\"\r\n      },\r\n      \"strengths\": \"Test\",\r\n      \"growthAreas\": \"Test\"\r\n    },\r\n    {\r\n      \"keyTitle\": \"TPE 5\",\r\n      \"title\": \"TPE 5: Assessing Student Learning\",\r\n      \"indicators\": [\r\n        {\r\n          \"title\": \"Universal TPE 5 & CRSP Tenets\",\r\n          \"description\": \"Beginning teachers use knowledge of students' academic readiness, language proficiency, cultural background, and individual development to plan instruction.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 5 Mild Moderate Support Needs TPEs\",\r\n          \"description\": \"Beginning special education teachers demonstrate assessing TPEs to support students with Mild Moderate Support Needs.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 5 Extensive Support Needs TPEs\",\r\n          \"description\": \"Beginning special education teachers demonstrate assessing TPEs to support students with Extensive Support Needs.\"\r\n        }\r\n      ],\r\n      \"evidence\": {\r\n        \"interview\": false,\r\n        \"journalReflection\": true,\r\n        \"sampleExample\": false,\r\n        \"directObservation\": false,\r\n        \"other\": false,\r\n        \"otherText\": \"\"\r\n      },\r\n      \"strengths\": \"Test\",\r\n      \"growthAreas\": \"Test\"\r\n    },\r\n    {\r\n      \"keyTitle\": \"TPE 6\",\r\n      \"title\": \"TPE 6: Developing as a Professional Educator (Collaboration)\",\r\n      \"indicators\": [\r\n        {\r\n          \"title\": \"Universal TPE 6 & CRSP Tenets\",\r\n          \"description\": \"Beginning teachers reflect on their teaching practice to support student learning.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 6 Mild Moderate Support Needs TPEs\",\r\n          \"description\": \"Beginning special education teachers demonstrate professional TPEs to support students with Mild Moderate Support Needs.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 6 Extensive Support Needs TPEs\",\r\n          \"description\": \"Beginning special education teachers demonstrate professional TPEs to support students with Extensive Support Needs.\"\r\n        }\r\n      ],\r\n      \"evidence\": {\r\n        \"interview\": false,\r\n        \"journalReflection\": false,\r\n        \"sampleExample\": true,\r\n        \"directObservation\": false,\r\n        \"other\": false,\r\n        \"otherText\": \"\"\r\n      },\r\n      \"strengths\": \"Test\",\r\n      \"growthAreas\": \"Test\"\r\n    },\r\n    {\r\n      \"keyTitle\": \"TPE 7\",\r\n      \"title\": \"TPE 7: Effective Literacy Instruction for Students with Disabilities\",\r\n      \"indicators\": [\r\n        {\r\n          \"title\": \"Universal TPE 7 & CRSP Tenets\",\r\n          \"description\": \"Beginning teachers plan and implement evidence-based literacy instruction grounded in academic standards, Universal Design for Learning, MTSS and the California Dyslexia Guidelines.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 7 Mild Moderate Support Needs TPEs\",\r\n          \"description\": \"Beginning special education teachers demonstrate literacy TPEs to support students with Mild Moderate Support Needs.\"\r\n        },\r\n        {\r\n          \"title\": \"TPE 7 Extensive Support Needs TPEs\",\r\n          \"description\": \"Beginning special education teachers demonstrate literacy TPEs to support students with Extensive Support Needs.\"\r\n        }\r\n      ],\r\n      \"evidence\": {\r\n        \"interview\": true,\r\n        \"journalReflection\": false,\r\n        \"sampleExample\": false,\r\n        \"directObservation\": false,\r\n        \"other\": false,\r\n        \"otherText\": \"\"\r\n      },\r\n      \"strengths\": \"Test\",\r\n      \"growthAreas\": \"Test\"\r\n    }\r\n  ],\r\n  \"dispositions\": [\r\n    {\r\n      \"label\": \"Demonstrates the belief that all children can learn. (U 4.5, MM 3.2, ESN 3.4)\",\r\n      \"score\": \"1\"\r\n    },\r\n    {\r\n      \"label\": \"Values equity and fairness. (U 3.1, 6.7)\",\r\n      \"score\": \"1\"\r\n    },\r\n    {\r\n      \"label\": \"Values diversity and considers all points of view. (MM 6.2, ESN 6.3)\",\r\n      \"score\": \"1\"\r\n    },\r\n    {\r\n      \"label\": \"Exhibits dependability, initiative, enthusiasm, and follow-through. (U 6.1, 6.3)\",\r\n      \"score\": \"1\"\r\n    },\r\n    {\r\n      \"label\": \"Demonstrates appropriate self-esteem, flexibility, resourcefulness, and positive response to constructive feedback. (U 6.3)\",\r\n      \"score\": \"1\"\r\n    },\r\n    {\r\n      \"label\": \"Engages in socially appropriate and professionally ethical behavior. (U 6.5, 6.6)\",\r\n      \"score\": \"1\"\r\n    },\r\n    {\r\n      \"label\": \"Collaborates and interacts professionally with colleagues, parents, staff, and the community. (MM 2.4, 4.6 ESN 2.4, 4.7)\",\r\n      \"score\": \"1\"\r\n    },\r\n    {\r\n      \"label\": \"Improves professional practice through continuous reflection. (U 6.1, MM 3.1)\",\r\n      \"score\": \"1\"\r\n    },\r\n    {\r\n      \"label\": \"Pursues opportunities to contribute and grow professionally. (U 6.1, 6.2, 6.3)\",\r\n      \"score\": \"1\"\r\n    },\r\n    {\r\n      \"label\": \"Honors legal/professional obligations and follows regulations. (U 6.7, MM 6.3, 6.6, ESN 6.4)\",\r\n      \"score\": \"1\"\r\n    },\r\n    {\r\n      \"label\": \"Reflects on how teacher biases and student individual culture and other identities impact behavior and teacher interpretation of behavior. (U 6.7, MM 6.3, ESN 6.4)\",\r\n      \"score\": \"1\"\r\n    },\r\n    {\r\n      \"label\": \"Considers students’ culture as an asset to learning, uses an asset-based perspective of students’ families. (MM 5.3, ESN 5.4)\",\r\n      \"score\": \"1\"\r\n    },\r\n    {\r\n      \"label\": \"Recognizes the positionality of the teacher and acknowledges power dynamics in the classroom and with families. (U 1.1, 1.2, 1.5, 1.6)\",\r\n      \"score\": \"1\"\r\n    }\r\n  ],\r\n  \"signatures\": {\r\n    \"candidate\": \"Test\",\r\n    \"candidateDate\": \"08/04/2026\",\r\n    \"teacher\": \"Test\",\r\n    \"teacherDate\": \"08/16/2026\",\r\n    \"supervisor\": \"Test\",\r\n    \"supervisorDate\": \"08/12/2026\"\r\n  },\r\n  \"info\": {\r\n    \"date\": \"August 13, 2026\",\r\n    \"studentName\": \"Selena Lopez\",\r\n    \"cooperatingTeacherName\": \"Jay Test\"\r\n  }\r\n}";
            if (input.programName == "ESCP")
            {
                try
                {
                    input.evaluationjson = input.evaluationjson.Replace("+", " ");
                    JObject schema = JObject.Parse(input.evaluationjson);

                    if (schema == null)
                    {
                        throw new Exception("evaluationJSON object was not found in the JSON.");
                    }
                    evaluationTemplateBody = GetDocumentBodyTemplate("ESCPStudentTeachingEvaluationTemplate.html");
                    evaluationTemplateBody = evaluationTemplateBody.Replace("[[logoPath]]", logoPath);

                    //Review Type
                    JObject reviewType = schema["reviewType"] as JObject;
                    if (reviewType != null)
                    {
                        string formative = reviewType["formative"]?.ToString() ?? "";
                        string summative = reviewType["summative"]?.ToString() ?? "";
                        evaluationTemplateBody = evaluationTemplateBody.Replace("[[FormativeReview]]",formative);
                        evaluationTemplateBody =evaluationTemplateBody.Replace("[[SummativeReview]]",summative);
                    }
                    //Meta
                    JObject meta = schema["meta"] as JObject;
                    if (meta != null)
                    {
                        evaluationTemplateBody = evaluationTemplateBody.Replace("[[TeacherCandidate]]",meta["teacherCandidate"]?.ToString() ?? "");
                        evaluationTemplateBody = evaluationTemplateBody.Replace("[[StudentTeachingSemester]]",meta["semester"]?.ToString() ?? "");
                        evaluationTemplateBody = evaluationTemplateBody.Replace("[[CourseEnrolled]]",meta["courseEnrolled"]?.ToString() ?? "");
                        evaluationTemplateBody = evaluationTemplateBody.Replace("[[CooperatingTeacher]]",meta["cooperatingTeacher"]?.ToString() ?? "");
                        evaluationTemplateBody = evaluationTemplateBody.Replace("[[UniversitySupervisor]]", meta["universitySupervisor"]?.ToString() ?? "");
                        evaluationTemplateBody = evaluationTemplateBody.Replace("[[SchoolSite]]", meta["schoolSite"]?.ToString() ?? "");
                    }
                    // TPE SECTIONS
                    JArray tpeSections = schema["tpeSections"] as JArray;
                    if (tpeSections != null)
                    {
                        for (int i = 0; i < tpeSections.Count; i++)
                        {
                            JObject tpeSection = tpeSections[i] as JObject;
                            if (tpeSection == null)
                                continue;
                            // TPE number - TPE 1 -> 1,TPE 2 -> 2
                            int tpeNumber = i + 1;
                            string tpeKey = tpeSection["keyTitle"]?.ToString() ?? "";

                            // EVIDENCE
                            JObject evidence = tpeSection["evidence"] as JObject;
                            if (evidence != null)
                            {
                                string interview = GetYesNoValue(evidence["interview"]);

                                string journalReflection = GetYesNoValue(evidence["journalReflection"]);

                                string sampleExample = GetYesNoValue(evidence["sampleExample"]);

                                string directObservation = GetYesNoValue(evidence["directObservation"]);

                                string other = GetYesNoValue(evidence["other"]);

                                string otherText = evidence["otherText"]?.ToString() ?? "";
                                evaluationTemplateBody = evaluationTemplateBody
                                    .Replace($"[[InterviewTPE{tpeNumber}]]",interview)
                                    .Replace($"[[JournalReflectionTPE{tpeNumber}]]",journalReflection)
                                    .Replace($"[[Sample/ExampleTPE{tpeNumber}]]",sampleExample)
                                    .Replace($"[[DirectObservationFormTPE{tpeNumber}]]",directObservation)
                                    .Replace($"[[StateTPE{tpeNumber}]]",otherText);
                            }
                            // STRENGTHS
                            string strengths = tpeSection["strengths"]?.ToString() ?? "";
                            evaluationTemplateBody = evaluationTemplateBody.Replace($"[[AreasOfStrengthTPE{tpeNumber}]]",strengths);
                            // GROWTH AREAS
                            string growthAreas = tpeSection["growthAreas"]?.ToString() ?? "";
                            evaluationTemplateBody = evaluationTemplateBody.Replace($"[[AreasOfDevelopmentTPE{tpeNumber}]]",growthAreas);
                        }
                    }
                    // DISPOSITIONS
                    JArray dispositions = schema["dispositions"] as JArray;
                    if (dispositions != null)
                    {
                        for (int i = 0; i < dispositions.Count; i++)
                        {
                            JObject disposition = dispositions[i] as JObject;
                            if (disposition == null)
                                continue;
                            int dispositionNumber = i + 1;
                            string label = disposition["label"]?.ToString() ?? "";
                            string score = disposition["score"]?.ToString()?.Trim() ?? "";
                            // Label
                            evaluationTemplateBody = evaluationTemplateBody.Replace( $"[[Disposition{dispositionNumber}_1]]",score == "1" ? "X" : "");
                            evaluationTemplateBody = evaluationTemplateBody.Replace($"[[Disposition{dispositionNumber}_2]]",score == "2" ? "X" : "");
                            evaluationTemplateBody = evaluationTemplateBody.Replace($"[[Disposition{dispositionNumber}_3]]",score == "3" ? "X" : "");
                            evaluationTemplateBody = evaluationTemplateBody.Replace($"[[Disposition{dispositionNumber}_4]]",score == "4" ? "X" : "");
                        }
                    }
                    // SIGNATURES
                    JObject signatures = schema["signatures"] as JObject;
                    if (signatures != null)
                    {
                        string candidate = signatures["candidate"]?.ToString() ?? "";
                        string candidateDate = signatures["candidateDate"]?.ToString() ?? "";
                        string teacher = signatures["teacher"]?.ToString() ?? "";
                        string teacherDate = signatures["teacherDate"]?.ToString() ?? "";
                        string supervisor = signatures["supervisor"]?.ToString() ?? "";
                        string supervisorDate = signatures["supervisorDate"]?.ToString() ?? "";
                        evaluationTemplateBody = evaluationTemplateBody.Replace("[[TeacherCandidate]]",candidate);
                        evaluationTemplateBody = evaluationTemplateBody.Replace("[[CandidateDate]]",candidateDate);
                        evaluationTemplateBody = evaluationTemplateBody.Replace("[[CooperatingTeacher]]",teacher);
                        evaluationTemplateBody = evaluationTemplateBody.Replace("[[TeacherDate]]",teacherDate);
                        evaluationTemplateBody = evaluationTemplateBody.Replace("[[UniversitySupervisor]]",supervisor);
                        evaluationTemplateBody = evaluationTemplateBody.Replace("[[SupervisorDate]]",supervisorDate);
                    }
                    //Teacher and mentor details
                    //Teacher and mentor Info
                    JObject info = schema["info"] as JObject;
                    if (info == null)
                    {
                        return pdfFileContent;
                    }

                    // Get values from JSON
                    string submittedDate = info["date"]?.ToString()?.Trim() ?? string.Empty;
                    string studentName = info["studentName"]?.ToString()?.Trim() ?? string.Empty;
                    string universityMentorName = info["universityMentorName"]?.ToString()?.Trim() ?? string.Empty;
                    string cooperatingTeacherName = info["cooperatingTeacherName"]?.ToString()?.Trim() ?? string.Empty;

                    // Create Teacher / University Mentor row
                    string teacherRow = string.Empty;

                    if (!string.IsNullOrWhiteSpace(universityMentorName))
                    {
                        teacherRow = $@"
        <tr>
            <td style='border:1px solid black;padding:4px;vertical-align:top;'>
                <b>University Mentor</b>
            </td>
            <td style='border:1px solid black;padding:4px;vertical-align:top;'>
                {System.Net.WebUtility.HtmlEncode(universityMentorName)}
            </td>
        </tr>";
                    }
                    else if (!string.IsNullOrWhiteSpace(cooperatingTeacherName))
                    {
                        teacherRow = $@"
        <tr>
            <td style='border:1px solid black;padding:4px;vertical-align:top;'>
                <b>Cooperating Teacher</b>
            </td>
            <td style='border:1px solid black;padding:4px;vertical-align:top;'>
                {System.Net.WebUtility.HtmlEncode(cooperatingTeacherName)}
            </td>
        </tr>";
                    }

                    // Replace dynamic row
                    evaluationTemplateBody = evaluationTemplateBody.Replace("[[TeacherRow]]", teacherRow);
                    evaluationTemplateBody = evaluationTemplateBody.Replace("[[StudentName]]", studentName);
                    evaluationTemplateBody = evaluationTemplateBody.Replace("[[SubmittedDate]]", submittedDate);
                    pdfFileContent = GetPDFFileContent(evaluationTemplateBody);
                    return pdfFileContent;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error generating ESCP PDF from evaluation JSON: " + ex.Message,ex);
                }
            }
            else
            {
                input.evaluationjson = input.evaluationjson.Replace("+", " ");

                JObject schema = JObject.Parse(input.evaluationjson);
                if (input.evaluationType == "MidTerm")
                {
                    evaluationTemplateBody = GetDocumentBodyTemplate("MidTermStudentTeachingEvaluationTemplate.html");
                }
                else
                {
                    evaluationTemplateBody = GetDocumentBodyTemplate("FinalTermStudentTeachingEvaluationTemplate.html");
                }

                evaluationTemplateBody = evaluationTemplateBody.Replace("[[logoPath]]", logoPath);

                JArray sections = (JArray)schema["sections"];

                for (int i = 0; i < sections.Count; i++)
                {
                    JObject section = (JObject)sections[i];

                    // Replace Section Title & Subtitle
                    evaluationTemplateBody = evaluationTemplateBody
                        .Replace($"[[Section{i + 1}Title]]", section["title"]?.ToString() ?? "")
                        .Replace($"[[Section{i + 1}Subtitle]]", section["subtitle"]?.ToString() ?? "");

                    JArray indicators = (JArray)section["indicators"];

                    char sectionLetter = GetSectionLetter(i);

                    for (int j = 0; j < indicators.Count; j++)
                    {
                        JObject indicator = (JObject)indicators[j];

                        string rating = indicator["rating"]?.ToString().Trim().ToUpper();
                        rating = rating?.Trim().ToUpper();

                        string prefix;

                        // Handle C-1 and C-2
                        if (i == 2)
                            prefix = "C1" + (j + 1);
                        else if (i == 3)
                            prefix = "C2" + (j + 1);
                        else
                            prefix = sectionLetter + (j + 1).ToString();

                        evaluationTemplateBody = evaluationTemplateBody
                       .Replace($"[[{prefix}_E]]", rating == "E" ? "X" : "")
                       .Replace($"[[{prefix}_P]]", rating == "P" ? "X" : "")
                       .Replace($"[[{prefix}_D]]", rating == "D" ? "X" : "")
                       .Replace($"[[{prefix}_NC]]", rating == "NC" ? "X" : "")
                       .Replace($"[[{prefix}_NO]]", (rating == "NO" || rating == "N/O") ? "X" : "");
                    }
                }
                //Teacher and mentor Info
                JObject info = schema["info"] as JObject;
                if (info == null)
                {
                    return pdfFileContent;
                }

                // Get values from JSON
                string submittedDate = info["date"]?.ToString()?.Trim() ?? string.Empty;
                string studentName = info["studentName"]?.ToString()?.Trim() ?? string.Empty;
                string universityMentorName = info["universityMentorName"]?.ToString()?.Trim() ?? string.Empty;
                string cooperatingTeacherName = info["cooperatingTeacherName"]?.ToString()?.Trim() ?? string.Empty;

                // Create Teacher / University Mentor row
                string teacherRow = string.Empty;

                if (!string.IsNullOrWhiteSpace(universityMentorName))
                {
                    teacherRow = $@"
        <tr>
            <td style='border:1px solid black;padding:4px;vertical-align:top;'>
                <b>University Mentor</b>
            </td>
            <td style='border:1px solid black;padding:4px;vertical-align:top;'>
                {System.Net.WebUtility.HtmlEncode(universityMentorName)}
            </td>
        </tr>";
                }
                else if (!string.IsNullOrWhiteSpace(cooperatingTeacherName))
                {
                    teacherRow = $@"
        <tr>
            <td style='border:1px solid black;padding:4px;vertical-align:top;'>
                <b>Cooperating Teacher</b>
            </td>
            <td style='border:1px solid black;padding:4px;vertical-align:top;'>
                {System.Net.WebUtility.HtmlEncode(cooperatingTeacherName)}
            </td>
        </tr>";
                }

                // Replace dynamic row
                evaluationTemplateBody = evaluationTemplateBody.Replace("[[TeacherRow]]", teacherRow);
                evaluationTemplateBody = evaluationTemplateBody.Replace("[[StudentName]]", studentName);
                evaluationTemplateBody = evaluationTemplateBody.Replace("[[SubmittedDate]]", submittedDate);
                evaluationTemplateBody = evaluationTemplateBody
                    .Replace("[[Strengths]]", schema["strengths"]?.ToString() ?? "")
                    .Replace("[[GrowthAreas]]", schema["growthAreas"]?.ToString() ?? "");
            }

            pdfFileContent = GetPDFFileContent(evaluationTemplateBody);

            return pdfFileContent;
        }
        private string GetYesNoValue(JToken value)
        {
            if (value == null)
                return "";

            bool result;

            if (bool.TryParse(value.ToString(), out result))
            {
                return result ? "Yes" : "";
            }

            return "";
        }
        private char GetSectionLetter(int index)
        {
            switch (index)
            {
                case 0: return 'A';
                case 1: return 'B';
                case 4: return 'D';
                case 5: return 'E';
                case 6: return 'F';
                case 7: return 'G';
                default: return 'C';
            }
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
                    string URL = _configuration["ApplicationKeys:PartnerUserBaseURL"] + obj.CommunitySiteUserIdentifier;
                    string body = string.Empty;
                    //string body = GetMailBodyTemplate("PartnerUserMailTemplate.html");
                    SqlParameter[] parameters1 ={
                                 new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = 0 },
                                 new SqlParameter("@SectionName", SqlDbType.VarChar,10) { Value = ""},
                                 new SqlParameter("@CategoryName", SqlDbType.VarChar,20) { Value = "" },
                                 new SqlParameter("@Identifier", SqlDbType.VarChar,20) { Value = "FieldWork Mail"},
                                 new SqlParameter("@Name", SqlDbType.VarChar,50) { Value = "PartnerUser" }
                            };
                    DataTable dtMailBody = _helper.GetDataTable("[dbo].[GetAdmissionRequirementsMailBody]", parameters1);
                    if (dtMailBody.Rows.Count > 0)
                    {
                        if (dtMailBody.Rows[0]["EmailBody"] != DBNull.Value)
                        {
                            body = Convert.ToString(dtMailBody.Rows[0]["EmailBody"]);

                            string beforeBody = string.Empty;
                            string afterBody = string.Empty;
                            beforeBody = "<html>\r\n<head>\r\n<style>\r\n\r\n#link {\r\ncolor: #0563C1;\r\n}\r\n.custom-link {\r\ncolor: #0563C1;\r\n}\r\n</style>\r\n</head>\r\n<body>\r\n<div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\"/></div>";
                            afterBody = "</body>\r\n</html>";
                            body = $"{beforeBody}{body}{afterBody}";
                        }
                    }
                    string link = @"<a href ='" + URL + "' target='_blank' class='custom-link'>Click here</a>";
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
            string logoPath = Path.GetFullPath("SupportFiles/Img/logo.jpeg");
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
                        //string body = GetMailBodyTemplate("Prerequisite_Expired_Mail.html");
                        string body = string.Empty;
                        SqlParameter[] parameters1 ={
                                 new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = 0 },
                                 new SqlParameter("@SectionName", SqlDbType.VarChar,10) { Value = ""},
                                 new SqlParameter("@CategoryName", SqlDbType.VarChar,20) { Value = "" },
                                 new SqlParameter("@Identifier", SqlDbType.VarChar,20) { Value = "FieldWork Mail"},
                                 new SqlParameter("@Name", SqlDbType.VarChar,50) { Value = "PrerequisiteExpired" }
                            };
                        DataTable dtMailBody = _helper.GetDataTable("[dbo].[GetAdmissionRequirementsMailBody]", parameters1);
                        if (dtMailBody.Rows.Count > 0)
                        {
                            if (dtMailBody.Rows[0]["EmailBody"] != DBNull.Value)
                            {
                                body = Convert.ToString(dtMailBody.Rows[0]["EmailBody"]);

                                string beforeBody = string.Empty;
                                string afterBody = string.Empty;
                                beforeBody = "<html>\r\n<head>\r\n </head>\r\n<body>\r\n<div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                                afterBody = "</body>\r\n</html>";
                                body = $"{beforeBody}{body}{afterBody}";
                            }
                        }
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
                            //string body = GetMailBodyTemplate("FinalApprovedTemplate.html");
                            SqlParameter[] parameters1 ={
                                 new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = 0 },
                                 new SqlParameter("@SectionName", SqlDbType.VarChar,10) { Value = ""},
                                 new SqlParameter("@CategoryName", SqlDbType.VarChar,20) { Value = "" },
                                 new SqlParameter("@Identifier", SqlDbType.VarChar,20) { Value = "FieldWork Mail"},
                                 new SqlParameter("@Name", SqlDbType.VarChar,50) { Value = "BothPrerequisiteApproved" }
                            };
                            DataTable dtMailBody = _helper.GetDataTable("[dbo].[GetAdmissionRequirementsMailBody]", parameters1);
                            string body = string.Empty;
                            if (dtMailBody.Rows.Count > 0)
                            {
                                if (dtMailBody.Rows[0]["EmailBody"] != DBNull.Value)
                                {
                                    body = Convert.ToString(dtMailBody.Rows[0]["EmailBody"]);

                                    string beforeBody = string.Empty;
                                    string afterBody = string.Empty;
                                    beforeBody = "<body>\r\n<div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                                    afterBody = "</body>\r\n</html>";
                                    body = $"{beforeBody}{body}{afterBody}";
                                }
                            }
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
            //iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50);
            iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
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
