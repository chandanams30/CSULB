using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Domain.Request.FieldWork;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class FieldWorkServiceImpl : IFieldWorkService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        public FieldWorkServiceImpl(ISqlDBUtility helper, IConfiguration configuration,ISendMail sendMail)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
        }
        public FieldWorkDataResponse GetFieldWorkDetailsById(int userId,int fieldWorkId)
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
                                                  Section= Convert.ToString(row["Section"]),
                                                  Term = Convert.ToString(row["Term"]),
                                                  FieldWorkPrerequisiteStatus = Convert.ToInt32(row["FieldWorkPrerequisiteStatus"])
                                                 //,UIHandler=Convert.ToString(row["UIHandler"])
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
                                              DocumentStatus= Convert.ToString(row["DocumentStatus"]),
                                              DocumentInfo = Convert.ToString(row["DocumentInfo"]),
                                              CanUpload=Convert.ToBoolean(row["CanUpload"]),
                                              CanValidate = Convert.ToBoolean(row["CanValidate"])
                                              }).ToList();

                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
            }
            catch(Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public byte[] GetFileContent(string userFolderPath,string fileName)
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


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserId", SqlDbType.Int, 50) { Value = userId }
                                        };

            DataTable dtFieldWorkList= _helper.GetDataTable("[dbo].[GetFieldWorkData]", parameters);
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
                                                  Section= Convert.ToString(row["Section"]),
                                                  Term = Convert.ToString(row["Term"]),
                                                  FieldWorkPrerequisiteStatus=Convert.ToInt32(row["FieldWorkPrerequisiteStatus"])

                                              }).ToList();

                    objList.FieldWorkResponse = obj;
                    objList.IsSuccess = true;
                    objList.Message = "Data Retrieved Successfully";

                }
            }
            catch(Exception ex)
            {
                objList.IsSuccess = false;
                objList.Message = "Data Retrievel Failed";
                objList.StackTrace =ex.Message;
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
                    ApproveRejectMailer approvedRejectMailer = GetEmailSubjectAndBody(input.ApprovalStatus, mailInfo.FileName, input.RejectedReason, input.Comments,mailInfo.DisplayName);
                    // send the approve / reject mail here 
                    _sendMail.SendEmail(mailInfo.Email, "", approvedRejectMailer.Subject, approvedRejectMailer.Body, "");
                }
                catch (Exception ee)
                {
                    response.IsSuccess = false;
                    response.Message = "Prerequisite submitted/not submitted status send mail failure.";
                   // _sendMail.SendEmail("asif.khan@thoughtfocus.com", "", "approved/Reject Mailer", ee.Message, "");
                }

                try
                {
                    EmailMessageModel emailModel = GetMessageBody(input.FieldWorkID);

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
                        _sendMail.SendEmail(toUser, "", subject, body, emailModel.Body);
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
            catch(Exception ex)
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
        private ApproveRejectMailer GetEmailSubjectAndBody(bool approvalStatus,string documentName,string rejectReason,string comments,string displayName)
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
                body=body.Replace("[[ApplicantName]]", displayName).Replace("[[DocumentName]]", documentName).Replace("[[logoPath]]", logoText);
                //body = "<html><body><p>Your document "+documentName+" has been approved</p><p>Thank you,</br>CSULB College of Education  </p></body></html>";
                model.Body = body;
            }
            else if (approvalStatus == false)
            {
                model.Subject = "MyCED prerequisites review not approved";
                // get the body from email template
                body = GetMailBodyTemplate("NotApprovedMailTemplate.html");
                body=body.Replace("[[ApplicantName]]", displayName).Replace("[[DocumentName]]", documentName).Replace("[[logoPath]]", logoText).Replace("[[Reason]]", rejectReason).Replace("[[Comment]]", comments);
                //body = "<html><body> <p>Your document "+ documentName +" has not been approved</p><p>Reason  : "+rejectReason+"</p><p>Comment : "+comments+"</p><p>Please upload a new document</p><p>Thank you,</br>CSULB College of Education  </p></body></html>";
                model.Body = body;
            }
            return model;
        }
        private StudentsDetails GetStudentDetailsFromFieldWorkId(int fieldWorkId,int fieldWorkAttachmentId)
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
                                            FileName= Convert.ToString(row["FileName"])+"."+ Convert.ToString(row["FileExtn"])
                                        }).FirstOrDefault();
            }
            return model;
        }

        private EmailMessageModel GetMessageBody(int fieldWorkattachmentID)
        {
            string body = string.Empty;
            string ApplicantName = string.Empty;
            string CSULBID = string.Empty;
            string Semester = string.Empty;
            EmailMessageModel model = new EmailMessageModel();
          
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@FieldWorkID", SqlDbType.Int, 50) { Value = fieldWorkattachmentID }
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
                    body = body.Replace("[[ApplicantName]]", ApplicantName).Replace("[[CSULBID]]", CSULBID).Replace("[[Semester]]", Semester).Replace("[[Date]]", DateTime.Now.ToString("MMM-dd-yyyy")).Replace("[[logopath]]",logopath);
                    model.toEmail= Convert.ToString(dtEmailData.Rows[0]["Email"]);
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
            if (input.FileName != string.Empty)
            {
                //string[] fileSplit = input.FileName.Split('.');
                //fileName = fileSplit[0].ToString();
                //fileExtension = fileSplit[1].ToString();
                //savedFileName = input.FieldWorkAttachmentId + fileSplit[0].ToString() + DateTime.Now.ToString("MMddyyyyHHmmss");
                AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.FileName);
                fileName = fileDetails.FileName;
                fileExtension = fileDetails.FileExtension;
                savedFileName = input.FieldWorkAttachmentId+fileDetails.FileName.ToString()+DateTime.Now.ToString("MMddyyyyHHmmss");
            }
            // call the current file name from DB  and delete the file from the file system and then run the below 
            // first time upload , the filename will be null 
            // call the SP to save the save the file details in fieldwork.attachments table 
            SqlParameter[] parameters =
                                      {
                                          new SqlParameter("@UserId", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FieldWorkAttachmentID", SqlDbType.BigInt) { Value = input.FieldWorkAttachmentId },
                                          new SqlParameter("@FileName", SqlDbType.VarChar, 250) { Value = fileName },
                                          new SqlParameter("@FileExtn", SqlDbType.VarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = savedFileName },
                                          new SqlParameter("@ValidTill", SqlDbType.DateTime) { Value = input.ValidTill },
                                          new SqlParameter("@Comments", SqlDbType.VarChar, -1) { Value = input.Comments }
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
                        File.WriteAllBytes(Path.Combine(dirFieldWork, savedFileName + "."+fileExtension), input.FileContent);
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
                    DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                    DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirFieldWork);
                    DirectorySecurity dSecurity = dirFieldWorkFolder.GetAccessControl();
                    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    dirFieldWorkFolder.SetAccessControl(dSecurity);

                    File.WriteAllBytes(Path.Combine(dirFieldWork, savedFileName + "." + fileExtension), input.FileContent);
                }
                // now delete the old file based on the file name return from DB call above 
            }

            response.IsSuccess = true;
            response.Message = "Field Work Document Uploaded Successfully";
            return response;
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

        public FieldWorkActivityLogResponse GetFieldWorkActivityLog(int userId, int fieldworkId)
        {
            FieldWorkActivityLogResponse obj = new FieldWorkActivityLogResponse();

            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userId },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = fieldworkId }
                                     };
            DataTable dtActivityLog = _helper.GetDataTable("[dbo].[GetFieldWorkActivityLog]", parameters);
            if (dtActivityLog.Rows.Count > 0)
            {
                obj = dtActivityLog.AsEnumerable().Select(row =>
                                              new FieldWorkActivityLogResponse
                                              {
                                                  FieldWorkId = Convert.ToInt32(row["FieldWorkID"]),
                                                  BaseSchema = Convert.ToString(row["BaseSchema"]),
                                                  ResponseSchema = Convert.ToString(row["ResponseSchema"])
                                              }).FirstOrDefault();
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

        public FieldWorkActivityLogResponse UpdateFieldWorkActivityLog(FieldWorkActivityLogRequest input)
        {
            FieldWorkActivityLogResponse obj = new FieldWorkActivityLogResponse();

            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserId },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = input.FieldWorkId },
                                          new SqlParameter("@ResponseSchema", SqlDbType.NVarChar) { Value = input.ResponseSchema }
                                     };
            DataTable dtActivityLog = _helper.GetDataTable("[dbo].[UpdateFieldWorkActivityLog]", parameters);
            if (dtActivityLog.Rows.Count > 0)
            {
                obj = dtActivityLog.AsEnumerable().Select(row =>
                                              new FieldWorkActivityLogResponse
                                              {
                                                  FieldWorkId = Convert.ToInt32(row["FieldWorkID"]),
                                                  BaseSchema = Convert.ToString(row["BaseSchema"]),
                                                  ResponseSchema = Convert.ToString(row["ResponseSchema"])
                                              }).FirstOrDefault();
                obj.IsSuccess = true;
                obj.Message = "Data Saved Successfully";
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
