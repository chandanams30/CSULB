using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
using ThoughtFocus.Domain.Request.InitialCredentialProgram;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Application;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class InitialCredentialProgramService: IInitialCredentialProgramService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        public ILogger<InitialCredentialProgramService> _logger;
        private readonly ICommonUtils _utils;
        public InitialCredentialProgramService(ISqlDBUtility helper
                                         , IConfiguration configuration
                                         , ISendMail sendMail
                                         , ILogger<InitialCredentialProgramService> logger,
                                           ICommonUtils utils)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
            _utils = utils;
        }
        public ApplicationProgramResponse GetApplicationPrograms(int userID, int applicationTypeID, string termCode)
        {
            ApplicationProgramResponse obj = new ApplicationProgramResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserId", SqlDbType.Int, 50) { Value = userID },
                                          new SqlParameter("@ApplicationTypeID", SqlDbType.Int, 50) { Value = applicationTypeID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = termCode }
                                        };

            DataSet dtApplicationPrograms = _helper.GetDataSet("[dbo].[GetApplicationPrograms]", parameters);
            try
            {
                if (dtApplicationPrograms.Tables.Count > 0)
                {


                    obj.ApplicationPrograms = dtApplicationPrograms.Tables[0].AsEnumerable().Select(row =>
                                              new ApplicationPrograms
                                              {
                                                  programID = Convert.ToInt32(row["ID"]),
                                                  programName = Convert.ToString(row["Name"]),
                                                  semester = Convert.ToString(row["Semester"]),
                                                  applicationOpens = Convert.ToDateTime(row["ApplicationOpens"]),
                                                  applicationCloseDate = Convert.ToDateTime(row["ApplicationCloseDate"]),
                                                  TotalCount = Convert.ToInt32(row["TotalCount"]),
                                                  AcceptedCount = Convert.ToInt32(row["AcceptedCount"]),
                                                  showApply = Convert.ToBoolean(row["showApply"]),
                                                  showView = Convert.ToBoolean(row["showView"])
                                              }).ToList();

                    obj.HeaderDetails = dtApplicationPrograms.Tables[1].AsEnumerable().Select(row =>
                                                new HeaderDetails
                                                {
                                                    semester = Convert.ToString(row["Semester"]),
                                                    TermCode = Convert.ToString(row["TermCode"]),
                                                    showApply = Convert.ToBoolean(row["showApply"]),
                                                    showView = Convert.ToBoolean(row["showView"]),
                                                    showAssignApplicationToReviewers = Convert.ToBoolean(row["showAssignApplicationToReviewers"])
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

        public AppliedFormsByProgramsResponse GetAppliedFormsByPrograms(int userID, int programID, string termcode, int formStateID)
        {
            AppliedFormsByProgramsResponse obj = new AppliedFormsByProgramsResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserId", SqlDbType.Int, 50) { Value = userID },
                                          new SqlParameter("@ProgramID", SqlDbType.Int, 50) { Value = programID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = termcode },
                                          new SqlParameter("@FormStateID", SqlDbType.Int, 50) { Value = formStateID }
                                        };

            DataSet dsAppliedFormsByProgram = _helper.GetDataSet("[dbo].[GetAppliedFormsByPrograms]", parameters);
            try
            {
                if (dsAppliedFormsByProgram.Tables.Count > 0)
                {

                    if (dsAppliedFormsByProgram.Tables[0].Rows.Count > 0)
                    {
                        obj.AppliedFormsByPrograms = dsAppliedFormsByProgram.Tables[0].AsEnumerable().Select(row =>
                                              new AppliedFormsByPrograms
                                              {
                                                  FormID = Convert.ToInt32(row["ID"]),
                                                  StudentName = Convert.ToString(row["StudentName"]),
                                                  FormStateID = Convert.ToInt32(row["FormStateID"]),
                                                  FormState = Convert.ToString(row["FormState"]),
                                                  AppliedDate = Convert.ToDateTime(row["AppliedDate"]),
                                                  ProgramID = Convert.ToInt32(row["ProgramID"]),
                                                  ProgramName = Convert.ToString(row["ProgramName"]),
                                                  Semester = Convert.ToString(row["Semester"]),
                                                  TermCode = Convert.ToString(row["TermCode"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"])

                                              }).ToList();
                    }
                    if (dsAppliedFormsByProgram.Tables[1].Rows.Count > 0)
                    {
                        obj.HeaderDetails = dsAppliedFormsByProgram.Tables[1].AsEnumerable().Select(row =>
                                              new HeaderDetails
                                              {
                                                  semester = Convert.ToString(row["Semester"]),
                                                  TermCode = Convert.ToString(row["TermCode"]),
                                                  programName = Convert.ToString(row["ProgramName"]),
                                                  programID = Convert.ToInt32(row["ProgramID"])

                                              }).FirstOrDefault();
                    }

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

        public FormStatesResponse GetFormStates(int userID)
        {
            FormStatesResponse obj = new FormStatesResponse();


            SqlParameter[] parameters ={
                                           new SqlParameter("@UserID", SqlDbType.NVarChar, 255) { Value = userID}
                                       };

            DataTable dtFormStates = _helper.GetDataTable("[dbo].[GetFormStates]", parameters);
            try
            {
                if (dtFormStates.Rows.Count > 0)
                {


                    obj.FormStates = dtFormStates.AsEnumerable().Select(row =>
                                              new FormStates
                                              {
                                                  StateID = Convert.ToInt32(row["StateID"]),
                                                  StateName = Convert.ToString(row["StateName"])
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

        public SemesterListResponse GetSemesterList()
        {
            SemesterListResponse obj = new SemesterListResponse();


            SqlParameter[] parameters = { };

            DataTable dtSemesters = _helper.GetDataTable("[dbo].[GetSemesterList]", parameters);
            try
            {
                if (dtSemesters.Rows.Count > 0)
                {


                    obj.Semesters = dtSemesters.AsEnumerable().Select(row =>
                                              new Semester
                                              {
                                                  TermCode = Convert.ToString(row["TermCode"]),
                                                  TermName = Convert.ToString(row["Name"])
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

        public AppliedFormsResponse GetAppliedForms(int userID, int applicationTypeID)
        {
            AppliedFormsResponse obj = new AppliedFormsResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserId", SqlDbType.Int, 50) { Value = userID },
                                          new SqlParameter("@ApplicationTypeID", SqlDbType.Int, 50) { Value = applicationTypeID }
                                        };

            DataTable dtAppliedForms = _helper.GetDataTable("[dbo].[GetAppliedForms]", parameters);
            try
            {
                if (dtAppliedForms.Rows.Count > 0)
                {


                    obj.appliedForms = dtAppliedForms.AsEnumerable().Select(row =>
                                              new AppliedForms
                                              {
                                                  formID = Convert.ToInt32(row["ID"]),
                                                  ProgramID = Convert.ToInt32(row["ProgramID"]),
                                                  formStateID = Convert.ToInt32(row["FormStateID"]),
                                                  status = Convert.ToString(row["Status"]),
                                                  programName = Convert.ToString(row["Name"]),
                                                  semester = Convert.ToString(row["Semester"]),
                                                  TermCode = Convert.ToString(row["TermCode"]),
                                                  appliedDate = Convert.ToDateTime(row["AppliedDate"])

                                              }).ToList();


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

        public OptionItemListResponse GetIntialCreditialOptionItemsList(int programID)
        {
            OptionItemListResponse obj = new OptionItemListResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@ProgramID", SqlDbType.Int, 50) { Value = programID }
                                        };

            DataTable dtOptionsList = _helper.GetDataTable("[dbo].[getIntialCreditialOptionItemsList]", parameters);
            try
            {
                if (dtOptionsList.Rows.Count > 0)
                {
                    obj.OptionItemList = dtOptionsList.AsEnumerable().Select(row =>
                                             new OptionItemList
                                             {
                                                 OptionItemsList = Convert.ToString(row["IntialCreditialOptionItemsList"])

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

        public FormDispositionsAssessmentResponse GetFormDispositionsAssessment(FormDispositionsAssessmentRequest input)
        {
            FormDispositionsAssessmentResponse obj = new FormDispositionsAssessmentResponse();


            SqlParameter[] parameters =
                                        {
                                          //new SqlParameter("@FormDispositionsAssessmentID", SqlDbType.BigInt) { Value = input.FormDispositionsAssessmentID },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode }
                                        };

            DataTable dtDispositions = _helper.GetDataTable("[Application].[GetFormDispositionsAssessment]", parameters);
            try
            {
                if (dtDispositions.Rows.Count > 0)
                {


                    obj = dtDispositions.AsEnumerable().Select(row =>
                                              new FormDispositionsAssessmentResponse
                                              {
                                                  FormDispositionsAssessmentID = Convert.ToInt32(row["FormDispositionsAssessmentID"]),
                                                  FormID = Convert.ToInt32(row["FormID"]),
                                                  DispositionsAssessmentForm = Convert.ToString(row["DispositionsAssessmentForm"])

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

        public BaseResponse UpsertFormDispositionsAssessment(UpsertFormDispositionsAssessmentRequest input)
        {
            BaseResponse obj = new BaseResponse();

            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@FormDispositionsAssessmentID", SqlDbType.BigInt) { Value = input.FormDispositionsAssessmentID },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@DispositionsAssessmentForm", SqlDbType.NVarChar, -1) { Value = input.DispositionsAssessmentForm }
                                        };

            int identity = _helper.InsertTable("[Application].[UpsertFormDispositionsAssessment]", parameters);
            obj.IsSuccess = true;
            obj.Message = "Disposition Attachment Saved";

            return obj;
        }

        public FormSubSectionResponse GetFormSubSection(FormSubsectionRequest input)
        {
            FormSubSectionResponse obj = new FormSubSectionResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@SubSectionIdentifiers", SqlDbType.VarChar, 10) { Value = input.SubSectionIdentifiers }
                                        };

            DataTable dtSubSection = _helper.GetDataTable("[Application].[GetFormSubSection]", parameters);
            try
            {
                if (dtSubSection.Rows.Count > 0)
                {


                    obj = dtSubSection.AsEnumerable().Select(row =>
                                              new FormSubSectionResponse
                                              {
                                                  FormSubSectionID = Convert.ToInt32(row["FormSubSectionID"]),
                                                  FormID = Convert.ToInt32(row["FormID"]),
                                                  SubSectionIdentifiers = Convert.ToString(row["SubSectionIdentifiers"]),
                                                  SubSectionForm = Convert.ToString(row["SubSectionForm"]),
                                                  showUpdateFormSubSection = Convert.ToBoolean(row["showUpdateFormSubSection"])

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

        public BaseResponse UpsertFormSubSection(UpsertFormSubSectionRequest input)
        {
            BaseResponse obj = new BaseResponse();

            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@FormSubSectionID", SqlDbType.BigInt) { Value = input.FormSubSectionID },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@SubSectionForm", SqlDbType.NVarChar, -1) { Value = input.SubSectionForm },
                                          new SqlParameter("@SubSectionIdentifiers", SqlDbType.VarChar, 10) { Value = input.SubSectionIdentifiers }
                                        };

            int identity = _helper.InsertTable("[Application].[UpsertFormSubSection]", parameters);
            obj.IsSuccess = true;
            obj.Message = "Form Sub-Section Saved";

            return obj;
        }

        public FormSectionAttachmentResponse GetFormSubSectionAttachmentList(FormSubsectionRequest input)
        {
            FormSectionAttachmentResponse obj = new FormSectionAttachmentResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@SubSectionIdentifiers", SqlDbType.VarChar, 10) { Value = input.SubSectionIdentifiers } 
                                        };

            DataTable dtSubSection = _helper.GetDataTable("[Application].[GetFormSubSectionAttachmentList]", parameters);
            try
            {
                if (dtSubSection.Rows.Count > 0)
                {


                    obj.FormSectionAttachmentList = dtSubSection.AsEnumerable().Select(row =>
                                              new FormSectionAttachmentList
                                              {
                                                  FormSubSectionAttachmentID = Convert.ToInt32(row["FormSubSectionAttachmentID"]),
                                                  FormID = Convert.ToInt32(row["FormID"]),
                                                  FormSubSectionID = Convert.ToInt32(row["FormSubSectionID"]),
                                                  SubSectionIdentifiers = Convert.ToString(row["SubSectionIdentifiers"]),
                                                  FileName = Convert.ToString(row["FileName"]),
                                                  FileExtn = Convert.ToString(row["FileExtn"]),
                                                  CanView = Convert.ToBoolean(row["CanView"])

                                              }).ToList();


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
        private FormAttachmentFileNames GetFormSubSectionAttachmentFileName(int formID, string SubSectionIdentifiers)
        {
            FormAttachmentFileNames fileNames = new FormAttachmentFileNames();
            string savedFileName = string.Empty;
            SqlParameter[] parameters =
                                    {

                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID },
                                          new SqlParameter("@SubSectionIdentifiers", SqlDbType.VarChar,10) { Value = SubSectionIdentifiers }
                                    };
            DataTable dtName = _helper.GetDataTable("[Application].[GetFormSubSectionAttachmentFileName]", parameters);

            if (dtName.Rows.Count > 0)
            {
                fileNames.SavedFileName = Convert.ToString(dtName.Rows[0]["SavedFileName"]);
                fileNames.FileName = Convert.ToString(dtName.Rows[0]["FileName"]);
                fileNames.UserFolder = Convert.ToString(dtName.Rows[0]["UserFolder"]);
            }

            return fileNames;
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

        public BaseResponse SaveFormSubSectionAttachment(SaveFormSubSectionAttachmentRequest input)
        {
            BaseResponse response = new BaseResponse();
            if (input.FileContent != null && input.FileContent.Length > 0)
            {
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                string fileExtensionWord = string.Empty;
                string userFolderName = string.Empty;
                string savedFileName = string.Empty;
                string subSectionName = string.Empty;
                string[] subSectionNameSplit;
                bool isNotPDFExtension = false;
                var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];

                var workingFolderPath = Path.Combine(fileRepoPath, "WorkingFolder");

                // pull the saved file name format SP Below
                FormAttachmentFileNames fileNames = GetFormSubSectionAttachmentFileName(input.FormID, input.SubSectionIdentifiers);
                if (input.FileName != string.Empty)
                {
                    AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.FileName);
                    subSectionNameSplit = fileNames.FileName.Split('_');
                    subSectionName= subSectionNameSplit[0];
                    fileExtension = fileDetails.FileExtension;
                    if (fileExtension.ToUpper() == "PNG" || fileExtension.ToUpper() == "JPG" || fileExtension.ToUpper() == "JPEG")
                    {
                        // isNotPDFExtension = true;
                        // logic to convert png to pdf 
                        //byte[] imageContent = null;
                        //imageContent = GetImageFilecontent(input.FileContent);
                        //input.FileContent = null;
                        //input.FileContent = imageContent;
                        //fileExtension = "pdf";
                    }
                 

                }
                SqlParameter[] parameters =
                                         {
                                          new SqlParameter("@FormSubSectionID", SqlDbType.BigInt) { Value = input.FormSubSectionID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@SubSectionIdentifiers", SqlDbType.VarChar,10) { Value = input.SubSectionIdentifiers },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 200) { Value = fileNames.FileName },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = fileNames.SavedFileName },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID }
                                        };
                DataTable dtFormAttachment = _helper.GetDataTable("[Application].[SaveFormSubSectionAttachment]", parameters);
                if (dtFormAttachment.Rows.Count > 0 && input.FileName != string.Empty)
                {
                    string[] folderSplit = dtFormAttachment.Rows[0]["FolderName"].ToString().Split('~');
                    userFolderName = folderSplit[0].ToString();
                    string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
                    if (Directory.Exists(dirUserFolderPath))
                    {
                        
                        string dirForm = Path.Combine(dirUserFolderPath, "Form");
                        string subSection = Path.Combine(dirForm, subSectionName);
                        if (Directory.Exists(dirForm))
                        {
                            subSection = Path.Combine(dirForm, subSectionName);
                            // copy the file here 
                            if (isNotPDFExtension)
                            {
                                //byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                            }
                            else
                            {
                                Directory.CreateDirectory(subSection);
                                File.WriteAllBytes(Path.Combine(subSection, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(dirForm);
                            if (isNotPDFExtension)
                            {
                                //byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                            }
                            else
                            {
                                Directory.CreateDirectory(subSection);
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                            }
                        }
                    }
                    else
                    {
                        string dirForm = Path.Combine(dirUserFolderPath, "Form");
                        string subSection= Path.Combine(dirForm, subSectionName);
                        DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                        DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirForm);
                        DirectoryInfo dirsubsectionFolder = System.IO.Directory.CreateDirectory(subSection);
                        DirectorySecurity dSecurity = dirsubsectionFolder.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dirsubsectionFolder.SetAccessControl(dSecurity);
                        if (isNotPDFExtension)
                        {
                            //byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                        }
                        else
                        {
                            File.WriteAllBytes(Path.Combine(subSection, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                        }
                    }
                    // now delete the old file based on the file name return from DB call above 
                }

                response.IsSuccess = true;
                response.Message = "Form attachment Uploaded Successfully";

                return response;
            }
            else
            {
                response.IsSuccess = true;
                response.Message = "No Attachment to upload";

                return response;
            }
        }



        public FormSubsectionAttachmentDownloadResponse GetFormSubSectionAttachment(SubsectionAttachmentDownloadRequest input)
        {
            FormSubsectionAttachmentDownloadResponse obj = new FormSubsectionAttachmentDownloadResponse();

            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@FormSubSectionID", SqlDbType.BigInt) { Value = input.FormSubSectionID },
                                          new SqlParameter("@SubSectionIdentifiers", SqlDbType.VarChar,10) { Value = input.SubSectionIdentifiers },
                                          new SqlParameter("@FormSubSectionAttachmentID", SqlDbType.BigInt) { Value = input.FormSubSectionAttachmentID }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[Application].[GetFormSubSectionAttachment]", parameters);

            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new FormSubsectionAttachmentDownloadResponse
                                          {
                                              ID = Convert.ToInt32(row["ID"]),
                                              FormID = Convert.ToInt32(row["FormID"]),
                                              FormSubSectionID = Convert.ToInt32(row["FormSubSectionID"]),
                                              FileName = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                              FolderName = Convert.ToString(row["FolderName"]),
                                              CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                                              CreatedDate = Convert.ToDateTime(row["CreatedDate"] == DBNull.Value ? null : row["CreatedDate"]),
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(_utils.GetAttachmentsFolderName(row["FolderName"].ToString()), _utils.GetAttachmentsSavedFileName(row["FolderName"].ToString()) + "." + Convert.ToString(row["FileExtn"]))
                                          }).FirstOrDefault();
            obj.IsSuccess= true;
            obj.Message = "Attachment retrieved Successfully";

            return obj;
        }
        public byte[] GetFileContent(string userFolderPath, string fileName)
        {
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            string[] pathSplitter = fileName.ToString().Split('_');
            string identifierFolder = pathSplitter[1].ToString();
            string userPath=Path.Combine(fileRepoPath, Path.Combine(userFolderPath, "Form"));
            string filepath= Path.Combine(userPath, Path.Combine(identifierFolder, fileName));
            //string filepath = Path.Combine(fileRepoPath, Path.Combine(userFolderPath, fileName));
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

        public byte[] GetUnOfficialTranscriptsFileContent(string userFolderPath, string fileName)
        {
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            string[] pathSplitter = fileName.ToString().Split('_');
            string fullFileName= pathSplitter[2].ToString();
           // string identifierFolder = pathSplitter[1].ToString();
            string userPath = Path.Combine(fileRepoPath, Path.Combine(userFolderPath, "Form"));
            string filepath = Path.Combine(userPath, fileName);
           // string filepath = Path.Combine(fileRepoPath, Path.Combine(userFolderPath, fileName));
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

        public FormPrerequisitesResponse GetFormPrerequisites(int UserId, int FormID)
        {
            FormPrerequisitesResponse obj = new FormPrerequisitesResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserId", SqlDbType.Int, 50) { Value = UserId },
                                          new SqlParameter("@FormID", SqlDbType.Int, 50) { Value = FormID }
                                        };

            DataTable dsAttachments = _helper.GetDataTable("[Application].[GetFormPrerequisites]", parameters);
            try
            {
                if (dsAttachments.Rows.Count > 0)
                {
                    obj.FormPrerequisites = dsAttachments.AsEnumerable().Select(row =>
                                              new FormPrerequisites
                                              {
                                                  fieldWorkAttachmentID = Convert.ToInt32(row["fieldWorkAttachmentID"]),
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
        public UpsertFormEducationInformationAttachmentResponse UpsertFormEducationInformationAttachment(UpsertFormEducationInformationAttachmentRequest input)
        {
            UpsertFormEducationInformationAttachmentResponse response = new UpsertFormEducationInformationAttachmentResponse();
            if (input.FileContent != null && input.FileContent.Length > 0)
            {
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                string fileExtensionWord = string.Empty;
                string userFolderName = string.Empty;
                string savedFileName = string.Empty;
                string subSectionName = string.Empty;
                string[] subSectionNameSplit;
                bool isNotPDFExtension = false;
                var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];

                var workingFolderPath = Path.Combine(fileRepoPath, "WorkingFolder");

                // pull the saved file name format SP Below
                FormAttachmentFileNames fileNames = GetFormEducationAttachmentFileName(input.FormID);
                if (input.FileName != string.Empty)
                {
                    AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.FileName);
                    subSectionNameSplit = fileNames.FileName.Split('_');
                    subSectionName = subSectionNameSplit[0];
                    fileExtension = fileDetails.FileExtension;
                    if (fileExtension.ToUpper() == "PNG" || fileExtension.ToUpper() == "JPG" || fileExtension.ToUpper() == "JPEG")
                    {
                        // isNotPDFExtension = true;
                        // logic to convert png to pdf 
                        //byte[] imageContent = null;
                        //imageContent = GetImageFilecontent(input.FileContent);
                        //input.FileContent = null;
                        //input.FileContent = imageContent;
                        //fileExtension = "pdf";
                    }


                }
                
                SqlParameter[] parameters =
                                         {
                                          new SqlParameter("@UniqueID", SqlDbType.UniqueIdentifier) { Value = input.UniqueID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 200) { Value = fileNames.FileName },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = fileNames.SavedFileName },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID }
                                        };
                DataTable dtFormAttachment = _helper.GetDataTable("[Application].[UpsertFormEducationInformationAttachment]", parameters);
                if (dtFormAttachment.Rows.Count > 0 && input.FileName != string.Empty)
                {
                    string[] folderSplit = dtFormAttachment.Rows[0]["FolderName"].ToString().Split('~');
                    userFolderName = folderSplit[0].ToString();
                    string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
                    if (Directory.Exists(dirUserFolderPath))
                    {

                        string dirForm = Path.Combine(dirUserFolderPath, "Form");
                        //string subSection = Path.Combine(dirForm, subSectionName);
                        if (Directory.Exists(dirForm))
                        {
                            //subSection = Path.Combine(dirForm, subSectionName);
                            // copy the file here 
                            if (isNotPDFExtension)
                            {
                                //byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                            }
                            else
                            {
                               // Directory.CreateDirectory(subSection);
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(dirForm);
                            if (isNotPDFExtension)
                            {
                                //byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                            }
                            else
                            {
                               // Directory.CreateDirectory(subSection);
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                            }
                        }
                    }
                    else
                    {
                        string dirForm = Path.Combine(dirUserFolderPath, "Form");
                        //string subSection = Path.Combine(dirForm, subSectionName);
                        DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                        DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirForm);
                        //DirectoryInfo dirsubsectionFolder = System.IO.Directory.CreateDirectory(subSection);
                        DirectorySecurity dSecurity = dirFieldWorkFolder.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dirFieldWorkFolder.SetAccessControl(dSecurity);
                        if (isNotPDFExtension)
                        {
                            //byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                        }
                        else
                        {
                            File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                        }
                    }
                    response.FileName = Convert.ToString(dtFormAttachment.Rows[0]["FileName"]);
                    response.FileExtn = Convert.ToString(dtFormAttachment.Rows[0]["FileExtn"]);
                    response.UniqueID = (Guid)(dtFormAttachment.Rows[0]["UniqueID"]);
                    response.FormEducationInformationAttachmentID = Convert.ToInt32(dtFormAttachment.Rows[0]["FormEducationInformationAttachmentID"]);
                    response.FormID = Convert.ToInt32(dtFormAttachment.Rows[0]["FormID"]);
                    response.IsSuccess = true;
                    response.Message = "Form attachment Uploaded Successfully";
                    // now delete the old file based on the file name return from DB call above 
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
        private FormAttachmentFileNames GetFormEducationAttachmentFileName(int formID)
        {
            FormAttachmentFileNames fileNames = new FormAttachmentFileNames();
            string savedFileName = string.Empty;
            SqlParameter[] parameters =
                                    {

                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID }
                                    };
            DataTable dtName = _helper.GetDataTable("[Application].[GetFormEducationInformationAttachmentFileName]", parameters);

            if (dtName.Rows.Count > 0)
            {
                fileNames.SavedFileName = Convert.ToString(dtName.Rows[0]["SavedFileName"]);
                fileNames.FileName = Convert.ToString(dtName.Rows[0]["FileName"]);
                fileNames.UserFolder = Convert.ToString(dtName.Rows[0]["UserFolder"]);
            }

            return fileNames;
        }
        public DownloadEducationalInformationalAttachment GetFormEducationInformationAttachment(int FormID, Guid UniqueID)
        {
            DownloadEducationalInformationalAttachment obj = new DownloadEducationalInformationalAttachment();

            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID },
                                          new SqlParameter("@UniqueID", SqlDbType.UniqueIdentifier) { Value = UniqueID }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[Application].[GetFormEducationInformationAttachment]", parameters);
            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new DownloadEducationalInformationalAttachment
                                          {
                                              FormEducationInformationAttachmentID = Convert.ToInt32(row["FormEducationInformationAttachmentID"]),
                                              FormID = Convert.ToInt32(row["FormID"]),
                                              FormUniqueID = Guid.Parse(row["UniqueID"].ToString()),
                                              FileName = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                              FolderName = Convert.ToString(row["FolderName"]),
                                              CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                                              CreatedDate = Convert.ToDateTime(row["CreatedDate"] == DBNull.Value ? null : row["CreatedDate"]),
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetUnOfficialTranscriptsFileContent(_utils.GetAttachmentsFolderName(row["FolderName"].ToString()), _utils.GetAttachmentsSavedFileName(row["FolderName"].ToString()) + "." + Convert.ToString(row["FileExtn"]))
                                          }).FirstOrDefault();
            obj.IsSuccess = true;
            obj.Message = "Attachment retrieved Successfully";

            return obj;
        }
        public BaseResponse UpdateFormSubSectionApproveral(UpdateFormSubSectionApproveralRequest input)
        {
            BaseResponse obj = new BaseResponse();

            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@FormSubSectionID", SqlDbType.BigInt) { Value = input.FormSubSectionID },
                                          new SqlParameter("@SubSectionIdentifiers", SqlDbType.VarChar, 10) { Value = input.SubSectionIdentifiers },
                                          new SqlParameter("@ApproverUserID", SqlDbType.BigInt) { Value = input.ApproverUserID },
                                          new SqlParameter("@ApproverComments", SqlDbType.NVarChar, -1) { Value = input.ApproverComments },
                                          new SqlParameter("@IsApproved", SqlDbType.Bit) { Value = input.IsApproved }
                                          
                                          
                                        };

            int identity = _helper.InsertTable("[Application].[UpdateFormSubSectionApproveral]", parameters);
            obj.IsSuccess = true;
            obj.Message = "Subsection saved successfully";

            return obj;
        }

        public FormSectionApprovalDetailsResponse GetFormSubSectionApproveralDetails(int FormID, int UserID, int FormSubSectionID, string SubSectionIdentifiers)
        {
            FormSectionApprovalDetailsResponse obj = new FormSectionApprovalDetailsResponse();

            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                          new SqlParameter("@FormSubSectionID", SqlDbType.BigInt) { Value = FormSubSectionID },
                                          new SqlParameter("@SubSectionIdentifiers", SqlDbType.VarChar,10) { Value = SubSectionIdentifiers }
                                     };
            DataTable dtApprovalDetails = _helper.GetDataTable("[Application].[GetFormSubSectionApproveralDetails]", parameters);
            if (dtApprovalDetails.Rows.Count > 0)
            {
                obj = dtApprovalDetails.AsEnumerable().Select(row =>
                                              new FormSectionApprovalDetailsResponse
                                              {
                                                  ID = Convert.ToInt32(row["ID"]),
                                                  FormID = Convert.ToInt32(row["FormID"]),
                                                  SubSectionIdentifiers = Convert.ToString(row["SubSectionIdentifiers"]),
                                                  ApproverUserID = Convert.ToInt32(row["ApproverUserID"]==DBNull.Value ? null : row["ApproverUserID"]),
                                                  ApprovedBy = Convert.ToString(row["ApprovedBy"]),
                                                  isApproved = Convert.ToBoolean(row["isApproved"]==DBNull.Value ? null: row["isApproved"]),
                                                  ApproverComments = Convert.ToString(row["ApproverComments"]),
                                                  ApprovedOn = Convert.ToDateTime(row["ApproveredOn"] == DBNull.Value ? null : row["ApproveredOn"]),
                                                  showSubSectionApproveral = Convert.ToBoolean(row["showSubSectionApproveral"] == DBNull.Value ? null : row["showSubSectionApproveral"]),
                                                  canUpdateSubSectionApproveral = Convert.ToBoolean(row["canUpdateSubSectionApproveral"] == DBNull.Value ? null : row["canUpdateSubSectionApproveral"]),
                                                  ReviewedByText = Convert.ToString(row["ReviewedByText"])

                                              }).FirstOrDefault();
                obj.IsSuccess = true;
                obj.Message = "Approval details fetched";
            }

            return obj;
        }
    }
}
