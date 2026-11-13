using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Domain.Request.GraduateProgram;
using ThoughtFocus.Domain.Request.StudentProfile;
using ThoughtFocus.Domain.Request.StudentTeachingMilestone;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Admin;
using ThoughtFocus.Domain.Response.FieldWork;
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

        public StudentTeachingMilestonesResponse GetStudentTeachingMilestoneDetails(string CsuldId, int UserID, int formID, int roleID, int programID)
        {
            StudentTeachingMilestonesResponse obj = new StudentTeachingMilestonesResponse();
            
            SqlParameter[] parameters = {
                                          new SqlParameter("@csulbid", SqlDbType.VarChar, 50) { Value = CsuldId },
                                          new SqlParameter("@UserID", SqlDbType.VarChar, 50) { Value = UserID },
                                          new SqlParameter("@FormID", SqlDbType.NVarChar, -1) { Value = formID },
                                          new SqlParameter("@ProgramID", SqlDbType.NVarChar, -1) { Value = programID },
                                          new SqlParameter("@LoggedInRole", SqlDbType.NVarChar, -1) { Value = roleID },
                                          //new SqlParameter("@MilestoneStateID", SqlDbType.BigInt) { Value = milestoneStateID}
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
                        ProgramStatusDesc = Convert.ToString(row["ProgramStatusDesc"]),
                        AddressST = Convert.ToString(row["AddressST"]),
                        ApartmentST = Convert.ToString(row["ApartmentST"]),
                        CityST = Convert.ToString(row["CityST"]),
                        PhoneST = Convert.ToString(row["PhoneST"]),
                        ZipST = Convert.ToString(row["ZipST"]),
                    }).FirstOrDefault();
                    
                    obj.AttachmentsInformation = dtStudentTeaching.Tables[1].AsEnumerable().Select(row => new AttachmentsInformation
                    {
                        ID = row["ID"] == DBNull.Value ? 0: Convert.ToInt32(row["ID"]),
                        DocumentID = Convert.ToInt32(row["DocumentID"]),
                        DocumentName = Convert.ToString(row["DocumentName"]),
                        FileName = Convert.ToString(row["FileName"]),
                        FileExtn = Convert.ToString(row["FileExtn"]),
                        UniqueID = row["UniqueID"] == DBNull.Value ? Guid.Empty: Guid.Parse(Convert.ToString(row["UniqueID"])),
                        FolderName = Convert.ToString(row["FolderName"])
                    }).ToList();

                    obj.studentTeachingPlacementConsiderations = dtStudentTeaching.Tables[2].AsEnumerable().Select(row => new StudentTeachingPlacementConsiderations
                    {
                        PlacementConsiderationsJSON = Convert.ToString(row["PlacementConsiderationsJSON"]),
                        CSULBID = Convert.ToString(row["CSULBID"]),
                        FormId = Convert.ToInt32(row["FormId"]),

                    }).FirstOrDefault();

                    obj.studentTeachingAcknowledgment = dtStudentTeaching.Tables[3].AsEnumerable().Select(row => new StudentTeachingAcknowledgment
                        {
                            AcknowledgmentJSON = Convert.ToString(row["AcknowledgementsJSON"]),
                            CSULBID = Convert.ToString(row["CSULBID"]),
                            FormId = Convert.ToInt32(row["FormId"]),
                    }).FirstOrDefault();

                    obj.BilaRequirements = dtStudentTeaching.Tables[2].AsEnumerable().Select(row => new BILARequirements
                    {
                        BILARequirementsJSON = Convert.ToString(row["BILARequirementJSON"]),
                    }).FirstOrDefault();

                    obj.MilestoneState = dtStudentTeaching.Tables[4].AsEnumerable().Select(row => new MilestoneState
                    {
                        StateID = Convert.ToInt32(row["MilestoneStateID"]),
                        StateName = Convert.ToString(row["MilestoneStateName"])

                    }).FirstOrDefault();
                    obj.MilestoneStateDetails = dtStudentTeaching.Tables[5].AsEnumerable().Select(row => new MilestoneStateDetails
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        Name = Convert.ToString(row["Name"])

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
        public BaseResponse AddCredentialArea(AddCredentialAreaRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@Name", SqlDbType.NVarChar,100) { Value = input.Name }
                                        };
            DataTable dtResponse = _helper.GetDataTable("[STMilestone].[AddCredentialArea]", parameters);
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
        public BaseResponse SaveStudentTeachingMilestone(StudentTeachingMilestoneRequest input)
        {
            BaseResponse response = new BaseResponse();
            string jsonString;

            if (input.JSON is JsonElement element)
            {
                jsonString = element.ValueKind == JsonValueKind.String
                    ? element.GetString()
                    : element.GetRawText();
            }
            else if (input.JSON is string s)
            {
                jsonString = s;

                if (!string.IsNullOrWhiteSpace(s) &&
                    s.TrimStart().StartsWith("\""))
                {
                    jsonString = JsonConvert.DeserializeObject<string>(s);
                }
            }
            else
            {
                jsonString = JsonConvert.SerializeObject(input.JSON);
            }

            JsonDocument.Parse(jsonString).Dispose();

            SqlParameter[] parameters =
            {
            new SqlParameter("@JSON", SqlDbType.NVarChar, -1)
            {
                Value = jsonString
            }
            };

            var id = _helper.GetDataTable("[Milestone].[SaveStudentTeachingData]", parameters);
            response.Message = "Student Teaching data saved successfully";
            response.IsSuccess = true;
            return response;
        }

        public BaseResponse AssignApproverAssigner(AssignApproverAssignerRequest input)
        {
            BaseResponse response = new BaseResponse();
            string jsonString;
            SqlParameter[] parameters =
            {
            new SqlParameter("@CSULBID", SqlDbType.NVarChar, -1){Value = input.CSULBID},
            new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID }
            };

            var id = _helper.GetDataTable("[STMilestone].[AssignApproverAssigner]", parameters);
            response.Message = "Assighner added successfully";
            response.IsSuccess = true;
            return response;
        }

        public ExternalApproversListResponse GetExternalApprovers()
        {
            ExternalApproversListResponse obj = new ExternalApproversListResponse();
            SqlParameter[] parameters ={
                                       };

            DataTable dtDD = _helper.GetDataTable("[STMilestone].[GetExternalApprovers]", parameters);
            try
            {
                if (dtDD.Rows.Count > 0)
                {

                    obj.externalApprovers = dtDD.AsEnumerable().Select(row =>
                                          new ExternalApprovers
                                          {
                                              ID = Convert.ToInt32(row["ID"]),
                                              LastName = Convert.ToString(row["LastName"]),
                                              FirstName = Convert.ToString(row["FirstName"]),
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
        public BaseResponse AddExternalApprover(AddExternalApproverRequest input)
        {
            BaseResponse response = new BaseResponse();
            string jsonString;
            string Message = string.Empty;
            SqlParameter[] parameters =
            {
            new SqlParameter("@CSULBID", SqlDbType.NVarChar, -1){Value = input.CSULBID},
            new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
            new SqlParameter("@ExternalApproverID", SqlDbType.BigInt) { Value = input.ExternalApproverID }

            };

            var result = _helper.GetDataTable("[STMilestone].[SaveExternalApprover]", parameters);
            if (result.Rows.Count > 0)
            {
                Message =  Convert.ToString(result.Rows[0]["Message"]);
            }
            response.Message = Message;
            response.IsSuccess = true;
            return response;
        }
        public ApproverAssignmentsResponse GetExternalApproverAssignments(ApproverAssignmentsRequest input)
        {
            ApproverAssignmentsResponse obj = new ApproverAssignmentsResponse();
            SqlParameter[] parameters ={
                                            new SqlParameter("@CSULBID", SqlDbType.NVarChar, -1){Value = input.CSULBID},
                                            new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID }
                                       };

            DataTable dtDD = _helper.GetDataTable("[STMilestone].[GetExternalApproverAssignments]", parameters);
            try
            {
                if (dtDD.Rows.Count > 0)
                {

                    obj.approverAssignments = dtDD.AsEnumerable().Select(row =>
                                          new ApproverAssignments
                                          {
                                              ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                                              ApproverAssighnerLastName = row["ApproverAssighnerLastName"] != DBNull.Value ? Convert.ToString(row["ApproverAssighnerLastName"]) : string.Empty,
                                              ApproverAssighnerFirstName = row["ApproverAssighnerFirstName"] != DBNull.Value ? Convert.ToString(row["ApproverAssighnerFirstName"]) : string.Empty,
                                              ExternalApproverFirstName = row["ExternalApproverFirstName"] != DBNull.Value ? Convert.ToString(row["ExternalApproverFirstName"]) : string.Empty,
                                              ExternalApproverLastName = row["ExternalApproverLastName"] != DBNull.Value ? Convert.ToString(row["ExternalApproverLastName"]) : string.Empty,
                                              ApprovalStatus = row["ApprovalStatus"] != DBNull.Value ? Convert.ToString(row["ApprovalStatus"]) : string.Empty,
                                              Active = row["Active"] != DBNull.Value && Convert.ToBoolean(row["Active"]),
                                              DateAdded = row["DateAdded"] != DBNull.Value ? Convert.ToDateTime(row["DateAdded"]) : DateTime.MinValue,
                                              CSULBID = row["CSULBID"] != DBNull.Value ? Convert.ToString(row["CSULBID"]) : string.Empty,
                                              FormId = row["FormID"] != DBNull.Value ? Convert.ToInt32(row["FormID"]) : 0,
                                              ProgramID = row["ProgramID"] != DBNull.Value ? Convert.ToInt32(row["ProgramID"]) : 0,
                                              ApproverSequence = row["ApproverSequence"] != DBNull.Value ? Convert.ToInt32(row["ApproverSequence"]) : 0
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
        public BaseResponse UpdateMilestoneFormState(UpdateFormStateRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                   {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@StateID", SqlDbType.BigInt) { Value = input.StateID }
                                        };
            DataTable dtResponse = _helper.GetDataTable("[STMilestone].[UpdateMilestoneFormState]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "SUCCESS")
                {
                    response.Message = "FormState Updated successfully";
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "FAILURE")
                {
                    response.Message = "Failed to update the FormState";
                    response.IsSuccess = false;
                }
            }
            return response;
        }
        public MandateFieldValidationResponse isSubmitEnableST(MandateFieldValidationRequest input)
        {
            MandateFieldValidationResponse obj = new MandateFieldValidationResponse();
            SqlParameter[] parameters ={
                                            new SqlParameter("@CSULBID", SqlDbType.NVarChar, -1){Value = input.CSULBID},
                                            new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                            new SqlParameter("@Term", SqlDbType.VarChar, 10) { Value = input.Term }
                                       };

            DataTable dtDD = _helper.GetDataTable("[STMilestone].[ValidateMandatoryFields]", parameters);
            try
            {
                if (dtDD.Rows.Count > 0)
                {

                    obj.isEnableSubmit = Convert.ToBoolean(dtDD.Rows[0]["ValidationFlag"]);
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
                obj.Message = "Data Retrieval Failed , Please contact site admin";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public ADGroupUsersListResponse GetADGroupListForSTMilestone()
        {
            ADGroupUsersListResponse obj = new ADGroupUsersListResponse();
        
            try
            {
                    var gradeAdminUserNameTest = _configuration["ApplicationKeys:StudentTeachingApprovers"];

                    if (!string.IsNullOrWhiteSpace(gradeAdminUserNameTest))
                    {
                        var csulbids = gradeAdminUserNameTest
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(id => id.Trim())
                            .Where(id => !string.IsNullOrWhiteSpace(id))
                            .Distinct(StringComparer.OrdinalIgnoreCase);
                                
                        obj.ADGroupUsersList ??= new List<ADGroupUsersList>();

                        foreach (var csulbid in csulbids)
                        {
                        ADGroupUsersList gradeAdminUser = GetUserIDByCSULBID(csulbid);

                            if (gradeAdminUser != null)
                            {
                                obj.ADGroupUsersList.Add(gradeAdminUser);
                            }
                        }
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
        public ADGroupUsersList GetUserIDByCSULBID(string CSULBID)
        {
            ADGroupUsersList user = null;

            SqlParameter[] parameters =
            {
                new SqlParameter("@CSULBID", SqlDbType.VarChar, 50){ Value = CSULBID}
            };

            try
            {
                DataTable dtUser = _helper.GetDataTable("[dbo].[GetUserIDByCSULBID]", parameters);

                if (dtUser.Rows.Count > 0)
                {
                    DataRow row = dtUser.Rows[0];

                    user = new ADGroupUsersList
                    {
                        UserID = Convert.ToInt32(row["ID"]),
                        FirstName = Convert.ToString(row["FirstName"]) ?? string.Empty,
                        LastName = Convert.ToString(row["LastName"]) ?? string.Empty,
                        EMAIL = Convert.ToString(row["Email"]) ?? string.Empty,
                        CSULBID = CSULBID,
                        UserRoles = "Grade Admin"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Error retrieving user details for CSULBID: {CSULBID}", CSULBID);
            }

            return user;
        }

        public ADGroupUsersListResponse SendSTMilestoneNotificationEmail(string CSULBID, int ProgramID)
        {
            ADGroupUsersListResponse obj = new ADGroupUsersListResponse();

            try
            {
                // Get AD group users
                ADGroupUsersListResponse response = GetADGroupListForSTMilestone();

                if (response == null || response.ADGroupUsersList == null ||
                    !response.ADGroupUsersList.Any())
                {
                    obj.IsSuccess = false;
                    obj.Message = "No users found to send the email.";
                    return obj;
                }

                // Get unique email addresses
                var emailList = response.ADGroupUsersList
                    .Where(x => !string.IsNullOrWhiteSpace(x.EMAIL))
                    .Select(x => x.EMAIL.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (!emailList.Any())
                {
                    obj.IsSuccess = false;
                    obj.Message = "No valid email addresses found.";
                    return obj;
                }

                // Hardcoded email body
                string body = @"
            <p>Dear Approver,</p>

            <p>
                This is a notification regarding the Student Teaching Milestone. 
            </p>

            <p>Thank you,<br/>Student Teaching Team</p>";

                string beforeBody = @"<html>
            <head>
             
            </head>
            <body>
                <div>
                    <img alt='logo' src='[[logoPath]]'
                    style='width:300px; height:auto;'/>
                </div>";

                string afterBody = "</body></html>";

                body = $"{beforeBody}{body}{afterBody}";

                string logoText = "cid:myImageID";

                body = body.Replace("[[logoPath]]", logoText);

                // Hardcoded subject
                string subject = "Student Teaching Milestone Notification";

                // Send email to all users using BCC
                string toUser = string.Join(",", emailList);
                foreach (string email in emailList)
                {
                    _sendMail.SendEmail(email, "", "COMMON", subject, body, "");
                }
                //_sendMail.SendEmail(toUser,"","COMMON",subject,body,"");

                obj.IsSuccess = true;
                obj.Message = "Student Teaching Milestone notification email sent successfully.";
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Email sending failed. Please contact site admin.";
                obj.StackTrace = ex.Message;
            }

            return obj;
        }
        public BaseResponse UpdateMilestoneWorkflowDetails(UpdateMilestoneWorkflowRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                   {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@FormState", SqlDbType.BigInt) { Value = input.FormState },
                                          new SqlParameter("@CSULBID", SqlDbType.VarChar, 50){Value = input.CSULBID},
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@ApproverCSULBID", SqlDbType.VarChar, 50) { Value = input.ApproverCSULBID },

                                        };
            DataTable dtResponse = _helper.GetDataTable("[STMilestone].[SaveMilestoneWorkflowDetails]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "1")
                {
                    response.Message = "FormState Updated successfully";
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "0")
                {
                    response.Message = "Failed to update the FormState";
                    response.IsSuccess = false;
                }
            }
            return response;
        }
        public MilestoneWorkflowDetailsListResponse GetMilestoneWorkflowDetails(string CSULBID, int FormID)
        {
            MilestoneWorkflowDetailsListResponse obj = new MilestoneWorkflowDetailsListResponse();
            SqlParameter[] parameters ={
                                            new SqlParameter("@CSULBID", SqlDbType.VarChar, 50){Value = CSULBID},
                                            new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID }
                                       };

            DataTable dtDD = _helper.GetDataTable("[STMilestone].[GetMilestoneWorkflowDetails]", parameters);
            try
            {
                if (dtDD.Rows.Count > 0)
                {

                    obj.milestoneWorkflowDetails = dtDD.AsEnumerable().Select(row =>
                                          new MilestoneWorkflowDetails
                                          {
                                              TermCode = row["TermCode"] != DBNull.Value ? Convert.ToString(row["TermCode"]) : string.Empty,
                                              ModifiedTime = row["ModifiedTime"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedTime"]) : DateTime.MinValue,
                                              CSULBID = row["CSULBID"] != DBNull.Value ? Convert.ToString(row["CSULBID"]) : string.Empty,
                                              ApproverCSULBID = row["ApproverCSULBID"] != DBNull.Value ? Convert.ToString(row["ApproverCSULBID"]) : string.Empty,
                                              FormID = row["FormID"] != DBNull.Value ? Convert.ToInt32(row["FormID"]) : 0,
                                              ProgramID = row["ProgramID"] != DBNull.Value ? Convert.ToInt32(row["ProgramID"]) : 0,
                                              FormState = row["ApproverSequence"] != DBNull.Value ? Convert.ToInt32(row["ApproverSequence"]) : 0
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



    }
}
