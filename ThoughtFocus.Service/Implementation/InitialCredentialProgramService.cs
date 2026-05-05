using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Domain.Request.GraduateProgram;
using ThoughtFocus.Domain.Request.InitialCredentialProgram;
using ThoughtFocus.Domain.Request.Travel;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Admin;
using ThoughtFocus.Domain.Response.Application;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;
using ThoughtFocus.Service.Interfaces;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Domain.Enumeration;
using static ThoughtFocus.Service.Implementation.InitialCredentialProgramService;
using FormAttachments = ThoughtFocus.Domain.Request.InitialCredentialProgram.FormAttachments;
using Newtonsoft.Json.Linq;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.Office.Interop.Word;
using ThoughtFocus.Domain.FormModels;
using System.Drawing;
using ThoughtFocus.Domain.Response.Program.TemplateResponse;

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
                                              new Domain.Response.GraduateProgram.Semester
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
                                          new SqlParameter("@SubSectionIdentifiers", SqlDbType.VarChar, 10) { Value = input.SubSectionIdentifiers },
                                          new SqlParameter("@Identifier", SqlDbType.VarChar, 20) { Value = input.Identifier }
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
                                          new SqlParameter("@SubSectionIdentifiers", SqlDbType.VarChar,100) { Value = SubSectionIdentifiers }
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

                    string dirForm = Path.Combine(dirUserFolderPath, "Form");
                    string subSection = Path.Combine(dirForm, subSectionName);
                    //if (Directory.Exists(dirUserFolderPath))
                    //{
                    if (!Directory.Exists(dirUserFolderPath))
                    {
                        DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                        DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirForm);
                        DirectoryInfo dirsubsectionFolder = System.IO.Directory.CreateDirectory(subSection);
                        DirectorySecurity dSecurity = dirsubsectionFolder.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dirsubsectionFolder.SetAccessControl(dSecurity);
                    }
                    //string dirForm = Path.Combine(dirUserFolderPath, "Form");
                    //string subSection = Path.Combine(dirForm, subSectionName);
                    if (!Directory.Exists(dirForm))
                    {
                        DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirForm);
                        DirectoryInfo dirsubsectionFolder = System.IO.Directory.CreateDirectory(subSection);
                        DirectorySecurity dSecurity = dirsubsectionFolder.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dirsubsectionFolder.SetAccessControl(dSecurity);

                    }
                    if (!Directory.Exists(subSection))
                    {
                        DirectoryInfo dirsubsectionFolder = System.IO.Directory.CreateDirectory(subSection);
                        DirectorySecurity dSecurity = dirsubsectionFolder.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dirsubsectionFolder.SetAccessControl(dSecurity);
                    }
                    File.WriteAllBytes(Path.Combine(subSection, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                    //if (Directory.Exists(dirForm))
                    //{
                    //    subSection = Path.Combine(dirForm, subSectionName);
                    //    // copy the file here 
                    //    //if (isNotPDFExtension)
                    //    //{
                    //    //    //byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                    //    //}
                    //    if (Directory.Exists(subSection))
                    //    {

                    //    }
                    //    else
                    //    {
                    //        Directory.CreateDirectory(subSection);
                    //        File.WriteAllBytes(Path.Combine(subSection, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                    //    }
                    //}
                    //else
                    //{
                    //    Directory.CreateDirectory(dirForm);
                    //    if (isNotPDFExtension)
                    //    {
                    //        //byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                    //    }
                    //    else
                    //    {
                    //        Directory.CreateDirectory(subSection);
                    //        File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                    //    }
                    //}
                    //}
                    //else
                    //{
                    //    string dirForm = Path.Combine(dirUserFolderPath, "Form");
                    //    string subSection= Path.Combine(dirForm, subSectionName);
                    //    DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                    //    DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirForm);
                    //    DirectoryInfo dirsubsectionFolder = System.IO.Directory.CreateDirectory(subSection);
                    //    DirectorySecurity dSecurity = dirsubsectionFolder.GetAccessControl();
                    //    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    //    dirsubsectionFolder.SetAccessControl(dSecurity);
                    //    if (isNotPDFExtension)
                    //    {
                    //        //byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                    //    }
                    //    else
                    //    {
                    //        File.WriteAllBytes(Path.Combine(subSection, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                    //    }
                    //}
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
                                          new SqlParameter("@IsApproved", SqlDbType.Bit) { Value = input.IsApproved },
                                          new SqlParameter("@Status", SqlDbType.VarChar, 20) { Value = input.Status}

                                        };

            DataTable applicantInfo = _helper.GetDataTable("[Application].[UpdateFormSubSectionApproveral]", parameters);
            string applicantName = string.Empty;
            string applicantEmail = string.Empty;
            string programName = string.Empty;
            int programID = 0;
            //string Subject = string.Empty;
            string body = string.Empty;
            string logopath = Path.GetFullPath("SupportFiles/Img/logo.jpeg");
            string logoText = "cid:myImageID";
            //string signatureText = "cid:mySignatureImageID";
            string date= DateTime.Now.ToString("MM-dd-yyyy");
            string templateName = string.Empty;
            if (applicantInfo.Rows.Count>=0)
            {
                applicantName = applicantInfo.Rows[0]["ApplicantName"].ToString();
                applicantEmail = applicantInfo.Rows[0]["ApplicantEmail"].ToString();
                //applicantEmail = "chandana.shankaregowda@thoughtfocus.com";
                programName = applicantInfo.Rows[0]["ProgramName"].ToString();
                programID = Convert.ToInt32(applicantInfo.Rows[0]["ProgramID"]);
            }
            if (input.Status != string.Empty)
            {
                EmailResult emailResult = GetMailTemplate(templateName, input, programName, logoText, applicantName, date,programID);
                try
                {
                    _sendMail.SendEmail(applicantEmail, "", "COMMON", emailResult.Subject, emailResult.Body, "");
                }
                catch (Exception ee)
                {
                    obj.IsSuccess = false;
                    obj.Message = "BSR/SMC/GPA submit or not submitted status send mail failure.";
                }
            }

            obj.IsSuccess = true;
            obj.Message = "Subsection saved successfully";

            return obj;
        }
        private EmailResult GetMailTemplate(string templateName, UpdateFormSubSectionApproveralRequest input,string programName,string logoText,string applicantName,string date,int programID)
        {
            string Subject = string.Empty;
            EmailResult obj=new EmailResult();
            //if (input.SubSectionIdentifiers == "BSR")
            //{
            //    switch (input.Status)
            //    {
            //        case "Met":
            //            templateName = "BSRMetMailTemplate.html";
            //            Subject = "MyCED BSR Review Met";
            //            break;
            //        case "Not Met":
            //            templateName = "BSRNotMetMailTemplate.html";
            //            Subject = "MyCED BSR Review Not Met";
            //            break;
            //    }
            //}
            //else if (input.SubSectionIdentifiers == "SMC")
            //{
            //    switch (input.Status)
            //    {
            //        case "Met":
            //            templateName = "SMC_Met_MailTemplate.html";
            //            Subject = "MyCED SMC Review Met";
            //            break;
            //        case "Not Met":
            //            if (programName == "Education Specialist Credential Program (ESCP)")
            //                templateName = "SMC_NotMet_For_ESCP_MailTemplate.html";
            //            else if (programName == "Multiple Subject Credential Program (MSCP)")
            //                templateName = "SMC_NotMet_For_MSCP_MailTemplate.html";
            //            else if (programName == "Single Subject Credential Program (SSCP)" || programName == "Urban Dual Credential Program (UDCP)")
            //                templateName = "SMC_NotMet_For_SSCP_UDCP_MailTemplate.html";
            //            else if (programName == "PK-3 Early Childhood Education Specialist Instruction Credential Program (PK-3CP)")
            //                templateName = "SMC_NotMet_For_PK3_MailTemplate.html";
            //            Subject = "MyCED SMC Review Not Met";
            //            break;
            //        case "Will Meet":
            //            templateName = "SMC_WillMeet_MailTemplate.html";
            //            Subject = "Subject Matter Competency - Will Meet";
            //            break;
            //    }
            //}
            //else if (input.SubSectionIdentifiers == "GPA")
            //{
            //    switch (input.Status)
            //    {
            //        case "Met":
            //            templateName = "GPA_Met_MailTemplate.html";
            //            Subject = "MyCED GPA Review Met";
            //            break;
            //        case "Not Met":
            //            if (programName == "Single Subject Credential Program (SSCP)")
            //                templateName = "GPA_Not_Met_For_SSCP_MailTemplate.html";
            //            else
            //                templateName = "GPA_NotMet_MailTemplate.html";
            //            Subject = "MyCED GPA Review Not Met";
            //            break;
            //    }
            //}
            //obj.Subject = Subject;
            //obj.Body = GetMailBodyTemplate(templateName);
            SqlParameter[] parameters ={
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
                                            new SqlParameter("@SectionName", SqlDbType.VarChar,10) { Value = input.SubSectionIdentifiers},
                                            new SqlParameter("@CategoryName", SqlDbType.VarChar,20) { Value = input.Status },
                                            new SqlParameter("@Identifier", SqlDbType.VarChar,20) { Value = ""},
                                            new SqlParameter("@Name", SqlDbType.VarChar,50) { Value = "" }
                                       };
            DataTable dtMailBody = _helper.GetDataTable("[dbo].[GetAdmissionRequirementsMailBody]", parameters);
            if (dtMailBody.Rows.Count > 0)
            {
                if (dtMailBody.Rows[0]["EmailBody"] != DBNull.Value)
                {
                    obj.Body = Convert.ToString(dtMailBody.Rows[0]["EmailBody"]);
                    obj.Subject = Convert.ToString(dtMailBody.Rows[0]["Subject"]);
                    string beforeBody = string.Empty;
                    string afterBody = string.Empty;
                    beforeBody = "<html>\r\n<head>\r\n</head>\r\n<body>\r\n<img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" />\r\n<div style=\"width: 100%; border-bottom: 2px solid black; font-family: Arial; margin-top: 10px;\">STUDENT SUCCESS AND ADVISING CENTER</div>\r\n<div>";
                    afterBody = "</div>\r\n\r\n</body>\r\n</html>";
                    obj.Body = $"{beforeBody}{obj.Body}{afterBody}";
                }
            }
            obj.Body = obj.Body.Replace("[[logoPath]]", logoText)
                               .Replace("[[applicantName]]", applicantName)
                               .Replace("[[subSectionIdentifer]]", input.SubSectionIdentifiers)
                               .Replace("[[programName]]", programName)
                               .Replace("[[date]]", date);
            return obj;

        }
        public class EmailResult
        {
            public string Body { get; set; }
            public string Subject { get; set; }
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
                                                  ReviewedByText = Convert.ToString(row["ReviewedByText"]),
                                                  Status = Convert.ToString(row["Status"])

                                              }).FirstOrDefault();
                obj.IsSuccess = true;
                obj.Message = "Approval details fetched";
            }

            return obj;
        }
        public AdditionalOfficialDocumentsResponse GetAdditionalOfficialDocuments(int UserID, int FormID, int ProgramID, string TermCode)
        {
            AdditionalOfficialDocumentsResponse obj = new AdditionalOfficialDocumentsResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar,10) { Value = TermCode }
                                     };
            DataTable dtOfficialDocumentsDetails = _helper.GetDataTable("[dbo].[GetAdditionalOfficialDocuments]", parameters);
            try
            {
                if (dtOfficialDocumentsDetails.Rows.Count > 0)
                {


                    obj.AdditionalOfficialDocuments = dtOfficialDocumentsDetails.AsEnumerable().Select(row =>
                                              new AdditionalOfficialDocuments
                                              {
                                                  AdditionalOfficialDocumentID = Convert.ToInt32(row["AdditionalOfficialDocumentID"]),
                                                  FormID = Convert.ToInt32(row["FormID"]),
                                                  DocumentID = Convert.ToInt32(row["DocumentID"]),
                                                  FileName = Convert.ToString(row["FileName"] == DBNull.Value ? null : row["FileName"]),
                                                  FileExtn = Convert.ToString(row["FileExtn"] == DBNull.Value ? null : row["FileExtn"]),
                                                  FolderName = Convert.ToString(row["FolderName"] == DBNull.Value ? null : row["FolderName"]),
                                                  UploadedBy = Convert.ToInt32(row["UploadedBy"] == DBNull.Value ? null : row["UploadedBy"]),
                                                  UploadedDate = Convert.ToDateTime(row["UploadedDate"] == DBNull.Value ? null : row["UploadedDate"]),
                                                  UploadedByName = Convert.ToString(row["UploadedByName"] == DBNull.Value ? null : row["UploadedByName"]),
                                                  CanView = Convert.ToString(row["CanView"])
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
        public BaseResponse UpdateAdditionalOfficialDocument(UpdateAdditionalOfficialDocumentRequest input)
        {
            BaseResponse response = new BaseResponse();
            string fileName = string.Empty;
            string fileExtension = string.Empty;
            string userFolderName = string.Empty;
            string savedFileName = string.Empty;
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];

            // pull the saved file name format SP Below
            FormAttachmentFileNames fileNames = GetFormAttachmentFileName(input.FormID, input.DocumentID);
            if (input.FileName != string.Empty)
            {
                AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.FileName);
                //fileName = fileDetails.FileName;
                fileExtension = fileDetails.FileExtension;
            }
            SqlParameter[] parameters =
                                      {
                                          new SqlParameter("@AdditionalOfficialDocumentID", SqlDbType.BigInt) { Value = input.AdditionalOfficialDocumentID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@FileName", SqlDbType.VarChar, 250) { Value = fileNames.FileName },
                                          new SqlParameter("@FileExtn", SqlDbType.VarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = fileNames.SavedFileName },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID }
                                      };
            DataTable dtFormAttachment = _helper.GetDataTable("[Application].[UpdateAdditionalOfficialDocument]", parameters);
            if (dtFormAttachment.Rows.Count > 0 && input.FileName != string.Empty)
            {
                string[] folderSplit = dtFormAttachment.Rows[0]["FolderName"].ToString().Split('~');
                userFolderName = folderSplit[0].ToString();
                string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
                if (Directory.Exists(dirUserFolderPath))
                {
                    string dirForm = Path.Combine(dirUserFolderPath, "Form");
                    if (Directory.Exists(dirForm))
                    {
                        File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                        
                    }
                    else
                    {
                        Directory.CreateDirectory(dirForm);
                        File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                        
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
                    File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                    
                }
                // now delete the old file based on the file name return from DB call above 
            }

            response.IsSuccess = true;
            response.Message = "Document Uploaded Successfully";
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
        public Domain.Request.InitialCredentialProgram.FormAttachments GetAdditionalOfficialDocument(GetAdditionalOfficialDocumentRequest input)
        {
            Domain.Request.InitialCredentialProgram.FormAttachments obj = new Domain.Request.InitialCredentialProgram.FormAttachments();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 20) { Value = input.TermCode },
                                          new SqlParameter("@AdditionalOfficialDocumentID", SqlDbType.BigInt) { Value = input.AdditionalOfficialDocumentID }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[Application].[GetAdditionalOfficialDocument]", parameters);

            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new Domain.Request.InitialCredentialProgram.FormAttachments
                                          {
                                              Filename = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContentAdditionalDocument(Path.Combine(GetAttachmentsFolderName(row["FolderName"].ToString()), "Form"), GetAttachmentsSavedFileName(row["FolderName"].ToString()) + "." + Convert.ToString(row["FileExtn"]))
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
        public byte[] GetFileContentAdditionalDocument(string userFolderPath, string fileName)
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
        public BaseResponse DeleteAdditionalOfficialDocument(DeleteAdditionalOfficialDocumentRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@AdditionalOfficialDocumentID ", SqlDbType.BigInt) { Value = input.AdditionalOfficialDocumentID },
                                        };
            int id = _helper.InsertTable("[Application].[deleteAdditionalOfficialDocument]", parameters);
            response.Message = "Attachment Deleted Successfully";
            response.IsSuccess = true;
            return response;
        }

        public BaseResponse UpdateFormSubSectionSubmitForReview(UpdateFormSubSectionSubmitForReviewRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@FormSubSectionID", SqlDbType.BigInt) { Value = input.FormSubSectionID },
                                          new SqlParameter("@SubSectionIdentifiers", SqlDbType.VarChar, 10) { Value = input.SubSectionIdentifiers },
                                          new SqlParameter("@IsSubmitForReview ", SqlDbType.Bit) { Value = input.IsSubmitForReview },
                                        };
            int id = _helper.InsertTable("[Application].[UpdateFormSubSectionSubmitForReview]", parameters);
            response.Message = "Form section submitted for Review";
            response.IsSuccess = true;
            return response;
        }

        public PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail PUNS_AutharizeCommunitySiteSupervisorDemonstrationTeacher(string CommunitySiteUserIdentifier, string CommunitySiteUserEmail)
        {
            PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail obj = new PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@CommunitySiteUserIdentifier", SqlDbType.Int, 50) { Value = CommunitySiteUserIdentifier },
                                          new SqlParameter("@CommunitySiteUserEmail", SqlDbType.Int, 50) { Value = CommunitySiteUserEmail }
                                        };

            DataTable dtAppliedForms = _helper.GetDataTable("[FieldWork].[PUNS_AutharizeCommunitySiteSupervisorDemonstrationTeacher]", parameters);
            try
            {
                if (dtAppliedForms.Rows.Count > 0)
                {


                    obj = dtAppliedForms.AsEnumerable().Select(row =>
                                              new PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail
                                              {
                                                  CSSDTID = Convert.ToInt32(row["CSSDTID"]),
                                                  CommunitySiteUserName = Convert.ToString(row["CommunitySiteUserName"]),
                                                  CommunitySiteUserEmail = Convert.ToString(row["CommunitySiteUserEmail"]),
                                                  CommunitySiteUserIdentifier = Convert.ToString(row["CommunitySiteUserIdentifier"])

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

        PUNS_GetCommunitySiteSupervisorDemonstrationTeacher_ToSendMail IInitialCredentialProgramService.PUNS_AutharizeCommunitySiteSupervisorDemonstrationTeacher(string CommunitySiteUserIdentifier, string CommunitySiteUserEmail)
        {
            throw new NotImplementedException();
        }
        public LetterOfRecommendationsByFormIDResponse GetLetterOfRecommendationsByFormID(int UserID, int FormID, int ProgramID, string TermCode)
        {
            LetterOfRecommendationsByFormIDResponse obj = new LetterOfRecommendationsByFormIDResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar,10) { Value = TermCode }
                                     };
            DataTable letterOfRecommendationDetails = _helper.GetDataTable("[Application].[GetLetterOfRecommendationsByFormID]", parameters);
            try
            {
                if (letterOfRecommendationDetails!=null)
                {
                    if (letterOfRecommendationDetails.Rows.Count > 0)
                    {
                        obj.LetterOfRecommendationsByFormID = letterOfRecommendationDetails.AsEnumerable().Select(row =>
                                           new LetterOfRecommendationsByFormID
                                           {
                                               LetterOfRecommendationID = Convert.ToInt32(row["LetterOfRecommendationID"]),
                                               FormID = Convert.ToInt32(row["FormID"]),
                                               RecommenderName = Convert.ToString(row["RecommenderName"]),
                                               RecommenderEmail = Convert.ToString(row["RecommenderEmail"]),
                                               CreatedBy = Convert.ToInt32(row["CreatedBY"]),
                                               CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                                               RecommenderURL = Convert.ToString(row["RecommenderURL"]),
                                               RecommenderURLValidTill = Convert.ToDateTime(row["RecommenderURLValidTill"] == DBNull.Value ? null : row["RecommenderURLValidTill"]),
                                               RecommenderIdentifier = Convert.ToString(row["RecommenderIdentifier"]),
                                               isMailSent = Convert.ToBoolean(row["isMailSent"]),
                                               LetterOfRecommendationJSON = Convert.ToString(row["LetterOfRecommendationJSON"] == DBNull.Value ? null : row["LetterOfRecommendationJSON"]),
                                               CanView = Convert.ToBoolean(row["CanView"]),
                                               FileLink = Convert.ToString(row["FileLink"]),
                                               ApplicationType = Convert.ToString(row["ApplicationType"])
                                           }).ToList();
                    }
                    else
                    {
                        List<LetterOfRecommendationsByFormID> lstRec = new List<LetterOfRecommendationsByFormID>();
                        LetterOfRecommendationsByFormID objRec=new LetterOfRecommendationsByFormID();
                        objRec.LetterOfRecommendationID = 0;
                        objRec.FormID = FormID;
                        objRec.RecommenderName = String.Empty;
                        objRec.RecommenderEmail = String.Empty;
                        lstRec.Add(objRec);
                        obj.LetterOfRecommendationsByFormID = lstRec; 
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
        public LetterOfRecommendationsByRecommenderIdentifierResponse GetLetterOfRecommendationsByRecommenderIdentifier(string recommenderIdentifier)
        {
            LetterOfRecommendationsByRecommenderIdentifierResponse obj = new LetterOfRecommendationsByRecommenderIdentifierResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@RecommenderIdentifier", SqlDbType.UniqueIdentifier) { Value = new Guid(recommenderIdentifier) }
                                     };
            DataTable letterOfRecommendationDetails = _helper.GetDataTable("[Application].[GetLetterOfRecommendationsByRecommenderIdentifier]", parameters);
                if (letterOfRecommendationDetails.Rows.Count > 0)
                {


                    obj.LetterOfRecommendationsByRecommenderIdentifier = letterOfRecommendationDetails.AsEnumerable().Select(row =>
                                              new LetterOfRecommendationsByRecommenderIdentifier
                                              {
                                                  LetterOfRecommendationID = Convert.ToInt32(row["LetterOfRecommendationID"]),
                                                  FormID = Convert.ToInt32(row["FormID"]),
                                                  RecommenderName = Convert.ToString(row["RecommenderName"]),
                                                  LetterOfRecommendationJSON = Convert.ToString(row["LetterOfRecommendationJSON"] == DBNull.Value ? null : row["LetterOfRecommendationJSON"]),
                                                  StudentName = Convert.ToString(row["StudentName"]),
                                                  StudentFirstName = Convert.ToString(row["StudentFirstName"]),
                                                  StudentLastName = Convert.ToString(row["StudentLastName"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),
                                                  StudentEmail = Convert.ToString(row["StudentEmail"]),
                                                  ProgramName = Convert.ToString(row["ProgramName"]),
                                                  TermName = Convert.ToString(row["TermName"]),
                                                  ProgramID = Convert.ToInt32(row["ProgramID"])
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
        public BaseResponse UpsertLetterOfRecommendations(UpsertLetterOfRecommendationsRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@LetterOfRecommendationID", SqlDbType.BigInt) { Value = input.LetterOfRecommendationID },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@RecommenderName", SqlDbType.VarChar,200) { Value = input.RecommenderName },
                                          new SqlParameter("@RecommenderEmail", SqlDbType.VarChar,200) { Value = input.RecommenderEmail }
                                        };

            DataSet recomDetails = _helper.GetDataSet("[Application].[UpsertLetterOfRecommendations]", parameters);
            string logoText = "cid:myImageID";
            string recommenderName = string.Empty;
            string recommenderEmail = string.Empty;
            string recommenderURL = string.Empty;
            string recommenderIdentifier = string.Empty;
            string applicantName = string.Empty;
            string body = string.Empty;
            string link = string.Empty;
            bool isMailSent = false;
            string subject = string.Empty;
            try
            {
                if (recomDetails.Tables[1].Rows.Count > 0)
                {
                    if (Convert.ToString(recomDetails.Tables[1].Rows[0]["Status"]) == "FAILURE")
                    {
                        response.Message = Convert.ToString(recomDetails.Tables[1].Rows[0]["Message"]);
                        response.IsSuccess = false;
                    }
                    else
                    {
                        if (recomDetails.Tables[0].Rows.Count > 0)
                        {
                            // send mail to the recommender with the URL link  
                            recommenderName = Convert.ToString(recomDetails.Tables[0].Rows[0]["RecommenderName"]);
                            recommenderEmail = Convert.ToString(recomDetails.Tables[0].Rows[0]["RecommenderEmail"]);
                            recommenderURL = Convert.ToString(recomDetails.Tables[0].Rows[0]["RecommenderURL"]);
                            recommenderIdentifier = Convert.ToString(recomDetails.Tables[0].Rows[0]["RecommenderIdentifier"]);
                            applicantName = Convert.ToString(recomDetails.Tables[0].Rows[0]["StudentName"]);
                            link = recommenderURL + recommenderIdentifier;
                            //get evaluation mail body
                            SqlParameter[] parameters1 ={
                                            new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt) { Value = 1 },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                            new SqlParameter("@Identifier", SqlDbType.NVarChar,50) { Value = "Evaluation Mail"}
                                       };
                            DataTable dtEval = _helper.GetDataTable("[Application].[GetEvaluatorEmail]", parameters1);
                            body = Convert.ToString(dtEval.Rows[0]["EvaluatorMailBody"]);
                            string beforeBody = string.Empty;
                            string afterBody = string.Empty;
                            beforeBody = "<html><body><div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                            afterBody = "</body></html>";
                            body = $"{beforeBody}{body}{afterBody}";
                            //body = GetMailBodyTemplateByProgramID(input.ProgramID);

                            body = body.Replace("[[logoPath]]", logoText)
                                .Replace("[[applicantname]]", applicantName)
                                .Replace("[[link]]", link);
                            if (input.ProgramID == 2)
                                subject = "CSULB MSCP Clinical Practice Evaluation Form";
                            if (input.ProgramID == 4)
                                subject = "CSULB SSCP Clinical Practice Evaluation Form";
                            _sendMail.SendEmail(recommenderEmail, "", "COMMON", subject, body, "");
                            isMailSent = true;
                            UpdateLetterOfRecommendationsMailSent(input, isMailSent);
                        }
                        response.Message = "Evaluation added and mail sent successfully";
                        response.IsSuccess = true;
                    }
                }


            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Recommendation mail send fail.";
            }
            return response;
        }
        private void UpdateLetterOfRecommendationsMailSent(UpsertLetterOfRecommendationsRequest input,bool isMailSent)
        {
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@LetterOfRecommendationID", SqlDbType.BigInt) { Value = input.LetterOfRecommendationID },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@IsMailSent", SqlDbType.Bit) { Value = isMailSent}
                                        };

            int ID = _helper.InsertTable("[Application].[UpdateLetterOfRecommendationsMailSent]", parameters);
         
        }
        public BaseResponse UpdateLetterOfRecommendationsJSON(UpdateLetterOfRecommendationsJSONRequest input)
        {
            BaseResponse response = new BaseResponse();
            UpsertLetterOfRecommendationsRequest upsertLetterOfRecommendations = new UpsertLetterOfRecommendationsRequest();
            string logoText = "cid:myImageID";
            string evaluatorEmail = string.Empty;
            string applicantName = string.Empty;
            string body = string.Empty;
            bool isMailSent = false;
            string subject = string.Empty;
            string beforeBody = string.Empty;
            string afterBody = string.Empty;
            string studentEmail = string.Empty;


            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@LetterOfRecommendationID", SqlDbType.BigInt) { Value = input.LetterOfRecommendationID },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@LetterOfRecommendationJSON", SqlDbType.VarChar, -1) { Value = input.LetterOfRecommendationJSON }
                                        };

            DataSet dtDLLOR = _helper.GetDataSet("[Application].[UpdateLetterOfRecommendationsJSON]", parameters);
            if (dtDLLOR.Tables[0].Rows.Count > 0)
            {
                evaluatorEmail = dtDLLOR.Tables[0].Rows[0]["EvaluatorEmail"] != DBNull.Value ? Convert.ToString(dtDLLOR.Tables[0].Rows[0]["EvaluatorEmail"]) : "";
                applicantName = dtDLLOR.Tables[0].Rows[0]["ApplicantName"] != DBNull.Value ? Convert.ToString(dtDLLOR.Tables[0].Rows[0]["ApplicantName"]) : "";
                studentEmail = dtDLLOR.Tables[0].Rows[0]["StudentEmail"] != DBNull.Value ? Convert.ToString(dtDLLOR.Tables[0].Rows[0]["StudentEmail"]) : "";
            }

            upsertLetterOfRecommendations.FormID = input.FormID;
            upsertLetterOfRecommendations.UserID = input.UserID;
            upsertLetterOfRecommendations.ProgramID = input.ProgramID;
            upsertLetterOfRecommendations.LetterOfRecommendationID = input.LetterOfRecommendationID;
            SqlParameter[] parameters1 ={
                                            new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt, 10) { Value = 1 },
                                            new SqlParameter("@ProgramId", SqlDbType.BigInt) { Value = input.ProgramID },
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
                    if (input.ProgramID == 2)
                        subject = "CSULB MSCP Clinical Practice Evaluation Submitted";
                    if (input.ProgramID == 4)
                        subject = "CSULB SSCP Clinical Practice Evaluation Submitted";
                    _sendMail.SendEmail(studentEmail, evaluatorEmail, "COMMON", subject, body, "");
                    isMailSent = true;
                    UpdateLetterOfRecommendationsMailSent(upsertLetterOfRecommendations, isMailSent);
                }
            }
            response.Message = "Data updated successfully";
            response.IsSuccess = true;
            return response;
        }
        public FormAttachments DownloadAttachment(DownloadAttachmentRequest input)
        {
            FormAttachments obj = new FormAttachments();
            byte[] fileContentJSONToPDF = new byte[0];
            if (input.ApplicationType == "ICP-SSCP" || input.ApplicationType == "FieldWork-SSCP")
            {
                fileContentJSONToPDF = GetPDFFromJSONForSSCP(input.LetterOfRecommendationJSON);
                obj.Filename = "Final Clinical Practice Evaluation" + "_" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf";
            }
            else if (input.ApplicationType == "ICP-MSCP" || input.ApplicationType == "FieldWork-MSCP")
            {
                fileContentJSONToPDF = GetPDFFromJSONForMSCP(input.LetterOfRecommendationJSON);
                obj.Filename = "Final Clinical Practice Evaluation" + "_" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf";
            }
            else if (input.ApplicationType == "FieldWork-PK3")
            {
                //input.LetterOfRecommendationJSON = "{\r\n\r\n\t\"context\": {\r\n\r\n\t\t\"courseNameAndNumber\": \"EDEC_400\",\r\n\r\n\t\t\"studentName\": \"Alexandria Chilver\",\r\n\r\n\t\t\"schoolName1\": \"california state university\",\r\n\r\n\t\t\"qualityOfWork\": \"5\",\r\n\r\n\t\t\"professionalDispositions\": [\r\n\r\n\t\t\t{\r\n\r\n\t\t\t\t\"criteria\": \"Preparation of materials and appropriate instruction\",\r\n\r\n\t\t\t\t\"value\": \"5 - Excellent\"\r\n\r\n\t\t\t},\r\n\r\n\t\t\t{\r\n\r\n\t\t\t\t\"criteria\": \"Appropriate behavior toward children\",\r\n\r\n\t\t\t\t\"value\": \"4 - Excellent\"\r\n\r\n\t\t\t},\r\n\r\n\t\t\t{\r\n\r\n\t\t\t\t\"criteria\": \"Professional dress and demeanor\",\r\n\r\n\t\t\t\t\"value\": \"3 - Acceptable\"\r\n\r\n\t\t\t},\r\n\r\n\t\t\t{\r\n\r\n\t\t\t\t\"criteria\": \"Courteous interactions with faculty, other students, staff and children\",\r\n\r\n\t\t\t\t\"value\": \"2 - Unacceptable\"\r\n\r\n\t\t\t},\r\n\r\n\t\t\t{\r\n\r\n\t\t\t\t\"criteria\": \"Regular, on-time attendance\",\r\n\r\n\t\t\t\t\"value\": \"1 - Unacceptable\"\r\n\r\n\t\t\t}\r\n\r\n\t\t],\r\n\r\n\t\t\"commentsOrConcerns\": \"comments added for test purpose 123 and abcd.\",\r\n\r\n\t\t\"teacherName\": \"sanju\",\r\n\r\n\t\t\"schoolName2\": \"california state university\",\r\n\r\n\t\t\"gradeLevel\": \"high\",\r\n\r\n\t\t\"signature\": \"sanju signature\",\r\n\r\n\t\t\"position\": \"dean\",\r\n\r\n\t\t\"date\": \"02/03/2026\",\r\n\r\n\t\t\"dateAndHours\": [\r\n\r\n\t\t\t{\r\n\r\n\t\t\t\t\"date\": \"02/11/2026\",\r\n\r\n\t\t\t\t\"hours\": 12\r\n\r\n\t\t\t},\r\n\r\n\t\t\t{\r\n\r\n\t\t\t\t\"date\": \"02/12/2026\",\r\n\r\n\t\t\t\t\"hours\": 13\r\n\r\n\t\t\t},\r\n\r\n\t\t\t{\r\n\r\n\t\t\t\t\"date\": \"02/19/2026\",\r\n\r\n\t\t\t\t\"hours\": 12.9\r\n\r\n\t\t\t}\r\n\r\n\t\t],\r\n\r\n\t\t\"totalHours\": \"37.9\"\r\n\r\n\t}\r\n\r\n}\r\n  ";
                fileContentJSONToPDF = GetPDFFromJSONForPK3(input.LetterOfRecommendationJSON);
                obj.Filename = "Final Clinical Practice Evaluation" + "_" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf";
            }
            obj.FileContent = fileContentJSONToPDF;
            return obj;
        }
        public BaseResponse DeleteSubSectionAttachments(DeleteSubSectionAttachmentRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@FileName", SqlDbType.VarChar, 100) { Value = input.FileName }
                                     };
            try
            {
                int id = _helper.InsertTable("[Application].[DeleteSubSectionAttachments]", parameters);
                response.Message = "Attachment Deleted Successfully";
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Data Retrieval Failed , Please contact site admin ";
                response.StackTrace = ex.Message;
            }
            return response;
        }
        public FormExperienceAttachmentResponse UpsertFormExperienceAttachment(FormExperienceAttachmentRequest input)
        {
            FormExperienceAttachmentResponse response = new FormExperienceAttachmentResponse();
            string currentDateTime = DateTime.Now.ToString("MMddyyyyHHmmss");
            if (input.FileContent != null && input.FileContent.Length > 0)
            {
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                string userFolderName = string.Empty;
                int userID = 0;
                var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];

                AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.GUID);
                input.GUID = fileDetails.FileName;
                fileName = input.FormID + "_" + "Experience" + "_" + currentDateTime;

                // get userID
                SqlParameter[] parameters =
                                 {
                                    new SqlParameter("@FormId", SqlDbType.Int) { Value = input.FormID }
                                 };

                DataTable dtForm = _helper.GetDataTable("[dbo].[GetFormDetails]", parameters);
                if (dtForm.Rows.Count > 0)
                {
                    userID = Convert.ToInt32(dtForm.Rows[0]["UserID"]);
                }
                string folderName = userID + "~" + fileName;
                if (input.GUID != string.Empty)
                {

                    fileExtension = fileDetails.FileExtension;
                    if (fileExtension.ToUpper() == "PNG" || fileExtension.ToUpper() == "JPG" || fileExtension.ToUpper() == "JPEG")
                    {
                        // logic to convert png to pdf 
                        byte[] imageContent = null;
                        imageContent = GetImageFilecontent(input.FileContent);
                        input.FileContent = null;
                        input.FileContent = imageContent;
                        fileExtension = "pdf";
                    }
                }

                SqlParameter[] parameters1 =
                                         {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userID},
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@GUID", SqlDbType.UniqueIdentifier, 250) { Value = new Guid(input.GUID) },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 250) { Value = fileName },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@FolderName", SqlDbType.VarChar, 100) { Value = folderName },
                                        };
                DataTable dtFormAttachment = _helper.GetDataTable("[dbo].[UpsertFormExperienceAttachment]", parameters1);
                string[] folderSplit = folderName.ToString().Split('~');
                userFolderName = folderSplit[0].ToString();
                string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
                if (Directory.Exists(dirUserFolderPath))
                {
                    string dirForm = Path.Combine(dirUserFolderPath, "Form");
                    if (Directory.Exists(dirForm))
                    {
                        {
                            File.WriteAllBytes(Path.Combine(dirForm, fileName + "." + fileExtension), input.FileContent);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(dirForm);
                        {
                            File.WriteAllBytes(Path.Combine(dirForm, fileName + "." + fileExtension), input.FileContent);
                        }
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
                    {
                        File.WriteAllBytes(Path.Combine(dirForm, fileName + "." + fileExtension), input.FileContent);
                    }
                }
                response.fileName = "Experience" + "_"+ currentDateTime;
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
        public FormAttachments DownloadFormExperienceAttachment(string GUID)
        {
            FormAttachments obj = new FormAttachments();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@GUID", SqlDbType.UniqueIdentifier) { Value = new Guid(GUID) }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[dbo].[GetFormExperienceAttachment]", parameters);

            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new FormAttachments
                                          {
                                              Filename = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtension"]),
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFormExperienceFileContent(Path.Combine(GetAttachmentsFolderName(row["FolderName"].ToString())), row["FileName"].ToString() + "." + Convert.ToString(row["FileExtension"]))
                                          }).FirstOrDefault();

            return obj;
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
        public byte[] GetFormExperienceFileContent(string userFolderPath, string fileName)
        {
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            string[] pathSplitter = fileName.ToString().Split('_');
            string fullFileName = pathSplitter[0].ToString();
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
        public byte[] GetPDFFromJSONForSSCP(string jsonString)
        {
                byte[] pdfFileContent = null;
                string evaluationTemplateBody = string.Empty;
                string logoPath = Path.GetFullPath("SupportFiles/Img/logo.jpeg");
                jsonString = jsonString.Replace("+", " ");
                JObject schema = JObject.Parse(jsonString);

                string candidateName = string.Empty;
                string demonstrationTeacherName = string.Empty;
                string candidateSpentHours = string.Empty;
                string spokenEnglish = string.Empty;
                string writtenEnglish = string.Empty;
                string demonstratesConfidence = string.Empty;
                string activeListeningSkills = string.Empty;
                string professionalAppearance = string.Empty;
                string relationshipsWithStaff = string.Empty;
                string professionalEthicalBehavior = string.Empty;
                string personalInteractionCourtesy = string.Empty;
                string dependability = string.Empty;
                string attendance = string.Empty;
                string punctuality = string.Empty;
                string sensitivityToDiversity = string.Empty;
                string interestEnthusiasmTeaching = string.Empty;
                string socialIntellectualMaturity = string.Empty;
                string asksAppropriateQuestions = string.Empty;
                string knowledgeOfSubjectMatter = string.Empty;
                string knowledgeOfStateContentStandards = string.Empty;
                string interestEnthusiasm = string.Empty;
                string appropriateReflection = string.Empty;
                string appropriateReflectionAnalysisOfObservedLessonDesign = string.Empty;
                string recognitionOfHowInstructionAligned = string.Empty;
                string recognitionOfHowInstructionDifferentiated = string.Empty;
                string appropriateInteractionsWithStudents = string.Empty;
                string attitudeOfClassTowardCandidate = string.Empty;
                string realisticExpectationsForBehavior = string.Empty;
                string supportsAndMotivatesStudents = string.Empty;
                string predictStudentFutureTeacher = string.Empty;
                string comment = string.Empty;


                JObject personalInfo = (JObject)schema["personalInfo"];
                JArray comunicationSkills = (JArray)personalInfo["comunicationSkills"];
                JArray professionalAttitudeBehavior = (JArray)personalInfo["professionalAttitudeBehavior"];
                JArray knowledgePedagogy = (JArray)personalInfo["knowledgePedagogy"];
                JArray StudentInteraction = (JArray)personalInfo["StudentInteraction"];
                JArray OverallAssessment = (JArray)personalInfo["OverallAssessment"];

                candidateName = Convert.ToString(personalInfo.GetValue("candidateName"));
                demonstrationTeacherName = Convert.ToString(personalInfo.GetValue("demonstrationTeacherName"));
                candidateSpentHours = Convert.ToString(personalInfo.GetValue("candidateSpentHours"));
                comment = Convert.ToString(schema.GetValue("comments"));
                //Comunication Skills
                foreach (JObject content in comunicationSkills.Children<JObject>())
                {
                    if (content["Category"].ToString() == "Spoken English")
                    {
                        spokenEnglish = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Written English (including emails)")
                    {
                        writtenEnglish = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Demonstrates confidence (e.g. body language, audible)")
                    {
                        demonstratesConfidence = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Active listening skills")
                    {
                        activeListeningSkills = Convert.ToString(content.GetValue("value"));
                    }
                }
                //Professional Attitude Behavior
                foreach (JObject content in professionalAttitudeBehavior.Children<JObject>())
                {
                    if (content["Category"].ToString() == "Professional appearance")
                    {
                        professionalAppearance = Convert.ToString(content.GetValue("value"));
                    }
                    if(content["Category"].ToString() == "Relationships with staff")
                    {
                        relationshipsWithStaff = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Professional/ethical behavior")
                    {
                        professionalEthicalBehavior = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Personal interaction and courtesy")
                    {
                        personalInteractionCourtesy = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Dependability")
                    {
                        dependability = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Attendance")
                    {
                        attendance = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Punctuality")
                    {
                        punctuality = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Sensitivity to diversity (e.g. gender, multicultural, LGBTQ , special needs)")
                    {
                        interestEnthusiasmTeaching= Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Interest & enthusiasm for teaching")
                    {
                        dependability = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Social and intellectual maturity")
                    {
                        socialIntellectualMaturity = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Asks appropriate questions about teaching practice and student learning")
                    {
                        asksAppropriateQuestions = Convert.ToString(content.GetValue("value"));
                    }
                }
                //knowledgePedalogy
                foreach (JObject content in knowledgePedagogy.Children<JObject>())
                {
                    if (content["Category"].ToString() == "Knowledge of subject matter")
                    {
                        knowledgeOfSubjectMatter = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Knowledge of state content standards & curriculum frameworks")
                    {
                        knowledgeOfStateContentStandards = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Interest & enthusiasm for subject matter")
                    {
                        interestEnthusiasm = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Appropriate reflection and analysis of observed strategies")
                    {
                        appropriateReflectionAnalysisOfObservedLessonDesign = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Appropriate reflection and analysis of observed lesson design")
                    {
                        appropriateReflectionAnalysisOfObservedLessonDesign = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Recognition of how instruction is aligned with content standards")
                    {
                        recognitionOfHowInstructionAligned = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Recognition of how instruction is differentiated to engage all learners")
                    {
                        recognitionOfHowInstructionDifferentiated = Convert.ToString(content.GetValue("value"));
                    }
                }
                //Student Interaction
                foreach (JObject content in StudentInteraction.Children<JObject>())
                {
                    if (content["Category"].ToString() == "Appropriate interactions with students")
                    {
                        appropriateInteractionsWithStudents = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Attitude of class toward candidate")
                    {
                        attitudeOfClassTowardCandidate = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Realistic expectations for behavior students")
                    {
                        realisticExpectationsForBehavior = Convert.ToString(content.GetValue("value"));
                    }
                    if (content["Category"].ToString() == "Supports and motivates")
                    {
                        supportsAndMotivatesStudents = Convert.ToString(content.GetValue("value"));
                    }
                }
                //Overall Assessment
                foreach (JObject content in OverallAssessment.Children<JObject>())
                {
                    if (content["Category"].ToString() == "What degree of success do you predict for this student as a future teacher:")
                    {
                        predictStudentFutureTeacher = Convert.ToString(content.GetValue("value"));
                    }
                }

            evaluationTemplateBody = GetDocumentBodyTemplate("SSCPEvaluationFormTemplate.html");
            // replace the values in the template 
            evaluationTemplateBody = evaluationTemplateBody.Replace("[[logoPath]]",logoPath)
                                                                     .Replace("[[CandidateName]]", candidateName)
                                                                     .Replace("[[DemonstrationTeacherName]]", demonstrationTeacherName)
                                                                     .Replace("[[CandidateSpentHours]]", candidateSpentHours)
                                                                     .Replace("[[SpokenEnglish]]", spokenEnglish)
                                                                     .Replace("[[WrittenEnglish]]", writtenEnglish)
                                                                     .Replace("[[DemonstratesConfidence]]", demonstratesConfidence)
                                                                     .Replace("[[ActiveListeningSkills]]", activeListeningSkills)
                                                                     .Replace("[[ProfessionalAppearance]]", professionalAppearance)
                                                                     .Replace("[[RelationshipsWithStaff]]", relationshipsWithStaff)
                                                                     .Replace("[[Professional/ethicalBehavior]]", professionalEthicalBehavior)
                                                                     .Replace("[[PersonalInteractionCourtesy]]", personalInteractionCourtesy)
                                                                     .Replace("[[Dependability]]", dependability)
                                                                     .Replace("[[Attendance]]", attendance)
                                                                     .Replace("[[Punctuality]]", punctuality)
                                                                     .Replace("[[SensitivityToDiversity]]", sensitivityToDiversity)
                                                                     .Replace("[[InterestEnthusiasmTeaching]]", interestEnthusiasmTeaching)
                                                                     .Replace("[[SocialIntellectualMaturity]]", socialIntellectualMaturity)
                                                                     .Replace("[[AsksAppropriateQuestions]]", asksAppropriateQuestions)
                                                                     .Replace("[[KnowledgeOfSubjectMatter]]", knowledgeOfSubjectMatter)
                                                                     .Replace("[[KnowledgeOfStateContentStandards]]", knowledgeOfStateContentStandards)
                                                                     .Replace("[[InterestEnthusiasm]]", interestEnthusiasm)
                                                                     .Replace("[[AppropriateReflection]]", appropriateReflection)
                                                                      .Replace("[[AppropriateReflectionAnalysisOfObservedLessonDesign]]", appropriateReflectionAnalysisOfObservedLessonDesign)
                                                                     .Replace("[[RecognitionOfHowInstructionAligned]]", recognitionOfHowInstructionAligned)
                                                                     .Replace("[[RecognitionOfHowInstructionDifferentiated]]", recognitionOfHowInstructionDifferentiated)
                                                                     .Replace("[[AppropriateInteractionsWithStudents]]", appropriateInteractionsWithStudents)
                                                                     .Replace("[[AttitudeOfClassTowardCandidate]]", attitudeOfClassTowardCandidate)
                                                                     .Replace("[[RealisticExpectationsForBehavior]]", realisticExpectationsForBehavior)
                                                                     .Replace("[[SupportsAndMotivatesStudents]]", supportsAndMotivatesStudents)
                                                                     .Replace("[[PredictStudentFutureTeacher]]", predictStudentFutureTeacher)
                                                                     .Replace("[[Comments]]", comment);
                // get the filecontent
                pdfFileContent = GetPDFFileContent(evaluationTemplateBody);

            return pdfFileContent;
        }
        public byte[] GetPDFFromJSONForMSCP(string jsonString)
        {
            byte[] pdfFileContent = null;
            string evaluationTemplateBody = string.Empty;
            string logoPath = Path.GetFullPath("SupportFiles/Img/logo.jpeg");
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

            evaluationTemplateBody = GetDocumentBodyTemplate("MSCPEvaluationFormTemplate.html");
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
                                                                     .Replace("[[CooperatingTeacherName]]", teacherName);
            // get the filecontent
            pdfFileContent = GetPDFFileContent(evaluationTemplateBody);

            return pdfFileContent;
        }
        private byte[] GetPDFFromJSONForPK3(string jsonString)
        {
            byte[] pdfFileContent = null;
            string evaluationTemplateBody = string.Empty;
            string logoPath = Path.GetFullPath("SupportFiles/Img/logo.jpeg");
            jsonString = jsonString.Replace("+", " ");
            JObject schema = JObject.Parse(jsonString);
            string courseNameAndNumber = string.Empty;
            string studentName = string.Empty;
            string schoolName1 = string.Empty;
            string qualityOfWork = string.Empty;
            string preparationMaterials = string.Empty;
            string appropriateBehavior = string.Empty;
            string professional = string.Empty;
            string courteousInteractions = string.Empty;
            string regularAttendance = string.Empty;
            string commentsOrConcerns = string.Empty;
            string teacherName = string.Empty;
            string schoolName2 = string.Empty;
            string gradeLevel = string.Empty;
            string signature = string.Empty;
            string position = string.Empty;
            string date = string.Empty;
            string totalHours = string.Empty;

            JObject context = (JObject)schema["context"];
            JArray professionalDispositions = (JArray)context["professionalDispositions"];
            JArray dateAndHours = (JArray)context["dateAndHours"];

            courseNameAndNumber = Convert.ToString(context["courseNameAndNumber"]);
            studentName = Convert.ToString(context.GetValue("studentName"));
            qualityOfWork = Convert.ToString(context["qualityOfWork"]);
            schoolName1 = Convert.ToString(context.GetValue("schoolName1"));
            commentsOrConcerns = Convert.ToString(context["commentsOrConcerns"]);
            teacherName = Convert.ToString(context["teacherName"]);
            schoolName2 = Convert.ToString(context.GetValue("schoolName2"));
            gradeLevel = Convert.ToString(context.GetValue("gradeLevel"));
            signature = Convert.ToString(context["signature"]);
            position = Convert.ToString(context.GetValue("position"));
            date = Convert.ToString(context["date"]);
            totalHours = Convert.ToString(context.GetValue("totalHours"));
            var dateHoursSb = new StringBuilder();

            foreach (JObject content in professionalDispositions.Children<JObject>())
            {
                if (content["criteria"].ToString() == "Preparation of materials and appropriate instruction")
                {
                    preparationMaterials = Convert.ToString(content.GetValue("value"));
                }
                if (content["criteria"].ToString() == "Appropriate behavior toward children")
                {
                    appropriateBehavior = Convert.ToString(content.GetValue("value"));
                }
                if (content["criteria"].ToString() == "Professional dress and demeanor")
                {
                    professional = Convert.ToString(content.GetValue("value"));
                }
                if (content["criteria"].ToString() == "Courteous interactions with faculty, other students, staff and children")
                {
                    courteousInteractions = Convert.ToString(content.GetValue("value"));
                }
                if (content["criteria"].ToString() == "Regular, on-time attendance")
                {
                    regularAttendance = Convert.ToString(content.GetValue("value"));
                }
            }

            foreach (JObject item in dateAndHours)
            {
                string dateValue = Convert.ToString(item["date"]);
                string hourValue = Convert.ToString(item["hours"]);

                dateHoursSb.Append($@"<tr>
                                          <td>{dateValue}</td>
                                          <td>{hourValue}</td>
                                      </tr>");
            }
            evaluationTemplateBody = GetDocumentBodyTemplate("PK3EvaluationFormTemplate.html");
            // replace the values in the template 
            evaluationTemplateBody = evaluationTemplateBody.Replace("[[logoPath]]", logoPath)
                                                                     .Replace("[[courseNameAndNumber]]", courseNameAndNumber)
                                                                     .Replace("[[studentName]]", studentName)
                                                                     .Replace("[[schoolName1]]", schoolName1)
                                                                     .Replace("[[qualityOfWork]]", qualityOfWork)
                                                                     .Replace("[[preparationMaterials]]", preparationMaterials)
                                                                     .Replace("[[appropriateBehavior]]", appropriateBehavior)
                                                                     .Replace("[[Professional]]", professional)
                                                                     .Replace("[[courteousInteractions]]", courteousInteractions)
                                                                     .Replace("[[regularAttendance]]", regularAttendance)
                                                                     .Replace("[[commentsOrConcerns]]", commentsOrConcerns)
                                                                     .Replace("[[teacherName]]", teacherName)
                                                                     .Replace("[[schoolName2]]", schoolName2)
                                                                     .Replace("[[gradeLevel]]", gradeLevel)
                                                                     .Replace("[[signature]]", signature)
                                                                     .Replace("[[position]]", position)
                                                                     .Replace("[[date]]", date)
                                                                     .Replace("[[totalHours]]", totalHours)
                                                                     .Replace("[[dateHoursRows]]",dateHoursSb.ToString());
            // get the filecontent
            pdfFileContent = GetPDFFileContent(evaluationTemplateBody);
            return pdfFileContent;

        }
        public BaseResponse SaveClinicalPracticeEquivalencyAttachment(SaveClinicalPracticeEquivalencyAttachmentRequest input)
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
                FormAttachmentFileNames fileNames = GetFormSubSectionAttachmentFileName(input.FormID, input.FileName);
                if (input.FileName != string.Empty)
                {
                    //AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.FileName);
                    //subSectionNameSplit = fileNames.FileName.Split('_');
                    //subSectionName = subSectionNameSplit[0];
                    fileExtension = input.FileExtn;
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
                                          new SqlParameter("@SubSectionIdentifiers", SqlDbType.VarChar,100) { Value = input.SubSectionIdentifiers },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 200) { Value = fileNames.FileName },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = input.FileExtn },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = fileNames.SavedFileName },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID }
                                        };
                DataTable dtFormAttachment = _helper.GetDataTable("[Application].[SaveFormSubSectionAttachment]", parameters);
                if (dtFormAttachment.Rows.Count > 0 && input.FileName != string.Empty)
                {
                    string[] folderSplit = dtFormAttachment.Rows[0]["FolderName"].ToString().Split('~');
                    userFolderName = folderSplit[0].ToString();
                    string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);

                    string dirForm = Path.Combine(dirUserFolderPath, "Form");
                    string subSection = Path.Combine(dirForm, input.SubSectionIdentifiers);
                    //if (Directory.Exists(dirUserFolderPath))
                    //{
                    if (!Directory.Exists(dirUserFolderPath))
                    {
                        DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                        DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirForm);
                        DirectoryInfo dirsubsectionFolder = System.IO.Directory.CreateDirectory(subSection);
                        DirectorySecurity dSecurity = dirsubsectionFolder.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dirsubsectionFolder.SetAccessControl(dSecurity);
                    }
                    //string dirForm = Path.Combine(dirUserFolderPath, "Form");
                    //string subSection = Path.Combine(dirForm, subSectionName);
                    if (!Directory.Exists(dirForm))
                    {
                        DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirForm);
                        DirectoryInfo dirsubsectionFolder = System.IO.Directory.CreateDirectory(subSection);
                        DirectorySecurity dSecurity = dirsubsectionFolder.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dirsubsectionFolder.SetAccessControl(dSecurity);

                    }
                    if (!Directory.Exists(subSection))
                    {
                        DirectoryInfo dirsubsectionFolder = System.IO.Directory.CreateDirectory(subSection);
                        DirectorySecurity dSecurity = dirsubsectionFolder.GetAccessControl();
                        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                        dirsubsectionFolder.SetAccessControl(dSecurity);
                    }
                    File.WriteAllBytes(Path.Combine(subSection, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                    //if (Directory.Exists(dirForm))
                    //{
                    //    subSection = Path.Combine(dirForm, subSectionName);
                    //    // copy the file here 
                    //    //if (isNotPDFExtension)
                    //    //{
                    //    //    //byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                    //    //}
                    //    if (Directory.Exists(subSection))
                    //    {

                    //    }
                    //    else
                    //    {
                    //        Directory.CreateDirectory(subSection);
                    //        File.WriteAllBytes(Path.Combine(subSection, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                    //    }
                    //}
                    //else
                    //{
                    //    Directory.CreateDirectory(dirForm);
                    //    if (isNotPDFExtension)
                    //    {
                    //        //byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                    //    }
                    //    else
                    //    {
                    //        Directory.CreateDirectory(subSection);
                    //        File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                    //    }
                    //}
                    //}
                    //else
                    //{
                    //    string dirForm = Path.Combine(dirUserFolderPath, "Form");
                    //    string subSection= Path.Combine(dirForm, subSectionName);
                    //    DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                    //    DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirForm);
                    //    DirectoryInfo dirsubsectionFolder = System.IO.Directory.CreateDirectory(subSection);
                    //    DirectorySecurity dSecurity = dirsubsectionFolder.GetAccessControl();
                    //    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    //    dirsubsectionFolder.SetAccessControl(dSecurity);
                    //    if (isNotPDFExtension)
                    //    {
                    //        //byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                    //    }
                    //    else
                    //    {
                    //        File.WriteAllBytes(Path.Combine(subSection, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                    //    }
                    //}
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
        public FormSubsectionAttachmentDownloadResponse GetClinicalPracticeEquivalencyAttachment(ClinicalPracticeEquivalencyAttachmentRequest input)
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
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetClinicalPracticeEquivalencyContent(_utils.GetAttachmentsFolderName(row["FolderName"].ToString()), _utils.GetAttachmentsSavedFileName(row["FolderName"].ToString()) + "." + Convert.ToString(row["FileExtn"]),input.SubSectionIdentifiers)
                                          }).FirstOrDefault();
            obj.IsSuccess = true;
            obj.Message = "Attachment retrieved Successfully";

            return obj;
        }
        public ClinicalPracticeEquivalencyAttachmentList GetClinicalPracticeEquivalencyAttachmentList(int UserID, int FormID)
        {
            ClinicalPracticeEquivalencyAttachmentList respone = new ClinicalPracticeEquivalencyAttachmentList();
            Attachments obj = new Attachments
            {
                pv = new PV(),
                pc = new PC()
            };

            SqlParameter[] parameters =
                                      {
                                            new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                            new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID }
                                       };

            DataTable dtCPEAL = _helper.GetDataTable("[Application].[GetClinicalPracticeEquivalencySection]", parameters);

            try
            {
                if (dtCPEAL.Rows.Count > 0)
                {
                    bool hasPV = dtCPEAL.AsEnumerable().Any(row => row.Field<string>("SubSectionIdentifiers") == "PV");
                    bool hasPC = dtCPEAL.AsEnumerable().Any(row => row.Field<string>("SubSectionIdentifiers") == "PC");

                    if (hasPV)
                    {
                        obj.pv.professionalVerificationForm = dtCPEAL.AsEnumerable()
                            .Where(row => row.Field<string>("SubSectionIdentifiers") == "PV" &&
                                          row.Field<string>("FileName").StartsWith("ProfessionalVerificationForm"))
                            .Select(row => new ProfessionalVerificationForm
                            {
                                FormSubSectionAttachmentID = Convert.ToInt32(row["FormSubSectionAttachmentID"]),
                                FormID = Convert.ToInt32(row["FormID"]),
                                FormSubSectionID = Convert.ToInt32(row["FormSubSectionID"]),
                                SubSectionIdentifiers = Convert.ToString(row["SubSectionIdentifiers"]),
                                FileName = Convert.ToString(row["FileName"]),
                                FileExtn = Convert.ToString(row["FileExtn"]),
                                CanView = Convert.ToBoolean(row["CanView"])
                            }).FirstOrDefault();

                        obj.pv.childDevelopmentPermit = dtCPEAL.AsEnumerable()
                            .Where(row => row.Field<string>("SubSectionIdentifiers") == "PV" &&
                                          row.Field<string>("FileName").StartsWith("ChildDevelopmentPermit"))
                            .Select(row => new ChildDevelopmentPermit
                            {
                                FormSubSectionAttachmentID = Convert.ToInt32(row["FormSubSectionAttachmentID"]),
                                FormID = Convert.ToInt32(row["FormID"]),
                                FormSubSectionID = Convert.ToInt32(row["FormSubSectionID"]),
                                SubSectionIdentifiers = Convert.ToString(row["SubSectionIdentifiers"]),
                                FileName = Convert.ToString(row["FileName"]),
                                FileExtn = Convert.ToString(row["FileExtn"]),
                                CanView = Convert.ToBoolean(row["CanView"])
                            }).FirstOrDefault();
                        obj.pv.additionalProfessionalVerificationDocumentation = dtCPEAL.AsEnumerable()
                          .Where(row => row.Field<string>("SubSectionIdentifiers") == "PV" &&
                                        row.Field<string>("FileName").StartsWith("AdditionalProfessionalVerificationDocumentation"))
                          .Select(row => new AdditionalProfessionalVerificationDocumentation
                          {
                              FormSubSectionAttachmentID = Convert.ToInt32(row["FormSubSectionAttachmentID"]),
                              FormID = Convert.ToInt32(row["FormID"]),
                              FormSubSectionID = Convert.ToInt32(row["FormSubSectionID"]),
                              SubSectionIdentifiers = Convert.ToString(row["SubSectionIdentifiers"]),
                              FileName = Convert.ToString(row["FileName"]),
                              FileExtn = Convert.ToString(row["FileExtn"]),
                              CanView = Convert.ToBoolean(row["CanView"])
                          }).ToList();
                        respone.attachements = obj;
                    }
                    else
                    {
                        obj.pv.professionalVerificationForm = null;
                        obj.pv.childDevelopmentPermit = null;
                        obj.pv.additionalProfessionalVerificationDocumentation = new List<AdditionalProfessionalVerificationDocumentation> { };
                    }

                    if (hasPC)
                    {
                        obj.pc.courseSyllabi = dtCPEAL.AsEnumerable()
                            .Where(row => row.Field<string>("SubSectionIdentifiers") == "PC" &&
                                          row.Field<string>("FileName").StartsWith("CourseSyllabi"))
                            .Select(row => new CourseSyllabi
                            {
                                FormSubSectionAttachmentID = Convert.ToInt32(row["FormSubSectionAttachmentID"]),
                                FormID = Convert.ToInt32(row["FormID"]),
                                FormSubSectionID = Convert.ToInt32(row["FormSubSectionID"]),
                                SubSectionIdentifiers = Convert.ToString(row["SubSectionIdentifiers"]),
                                FileName = Convert.ToString(row["FileName"]),
                                FileExtn = Convert.ToString(row["FileExtn"]),
                                CanView = Convert.ToBoolean(row["CanView"])
                            }).FirstOrDefault();

                        obj.pc.transcripts = dtCPEAL.AsEnumerable()
                            .Where(row => row.Field<string>("SubSectionIdentifiers") == "PC" &&
                                          row.Field<string>("FileName").StartsWith("Transcripts"))
                            .Select(row => new Transcripts
                            {
                                FormSubSectionAttachmentID = Convert.ToInt32(row["FormSubSectionAttachmentID"]),
                                FormID = Convert.ToInt32(row["FormID"]),
                                FormSubSectionID = Convert.ToInt32(row["FormSubSectionID"]),
                                SubSectionIdentifiers = Convert.ToString(row["SubSectionIdentifiers"]),
                                FileName = Convert.ToString(row["FileName"]),
                                FileExtn = Convert.ToString(row["FileExtn"]),
                                CanView = Convert.ToBoolean(row["CanView"])
                            }).ToList();
                        obj.pc.additionalPracticumCourseWorkDocumentation = dtCPEAL.AsEnumerable()
                            .Where(row => row.Field<string>("SubSectionIdentifiers") == "PC" &&
                                          row.Field<string>("FileName").StartsWith("AdditionalPracticumCourseWorkDocumentation"))
                            .Select(row => new AdditionalPracticumCourseWorkDocumentation
                            {
                                FormSubSectionAttachmentID = Convert.ToInt32(row["FormSubSectionAttachmentID"]),
                                FormID = Convert.ToInt32(row["FormID"]),
                                FormSubSectionID = Convert.ToInt32(row["FormSubSectionID"]),
                                SubSectionIdentifiers = Convert.ToString(row["SubSectionIdentifiers"]),
                                FileName = Convert.ToString(row["FileName"]),
                                FileExtn = Convert.ToString(row["FileExtn"]),
                                CanView = Convert.ToBoolean(row["CanView"])
                            }).ToList();
                        respone.attachements = obj;
                    }
                    else
                    {
                        obj.pc.courseSyllabi = null;
                        obj.pc.transcripts = new List<Transcripts> { };
                        obj.pc.additionalPracticumCourseWorkDocumentation = new List<AdditionalPracticumCourseWorkDocumentation> { };
                    }

                    respone.IsSuccess = true;
                    respone.Message = "Data Retrieved Successfully";
                }
                else
                {
                    obj = new Attachments
                    {
                        pv = new PV
                        {
                            professionalVerificationForm = null,
                            childDevelopmentPermit = null,
                            additionalProfessionalVerificationDocumentation = new List<AdditionalProfessionalVerificationDocumentation> { }
                        },
                        pc = new PC
                        {
                            courseSyllabi = null,
                            transcripts = new List<Transcripts> { },
                            additionalPracticumCourseWorkDocumentation = new List<AdditionalPracticumCourseWorkDocumentation> { }
                        },
                    };
                    respone.attachements = obj;
                    respone.IsSuccess = true;
                    respone.Message = "No Data Available";
                }
            }
            catch (Exception ex)
            {
                respone.IsSuccess = false;
                respone.Message = "Data Retrieval Failed, Please contact site admin";
                respone.StackTrace = ex.Message;
            }
            return respone;
        }

        public byte[] GetClinicalPracticeEquivalencyContent(string userFolderPath, string fileName,string subSectionIdentifiers)
        {
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            string userPath = Path.Combine(fileRepoPath, Path.Combine(userFolderPath, "Form"));
            string filepath = Path.Combine(userPath, Path.Combine(subSectionIdentifiers, fileName));
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
        public BaseResponse UpdateAdmissionRequirementsMailBody(AdmissionRequirementsBody input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@ID", SqlDbType.BigInt) { Value = input.ID },
                                          new SqlParameter("@EmailBody", SqlDbType.NVarChar, -1) { Value = input.EmailBody },
                                          new SqlParameter("@Identifier", SqlDbType.VarChar,20) { Value = input.Identifier }
                                        };
            DataTable dtResponse = _helper.GetDataTable("[dbo].[UpdateAdmissionRequirementsMailBody]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "SUCCESS")
                {
                    response.Message = "Updated recommender mail body successfully";
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "FAILURE")
                {
                    response.Message = "Failed to update the recommender mail body";
                    response.IsSuccess = false;
                }
            }
            return response;
        }
        public AdmissionRequirementsBodyResponse GetAdmissionRequirementsMailBody(int programID, string sectionName, string categoryName, string identifier, string name)
        {
            AdmissionRequirementsBodyResponse obj = new AdmissionRequirementsBodyResponse();
            SqlParameter[] parameters ={
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
                                            new SqlParameter("@SectionName", SqlDbType.VarChar,10) { Value = sectionName },
                                            new SqlParameter("@CategoryName", SqlDbType.VarChar,20) { Value = categoryName },
                                            new SqlParameter("@Identifier", SqlDbType.VarChar,20) { Value = identifier },
                                            new SqlParameter("@Name", SqlDbType.VarChar,50) { Value = name }
                                       };

            DataTable dtMailBody = _helper.GetDataTable("[dbo].[GetAdmissionRequirementsMailBody]", parameters);
            try
            {
                if (dtMailBody.Rows.Count > 0)
                {
                    if (identifier == "FieldWork Mail")
                    {
                        obj.admissionRequirementResponse = dtMailBody.AsEnumerable().Select(row =>
                                                  new AdmissionRequirementBody
                                                  {
                                                      ID = Convert.ToInt32(row["ID"]),
                                                      EmailBody = Convert.ToString(row["EmailBody"])
                                                  }).FirstOrDefault();
                    }
                    else
                    {
                        obj.admissionRequirementResponse = dtMailBody.AsEnumerable().Select(row =>
                                                  new AdmissionRequirementBody
                                                  {
                                                      ID = Convert.ToInt32(row["ID"]),
                                                      EmailBody = Convert.ToString(row["EmailBody"]),
                                                      Subject = Convert.ToString(row["Subject"])
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
        private string GetMailBodyTemplateByProgramID(int programID)
        {
            string body = string.Empty;
            string templateName = string.Empty;
            if (programID == 2)
                templateName = "MSCP_Clinical_Practice_Evaluation_Form.html";
            if (programID == 4)
                templateName = "SSCP_Clinical_Practice_Evaluation_Form.html";
            string filepath = Path.Combine("SupportFiles/EmailTemplates", templateName);
            using (StreamReader reader = new StreamReader(Path.GetFullPath(filepath)))
            {
                body = reader.ReadToEnd();
            }
            return body;
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
}
