using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PdfSharp.Pdf.Content.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Domain.FormModels;
using ThoughtFocus.Domain.Request.GraduateProgram;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.TemplateModels;
using ThoughtFocus.Service.Interfaces;
using ThoughtFocus.Domain.Request.FieldWork;
using iTextSharp.text.pdf.codec.wmf;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;

namespace ThoughtFocus.Service.Implementation
{
    public class GraduateProgramServiceImpl : IGraduateProgramService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        public ILogger<GraduateProgramServiceImpl> _logger;
        public IInitialCredentialProgramService _initialCredentialProgramService;
        public IFieldWorkService _fieldWorkService;
        public GraduateProgramServiceImpl(ISqlDBUtility helper
                                         , IConfiguration configuration
                                         , ISendMail sendMail
                                         , ILogger<GraduateProgramServiceImpl> logger
                                         , IInitialCredentialProgramService initialCredentialProgramService
                                         ,IFieldWorkService fieldWorkService)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
            _initialCredentialProgramService = initialCredentialProgramService;
            _fieldWorkService = fieldWorkService;
        }
        public ApplicationProgramResponse GetApplicationPrograms(int userID, int applicationTypeID,string termCode)
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
                                                  TermCode = Convert.ToString(row["TermCode"]),
                                                  applicationOpens = Convert.ToDateTime(row["ApplicationOpens"]),
                                                  applicationCloseDate = Convert.ToDateTime(row["ApplicationCloseDate"]),
                                                  TotalCount = Convert.ToInt32(row["TotalCount"]),
                                                  AcceptedCount = Convert.ToInt32(row["AcceptedCount"]),
                                                  showApply = Convert.ToBoolean(row["showApply"]),
                                                  showView = Convert.ToBoolean(row["showView"]),
                                                  ProgramSetting = Convert.ToString(row["ProgramSetting"]),
                                                  SubmittedCount = Convert.ToInt32(row["SubmittedCount"])
                                              }).ToList();

                    obj.HeaderDetails= dtApplicationPrograms.Tables[1].AsEnumerable().Select(row =>
                                               new HeaderDetails
                                               {
                                                   semester = Convert.ToString(row["Semester"]),
                                                   TermCode = Convert.ToString(row["TermCode"]),
                                                   showApply = Convert.ToBoolean(row["showApply"]),
                                                   showView = Convert.ToBoolean(row["showView"]),
                                                   showAssignApplicationToReviewers= Convert.ToBoolean(row["showAssignApplicationToReviewers"]),
                                                   showSettings = Convert.ToBoolean(row["showSettings"])
                                               }).FirstOrDefault();

                    obj.Semesters = dtApplicationPrograms.Tables[1].AsEnumerable().Select(row =>
                                             new Semester
                                             {
                                                 TermCode = Convert.ToString(row["TermCode"]),
                                                 TermName = Convert.ToString(row["Semester"])
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
                                                  StudentFirstName = Convert.ToString(row["StudentFirstName"]),
                                                  StudentLastName = Convert.ToString(row["StudentLastName"]),
                                                  FormStateID = Convert.ToInt32(row["FormStateID"]),
                                                  FormState = Convert.ToString(row["FormState"]),
                                                  AppliedDate = Convert.ToDateTime(row["AppliedDate"]),
                                                  ProgramID = Convert.ToInt32(row["ProgramID"]),
                                                  ProgramName = Convert.ToString(row["ProgramName"]),
                                                  Semester = Convert.ToString(row["Semester"]),
                                                  TermCode = Convert.ToString(row["TermCode"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),
                                                  ReviewersName = Convert.ToString(row["ReviewersName"]),
                                                  BSRStatus = Convert.ToString(row["BSR"]),
                                                  CTCStatus = Convert.ToString(row["CTC Clearance"]),
                                                  GPAStatus = Convert.ToString(row["GPA"]),
                                                  SMCStatus = Convert.ToString(row["SMC"]),
                                                  TBTestStatus = Convert.ToString(row["TB Test"]),
                                                  CredentialPathway = Convert.ToString(row["Credential Pathway"]),
                                                  RecommendationsSubmittedCount = Convert.ToString(row["RecommendationsSubmittedCount"]),
                                                  IsInterviewRatingSheetSubmitted = Convert.ToString(row["IsInterviewRatingSheetSubmitted"]),
                                                  LastUpdatedDate = Convert.ToDateTime(row["LastUpdatedDate"] == DBNull.Value ? null : row["LastUpdatedDate"]),
                                                  SubmittedDate = Convert.ToDateTime(row["SubmittedDate"] == DBNull.Value ? null : row["SubmittedDate"]),
                                                  EDEL200380FinalFieldworkEvaluation_Status = Convert.ToString(row["EDEL200380FinalFieldworkEvaluation_Status"]),
                                                  InstructorEvaluationForm_Status = Convert.ToString(row["InstructorEvaluationForm_Status"]),
                                                  AdvisementConfirmationForm_Status = Convert.ToString(row["AdvisementConfirmationForm_Status"]),
                                                  GridNotes = Convert.ToString(row["GridNotes"]),
                                                  ReviewerRecommendation = Convert.ToString(row["ReviewerRecommendation"]),
                                                  FinalDecision = Convert.ToString(row["FinalDecision"]),
                                                  WaitlistNumber = Convert.ToInt32(row["WaitlistNumber"]),
                                                  ShowBulkCheckBox = Convert.ToBoolean(row["ShowBulkCheckBox"]),
                                                  Email = Convert.ToString(row["Email"]),
                                                  AlternateEmail = Convert.ToString(row["AlternateEmail"]),
                                                  ResumeUploadStatus = Convert.ToString(row["ResumeUploadStatus"]),
                                                  SOPUploadStatus = Convert.ToString(row["SOPUploadStatus"]),
                                                  PersonalStatementDoc = Convert.ToString(row["PersonalStatementDoc"]),
                                                  UniversityApplicationDetails = Convert.ToString(row["UniversityApplicationDetails"]),
                                                  DocumentNumber = Convert.ToString(row["DocumentNumber"])
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
                                                  programID = Convert.ToInt32(row["ProgramID"]),
                                                  showAssignApplicationToReviewers = Convert.ToBoolean(row["showAssignApplicationToReviewers"]),
                                                  showBulkDeny = Convert.ToBoolean(row["showBulkDeny"]),
                                                  showBulkOffer = Convert.ToBoolean(row["showBulkOffer"]),
                                                  showDeletedFormsButton = Convert.ToBoolean(row["showDeletedFormsButton"])
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
                    var deletedState = obj.FormStates.FirstOrDefault(state => state.StateID == -1 && state.StateName == "Deleted");
                    if (deletedState != null)
                    {
                        obj.FormStates.Remove(deletedState);
                        obj.FormStates.Add(deletedState);
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
        public GraduateProgramFormResponse GetForm(int userID, int formID, int programID, string termCode,bool showMilestone)
        {
            GraduateProgramFormResponse obj = new GraduateProgramFormResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserID", SqlDbType.Int, 50) { Value = userID },
                                          new SqlParameter("@FormID", SqlDbType.Int, 50) { Value = formID },
                                          new SqlParameter("@ProgramID", SqlDbType.Int, 50) { Value = programID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = termCode },
                                          new SqlParameter("@ShowMileStone", SqlDbType.Bit) { Value = showMilestone }
                                        };

            DataSet dtFormData = _helper.GetDataSet("[dbo].[GetForm]", parameters);
            try
            {
                if (dtFormData.Tables.Count > 0)
                {
                    obj.FormBasicInformation = dtFormData.Tables[0].AsEnumerable().Select(row =>
                                               new FormBasicInformation
                                               {
                                                   FormID = Convert.ToInt32(row["FormID"]),
                                                   StudentID = Convert.ToInt32(row["StudentID"]),
                                                   ProgramID = Convert.ToInt32(row["ProgramID"]),
                                                   ProgramName = Convert.ToString(row["ProgramName"]),
                                                   TermCode = Convert.ToString(row["TermCode"]),
                                                   Semester = Convert.ToString(row["Semester"]),
                                                   Form = Convert.ToString(row["Form"]),
                                                   FormStateID = Convert.ToInt32(row["FormStateID"]),
                                                   FormState = Convert.ToString(row["FormState"]),
                                                   ApplicationNumber = Convert.ToString(row["ApplicationNumber"]),
                                                   ModifiedDateTime = Convert.ToDateTime(row["ModifiedDateTime"]),
                                                   ModifiedBy = Convert.ToInt32(row["ModifiedBy"]),
                                                   MessageBoard = Convert.ToString(row["MessageBoard"]),
                                                   CompletingYourApplication = Convert.ToString(row["CompletingYourApplication"]),
                                                   ApplicationTypeID = Convert.ToInt32(row["ApplicationTypeID"]),
                                                   ProgramFormIdentifier = Convert.ToString(row["ProgramFormIdentifier"]),
                                                   CreatedDateTime = Convert.ToDateTime(row["CreatedDateTime"]),
                                                   SubmittedDateTime = Convert.ToDateTime(row["SubmittedDateTime"] == DBNull.Value ? null : row["SubmittedDateTime"]),
                                                   WaitlistNumber = Convert.ToInt32(row["WaitlistNumber"] == DBNull.Value ? null : row["WaitlistNumber"]),
                                                   WaitlistComments = Convert.ToString(row["WaitlistComments"] == DBNull.Value ? null : row["WaitlistComments"]),
                                                   FinalDecision = Convert.ToString(row["FinalDecision"] == DBNull.Value ? null : row["FinalDecision"]),
                                                   FinalDecisionComments = Convert.ToString(row["FinalDecisionComments"] == DBNull.Value ? null : row["FinalDecisionComments"]),
                                                   FinalDecisionDate = Convert.ToString(row["FinalDecisionDate"] == DBNull.Value ? null : row["FinalDecisionDate"]),
                                                   CertifyDescription = Convert.ToString(row["CertifyDescription"] == DBNull.Value ? null : row["CertifyDescription"]),
                                                   UniversityApplicationStatus = Convert.ToString(row["UniversityApplicationStatus"] == DBNull.Value ? null : row["UniversityApplicationStatus"]),
                                                   GPA = Convert.ToString(row["UniversityApplicationStatus"] == DBNull.Value ? null : row["GPA"]),
                                                   StatusDate = Convert.ToDateTime(row["StatusDate"] == DBNull.Value ? null : row["StatusDate"])
                                               }).FirstOrDefault();

                    obj.FormStateHandler = dtFormData.Tables[1].AsEnumerable().Select(row =>
                                                new FormStateHandler
                                                {
                                                    StateHandler = Convert.ToString(row["StateHandler"])

                                                }).FirstOrDefault();

                    obj.FormAttachmentsInformation = dtFormData.Tables[2].AsEnumerable().Select(row =>
                                       new FormAttachmentsInformation
                                       {
                                           FormAttachmentID = (row["FormAttachmentID"]==DBNull.Value)?0:Convert.ToInt32(row["FormAttachmentID"]),
                                           DocumentID = Convert.ToInt32(row["DocumentID"]),
                                           ProgramID = Convert.ToInt32(row["ProgramID"]),
                                           FormID = (row["FormID"]==DBNull.Value)?0:Convert.ToInt32(row["FormID"]),
                                           AttachmentTitle = Convert.ToString(row["AttachmentTitle"]),
                                           FileName = Convert.ToString(row["FileName"]),
                                           FileExtn = Convert.ToString(row["FileExtn"]),
                                           IsOptional = Convert.ToBoolean(row["IsOptional"]),
                                           Instruction = Convert.ToString(row["Instruction"])

                                       }).ToList();

                    obj.RecommendersInformation = dtFormData.Tables[3].AsEnumerable().Select(row =>
                                       new RecommendersInformation
                                       {
                                            Recommendations= Convert.ToString(row["Recommendations"])

                                       }).FirstOrDefault();

                    obj.ReviewerInformation = dtFormData.Tables[4].AsEnumerable().Select(row =>
                                  new ReviewerInformation
                                  {
                                      Reviewer = Convert.ToString(row["Reviewer"])

                                  }).FirstOrDefault();

                    obj.FormControlHandler = dtFormData.Tables[5].AsEnumerable().Select(row =>
                               new FormControlHandler
                               {
                                   FormControls = Convert.ToString(row["FormControlHandler"])

                               }).FirstOrDefault();

                    obj.Instructor = dtFormData.Tables[6].AsEnumerable().Select(row =>
                               new InstructorInformation
                               {
                                   Instructor = Convert.ToString(row["Instructor"])

                               }).FirstOrDefault();

                    obj.Interviewer = dtFormData.Tables[7].AsEnumerable().Select(row =>
                            new InterviwerInformation
                            {
                                Interviewer = Convert.ToString(row["Interviewer"])

                            }).FirstOrDefault();

                    obj.ProgramCoordinator = dtFormData.Tables[8].AsEnumerable().Select(row =>
                        row["ProgramsforCoordinator"] != DBNull.Value ? new ProgramCoordinator
                        {
                            ProgramControll = Convert.ToString(row["ProgramsforCoordinator"])
                        }: null
                        ).FirstOrDefault();

                    obj.FinalDecision = dtFormData.Tables[9].AsEnumerable().Select(row =>
                        new FinalDecisionJSON
                        {
                            FinalDecision = Convert.ToString(row["FinalDecision"])
                        }).FirstOrDefault();
                    obj.EDELFieldWorkAttachmentsInformation = dtFormData.Tables[10].AsEnumerable().Select(row =>
                                       new EDELFieldWorkAttachmentsInformation
                                       {
                                           FormAttachmentID = (row["FormAttachmentID"] == DBNull.Value) ? 0 : Convert.ToInt32(row["FormAttachmentID"]),
                                           DocumentID = Convert.ToInt32(row["DocumentID"]),
                                           ProgramID = Convert.ToInt32(row["ProgramID"]),
                                           FormID = (row["FormID"] == DBNull.Value) ? 0 : Convert.ToInt32(row["FormID"]),
                                           AttachmentTitle = Convert.ToString(row["AttachmentTitle"]),
                                           FileName = Convert.ToString(row["FileName"]),
                                           FileExtn = Convert.ToString(row["FileExtn"]),
                                           IsOptional = Convert.ToBoolean(row["IsOptional"]),
                                           Instruction = Convert.ToString(row["Instruction"])
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

        public BaseResponse UpdatePersonalInfoSchema(FormPersonalInfoSchemaRequest input)
        {
            BaseResponse obj = new BaseResponse();

            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserID", SqlDbType.BigInt, 50) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt, 50) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt, 50) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@FormSchema", SqlDbType.NVarChar, -1) { Value = input.FormSchema }
                                        };
        
                int identity = _helper.InsertTable("[dbo].[UpdateFormPersonalInfoSchema]", parameters);
            obj.IsSuccess = true;
            obj.Message = "Personal Information Saved";

            return obj;
        }
        
        public BaseResponse UpdateMessageBoardSchema(FormMessageBoardSchema input)
        {
            BaseResponse obj = new BaseResponse();

            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserID", SqlDbType.BigInt, 50) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt, 50) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt, 50) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@MessageBoardSchema", SqlDbType.NVarChar, -1) { Value = input.MessageBoardSchema }
                                        };

            int identity = _helper.InsertTable("[dbo].[UpdateFormMessageBoardSchema]", parameters);
            obj.IsSuccess = true;
            obj.Message = "Message Board Information Saved";

            return obj;
        }

        public BaseResponse UpdateFinalDecisionComment(FinalDecisionComment input)
        {
            BaseResponse obj = new BaseResponse();

            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserID", SqlDbType.BigInt, 50) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt, 50) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt, 50) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@FDComment", SqlDbType.NVarChar, -1) { Value = input.FDComment }
                                        };

            int identity = _helper.InsertTable("[dbo].[UpdateFinalDecisionComment]", parameters);
            obj.IsSuccess = true;
            obj.Message = "Information Saved Successfully";

            return obj;
        }

        public BaseResponse UpsertFormAttachment(FormUpsertAttachmentRequest input)
        {
            BaseResponse response = new BaseResponse();
            if (input.FileContent != null && input.FileContent.Length > 0)
            {
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                string fileExtensionWord = string.Empty;
                string userFolderName = string.Empty;
                string savedFileName = string.Empty;
                bool isNotPDFExtension = false;
                var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];

                var workingFolderPath = Path.Combine(fileRepoPath, "WorkingFolder");

                // pull the saved file name format SP Below
                FormAttachmentFileNames fileNames = GetFormAttachmentFileName(input.FormID, input.DocumentID);
                if (input.FileName != string.Empty)
                {
                    AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.FileName);
                    //fileName = fileDetails.FileName;
                    fileExtension = fileDetails.FileExtension;
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
                    if (fileExtension.ToUpper() == "DOC" || fileExtension.ToUpper() == "DOCX")
                    {
                        isNotPDFExtension = true;
                        bool isFileSaved = SaveWordFileInTempFolder(input.FileContent, fileNames.FileName, fileExtension, workingFolderPath);
                        fileExtensionWord = fileExtension;
                        fileExtension = "pdf";
                    }

                }

                SqlParameter[] parameters =
                                         {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@DocumentID", SqlDbType.BigInt) { Value = input.DocumentID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 250) { Value = fileNames.FileName },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = fileNames.SavedFileName },
                                        };
                DataTable dtFormAttachment = _helper.GetDataTable("[dbo].[UpsertFormAttachment]", parameters);
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
                            // copy the file here 
                            if (isNotPDFExtension)
                            {
                                byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                            }
                            else
                            {
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(dirForm);
                            if (isNotPDFExtension)
                            {
                                byte[] inputStr = word2PDF(Path.Combine(workingFolderPath,fileNames.FileName+"."+ fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                            }
                            else
                            {
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
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
                        if (isNotPDFExtension)
                        {
                            byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                        }
                        else
                        {
                            File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
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
        public BaseResponse DeleteFormAttachment(DeleteFormAttachmentRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@DocumentID", SqlDbType.BigInt) { Value = input.DocumentID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@FormAttachmentID ", SqlDbType.BigInt) { Value = input.FormAttachmentID },
                                        };
            DataTable dtDeleteAttachment = _helper.GetDataTable("[dbo].[deleteFormAttachment]", parameters);
            if (dtDeleteAttachment.Rows.Count > 0)
            {
                response.Message = "Attachment Deleted Successfully";
                response.IsSuccess = true;
            }
            else
            {
                response.Message = "Failed To Delete Attachment";
                response.IsSuccess = false;
            }
            return response;
        }

        public BaseResponse DeleteInsructorAttachment(DeleteInsructorAttachmentRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@InstructionID", SqlDbType.BigInt) { Value = input.InstructionID },
                                          new SqlParameter("@InstructorUserID", SqlDbType.BigInt) { Value = input.InstructorUserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@InstructionAttachmentID ", SqlDbType.BigInt) { Value = input.InstructionAttachmentID },
                                        };
            DataTable dtDeleteAttachment = _helper.GetDataTable("[Application].[deleteInstructorAttachment]", parameters);
            if (dtDeleteAttachment.Rows.Count > 0)
            {
                response.Message = "Attachment Deleted Successfully";
                response.IsSuccess = true;
            }
            else
            {
                response.Message = "Failed To Delete Attachment";
                response.IsSuccess = false;
            }
            return response;
        }
        private bool SaveWordFileInTempFolder(byte[] fileContent,string fileName,string fileExtension,string workingFolderPath)
        {
            bool isFileSaved = false;
            if (Directory.Exists(workingFolderPath))
            {
                File.WriteAllBytes(Path.Combine(workingFolderPath, fileName + "." + fileExtension), fileContent);
                isFileSaved = true;
            }
            else
            {
                System.IO.Directory.CreateDirectory(workingFolderPath);
                File.WriteAllBytes(Path.Combine(workingFolderPath, fileName + "." + fileExtension), fileContent);
                isFileSaved = true;
            }
            return isFileSaved;
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
                // delete the source word file 
                File.Delete(Source.ToString());

            }
            return InputStream;
        }
        private byte[] GetImageFilecontent(byte[] fileContent)
        {
            byte[] inputStream = null;
            string documentName = string.Empty;
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                //Initialize the PDF document object.
                using (Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f))
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
        private FormAttachmentFileNames GetFormRecommendAttachmentFileName(string recommenderIdentifier, int documentID)
        {
            FormAttachmentFileNames fileNames = new FormAttachmentFileNames();
            string savedFileName = string.Empty;
            SqlParameter[] parameters =
                                    {

                                          new SqlParameter("@RecommenderIdentifier", SqlDbType.UniqueIdentifier) { Value = new Guid(recommenderIdentifier) },
                                          new SqlParameter("@DocumentID", SqlDbType.BigInt) { Value = documentID }
                                    };
            DataTable dtName = _helper.GetDataTable("[dbo].[GetFormRecommendAttachmentFileName]", parameters);

            if (dtName.Rows.Count > 0)
            {
                fileNames.SavedFileName = Convert.ToString(dtName.Rows[0]["SavedFileName"]);
                fileNames.FileName= Convert.ToString(dtName.Rows[0]["FileName"]);
                fileNames.UserFolder= Convert.ToString(dtName.Rows[0]["UserFolder"]);
            }


            return fileNames;
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

        public BaseResponse SaveForm(FormSaveRequest input)
        {
            BaseResponse response = new BaseResponse();

            //  1-call UpdatePersonalInfoSchema
            FormPersonalInfoSchemaRequest pinfoRes = new FormPersonalInfoSchemaRequest();
            pinfoRes.UserID = input.UserID;
            pinfoRes.ProgramID = input.ProgramID;
            pinfoRes.FormID = input.FormID;
            pinfoRes.TermCode = input.TermCode;
            pinfoRes.FormSchema = input.PersonalInfo.formSchema;

            BaseResponse PersonalInfoResponse = UpdatePersonalInfoSchema(pinfoRes);
            //  2-call UpsertFormAttachment
            foreach(var attachment in input.FormAttachments)
            {
                if (attachment.FileContent != null && attachment.FileContent.Length > 0 && !string.IsNullOrEmpty(attachment.FileName))
                {
                    FormUpsertAttachmentRequest objAtt = new FormUpsertAttachmentRequest();
                    objAtt.UserID = input.UserID;
                    objAtt.ProgramID = input.ProgramID;
                    objAtt.FormID = input.FormID;
                    objAtt.TermCode = input.TermCode;
                    objAtt.FileName = attachment.FileName;
                    objAtt.FileContent = attachment.FileContent;
                    objAtt.DocumentID = attachment.DocumentID;
                    BaseResponse attachRes = UpsertFormAttachment(objAtt);
                }
            }
            //EDEL Field Work Attachment
            if (input.edelFieldWorkAttachmentsInformation != null)
            {
                if (input.edelFieldWorkAttachmentsInformation.FileContent != null && input.edelFieldWorkAttachmentsInformation.FileContent.Length > 0 && !string.IsNullOrEmpty(input.edelFieldWorkAttachmentsInformation.FileName))
                {
                    FormUpsertAttachmentRequest objAtt = new FormUpsertAttachmentRequest();
                    objAtt.UserID = input.UserID;
                    objAtt.ProgramID = input.ProgramID;
                    objAtt.FormID = input.FormID;
                    objAtt.TermCode = input.TermCode;
                    objAtt.FileName = input.edelFieldWorkAttachmentsInformation.FileName;
                    objAtt.FileContent = input.edelFieldWorkAttachmentsInformation.FileContent;
                    objAtt.DocumentID = input.edelFieldWorkAttachmentsInformation.DocumentID;
                    BaseResponse attachRes = UpsertFormAttachment(objAtt);
                }
            }
            //  3-Call Add Recommender

            foreach (var recommender in input.Recommenders)
            {
                if (!string.IsNullOrEmpty(recommender.RecommenderName)&& !string.IsNullOrEmpty(recommender.RecommenderEmail)) {
                    FormAddRecommenderRequest objRec = new FormAddRecommenderRequest();
                    objRec.UserID = input.UserID;
                    objRec.ProgramID = input.ProgramID;
                    objRec.FormID = input.FormID;
                    objRec.TermCode = input.TermCode;
                    objRec.RecommenderName = recommender.RecommenderName;
                    objRec.RecommenderEmail = recommender.RecommenderEmail;
                    objRec.RecommenderAffiliation = recommender.RecommenderAffiliation;
                    BaseResponse attachRes = AddRecommender(objRec);
                }
            }
            // update the form status from open to draft , check and update SP from backend 
            // call checkAndUpdateFormState
            checkAndUpdateFormState(input.UserID,input.FormID,input.ProgramID,input.TermCode);
            // for sending recommender mail 
            sendFormRecommender(input.UserID, input.FormID, input.ProgramID, input.TermCode);
            response.IsSuccess = true;
            response.Message = "Form Saved Successfully";

            return response;
        }

        private void checkAndUpdateFormState(int userID, int formID, int programID, string termCode)
        {
            SqlParameter[] parameters =
                                  {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userID},
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = termCode }
                                        };

            int id = _helper.InsertTable("[dbo].[checkAndUpdateFormState]", parameters);

        }

        public BaseResponse UpdateFormState(FormStatusUpdateRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                   {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@FormStateID", SqlDbType.Int) { Value = input.FormStateID },
                                          new SqlParameter("@WaitlistNumber", SqlDbType.BigInt) { Value = input.WaitlistNumber},
                                          new SqlParameter("@WaitlistComments", SqlDbType.NVarChar,500) { Value = input.WaitlistComments },
                                          new SqlParameter("@FinalDecision", SqlDbType.BigInt) { Value = input.FinalDecision }
                                        };
            int id = _helper.InsertTable("[dbo].[UpdateFormState]", parameters);
            // check if the form state ID is submit then Send mails to Recommenders and applicant .
            // getFormDetailsByFormID
            if (input.FormStateID == 3)
            {
                SqlParameter[] parameter =
                                  {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID }
                                  };
                DataTable dt = _helper.GetDataTable("[dbo].[SendSubmittedMailNotifications]", parameter);
                bool sendSubmittedMail=false;
                if (dt.Rows.Count > 0)
                {
                    sendSubmittedMail = Convert.ToBoolean(dt.Rows[0]["SendSubmittedMail"]);
                    if (sendSubmittedMail == true)
                    {
                        sendFormSubmitted(input.UserID, input.FormID, input.ProgramID, input.TermCode);
                    }
                }
            }
            else if (input.FormStateID == 10 || input.FormStateID == 11)
            {
                sendFormOfferedNotOffered(input.UserID, input.FormID, input.ProgramID, input.TermCode, input.FormStateID);
            }
            response.IsSuccess = true;
            response.Message = "Data updated successfully";
            return response;
        }

        private void sendFormSubmitted(int userID, int formID, int programID, string termCode)
        {
            // call SP getFormDetailsByFormID
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                   {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value =userID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = termCode }    
            };

            DataSet dsRec = _helper.GetDataSet("[dbo].[getFormDetailsByFormID]", parameters);
            try
            {
                if (dsRec.Tables[0].Rows.Count > 0)
                {
                    // applicant mail
                    string logoText = "cid:myImageID";
                    string applicantsName = string.Empty;
                    string toMail = string.Empty;
                    string ccMail = string.Empty;
                    string subject = string.Empty;
                    string body = string.Empty;
                    string programName = string.Empty;
                    int applicationTypeID = 0;
                    
                    applicantsName = Convert.ToString(dsRec.Tables[0].Rows[0]["ApplicantName"]);
                    toMail = Convert.ToString(dsRec.Tables[0].Rows[0]["cusulbEmail"]);
                    ccMail= Convert.ToString(dsRec.Tables[0].Rows[0]["altEmail"]);
                    programName = Convert.ToString(dsRec.Tables[0].Rows[0]["programName"]);
                    applicationTypeID = Convert.ToInt32(dsRec.Tables[1].Rows[0]["ApplicationTypeID"]);
                    subject = "Application Submitted";
                    //if(programID == 1 || programID == 2 || programID == 3 || programID == 4 || programID == 6)
                    //    body = GetMailBodyTemplate("Student_FormSubmit_Confirmation_ICP.html");
                    //else
                    //    body = GetMailBodyTemplate("Student_FormSubmit_Confirmation.html");
                    SqlParameter[] parameters1 ={
                                            new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt) { Value = applicationTypeID },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
                                            new SqlParameter("@Identifier", SqlDbType.NVarChar,50) { Value = "Application Submitted"}
                                       };
                    DataTable dtRec = _helper.GetDataTable("[dbo].[GetRecommenderMailBody]", parameters1);
                    if (dtRec.Rows.Count > 0)
                    {
                        if (dtRec.Rows[0]["RecommenderMailBody"] != DBNull.Value)
                        {
                            body = Convert.ToString(dtRec.Rows[0]["RecommenderMailBody"]);

                            string beforeBody = string.Empty;
                            string afterBody = string.Empty;
                            beforeBody = "<html><body><div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                            afterBody = "</body></html>";
                            body = $"{beforeBody}{body}{afterBody}";
                        }
                    }
                    body = body.Replace("[[logoPath]]", logoText) 
                               .Replace("[[ApplicantName]]", applicantsName)
                               .Replace("[[programName]]", programName);
                    byte[] inputStr = null;
                    _sendMail.SendEmail(toMail, ccMail,"COMMON", subject, body, inputStr);
                }
                // commented to restrict multiple mails to recommender 
                //if (dsRec.Tables[1].Rows.Count > 0)
                //{
                //    // loop through the data table for recommender mail
                //    for (int i = 0; i < dsRec.Tables[1].Rows.Count; i++) 
                //    {
                //        // send mail to the recommender with the attachment and the URL link  
                //    string logoText = "cid:myImageID";
                //    string userFolderPath = string.Empty;
                //    string templateFileName = string.Empty;
                //    string recommenderName = string.Empty;
                //    string recommenderEmail = string.Empty;
                //    string recommenderURL = string.Empty;
                //    string applicantsName = string.Empty;
                //    DateTime applicationDeadline;
                //    string body = string.Empty;
                //    string link = string.Empty;

                //    recommenderURL = Convert.ToString(dsRec.Tables[1].Rows[i]["RecommenderURL"]);
                //    body = Convert.ToString(dsRec.Tables[1].Rows[i]["MailBody"]);
                //    recommenderName = Convert.ToString(dsRec.Tables[1].Rows[i]["RecommenderName"]);
                //    recommenderEmail = Convert.ToString(dsRec.Tables[1].Rows[i]["RecommenderEmail"]);
                //    applicantsName = Convert.ToString(dsRec.Tables[1].Rows[i]["ApplicantName"]);
                //    applicationDeadline = Convert.ToDateTime(dsRec.Tables[1].Rows[i]["ApplicationDeadline"]);
                //    link = @"<a href ='" + recommenderURL + "' target='_blank'>here</a>";
                //    body = body.Replace("[[logoPath]]", logoText)
                //        .Replace("[[RecommenderName]]", recommenderName)
                //        .Replace("[[applicantname]]", applicantsName)
                //        .Replace("[[deadline]]", applicationDeadline.ToString("MM/dd/yyyy"))
                //        .Replace("[[link]]", link)
                //        ;
                //    //body = "Body section needs to be revisited with format and <a href=\"" + recommenderURL + "\" target=\"_blank\">the URL</a> for adding recommendations";
                //    userFolderPath = "SupportFiles/EmailAttachments";
                //    templateFileName = "Recommender_Template.pdf";
                //    byte[] fileContent = GetAttachmentContent(userFolderPath, templateFileName);
                //    string subject = "Attention: CSULB Recommendation Request";
                //    if ((!string.IsNullOrEmpty(recommenderEmail)) && (!string.IsNullOrEmpty(body)))
                //    {
                //        if (fileContent != null && fileContent.Length > 0)
                //        {
                //            _sendMail.SendEmail(recommenderEmail, "", subject, body, fileContent);
                //        }
                //    }
                //}

                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.StackTrace);
            }
        }

        private void sendFormOfferedNotOffered(int userID, int formID, int programID, string termCode, int formStateID)
        {
            // call SP getFormDetailsByFormID
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                   {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value =userID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = termCode }
            };

            DataSet dsRec = _helper.GetDataSet("[dbo].[getFormDetailsByFormID]", parameters);
            try
            {
                if (dsRec.Tables[0].Rows.Count > 0 && dsRec.Tables[1].Rows.Count > 0)
                {
                    int applicationTypeID = Convert.ToInt32(dsRec.Tables[1].Rows[0]["ApplicationTypeID"]);
                    string csulbid= Convert.ToString(dsRec.Tables[1].Rows[0]["CSULBID"]);
                    if (applicationTypeID == 1 || applicationTypeID == 2)
                    {
                        if (formStateID == 10)
                        {
                            // applicant mail
                            string logoText = "cid:myImageID";
                            string applicantsName = string.Empty;
                            string toMail = string.Empty;
                            string ccMail = string.Empty;
                            string subject = string.Empty;
                            string body = string.Empty;
                            string programName = string.Empty;
                            string finalDecision = string.Empty;
                            string decisionType = string.Empty;
                            string beforeBody = string.Empty;
                            string afterBody = string.Empty;
                            string programSelected = string.Empty;
                            string date = DateTime.Now.ToString("MMMM d, yyyy");
                            decisionType = "Offered";

                            applicantsName = Convert.ToString(dsRec.Tables[0].Rows[0]["ApplicantName"]);
                            toMail = Convert.ToString(dsRec.Tables[0].Rows[0]["cusulbEmail"]);
                            ccMail = Convert.ToString(dsRec.Tables[0].Rows[0]["altEmail"]);
                            programName = Convert.ToString(dsRec.Tables[0].Rows[0]["programName"]);
                            finalDecision = Convert.ToString(dsRec.Tables[0].Rows[0]["FinalDecision"]);
                            programSelected = Convert.ToString(dsRec.Tables[0].Rows[0]["ProgramSelected"]);

                            SqlParameter[] parameters2 ={
                                            new SqlParameter("@ProgramIdentifier", SqlDbType.NVarChar, 10) { Value = "" },
                                            new SqlParameter("@OfferedCategories", SqlDbType.NVarChar , 50) { Value = finalDecision },
                                            new SqlParameter("@DecisionType", SqlDbType.NVarChar , 50) { Value = decisionType },
                                            new SqlParameter("@ProgramId", SqlDbType.BigInt) { Value = programID }
                                       };

                            DataSet dtDL = _helper.GetDataSet("[Application].[GetDecisionLetters]", parameters2);
                            if (dtDL.Tables[1].Rows.Count > 0)
                            {
                                if (dtDL.Tables[1].Rows[0]["MailBody"] != DBNull.Value)
                                {
                                    body = Convert.ToString(dtDL.Tables[1].Rows[0]["MailBody"]);
                                }
                            }
                            if (applicationTypeID == 1)
                            {
                                string beforeContent = string.Empty;
                                (subject,beforeContent) = programID switch
                                {
                                    1 => (
                                        "CSULB Education Specialist Credential Program Admissions",
                                        "<h3>Education Specialist Credential Program</h3>"
                                    ),

                                    2 => (
                                        "CSULB Multiple Subject Credential Program Admissions",
                                        "<h3>Multiple Subject Credential Program</h3>"
                                    ),

                                    3 => (
                                        "CSULB PK3 Early Childhood Education Specialist Credential Program Admissions",
                                        "<h3>PK3 Early Childhood Education Specialist Credential Program</h3>"
                                    ),

                                    4 => (
                                        "CSULB Single Subject Credential Program Admissions",
                                        "<h3>Single Subject Credential Program</h3>"
                                    ),

                                    6 => (
                                        "CSULB Urban Dual Credential Program Admissions",
                                        "<h3>Urban Dual Credential Program</h3>"
                                    ),

                                    _ => (
                                        "CSULB Program Admissions",
                                        "<h3>CSULB Program Admissions</h3>"
                                    )
                                };

                                beforeBody = $@"<html><body><img alt=""logo"" src=[[logoPath]] style=""width:300px; height:auto;"" /><div style=""width: 100%; border-bottom: 2px solid black; font-family: Arial; margin-top: 10px;"">{beforeContent}</div>";
                                //if(programID == 1 || programID == 2 || programID == 4 || programID == 6)
                                //    //body = GetMailBodyTemplate("Student_FormOffer_Confirmation_ICP.html");
                                //else
                                //    body = GetMailBodyTemplate("Student_FormOffer_Confirmation.html");
                            }
                            else if (applicationTypeID == 2)
                            {
                                subject = "Application Offered";
                                beforeBody = "<html><body><div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                            }
                            //else(formStateID == 11)
                            //{
                            //   subject = "Application Not Offered";
                            //   decisionType = "Not Offered";
                            //   beforeBody = "<html><body><div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                            //}
                            //beforeBody = "<html><body><div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                            afterBody = "</body></html>";
                            body = $"{beforeBody}{body}{afterBody}";
                            body = body.Replace("[[logoPath]]", logoText)
                                       .Replace("[[ApplicantName]]", applicantsName)
                                       .Replace("[[programName]]", programName)
                                       .Replace("[[finalDecision]]", finalDecision)
                                       .Replace("[[CSULBID]]", csulbid)
                                       .Replace("[[date]]", date)
                                       .Replace("[[ProgramSelected]]", programSelected);
                            byte[] inputStr = null;
                            _sendMail.SendEmail(toMail, ccMail, "COMMON", subject, body, inputStr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.StackTrace);
            }
        }

        private void sendFormRecommender(int userID, int formID, int programID, string termCode)
        {
            // call SP getFormDetailsByFormID
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                   {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value =userID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = termCode }
            };

            DataSet dsRec = _helper.GetDataSet("[dbo].[getFormRecommenderDetailsByFormID]", parameters);
            try
            {
                if (dsRec.Tables[0].Rows.Count > 0)
                {
                   
                    // loop through the data table for recommender mail
                    for (int i = 0; i < dsRec.Tables[0].Rows.Count; i++)
                    {
                        // send mail to the recommender with the attachment and the URL link  
                        string logoText = "cid:myImageID";
                        string userFolderPath = string.Empty;
                        string templateFileName = string.Empty;
                        string recommenderName = string.Empty;
                        string recommenderEmail = string.Empty;
                        string recommenderURL = string.Empty;
                        string applicantsName = string.Empty;
                        DateTime applicationDeadline;
                        string body = string.Empty;
                        string link = string.Empty;
                        bool RecommenderMailTemplateAttachement = false;

                        recommenderURL = Convert.ToString(dsRec.Tables[0].Rows[i]["RecommenderURL"]);
                        body = Convert.ToString(dsRec.Tables[0].Rows[i]["MailBody"]);
                        recommenderName = Convert.ToString(dsRec.Tables[0].Rows[i]["RecommenderName"]);
                        recommenderEmail = Convert.ToString(dsRec.Tables[0].Rows[i]["RecommenderEmail"]);
                        applicantsName = Convert.ToString(dsRec.Tables[0].Rows[i]["ApplicantName"]);
                        applicationDeadline = Convert.ToDateTime(dsRec.Tables[0].Rows[i]["ApplicationDeadline"]);
                        RecommenderMailTemplateAttachement = Convert.ToBoolean(dsRec.Tables[0].Rows[0]["RecommenderMailTemplateAttachement"]);
                        string beforeBody = string.Empty;
                        string afterBody = string.Empty;
                        if (programID == 1 || programID == 2 || programID == 3 || programID == 4 || programID == 6 || programID == 27 || programID == 28)
                        {
                            beforeBody = "<html><body><div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                            afterBody = "</body></html>";
                            body = $"{beforeBody}{body}{afterBody}";
                        }
                        else
                        {
                            beforeBody = "<html><head><style>body{width:  210mm;height: 297mm;margin-left: auto;margin-right: auto;}#link { color: #0563C1; }</style></head><body><div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                            afterBody = "</body></html>";
                            body = $"{beforeBody}{body}{afterBody}";
                        }

                        link = @"<a href ='" + recommenderURL + "' target='_blank'>here</a>";
                        body = body.Replace("[[logoPath]]", logoText)
                            .Replace("[[RecommenderName]]", recommenderName)
                            .Replace("[[applicantname]]", applicantsName)
                            .Replace("[[deadline]]", applicationDeadline.ToString("MM/dd/yyyy"))
                            .Replace("[[link]]", link);

                        string subject = "Attention: CSULB Recommendation Request";
                        if (RecommenderMailTemplateAttachement)
                        {
                            userFolderPath = "SupportFiles/EmailAttachments";
                            templateFileName = "Recommender_Template_"+programID+".pdf";
                            byte[] fileContent = GetAttachmentContent(userFolderPath, templateFileName);
                            if (fileContent == null || fileContent.Length <1)
                            {
                                templateFileName = "Recommender_Template.pdf";
                                fileContent = GetAttachmentContent(userFolderPath, templateFileName);
                            }
                            if ((!string.IsNullOrEmpty(recommenderEmail)) && (!string.IsNullOrEmpty(body)))
                            {
                                if (fileContent != null && fileContent.Length > 0)
                                {
                                    _sendMail.SendEmail(recommenderEmail, "", "RECOMMENDER", subject, body, fileContent);
                                }
                            }
                        }
                        else
                        {
                            if ((!string.IsNullOrEmpty(recommenderEmail)) && (!string.IsNullOrEmpty(body)))
                            {
                                _sendMail.SendEmail(recommenderEmail, "", "RECOMMENDER", subject, body, "");

                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.StackTrace);
            }
        }

        public FormAttachments DownloadFormAttachments(int userID, int formattachmentID)
        {
            FormAttachments obj = new FormAttachments();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userID },
                                          new SqlParameter("@FormAttachmentID", SqlDbType.BigInt) { Value = formattachmentID }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[dbo].[GetFormAttachment]", parameters);

            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new FormAttachments
                                          {
                                              Filename = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(Path.Combine(GetAttachmentsFolderName(row["FolderName"].ToString()), "Form"), GetAttachmentsSavedFileName(row["FolderName"].ToString()) + "." + Convert.ToString(row["FileExtn"]))
                                          }).FirstOrDefault();

            return obj;
        }
        public InstructorAttachments GetInstructionAttachment(int UserID, int InstructionAttachmentID)
        {
            InstructorAttachments obj = new InstructorAttachments();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                          new SqlParameter("@InstructionAttachmentID", SqlDbType.BigInt) { Value = InstructionAttachmentID }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[dbo].[GetInstructionAttachment]", parameters);

            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new InstructorAttachments
                                          {
                                              Filename = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(Path.Combine(GetAttachmentsFolderName(row["FolderName"].ToString()), "Form"), GetAttachmentsSavedFileName(row["FolderName"].ToString()) + "." + Convert.ToString(row["FileExtn"]))
                                          }).FirstOrDefault();

            return obj;
        }
        public InterviewerAttachments GetInterviewAttachments(int UserID, int InterviewAttachmentID)
        {
            InterviewerAttachments obj = new InterviewerAttachments();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                          new SqlParameter("@InterviewAttachmentID", SqlDbType.BigInt) { Value = InterviewAttachmentID }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[dbo].[GetInterviewAttachments]", parameters);

            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new InterviewerAttachments
                                          {
                                              Filename = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(Path.Combine(GetAttachmentsFolderName(row["FolderName"].ToString()), "Form"), GetAttachmentsSavedFileName(row["FolderName"].ToString()) + "." + Convert.ToString(row["FileExtn"]))
                                          }).FirstOrDefault();

            return obj;
        }
        public RecommendationAttachments GetFormRecommendations(int userID, int recommendationAttachmentID)
        {
            RecommendationAttachments obj = new RecommendationAttachments();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userID },
                                          new SqlParameter("@RecommendationttachmentID", SqlDbType.BigInt) { Value = recommendationAttachmentID }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[dbo].[GetFormRecommendations]", parameters);

            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new RecommendationAttachments
                                          {
                                              Filename = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(Path.Combine(GetAttachmentsFolderName(row["FolderName"].ToString()), "Form"), GetAttachmentsSavedFileName(row["FolderName"].ToString()) + "." + Convert.ToString(row["FileExtn"]))
                                          }).FirstOrDefault();

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

        public byte[] GetAttachmentContent(string userFolderPath, string fileName)
        {
            string filepath = Path.Combine(userFolderPath, fileName);
            byte[] fileContent = null;
            if (File.Exists(Path.GetFullPath(filepath))) 
            { 
            System.IO.FileStream fs = new System.IO.FileStream(Path.GetFullPath(filepath), System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            long byteLength = new System.IO.FileInfo(filepath).Length;
            fileContent = binaryReader.ReadBytes((Int32)byteLength);
            fs.Close();
            fs.Dispose();
            binaryReader.Close(); 
            }
            return fileContent;
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

        public BaseResponse AddRecommender(FormAddRecommenderRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@RecommenderName", SqlDbType.NVarChar, 250) { Value = input.RecommenderName },
                                          new SqlParameter("@RecommenderEmail", SqlDbType.NVarChar, 250) { Value = input.RecommenderEmail },
                                          new SqlParameter("@RecommenderAffiliation", SqlDbType.NVarChar, 200) { Value = input.RecommenderAffiliation },

                                          new SqlParameter("@DocumentID", SqlDbType.BigInt) { Value = DBNull.Value },
                                          new SqlParameter("@RecommenderIdentifier", SqlDbType.UniqueIdentifier, 250) { Value = DBNull.Value },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 250) { Value = DBNull.Value },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = DBNull.Value },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = DBNull.Value }
                                          

                                        };

            // int id = _helper.InsertTable("[dbo].[UpsertFormRecommend]", parameters);
            DataTable dtRec = _helper.GetDataTable("[dbo].[UpsertFormRecommend]", parameters);
            #region Old Codes
            //try
            //{


            //    if (dtRec.Rows.Count > 0)
            //    {
            //        // send mail to the recommender with the attachment and the URL link 

            //        // Pull the attachment and email template from supporting files folder 
            //        string logoText = "cid:myImageID";
            //        string userFolderPath = string.Empty;
            //        string templateFileName = string.Empty;
            //        string recommenderName = string.Empty;
            //        string recommenderEmail = string.Empty;
            //        string recommenderURL = string.Empty;
            //        string applicantsName = string.Empty;
            //        DateTime applicationDeadline;
            //        string body = string.Empty;
            //        string link = string.Empty;

            //        recommenderURL = Convert.ToString(dtRec.Rows[0]["RecommenderURL"]);
            //        body = Convert.ToString(dtRec.Rows[0]["MailBody"]);
            //        recommenderName = Convert.ToString(dtRec.Rows[0]["RecommenderName"]);
            //        recommenderEmail = Convert.ToString(dtRec.Rows[0]["RecommenderEmail"]);
            //        applicantsName = Convert.ToString(dtRec.Rows[0]["ApplicantName"]);
            //        applicationDeadline = Convert.ToDateTime(dtRec.Rows[0]["ApplicationDeadline"]);
            //        link = @"<a href ='" + recommenderURL + "' target='_blank'>here</a>";
            //        body = body.Replace("[[logoPath]]", logoText)
            //            .Replace("[[RecommenderName]]", recommenderName)
            //            .Replace("[[applicantname]]", applicantsName)
            //            .Replace("[[deadline]]", applicationDeadline.ToString("MM/dd/yyyy"))
            //            .Replace("[[link]]", link)
            //            ;
            //        //body = "Body section needs to be revisited with format and <a href=\"" + recommenderURL + "\" target=\"_blank\">the URL</a> for adding recommendations";
            //        userFolderPath = "SupportFiles/EmailAttachments";
            //        templateFileName = "Recommender_Template.pdf";
            //        byte[] fileContent = GetAttachmentContent(userFolderPath, templateFileName);
            //        string subject = "Attention: CSULB Recommendation Request";
            //        if ((!string.IsNullOrEmpty(recommenderEmail)) && (!string.IsNullOrEmpty(body))) 
            //            {
            //            if (fileContent != null && fileContent.Length > 0)
            //            {
            //                _sendMail.SendEmail(recommenderEmail, "", subject, body, fileContent);
            //            }
            //        }

            //    }
            //}
            //catch (Exception)
            //{

            //}
            #endregion
            response.IsSuccess = true;
            response.Message = "Recommender Added Successfully";

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
        private string GetDocumentTemplate(string templateName)
        {
            string body = string.Empty;
            string filepath = Path.Combine("SupportFiles/DocumentTemplates", templateName);
            using (StreamReader reader = new StreamReader(Path.GetFullPath(filepath)))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }

        private string GetDocumentBodyTemplate(string programIdentifier)
        {
            string body = string.Empty;
            string templateName = string.Empty;
            if (programIdentifier.ToUpper() == "SSCP")
                templateName = "SSCPRecommendationFormTemplate.htm";
            else if (programIdentifier.ToUpper() == "MSCP")
                templateName = "MSCPRecommendationFormTemplate.htm";
            else if (programIdentifier.ToUpper() == "UDCP")
                templateName = "UDCPRecommendationFormTemplate.htm";
            else if (programIdentifier.ToUpper() == "ESCP")
                templateName = "ESCPRecommendationFormTemplate.htm";
            else if (programIdentifier.ToUpper() == "PK-3CP")
                templateName = "PK-3CPRecommendationFormTemplate.htm";
            else if (programIdentifier == "MS Special Education (SPED)")
                templateName = "MSSPEDRecommendationFormTemplate.html";
            else
                templateName = "GraduateRecommendationFormTemplate.htm";

            string filepath = Path.Combine("SupportFiles/DocumentTemplates", templateName);
            using (StreamReader reader = new StreamReader(Path.GetFullPath(filepath)))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }
        public BaseResponse SendReminderToRecommender(int recommendationID)
        {
            BaseResponse obj = new BaseResponse();
            SqlParameter[] parameters =
                                   {
                                          new SqlParameter("@RecommendationID", SqlDbType.BigInt) { Value = recommendationID }

                                   };
            DataSet dsRec = _helper.GetDataSet("[dbo].[getFormRecommenderDetailsByID]", parameters);
            try
            {
                if (dsRec.Tables[0].Rows.Count > 0)
                {
                        // send mail to the recommender with the attachment and the URL link  
                        string logoText = "cid:myImageID";
                        string userFolderPath = string.Empty;
                        string templateFileName = string.Empty;
                        string recommenderName = string.Empty;
                        string recommenderEmail = string.Empty;
                        string recommenderURL = string.Empty;
                        string applicantsName = string.Empty;
                        DateTime applicationDeadline;
                        string body = string.Empty;
                        string link = string.Empty;
                        bool RecommenderMailTemplateAttachement = false;
                        int programID = 0;

                        recommenderURL = Convert.ToString(dsRec.Tables[0].Rows[0]["RecommenderURL"]);
                        body = Convert.ToString(dsRec.Tables[0].Rows[0]["MailBody"]);
                        recommenderName = Convert.ToString(dsRec.Tables[0].Rows[0]["RecommenderName"]);
                        recommenderEmail = Convert.ToString(dsRec.Tables[0].Rows[0]["RecommenderEmail"]);
                        applicantsName = Convert.ToString(dsRec.Tables[0].Rows[0]["ApplicantName"]);
                        applicationDeadline = Convert.ToDateTime(dsRec.Tables[0].Rows[0]["ApplicationDeadline"]);
                        RecommenderMailTemplateAttachement = Convert.ToBoolean(dsRec.Tables[0].Rows[0]["RecommenderMailTemplateAttachement"]);
                        programID = Convert.ToInt32(dsRec.Tables[0].Rows[0]["ProgramID"]);
                        string beforeBody = string.Empty;
                        string afterBody = string.Empty;
                        if (programID == 1 || programID == 2 || programID == 3 || programID == 4 || programID == 6 || programID == 27 || programID == 28)
                        {
                            beforeBody = "<html><body><div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                            afterBody = "</body></html>";
                            body = $"{beforeBody}{body}{afterBody}";
                        }
                        else
                        {
                            beforeBody = "<html><head><style>body{width:  210mm;height: 297mm;margin-left: auto;margin-right: auto;}#link { color: #0563C1; }</style></head><body><div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                            afterBody = "</body></html>";
                            body = $"{beforeBody}{body}{afterBody}";
                        }
                        link = @"<a href ='" + recommenderURL + "' target='_blank'>here</a>";
                        body = body.Replace("[[logoPath]]", logoText)
                            .Replace("[[RecommenderName]]", recommenderName)
                            .Replace("[[applicantname]]", applicantsName)
                            .Replace("[[deadline]]", applicationDeadline.ToString("MM/dd/yyyy"))
                            .Replace("[[link]]", link)
                            ;
                        string subject = "Attention: CSULB Recommendation Request";
                        if (RecommenderMailTemplateAttachement)
                        {
                            userFolderPath = "SupportFiles/EmailAttachments";
                            templateFileName = "Recommender_Template_" + programID + ".pdf";
                            byte[] fileContent = GetAttachmentContent(userFolderPath, templateFileName);
                            //_logger.LogInformation(fileContent.Length.ToString());
                            if (fileContent == null || fileContent.Length < 1)
                            {
                                templateFileName = "Recommender_Template.pdf";
                                fileContent = GetAttachmentContent(userFolderPath, templateFileName);
                            }
                            if ((!string.IsNullOrEmpty(recommenderEmail)) && (!string.IsNullOrEmpty(body)))
                            {
                                if (fileContent != null && fileContent.Length > 0)
                                {
                                    _sendMail.SendEmail(recommenderEmail, "", "RECOMMENDER", subject, body, fileContent);
                                    obj.IsSuccess = true;
                                    obj.Message = "Mail sent successfully !";
                                }
                            }
                        }
                        else
                        {
                            if ((!string.IsNullOrEmpty(recommenderEmail)) && (!string.IsNullOrEmpty(body)))
                            {
                                    _sendMail.SendEmail(recommenderEmail, "", "RECOMMENDER", subject, body, "");
                                    obj.IsSuccess = true;
                                    obj.Message = "Mail sent successfully !";
                            
                            }
                        }

                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Error sending mail , please try after sometime !";
                _logger.LogError(ex.Message, ex.StackTrace);
            }
            return obj;
        }
        public BaseResponse UpdateReviwerReview(FormReviewerReviewRequest input)
        {
            BaseResponse obj = new BaseResponse();
            SqlParameter[] parameters =
                                   {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ReviewID", SqlDbType.BigInt) { Value = input.ReviewID },
                                          new SqlParameter("@ReviewerID", SqlDbType.BigInt) { Value = input.ReviewerID },
                                          new SqlParameter("@ReviewerRecommendation", SqlDbType.NVarChar, 200) { Value = input.ReviewerRecommendation },
                                          new SqlParameter("@ReviewerComments", SqlDbType.NVarChar, 2000) { Value = input.ReviewerComments },

                                   };

            int id = _helper.InsertTable("[dbo].[UpdateReviewerReview]", parameters);
            obj.IsSuccess = true;
            obj.Message = "Review Saved Successfully";
            return obj;
        }

        public BaseResponse AddRecommendation(FormAddRecommendationRequest input)
        {
            BaseResponse response= new BaseResponse(); 
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            bool sendMail = false;
            byte[] fileContentJSONToPDF = null;
            //input.LetterOfRecommendationJSON = "{\r\n  \"personalInfo\": {\r\n    \"job_title\": \"test\",\r\n    \"recommenderName\": \"Nandini\",\r\n    \"studentName\": \"Kaylee Brubaker\",\r\n    \"occupation\": \"t\",\r\n    \"organization\": \"t\",\r\n    \"email\": \"nandinim.prakash@gmail.com\",\r\n    \"phone\": \"(555) 555 - 5555\"\r\n  },\r\n  \"relationship\": {\r\n    \"howLongApplicantKnown\": \"1-2 Years\",\r\n    \"inWhatCapacityApplicantKnown\": \"Employee/Supervisor\"\r\n  },\r\n  \"referrenceRatings\": [\r\n    {\r\n      \"quality\": \"Communication Skills, Oral:\",\r\n      \"scale\": \"1\"\r\n    },\r\n    {\r\n      \"quality\": \"Communication Skills, Written:\",\r\n      \"scale\": \"3\"\r\n    },\r\n    {\r\n      \"quality\": \"Technology Skills:\",\r\n      \"scale\": \"2\"\r\n    },\r\n    {\r\n      \"quality\": \"Initiative:\",\r\n      \"scale\": \"2\"\r\n    },\r\n    {\r\n      \"quality\": \"Maturity:\",\r\n      \"scale\": \"1\"\r\n    },\r\n    {\r\n      \"quality\": \"Motivation for this program of study:\",\r\n      \"scale\": \"2\"\r\n    },\r\n    {\r\n      \"quality\": \"Creativity:\",\r\n      \"scale\": \"1\"\r\n    },\r\n    {\r\n      \"quality\": \"Ability to work with others:\",\r\n      \"scale\": \"2\"\r\n    },\r\n    {\r\n      \"quality\": \"Intellectual potential:\",\r\n      \"scale\": \"4\"\r\n    },\r\n    {\r\n      \"quality\": \"Present academic performance:\",\r\n      \"scale\": \"4\"\r\n    },\r\n    {\r\n      \"quality\": \"Potential for graduate work:\",\r\n      \"scale\": \"3\"\r\n    }\r\n  ],\r\n  \"overAllRecommendationAdmission\": \"Without Reservation\"\r\n}";
            //input.LetterOfRecommendationJSON = "{\r\n  personalInfo: {\r\n    job_title: \"xxx\",\r\n    recommenderName: \"chandana\",\r\n    studentName: \"xello\",\r\n    occupation: \"ggg\",\r\n    organization: \"ttt\",\r\n    email: \"ttt@gmail.com\",\r\n    phone: \"34456456546\",\r\n  },\r\n  relationship: {\r\n    howLongApplicantKnown: \"1-2 Years\",\r\n    inWhatCapacityApplicantKnown: \"1-2 Years\",\r\n  },\r\n  referrenceRatings: [\r\n    {\r\n      quality: \"Communication Skills, Oral:\",\r\n      scale: \"1\",\r\n    },\r\n    {\r\n      quality: \"Communication Skills, Written:\",\r\n      scale: \"2\",\r\n    },\r\n    {\r\n      quality: \"Technology Skills:\",\r\n      scale: \"3\",\r\n    },\r\n    {\r\n      quality: \"Initiative:\",\r\n      scale: \"4\",\r\n    },\r\n    {\r\n      quality: \"Maturity:\",\r\n      scale: \"5\",\r\n    },\r\n    {\r\n      quality: \"Motivation for this program of study:\",\r\n      scale: \"1\",\r\n    },\r\n    {\r\n      quality: \"Creativity:\",\r\n      scale: \"2\",\r\n    },\r\n    {\r\n      quality: \"Ability to work with others:\",\r\n      scale: \"3\",\r\n    },\r\n    {\r\n      quality: \"Intellectual potential:\",\r\n      scale: \"4\",\r\n    },\r\n    {\r\n      quality: \"Present academic performance:\",\r\n      scale: \"5\",\r\n    },\r\n    {\r\n      quality: \"Potential for graduate work:\",\r\n      scale: \"1\",\r\n    },\r\n  ],\r\n  overAllRecommendationAdmission: \"Highest\",\r\nquestion1:\"hello\",\r\nquestion2:\"world\",\r\n}";
            if ((input.LetterOfRecommendationJSON != string.Empty) && (input.LetterOfRecommendationJSON != null))
            {
                fileContentJSONToPDF = GetPDFFromJSON(input.LetterOfRecommendationJSON, input.ProgramName);
            }
            if((input.FormAddRecommendationRequestAttachment.Count < 2) && (input.LetterOfRecommendationJSON != string.Empty) && (input.LetterOfRecommendationJSON != null))
            {
                var attachment = new FormAddRecommendationRequestAttachment
                {
                    DocumentID = 3,
                    FileName = "",
                    FileContent = null
                };
                input.FormAddRecommendationRequestAttachment.Add(attachment);
            }
            foreach (var attachment in input.FormAddRecommendationRequestAttachment)
            {
                //response = new BaseResponse();
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                string userFolderName = string.Empty;
                string savedFileName = string.Empty;
                string letterOfRecommendationJSON = string.Empty;

                // pull the saved file name format SP Below
                FormAttachmentFileNames fileNames = new FormAttachmentFileNames();
                if ((input.ProgramFormIdentifier == "GACP") && (input.LetterOfRecommendationJSON != string.Empty) && (input.LetterOfRecommendationJSON != null))
                {
                    if (attachment.DocumentID == 3)
                    {
                        letterOfRecommendationJSON = input.LetterOfRecommendationJSON;
                        attachment.FileContent = fileContentJSONToPDF;
                        //attachment.FileName = fileNames.FileName + ".pdf";
                    }
                }
                if (attachment.FileContent != null && attachment.FileContent.Length > 0)
                {
                    fileNames = GetFormRecommendAttachmentFileName(input.RecommenderIdentifier, attachment.DocumentID);
                    attachment.FileName = fileNames.FileName + ".pdf";
                }
                else
                {
                    fileNames = GetFormRecommendAttachmentFileName(input.RecommenderIdentifier, attachment.DocumentID);
                    string[] folderSplit = fileNames.SavedFileName.ToString().Split('_');
                    string folderName = folderSplit[0].ToString();
                    fileNames.SavedFileName = folderName + '_' + attachment.FileName;
               
                    fileNames.FileName = attachment.FileName;
                }
                //if(attachment.DocumentID ==18)
                //{
                //    string filepath = "D:\\TestFiles\\Test doc.pdf";
                //    byte[] fileContent = null;
                //    System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                //    System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
                //    long byteLength = new System.IO.FileInfo(filepath).Length;
                //    fileContent = binaryReader.ReadBytes((Int32)byteLength);
                //    attachment.FileContent = fileContent;
                //    fs.Close();
                //    fs.Dispose();
                //    binaryReader.Close();
                //}
                if (!string.IsNullOrEmpty(fileNames.FileName) && attachment.FileContent != null && !string.IsNullOrEmpty(attachment.FileName))
                {
                    if (!sendMail) { sendMail = true; }

                    if (attachment.FileName != string.Empty)
                    {
                        fileName = attachment.FileName;
                        if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            fileName += ".pdf";
                        }

                        AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(fileName);
                        // fileName = fileDetails.FileName;
                        fileExtension = fileDetails.FileExtension;
                    }

                    SqlParameter[] parameters =
                                            {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = DBNull.Value },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = DBNull.Value },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = DBNull.Value },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = DBNull.Value },
                                          new SqlParameter("@RecommenderName", SqlDbType.NVarChar, 250) { Value = DBNull.Value },
                                          new SqlParameter("@RecommenderEmail", SqlDbType.NVarChar, 250) { Value = DBNull.Value },

                                          new SqlParameter("@DocumentID", SqlDbType.BigInt) { Value = attachment.DocumentID },
                                          new SqlParameter("@RecommenderIdentifier", SqlDbType.UniqueIdentifier, 250) { Value = new Guid(input.RecommenderIdentifier) },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 250) { Value = fileNames.FileName },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = fileNames.SavedFileName },
                                          new SqlParameter("@LetterOfRecommendationJSON", SqlDbType.VarChar, -1) { Value = letterOfRecommendationJSON },
                                          new SqlParameter("@State", SqlDbType.NVarChar, 10) { Value = input.Action }
                                        };

                    //int id = _helper.InsertTable("[dbo].[UpsertFormRecommend]", parameters);
                    DataTable dtRec = _helper.GetDataTable("[dbo].[UpsertFormRecommend]", parameters);

                    // save the file in physicalpath
                    // check if the userFolder exists and if it exists then check if if the FieldWork Folder exists
                    //string[] folderSplit = fileNames.SavedFileName.ToString().Split('~');
                    if (attachment.FileContent != null && attachment.FileContent.Length > 0)
                    {
                        userFolderName = fileNames.UserFolder.ToString();
                        string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
                  
                        if (Directory.Exists(dirUserFolderPath))
                        {
                            string dirForm = Path.Combine(dirUserFolderPath, "Form");
                            if (Directory.Exists(dirForm))
                            {
                                // copy the file here 
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), attachment.FileContent);
                            }
                            else
                            {
                                Directory.CreateDirectory(dirForm);
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), attachment.FileContent);
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

                            File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), attachment.FileContent);
                        }
                    }
                    // pull the applicant details based on the recommenderIdentifier 
                    // call getApplicantByRecommenderIdentifier
                    
                    response.IsSuccess = true;
                    response.Message = "Recommendation attached successfully";
                }
                //else
                //{
                //    response.IsSuccess = false;
                //    response.Message = "Failed to attach recommendations";
                    
                //}
            }

            if (sendMail && input.Action =="submit")
            {
                // send mail to the applicant 
                //string programIdentifier = "";
                SendRecommendedConfirmMailToApplicant(input.RecommenderIdentifier,input.ProgramFormIdentifier);
            }


            return response;
        }
        public BaseResponse AddRecommendationForm(FormAddRecommendation input)
        {
            BaseResponse response = new BaseResponse();
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            bool sendMail = false;
            //input.LetterOfRecommendationJSON = "{\"personalInfo\":{\"position_title\":\"\",\"recommenderFirstName\":\"chandana\",\"recommenderLastName\":\"\",\"studentName\":\"Amanda Amaro\",\"campusID\":\"008081825\",\"email\":\"Amanda.Amaro01@student.csulb.edu\"},\"signatureOfRecommender\":{\"name\":\"\",\"date\":\"04/01/2026\"},\"academicCompetency\":{\"comments\":\"\",\"scale\":\"Area Needs Improvement\"},\"professionalism\":{\"comments\":\"\",\"scale\":\"Average (Satisfactory)\"},\"dispositionsPersonalityCharacter\":{\"comments\":\"\",\"scale\":\"Average (Satisfactory)\"},\"specialEducation\":{\"comments\":\"\",\"scale\":\"Area Needs Improvement\"},\"studentOverAllRank\":\"Top 5% One of the best\"}";
            // convert JSON to PDF - delete the existing letter of recommendation and create new 
            byte[] fileContentJSONToPDF = GetPDFFromJSON(input.LetterOfRecommendationJSON,input.ProgramFormIdentifier);
            //byte[] fileContentJSONToPDF = GetFileContent("Recommender_Template.pdf");
            // update the JSON and save to DB - call [dbo].[UpdateApplicationRecommendations]
            // FormAddRecommendationRequestAttachment - add file content to this class 
            foreach (var attachment in input.FormAddRecommendationRequestAttachment)
            {
                //response = new BaseResponse();
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                string userFolderName = string.Empty;
                string savedFileName = string.Empty;
                string letterOfRecommendationJSON = string.Empty;
                // pull the saved file name format SP Below
                FormAttachmentFileNames fileNames = new FormAttachmentFileNames();
                if (attachment.DocumentID == 3)
                {
                    letterOfRecommendationJSON = input.LetterOfRecommendationJSON;
                    attachment.FileContent =fileContentJSONToPDF;
                    //attachment.FileName = fileNames.FileName + ".pdf";
                }
                if (attachment.FileContent != null && attachment.FileContent.Length > 0)
                {
                    fileNames = GetFormRecommendAttachmentFileName(input.RecommenderIdentifier, attachment.DocumentID);
                    attachment.FileName = fileNames.FileName + ".pdf";
                }
                else
                {
                    fileNames = GetFormRecommendAttachmentFileName(input.RecommenderIdentifier, attachment.DocumentID);
                    string[] folderSplit = fileNames.SavedFileName.ToString().Split('_');
                    string folderName = folderSplit[0].ToString();
                    fileNames.SavedFileName = folderName + '_' + attachment.FileName;

                    fileNames.FileName = attachment.FileName;
                }

                if (!string.IsNullOrEmpty(fileNames.FileName) && attachment.FileContent != null && !string.IsNullOrEmpty(attachment.FileName))
                {
                    if (!sendMail) { sendMail = true; }

                    if (attachment.FileName != string.Empty)
                    {
                        fileName = attachment.FileName;
                        if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            fileName += ".pdf";
                        }

                        AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(fileName);
                        // fileName = fileDetails.FileName;
                        fileExtension = fileDetails.FileExtension;
                    }

                    SqlParameter[] parameters =
                                            {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = DBNull.Value },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = DBNull.Value },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = DBNull.Value },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = DBNull.Value },
                                          new SqlParameter("@RecommenderName", SqlDbType.NVarChar, 250) { Value = DBNull.Value },
                                          new SqlParameter("@RecommenderEmail", SqlDbType.NVarChar, 250) { Value = DBNull.Value },

                                          new SqlParameter("@DocumentID", SqlDbType.BigInt) { Value = attachment.DocumentID },
                                          new SqlParameter("@RecommenderIdentifier", SqlDbType.UniqueIdentifier, 250) { Value = new Guid(input.RecommenderIdentifier) },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 250) { Value = fileNames.FileName },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = fileNames.SavedFileName },
                                          new SqlParameter("@LetterOfRecommendationJSON", SqlDbType.VarChar, -1) { Value = letterOfRecommendationJSON },
                                          new SqlParameter("@State", SqlDbType.NVarChar, 10) { Value = input.Action }
                                        };

                    //int id = _helper.InsertTable("[dbo].[UpsertFormRecommend]", parameters);
                    DataTable dtRec = _helper.GetDataTable("[dbo].[UpsertFormRecommend]", parameters);

                    // save the file in physicalpath
                    // check if the userFolder exists and if it exists then check if if the FieldWork Folder exists
                    //string[] folderSplit = fileNames.SavedFileName.ToString().Split('~');
                    if (attachment.FileContent != null && attachment.FileContent.Length > 0)
                    {
                        userFolderName = fileNames.UserFolder.ToString();
                        string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
                        if (Directory.Exists(dirUserFolderPath))
                        {
                            string dirForm = Path.Combine(dirUserFolderPath, "Form");
                            if (Directory.Exists(dirForm))
                            {
                                // copy the file here 
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), attachment.FileContent);
                            }
                            else
                            {
                                Directory.CreateDirectory(dirForm);
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), attachment.FileContent);
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

                            File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), attachment.FileContent);
                        }
                    }
                    // pull the applicant details based on the recommenderIdentifier 
                    // call getApplicantByRecommenderIdentifier

                    response.IsSuccess = true;
                    response.Message = "Recommendation attached successfully";
                }
                //else
                //{
                //    response.IsSuccess = false;
                //    response.Message = "Failed to attach recommendations";

                //}
            }

            if (sendMail && input.Action == "submit")
            {
                // send mail to the applicant 
                SendRecommendedConfirmMailToApplicant(input.RecommenderIdentifier,input.ProgramFormIdentifier);
            }


            return response;
        }
        private byte[] GetPDFFromJSON(string jsonString,string programIdentifier)
        {
            byte[] pdfFileContent = null;
            string recommendationTemplateBody = string.Empty;
            JObject schema = JObject.Parse(jsonString);
            if (programIdentifier.ToUpper() == "MSCP"|| programIdentifier.ToUpper() == "SSCP"|| programIdentifier.ToUpper() == "UDCP")
            {

                string applicantFirstName = string.Empty;
                string applicantLastName = string.Empty;
                string credentialSubjectArea = string.Empty;
                string campusID = string.Empty;
                string recommenderFirstName = string.Empty;
                string recommenderLastName = string.Empty;
                string institution = string.Empty;
                string position_title = string.Empty;
                string telephoneContact = string.Empty;
                string email = string.Empty;

                string answer1 = string.Empty;
                string answer2 = string.Empty;
                string answer3 = string.Empty;
                string answer4 = string.Empty;
                string intellectualCapacity = string.Empty;
                string maturity = string.Empty;
                string potentialForTeaching = string.Empty;
                string professionalConductDeposition = string.Empty;
                string abilityToWorkWithOthers = string.Empty;
                string recommendationForLongBeach = string.Empty;
                string signature = string.Empty;
                string date = string.Empty;
                string comment = string.Empty;

                
                JObject personalInfo = (JObject)schema["personalInfo"];
                JObject answers= (JObject)schema["applicantRelatedAnswers"];
                JObject signatureOfRecommender = (JObject)schema["signatureOfRecommender"];
                JArray answer5 = (JArray)answers["answer5"];
                //JArray recommendationsForLB = (JArray)schema["recommendationForLongBeach"];

                // section for personal info 
                applicantFirstName = Convert.ToString(personalInfo.GetValue("applicantFirstName"));
                applicantLastName = Convert.ToString(personalInfo.GetValue("applicantLastName"));
                credentialSubjectArea = Convert.ToString(personalInfo.GetValue("credentialSubjectArea"));
                campusID = Convert.ToString(personalInfo.GetValue("campusID"));
                recommenderFirstName = Convert.ToString(personalInfo.GetValue("recommenderFirstName"));
                recommenderLastName = Convert.ToString(personalInfo.GetValue("recommenderLastName"));
                institution = Convert.ToString(personalInfo.GetValue("institution"));
                position_title = Convert.ToString(personalInfo.GetValue("position_title"));
                telephoneContact = Convert.ToString(personalInfo.GetValue("telephoneContact"));
                email = Convert.ToString(personalInfo.GetValue("email"));
                // section for answers 1 to 4
                answer1 = Convert.ToString(answers.GetValue("answer1"));
                answer2 = Convert.ToString(answers.GetValue("answer2"));
                answer3 = Convert.ToString(answers.GetValue("answer3"));
                answer4 = Convert.ToString(answers.GetValue("answer4"));
                recommendationForLongBeach= Convert.ToString(answers.GetValue("answer6"));
                comment= Convert.ToString(schema.GetValue("comments"));
                signature = Convert.ToString(signatureOfRecommender.GetValue("name"));
                date = Convert.ToString(signatureOfRecommender.GetValue("date"));

                // section for answer 5
                foreach (JObject content in answer5.Children<JObject>())
                {
                    if(content["qualities"].ToString()== "Intellectual Capacity")
                    {
                        intellectualCapacity = Convert.ToString(content.GetValue("value"));
                   
                    }
                    if (content["qualities"].ToString() == "Ability To Work With Others")
                    {
                        abilityToWorkWithOthers= Convert.ToString(content.GetValue("value"));
                    
                    }
                    if (content["qualities"].ToString() == "Maturity")
                    {
                        maturity= Convert.ToString(content.GetValue("value"));
                      
                    }
                    if (content["qualities"].ToString() == "Potential for Teaching")
                    {
                        potentialForTeaching= Convert.ToString(content.GetValue("value"));
                   
                    }
                    if (content["qualities"].ToString() == "Professional Conduct / Deposition")
                    {
                        professionalConductDeposition= Convert.ToString(content.GetValue("value"));
                    
                    }
                }
                
                recommendationTemplateBody = GetDocumentBodyTemplate(programIdentifier);
                // replace the values in the template 
                recommendationTemplateBody = recommendationTemplateBody.Replace("[[ApplicantLastName]]", applicantLastName)
                                                                     .Replace("[[ApplicantFirstName]]", applicantFirstName)
                                                                     .Replace("[[CredentialSubjectArea]]", credentialSubjectArea)
                                                                     .Replace("[[CampusID]]", campusID)
                                                                     .Replace("[[RecommenderLastName]]", recommenderLastName)
                                                                     .Replace("[[RecommenderFirstName]]", recommenderFirstName)
                                                                     .Replace("[[Institution]]", institution)
                                                                     .Replace("[[PositionTitle]]", position_title)
                                                                     .Replace("[[ContactNumber]]", telephoneContact)
                                                                     .Replace("[[EmailID]]", email)
                                                                     .Replace("[[Answer1]]", answer1)
                                                                     .Replace("[[Answer2]]", answer2)
                                                                     .Replace("[[Answer3]]", answer3)
                                                                     .Replace("[[Answer4]]", answer4)
                                                                     .Replace("[[IntellectualCapacity]]", intellectualCapacity)
                                                                     .Replace("[[AbilityToWorkWithOthers]]", abilityToWorkWithOthers)
                                                                     .Replace("[[Maturity]]", maturity)
                                                                     .Replace("[[PotentialForTeaching]]", potentialForTeaching)
                                                                     .Replace("[[ProfessionalConductDisposition]]", professionalConductDeposition)
                                                                     .Replace("[[RecommendationForLongBeach]]", recommendationForLongBeach)
                                                                     .Replace("[[SignatureOfRecommender]]", signature)
                                                                     .Replace("[[Date]]", date)
                                                                     .Replace("[[Comments]]", comment);
                // get the filecontent
                pdfFileContent = GetPDFFileContent(recommendationTemplateBody);

            }
            else if (programIdentifier.ToUpper() == "PK-3CP")
            {

                string applicantFirstName = string.Empty;
                string applicantLastName = string.Empty;
                string credentialSubjectArea = string.Empty;
                string campusID = string.Empty;
                string recommenderFirstName = string.Empty;
                string recommenderLastName = string.Empty;
                string institution = string.Empty;
                string position_title = string.Empty;
                string telephoneContact = string.Empty;
                string email = string.Empty;

                string answer1 = string.Empty;
                string answer2 = string.Empty;
                string answer3 = string.Empty;
                string ECESpecialist = string.Empty;
                string potential = string.Empty;
                string disposition = string.Empty;
                string professionalConduct = string.Empty;
                string teaching = string.Empty;
                string interpersonalSkills = string.Empty;
                string signature = string.Empty;
                string date = string.Empty;
                string comment = string.Empty;


                JObject personalInfo = (JObject)schema["personalInfo"];
                JObject answers = (JObject)schema["applicantRelatedAnswers"];
                JObject signatureOfRecommender = (JObject)schema["signatureOfRecommender"];
                JArray answer4 = (JArray)answers["answer4"];
                JArray answer5 = (JArray)answers["answer5"];
                //JArray recommendationsForLB = (JArray)schema["recommendationForLongBeach"];

                // section for personal info 
                applicantFirstName = Convert.ToString(personalInfo.GetValue("applicantFirstName"));
                applicantLastName = Convert.ToString(personalInfo.GetValue("applicantLastName"));
                credentialSubjectArea = Convert.ToString(personalInfo.GetValue("credentialSubjectArea"));
                campusID = Convert.ToString(personalInfo.GetValue("campusID"));
                recommenderFirstName = Convert.ToString(personalInfo.GetValue("recommenderFirstName"));
                recommenderLastName = Convert.ToString(personalInfo.GetValue("recommenderLastName"));
                institution = Convert.ToString(personalInfo.GetValue("institution"));
                position_title = Convert.ToString(personalInfo.GetValue("position_title"));
                telephoneContact = Convert.ToString(personalInfo.GetValue("telephoneContact"));
                email = Convert.ToString(personalInfo.GetValue("email"));
                // section for answers 1 to 3
                answer1 = Convert.ToString(answers.GetValue("answer1"));
                answer2 = Convert.ToString(answers.GetValue("answer2"));
                answer3 = Convert.ToString(answers.GetValue("answer3"));
                comment = Convert.ToString(schema.GetValue("comments"));
                signature = Convert.ToString(signatureOfRecommender.GetValue("name"));
                date = Convert.ToString(signatureOfRecommender.GetValue("date"));

                // section for answer 4
                foreach (JObject content in answer4.Children<JObject>())
                {
                    if (content["Category"].ToString() == "1.\tApplicant’s potential for academic performance (e.g., former academic, work ethic, diligence, capacity to learn).")
                    {
                        potential = Convert.ToString(content.GetValue("value"));

                    }
                    if (content["Category"].ToString() == "2.\tApplicant’s interpersonal skills (e.g., communication, ability to work with others, good listener, positive contributions to group-to-group interactions).")
                    {
                        interpersonalSkills = Convert.ToString(content.GetValue("value"));

                    }
                    if (content["Category"].ToString() == "3.\tDisposition (e.g., receptive to constructive suggestions, accepts responsibility for their actions, responds to diverse perspectives).")
                    {
                        disposition = Convert.ToString(content.GetValue("value"));

                    }
                    if (content["Category"].ToString() == "4.\tProfessional Conduct (e.g., responds appropriately, always on time).")
                    {
                        professionalConduct = Convert.ToString(content.GetValue("value"));

                    }
                    if (content["Category"].ToString() == "5.\tApplicant’s potential for teaching (e.g., knowledge and skills, creativity, problem-solving skills, attention to detail).")
                    {
                        teaching = Convert.ToString(content.GetValue("value"));

                    }
                }
                // section for answer 5
                foreach (JObject content in answer5.Children<JObject>())
                {
                    if (content["Category"].ToString() == "5. Recommendation for admission to the PK-3 ECE Specialist Instruction Credential Program at California State University, Long Beach:")
                    {
                        ECESpecialist = Convert.ToString(content.GetValue("value"));

                    }
                }

                   recommendationTemplateBody = GetDocumentBodyTemplate(programIdentifier);
                // replace the values in the template 
                recommendationTemplateBody = recommendationTemplateBody.Replace("[[ApplicantLastName]]", applicantLastName)
                                                                     .Replace("[[ApplicantFirstName]]", applicantFirstName)
                                                                     .Replace("[[CredentialSubjectArea]]", credentialSubjectArea)
                                                                     .Replace("[[CampusID]]", campusID)
                                                                     .Replace("[[RecommenderLastName]]", recommenderLastName)
                                                                     .Replace("[[RecommenderFirstName]]", recommenderFirstName)
                                                                     .Replace("[[Institution]]", institution)
                                                                     .Replace("[[PositionTitle]]", position_title)
                                                                     .Replace("[[ContactNumber]]", telephoneContact)
                                                                     .Replace("[[EmailID]]", email)
                                                                     .Replace("[[Answer1]]", answer1)
                                                                     .Replace("[[Answer2]]", answer2)
                                                                     .Replace("[[Answer3]]", answer3)
                                                                     .Replace("[[Potential]]", potential)
                                                                     .Replace("[[InterpersonalSkills]]", interpersonalSkills)
                                                                     .Replace("[[Disposition]]", disposition)
                                                                     .Replace("[[ProfessionalConduct]]", professionalConduct)
                                                                     .Replace("[[Teaching]]", teaching)
                                                                     .Replace("[[ECESpecialist]]", ECESpecialist)
                                                                     .Replace("[[SignatureOfRecommender]]", signature)
                                                                     .Replace("[[Date]]", date)
                                                                     .Replace("[[Comments]]", comment);
                // get the filecontent
                pdfFileContent = GetPDFFileContent(recommendationTemplateBody);

            }
            else if (programIdentifier.ToUpper() == "ESCP")
            {

                //fields for report generation 
                string recommenderName = string.Empty;
                string applicantName = string.Empty;
                string positionTitle = string.Empty;
                string campusID = string.Empty;
                string email = string.Empty;
                string signatureName = string.Empty;
                string signatureDate = string.Empty;
                string academicCompetencyComments = string.Empty;
                string academicCompetencyScale = string.Empty;
                string professionalismComments = string.Empty;
                string professionalismScale = string.Empty;
                string dispositionsPersonalityCharacterComments = string.Empty;
                string dispositionsPersonalityCharacterScale = string.Empty;
                string specialEducationComments = string.Empty;
                string specialEducationScale = string.Empty;
                string studentOverAllRank = string.Empty;

                JObject personalInfo = (JObject)schema["personalInfo"];
                JObject signatureOfRecommender = (JObject)schema["signatureOfRecommender"];
                JObject academicCompetency = (JObject)schema["academicCompetency"];
                JObject professionalism = (JObject)schema["professionalism"];
                JObject dispositionsPersonalityCharacter = (JObject)schema["dispositionsPersonalityCharacter"];
                JObject specialEducation = (JObject)schema["specialEducation"];

                // personalInfo 
                recommenderName = Convert.ToString(personalInfo.GetValue("recommenderFirstName"));
                applicantName = Convert.ToString(personalInfo.GetValue("studentName"));
                positionTitle = Convert.ToString(personalInfo.GetValue("position_title"));
                campusID = Convert.ToString(personalInfo.GetValue("campusID"));
                email = Convert.ToString(personalInfo.GetValue("email"));
                // signature of recommender 
                signatureName = Convert.ToString(signatureOfRecommender.GetValue("name"));
                signatureDate = Convert.ToString(signatureOfRecommender.GetValue("date"));
                // academicCompetency
                academicCompetencyComments = Convert.ToString(academicCompetency.GetValue("comments"));
                academicCompetencyScale = Convert.ToString(academicCompetency.GetValue("scale"));
                // professionalism
                professionalismComments = Convert.ToString(professionalism.GetValue("comments"));
                professionalismScale = Convert.ToString(professionalism.GetValue("scale"));
                //dispositionsPersonalityCharacter
                dispositionsPersonalityCharacterComments = Convert.ToString(dispositionsPersonalityCharacter.GetValue("comments"));
                dispositionsPersonalityCharacterScale = Convert.ToString(dispositionsPersonalityCharacter.GetValue("scale"));
                //specialEducation
                specialEducationComments = Convert.ToString(specialEducation.GetValue("comments"));
                specialEducationScale = Convert.ToString(specialEducation.GetValue("scale"));
                //studentOverAllRank
                studentOverAllRank= Convert.ToString(schema.GetValue("studentOverAllRank"));

                recommendationTemplateBody = GetDocumentBodyTemplate(programIdentifier);
                // replace the values in the template 
                recommendationTemplateBody = recommendationTemplateBody.Replace("[[ApplicantName]]", applicantName)
                                                                     .Replace("[[CampusID]]", campusID)
                                                                     .Replace("[[EmailAddress]]", email)
                                                                     .Replace("[[RecommenderName]]", recommenderName)
                                                                     .Replace("[[PositionTitle]]", positionTitle)
                                                                     .Replace("[[RecommenderSignature]]", signatureName)
                                                                     .Replace("[[Date]]", signatureDate)
                                                                     .Replace("[[academicCompetencyScale]]", academicCompetencyScale)
                                                                     .Replace("[[academicCompetencyComments]]", academicCompetencyComments)
                                                                     .Replace("[[professionalismScale]]", professionalismScale)
                                                                     .Replace("[[professionalismComments]]", professionalismComments)
                                                                     .Replace("[[dispositionsPersonalityCharacterScale]]", dispositionsPersonalityCharacterScale)
                                                                     .Replace("[[dispositionsPersonalityCharacterComments]]", dispositionsPersonalityCharacterComments)
                                                                     .Replace("[[specialEducationScale]]", specialEducationScale)
                                                                     .Replace("[[specialEducationComments]]", specialEducationComments)
                                                                     .Replace("[[studentOverAllRank]]", studentOverAllRank);

                // get the filecontent
                pdfFileContent = GetPDFFileContent(recommendationTemplateBody);
            }
            else
            {
                string studentName = string.Empty;
                string recommenderName = string.Empty;
                string job_title = string.Empty;
                string occupation = string.Empty;
                string organization = string.Empty;
                string email = string.Empty;
                string phone = string.Empty;
                string howLongApplicantKnown = string.Empty;
                string inWhatCapacityApplicantKnown = string.Empty;

                string communicationSkillsOral = string.Empty;
                string communicationSkillsWritten = string.Empty;
                string technologySkills = string.Empty;
                string initiative = string.Empty;
                string maturity = string.Empty;
                string motivation = string.Empty;
                string creativity = string.Empty;
                string abilityToWork = string.Empty;
                string intellectualPotential = string.Empty;
                string academicPerformance = string.Empty;
                string graduateWork = string.Empty;
                string overAllRecommendationAdmission = string.Empty;
                string comment = string.Empty;
                string question1 = string.Empty;
                string question2 = string.Empty;

                JObject personalInfo = (JObject)schema["personalInfo"];
                JObject relationship = (JObject)schema["relationship"];
                JArray referrenceRatings = (JArray)schema["referrenceRatings"];

                // section for personal info 
                studentName = Convert.ToString(personalInfo.GetValue("studentName"));
                recommenderName = Convert.ToString(personalInfo.GetValue("recommenderName"));
                job_title = Convert.ToString(personalInfo.GetValue("job_title"));
                occupation = Convert.ToString(personalInfo.GetValue("occupation"));
                organization = Convert.ToString(personalInfo.GetValue("organization"));
                email = Convert.ToString(personalInfo.GetValue("email"));
                phone = Convert.ToString(personalInfo.GetValue("phone"));
                //relationship to applicant
                howLongApplicantKnown= Convert.ToString(relationship.GetValue("howLongApplicantKnown"));
                inWhatCapacityApplicantKnown = Convert.ToString(relationship.GetValue("inWhatCapacityApplicantKnown"));
                overAllRecommendationAdmission = Convert.ToString(schema.GetValue("overAllRecommendationAdmission"));
                if(programIdentifier == "MS Special Education (SPED)")
                {
                    if (schema.ContainsKey("question1"))
                    {
                        question1 = Convert.ToString(schema.GetValue("question1"));
                    }
                    if (schema.ContainsKey("question2"))
                    {
                        question2 = Convert.ToString(schema.GetValue("question2"));
                    }
                }

                // referrence ratings
                foreach (JObject content in referrenceRatings.Children<JObject>())
                {
                    if (content["quality"].ToString() == "Communication Skills, Oral:")
                    {
                        communicationSkillsOral = Convert.ToString(content.GetValue("scale"));
                    }
                    if (content["quality"].ToString() == "Communication Skills, Written:")
                    {
                        communicationSkillsWritten = Convert.ToString(content.GetValue("scale"));
                    }
                    if (content["quality"].ToString() == "Technology Skills:")
                    {
                        technologySkills = Convert.ToString(content.GetValue("scale"));
                    }
                    if (content["quality"].ToString() == "Initiative:")
                    {
                        initiative = Convert.ToString(content.GetValue("scale"));
                    }
                    if (content["quality"].ToString() == "Maturity:")
                    {
                        maturity = Convert.ToString(content.GetValue("scale"));
                    }
                    if (content["quality"].ToString() == "Motivation for this program of study:")
                    {
                        motivation = Convert.ToString(content.GetValue("scale"));
                    }
                    if (content["quality"].ToString() == "Creativity:")
                    {
                        creativity = Convert.ToString(content.GetValue("scale"));
                    }
                    if (content["quality"].ToString() == "Ability to work with others:")
                    {
                        abilityToWork = Convert.ToString(content.GetValue("scale"));
                    }
                    if (content["quality"].ToString() == "Intellectual potential:")
                    {
                        intellectualPotential = Convert.ToString(content.GetValue("scale"));
                    }
                    if (content["quality"].ToString() == "Present academic performance:")
                    {
                        academicPerformance = Convert.ToString(content.GetValue("scale"));
                    }
                    if (content["quality"].ToString() == "Potential for graduate work:")
                    {
                        graduateWork = Convert.ToString(content.GetValue("scale"));
                    }
                }

                recommendationTemplateBody = GetDocumentBodyTemplate(programIdentifier);
                // replace the values in the template 
                recommendationTemplateBody = recommendationTemplateBody.Replace("[[studentName]]", studentName)
                                                                     .Replace("[[recommenderName]]", recommenderName)
                                                                     .Replace("[[job_title]]", job_title)
                                                                     .Replace("[[email]]", email)
                                                                     .Replace("[[occupation]]", occupation)
                                                                     .Replace("[[organization]]", organization)
                                                                     .Replace("[[phone]]", phone)
                                                                     .Replace("[[howLongApplicantKnown]]", howLongApplicantKnown)
                                                                     .Replace("[[inWhatCapacityApplicantKnown]]", inWhatCapacityApplicantKnown)
                                                                     .Replace("[[communicationSkillsOral]]", communicationSkillsOral)
                                                                     .Replace("[[communicationSkillsWritten]]", communicationSkillsWritten)
                                                                     .Replace("[[technologySkills]]", technologySkills)
                                                                     .Replace("[[initiative]]", initiative)
                                                                     .Replace("[[maturity]]", maturity)
                                                                     .Replace("[[motivation]]", motivation)
                                                                     .Replace("[[creativity]]", creativity)
                                                                     .Replace("[[academicPerformance]]", academicPerformance)
                                                                     .Replace("[[abilityToWork]]", abilityToWork)
                                                                     .Replace("[[intellectualPotential]]", intellectualPotential)
                                                                     .Replace("[[graduateWork]]", graduateWork)
                                                                     .Replace("[[overAllRecommendationAdmission]]", overAllRecommendationAdmission)
                                                                     .Replace("[[question1]]", question1)
                                                                     .Replace("[[question2]]", question2);
                // get the filecontent
                pdfFileContent = GetPDFFileContent(recommendationTemplateBody);

            }
            return pdfFileContent;
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
        private byte[] GetFileContent(string templateName)
        {
            byte[] fileContent = null;
            string filepath = Path.Combine("SupportFiles/EmailAttachments", templateName);
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            long byteLength = new System.IO.FileInfo(filepath).Length;
            fileContent = binaryReader.ReadBytes((Int32)byteLength);
            fs.Close();
            fs.Dispose();
            binaryReader.Close();
            return fileContent;
        }

        private void SendRecommendedConfirmMailToApplicant(string recommenderIdentifier, string programIdentifier)
        {
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@RecommenderIdentifier", SqlDbType.UniqueIdentifier) { Value = new Guid(recommenderIdentifier) }
                                     };
            DataTable dtResponse = _helper.GetDataTable("[dbo].[getApplicantByRecommenderIdentifier]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                string applicantsName = string.Empty;
                string toMail = string.Empty;
                string ccMail = string.Empty;
                string subject = string.Empty;
                string body = string.Empty;
                string logoText = "cid:myImageID";
                int programID = 0;
                int applicationTypeID = 0;
                applicantsName = Convert.ToString(dtResponse.Rows[0]["ApplicantName"]);
                toMail = Convert.ToString(dtResponse.Rows[0]["cusulbEmail"]);
                ccMail= Convert.ToString(dtResponse.Rows[0]["RecommenderEmail"]);
                applicationTypeID = Convert.ToInt32(dtResponse.Rows[0]["ApplicationTypeID"]);
                programID = Convert.ToInt32(dtResponse.Rows[0]["ProgramID"]);
                subject = "Recommendation Submitted";
                SqlParameter[] parameters1 ={
                                            new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt) { Value = applicationTypeID },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
                                            new SqlParameter("@Identifier", SqlDbType.NVarChar,50) { Value = "Student Confirmation Mail"}
                                       };
                DataTable dtRec = _helper.GetDataTable("[dbo].[GetRecommenderMailBody]", parameters1);
                if (dtRec.Rows.Count > 0)
                {
                    if (dtRec.Rows[0]["RecommenderMailBody"] != DBNull.Value)
                    {
                        body = Convert.ToString(dtRec.Rows[0]["RecommenderMailBody"]);

                        string beforeBody = string.Empty;
                        string afterBody = string.Empty;
                        beforeBody = "<html><body><div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                        afterBody = "</body></html>";
                        body = $"{beforeBody}{body}{afterBody}";
                    }
                }
                body = body.Replace("[[logoPath]]", logoText)
                           .Replace("[[ApplicantName]]", applicantsName);
                _sendMail.SendEmail(toMail, ccMail, "COMMON", subject, body, "");
            }
        }

        public AuthorizeRecommenderResponse GetDetailsForRecommendation(string recommenderIdentifier)
        {
            AuthorizeRecommenderResponse obj = new AuthorizeRecommenderResponse();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@RecommenderIdentifier", SqlDbType.UniqueIdentifier) { Value = new Guid(recommenderIdentifier) }
                                     };
            DataTable dtResponse = _helper.GetDataTable("[dbo].[AutharizeRecommender]", parameters);
            if (dtResponse.Rows.Count > 0 && !string.IsNullOrEmpty(dtResponse.Rows[0]["Recommendations"].ToString()))
            {
                    obj = dtResponse.AsEnumerable().Select(row =>
                                                  new AuthorizeRecommenderResponse
                                                  {
                                                      recommendations = Convert.ToString(row["Recommendations"])

                                                  }).FirstOrDefault();
                    obj.IsSuccess = true;
                    obj.Message = "Data retrieved succesfully ";
             
            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "The page you are trying to reach has either expired or is not valid.";
            }
            return obj;
        }

        public byte[] GetMergedDocument(int formID)
        {
            byte[] mergedFileStream=null;
            List<FormAttachmentEntity> objList = new List<FormAttachmentEntity>();
            DataTable dtPersonalInfo = null;
            DataTable dtEducationInfo = null;
            DataTable dtDisposition = null;
            string UserFolderName = string.Empty;
            SqlParameter[] parameters =
                                   {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID }
                                        };

            DataSet dsDoc = _helper.GetDataSet("[dbo].[GetFormAttachmentsForMerge]", parameters);
            if (dsDoc.Tables.Count > 0)
            {
                if (dsDoc.Tables[0].Rows.Count > 0)
                {
                    dtPersonalInfo = dsDoc.Tables[0].Copy();
                }
                if (dsDoc.Tables[1].Rows.Count > 0)
                {
                    objList = dsDoc.Tables[1].AsEnumerable().Select(row =>
                                 new FormAttachmentEntity
                                 {
                                     FileName = Convert.ToString(row["FileName"]),
                                     FileExtension = Convert.ToString(row["FileExtn"]),
                                     FolderName = Convert.ToString(row["FolderName"])

                                 }).ToList();
                }
                if (dsDoc.Tables[2].Rows.Count > 0)
                {
                    UserFolderName = Convert.ToString(dsDoc.Tables[2].Rows[0]["UserFolder"]);
                }
                dtEducationInfo= dsDoc.Tables[3].Copy();

                if(dsDoc.Tables[4]!=null && dsDoc.Tables[4].Rows.Count > 0)
                {
                    dtDisposition = dsDoc.Tables[4].Copy();
                }
            }

            mergedFileStream = Merge(UserFolderName, objList, dtPersonalInfo,dtEducationInfo,dtDisposition,formID);

            return mergedFileStream;
        }
        private byte[] Merge(string UserFolderName, List<FormAttachmentEntity> lstAttachments,DataTable dtPersonalInfo,DataTable dtEducationInfo,DataTable dtDisposition,int formID)
        {
            byte[] inputStream = null;
            var folderPath = _configuration["ApplicationKeys:FileRepository"];
            string userFolderName = Path.Combine(folderPath, UserFolderName);
            string workingFolderName= Path.Combine(userFolderName, "Form");
            string MergedPDFFolderName = Path.Combine(userFolderName, "Form");
            string dispositionsAssessmentForm = string.Empty;
            int formDispositionAssessmentID;
            string programIdentifier = string.Empty;
            string OutFile = Path.Combine(MergedPDFFolderName, "Merged" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf");
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
            // first phase is to draw the HTML
            //string fileList=GetFileList(lstAttachments);
            GetHTMLForPersonalInfo(dtPersonalInfo,copyProvider,workingFolderName,"", dtEducationInfo);

            foreach (var attachment in lstAttachments)
            {

                string fileName = string.Empty;
                string[] splitter = attachment.FolderName.Split('~');
                fileName = splitter[1].ToString()+".pdf";
                if (!string.IsNullOrEmpty(fileName))
                {
                    PdfReader.unethicalreading = true;
                    iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(Path.Combine(workingFolderName,fileName));
                    copyProvider.AddDocument(pdfReader);
                    pdfReader.Close();
                }
            }
            // Section to add disposition assessments
            if (dtDisposition != null && dtDisposition.Rows.Count > 0)
            {
                // check if the program is not Graduate or SSCP and ESCP
                if (!string.IsNullOrEmpty(Convert.ToString(dtDisposition.Rows[0]["DispositionsAssessmentForm"])))
                {
                    dispositionsAssessmentForm = Convert.ToString(dtDisposition.Rows[0]["DispositionsAssessmentForm"]);
                    formDispositionAssessmentID = Convert.ToInt32(dtDisposition.Rows[0]["FormDispositionsAssessmentID"]);
                    programIdentifier= Convert.ToString(dtDisposition.Rows[0]["ProgramFormIdentifier"]);
                    // call the method to generate the dispositions document 
                    GenerateDispositionDocument(copyProvider,formDispositionAssessmentID,dispositionsAssessmentForm,programIdentifier, workingFolderName);
                
                }
            }
            if (dtDisposition!=null || dtDisposition.Rows.Count > 0)
            {
                programIdentifier = Convert.ToString(dtDisposition.Rows[0]["ProgramFormIdentifier"]);

                //Section to add evaluation form for SSCP and MSCP
                if (programIdentifier == "SSCP" || programIdentifier == "MSCP")
                {
                    SqlParameter[] parameters =
                                   {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = 0 },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = 0 },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar,10) { Value = "" }
                               };
                    DataTable dtRec = _helper.GetDataTable("[Application].[GetLetterOfRecommendationsByFormID]", parameters);
                    string json = string.Empty;
                    byte[] fileContentJSONToPDF = new byte[0];
                    if (dtRec != null && dtRec.Rows.Count > 0)
                    {
                        foreach (DataRow row in dtRec.Rows)
                        {
                            json = Convert.ToString(row["LetterOfRecommendationJSON"]);
                            if (!string.IsNullOrEmpty(json))
                            {
                                if (programIdentifier == "SSCP")
                                {
                                    fileContentJSONToPDF = _initialCredentialProgramService.GetPDFFromJSONForSSCP(json);
                                }
                                else
                                {
                                    fileContentJSONToPDF = _initialCredentialProgramService.GetPDFFromJSONForMSCP(json);
                                }

                                string PDFFilePath = Path.Combine(workingFolderName, "EvaluationForm" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf");
                                File.WriteAllBytes(PDFFilePath, fileContentJSONToPDF);
                                iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(PDFFilePath);
                                copyProvider.AddDocument(pdfReader);
                                pdfReader.Close();
                            }
                        }
                    }
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

        private void GenerateDispositionDocument(PdfCopy copyprovider,int dispositionAttachmentID,string dispositionForm,string programIdentifier,string fileStoringPath)
        {
            if (programIdentifier.ToUpper() == "MSCP")
            {
                DispositionMSCPList data = JsonSerializer.Deserialize<DispositionMSCPList>(dispositionForm);
                DataTable dataTable = new DataTable();
                DataTable dataTable1 = new DataTable();
                DataTable dataTable2 = new DataTable();
                dataTable.Columns.Add("professionalColumnA");
                dataTable.Columns.Add("professionalColumnB");
                dataTable.Columns.Add("professionalColumnC");
                dataTable1.Columns.Add("attendanceColumnA");
                dataTable1.Columns.Add("attendanceColumnB");
                dataTable1.Columns.Add("attendanceColumnC");
                dataTable2.Columns.Add("communicationColumnA");
                dataTable2.Columns.Add("communicationColumnB");
                dataTable2.Columns.Add("communicationColumnC");
                foreach (var professional in data.professionals.ToList())
                {
                    DataRow newRow = dataTable.NewRow();
                    if (professional.options.Count >= 3)
                    {
                        newRow["professionalColumnA"] = professional.options[0].label;
                        newRow["professionalColumnB"] = professional.options[1].label;
                        newRow["professionalColumnC"] = professional.options[2].label;
                    }
                    dataTable.Rows.Add(newRow);
                }
                foreach (var attendance in data.attendance.ToList())
                {
                    DataRow newRow = dataTable1.NewRow();
                    if (attendance.options.Count >= 3)
                    {
                        newRow["attendanceColumnA"] = attendance.options[0].label;
                        newRow["attendanceColumnB"] = attendance.options[1].label;
                        newRow["attendanceColumnC"] = attendance.options[2].label;
                    }

                    dataTable1.Rows.Add(newRow);
                }
                foreach (var communication in data.communication.ToList())
                {
                    DataRow newRow = dataTable2.NewRow();
                    if (communication.options.Count >= 3)
                    {
                        newRow["communicationColumnA"] = communication.options[0].label;
                        newRow["communicationColumnB"] = communication.options[1].label;
                        newRow["communicationColumnC"] = communication.options[2].label;
                    }

                    dataTable2.Rows.Add(newRow);
                }
                StringBuilder sbProffesionalData = new StringBuilder();
                StringBuilder sbAttendanceData = new StringBuilder();
                StringBuilder sbCommunicationData = new StringBuilder();
                for (int y = 0; y < dataTable.Rows.Count; y++)
                {
                    string proffesionalValueA = string.Empty;
                    string proffesionalValueB = string.Empty;
                    string proffesionalValueC = string.Empty;
                    proffesionalValueA = Convert.ToString(dataTable.Rows[y]["professionalColumnA"]);
                    proffesionalValueB = Convert.ToString(dataTable.Rows[y]["professionalColumnB"]);
                    proffesionalValueC = Convert.ToString(dataTable.Rows[y]["professionalColumnC"]);

                    string strProffesional = ConstructDataRowsForProffesional(data.professionals[y], proffesionalValueA, proffesionalValueB, proffesionalValueC);
                    sbProffesionalData.Append(strProffesional);
                }
                for (int y = 0; y < dataTable1.Rows.Count; y++)
                {
                    string attendanceValueA = string.Empty;
                    string attendanceValueB = string.Empty;
                    string attendanceValueC = string.Empty;
                    attendanceValueA = Convert.ToString(dataTable1.Rows[y]["attendanceColumnA"]);
                    attendanceValueB = Convert.ToString(dataTable1.Rows[y]["attendanceColumnB"]);
                    attendanceValueC = Convert.ToString(dataTable1.Rows[y]["attendanceColumnC"]);

                    string strAttendance = ConstructDataRowsForAttendance(data.attendance[y], attendanceValueA, attendanceValueB, attendanceValueC);
                    sbAttendanceData.Append(strAttendance);
                }
                for (int y = 0; y < dataTable2.Rows.Count; y++)
                {
                    string communicationValueA = string.Empty;
                    string communicationValueB = string.Empty;
                    string communicationValueC = string.Empty;
                    communicationValueA = Convert.ToString(dataTable2.Rows[y]["communicationColumnA"]);
                    communicationValueB = Convert.ToString(dataTable2.Rows[y]["communicationColumnB"]);
                    communicationValueC = Convert.ToString(dataTable2.Rows[y]["communicationColumnC"]);

                    string strCommunication = ConstructDataRowsForCommunication(data.communication[y], communicationValueA, communicationValueB, communicationValueC);
                    sbCommunicationData.Append(strCommunication);
                }
                string rating1 = string.Empty;
                string rating2 = string.Empty;
                string rating3 = string.Empty;
                string rating4 = string.Empty;
                foreach (var rating in data.ratings)
                {
                    switch (rating.label)
                    {
                        case "Goal 1":
                            rating1 = rating.value;
                            break;
                        case "Goal 2":
                            rating2 = rating.value;
                            break;
                        case "Goal 3":
                            rating3 = rating.value;
                            break;
                        case "Goal 4":
                            rating4 = rating.value;
                            break;

                    }
                }
                //var htmlBody = GetMailBodyTemplate("GeneratePdfReport.html");
                var htmlBody = GetDocumentTemplate("DispositionMSCPTemplate.html");
                htmlBody = htmlBody.Replace("[[professionalList]]", sbProffesionalData.ToString())
                                   .Replace("[[attendanceList]]", sbAttendanceData.ToString())
                                   .Replace("[[communicationList]]", sbCommunicationData.ToString())
                                   .Replace("[[rating1]]", rating1.ToString())
                                   .Replace("[[rating2]]", rating2.ToString())
                                   .Replace("[[rating3]]", rating3.ToString())
                                   .Replace("[[rating4]]", rating4.ToString());
                

                StringReader sr = new StringReader(htmlBody.ToString());
                Document pdfDoc = new Document(PageSize.A4, 50f, 50f, 200f, 0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                byte[] htmlContent = null;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    pdfDoc.Open();

                    htmlparser.Parse(sr);
                    pdfDoc.Close();

                    htmlContent = memoryStream.ToArray();
                    memoryStream.Close();
                }
               
                string html2PDFFilePath = Path.Combine(fileStoringPath, "Disposition_MSCP" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf");
                File.WriteAllBytes(html2PDFFilePath, htmlContent);
                iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(html2PDFFilePath);
                //pdfReader.setUnethicalReading(true);
                copyprovider.AddDocument(pdfReader);
                pdfReader.Close();

            }
            else 
            {
                DispositionUDCP udcpData = JsonSerializer.Deserialize<DispositionUDCP>(dispositionForm);
                DataTable dt = new DataTable();
                dt.Columns.Add("Professional Disposition");
                dt.Columns.Add("Proficiency");
                dt.Columns.Add("Evidence");
                foreach (var data in udcpData.basicCredentials.ToList())
                {
                    DataRow newRow = dt.NewRow();
                    newRow["Professional Disposition"] = data.label;
                    newRow["Proficiency"] = data.value;
                    newRow["Evidence"] = data.comment;
                    dt.Rows.Add(newRow);
                }
                StringBuilder sb = new StringBuilder();
                for (int y = 0; y < dt.Rows.Count; y++)
                {
                    string label = string.Empty;
                    string val = string.Empty;
                    string comment = string.Empty;
                    label = Convert.ToString(dt.Rows[y]["Professional Disposition"]);
                    val = Convert.ToString(dt.Rows[y]["Proficiency"]);
                    comment = Convert.ToString(dt.Rows[y]["Evidence"]);
                    string rowValues = ConstructDataRows(label, val, comment);
                    sb.Append(rowValues);
                }
                var htmlBody = string.Empty;
                if (programIdentifier.ToUpper() == "UDCP")
                {
                     htmlBody = GetDocumentTemplate("DispositionUDCPTemplate.html");
                }
                else 
                {  
                    htmlBody = GetDocumentTemplate("DispositionESCPTemplate.html"); 
                }

                    htmlBody = htmlBody.Replace("[[UDCPData]]", sb.ToString());
                    StringReader sr = new StringReader(htmlBody.ToString());
                    Document pdfDoc = new Document(PageSize.A4, 50f, 50f, 200f, 0f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    byte[] htmlContent = null;
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                        pdfDoc.Open();

                        htmlparser.Parse(sr);
                        pdfDoc.Close();

                        htmlContent = memoryStream.ToArray();
                        memoryStream.Close();
                    }

                    string html2PDFFilePath = Path.Combine(fileStoringPath, "Disposition" + "_" + programIdentifier.ToUpper() + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf");
                    File.WriteAllBytes(html2PDFFilePath, htmlContent);
                    iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(html2PDFFilePath);
                    //pdfReader.setUnethicalReading(true);
                    copyprovider.AddDocument(pdfReader);
                    pdfReader.Close();

                
            }
            
        }
        private string ConstructDataRowsForProffesional(ThoughtFocus.Domain.Request.GraduateProgram.DispositionMSCPFiledata.Professional data, string proffesionalValueA, string proffesionalValueB, string proffesionalValueC)
        {
            
            StringBuilder sbRows = new StringBuilder();
            sbRows.Append("<tr>");
            foreach (var keyvalue in data.options)
            {
                if (keyvalue.value == data.value)
                {
                    if (proffesionalValueA == keyvalue.label)
                    {
                        sbRows.Append("<td width='20%' > <b>" + proffesionalValueA + "</b></td>");
                    }
                    else if (proffesionalValueB == keyvalue.label)
                    {
                        sbRows.Append("<td width='20%' ><b>" + proffesionalValueB + "</b></td>");
                    }
                    else
                    {
                        sbRows.Append("<td width='20%' ><b>" + proffesionalValueC + "</b></td>");
                    }
                }
                else
                {
                    if (proffesionalValueA == keyvalue.label)
                    {
                        sbRows.Append("<td width='20%' >" + proffesionalValueA + "</td>");
                    }
                    else if (proffesionalValueB == keyvalue.label)
                    {
                        sbRows.Append("<td width='20%' >" + proffesionalValueB + "</td>");
                    }
                    else
                    {
                        sbRows.Append("<td width='20%' >" + proffesionalValueC + "</td>");
                    }
                }

            }
            sbRows.Append("</tr>");
            return sbRows.ToString();
        }
        private string ConstructDataRowsForAttendance(ThoughtFocus.Domain.Request.GraduateProgram.DispositionMSCPFiledata.Attendance data, string attendanceValueA, string attendanceValueB, string attendanceValueC)
        {
            StringBuilder sbRows = new StringBuilder();
            sbRows.Append("<tr>");
            foreach (var keyvalue in data.options)
            {
                if (keyvalue.value == data.value)
                {
                    if (attendanceValueA == keyvalue.label)
                    {
                        sbRows.Append("<td width='20%' > <b>" + attendanceValueA + "</b></td>");
                    }
                    else if (attendanceValueB == keyvalue.label)
                    {
                        sbRows.Append("<td width='20%' ><b>" + attendanceValueB + "</b></td>");
                    }
                    else
                    {
                        sbRows.Append("<td width='20%' ><b>" + attendanceValueC + "</b></td>");
                    }
                }
                else
                {
                    if (attendanceValueA == keyvalue.label)
                    {
                        sbRows.Append("<td width='20%' >" + attendanceValueA + "</td>");
                    }
                    else if (attendanceValueB == keyvalue.label)
                    {
                        sbRows.Append("<td width='20%' >" + attendanceValueB + "</td>");
                    }
                    else
                    {
                        sbRows.Append("<td width='20%' >" + attendanceValueC + "</td>");
                    }
                }

            }
            sbRows.Append("</tr>");
            return sbRows.ToString();
        }
        private string ConstructDataRowsForCommunication(ThoughtFocus.Domain.Request.GraduateProgram.DispositionMSCPFiledata.Communication data, string communicationValueA, string communicationValueB, string communicationValueC)
        {
            StringBuilder sbRows = new StringBuilder();
            sbRows.Append("<tr>");
            foreach (var keyvalue in data.options)
            {
                if (keyvalue.value == data.value)
                {
                    if (communicationValueA == keyvalue.label)
                    {
                        sbRows.Append("<td width='20%' > <b>" + communicationValueA + "</b></td>");
                    }
                    else if (communicationValueB == keyvalue.label)
                    {
                        sbRows.Append("<td width='20%' ><b>" + communicationValueB + "</b></td>");
                    }
                    else
                    {
                        sbRows.Append("<td width='20%' ><b>" + communicationValueC + "</b></td>");
                    }
                }
                else
                {
                    if (communicationValueA == keyvalue.label)
                    {
                        sbRows.Append("<td width='20%' >" + communicationValueA + "</td>");
                    }
                    else if (communicationValueB == keyvalue.label)
                    {
                        sbRows.Append("<td width='20%' >" + communicationValueB + "</td>");
                    }
                    else
                    {
                        sbRows.Append("<td width='20%' >" + communicationValueC + "</td>");
                    }
                }

            }
            sbRows.Append("</tr>");
            return sbRows.ToString();
        }
        private string ConstructDataRows(string label, string val, string comment)
        {
            StringBuilder sbRows = new StringBuilder();
            sbRows.Append("<tr>");
            sbRows.Append("<td width='30%'>" + label + "</td>");
            sbRows.Append("<td width='10%'>" + val + "</td>");
            sbRows.Append("<td width='22%'>" + comment + "</td>");
            sbRows.Append("</tr>");
            return sbRows.ToString();
        }
        private string GetFileList(List<FormAttachmentEntity> lstAttachments)
        {
            string strFileList = string.Empty;
            System.Text.StringBuilder sbFiles = new System.Text.StringBuilder();
            sbFiles.Append("<ol>");
            foreach (var attachment in lstAttachments)
            {
                string fileName = string.Empty;
                fileName = attachment.FileName + ".pdf";
                if (!string.IsNullOrEmpty(fileName))
                {
                    sbFiles.Append("<li>"+fileName+"</li>");
                }
            }
            sbFiles.Append("</ol>");
            return sbFiles.ToString();
        }
        private void GetHTMLForPersonalInfo(DataTable dtPersonalInfo,PdfCopy copyprovider,string fileStoringPath, string fileList,DataTable dtEducationInfo)
        {
            string htmlString = string.Empty;
            string htmlStringEducationalInfo = string.Empty;
            string body = string.Empty;
            String firstName = string.Empty;
            String lastName = string.Empty;
            String preferredName = string.Empty;
            String otherName = string.Empty;
            String cusulbEmail = string.Empty;
            String altEmail = string.Empty;
            String phoneNumber = string.Empty;
            String csulbCampusId = string.Empty;
            String Semester = string.Empty;
            String Program = string.Empty;
            String languages = string.Empty;

            string filepath = Path.Combine("SupportFiles/DocumentTemplates", "MergedDocumentTemplate.html");
         
            if (dtPersonalInfo.Rows.Count > 0)
            {
                firstName = Convert.ToString(dtPersonalInfo.Rows[0]["firstName"]);
                lastName = Convert.ToString(dtPersonalInfo.Rows[0]["lastName"]);
                preferredName = Convert.ToString(dtPersonalInfo.Rows[0]["preferredName"]);
                otherName = Convert.ToString(dtPersonalInfo.Rows[0]["otherName"]);
                cusulbEmail = Convert.ToString(dtPersonalInfo.Rows[0]["cusulbEmail"]);
                altEmail = Convert.ToString(dtPersonalInfo.Rows[0]["altEmail"]);
                phoneNumber = Convert.ToString(dtPersonalInfo.Rows[0]["phoneNumber"]);
                csulbCampusId = Convert.ToString(dtPersonalInfo.Rows[0]["csulbCampusId"]);
                Semester = Convert.ToString(dtPersonalInfo.Rows[0]["Semester"]);
                Program = Convert.ToString(dtPersonalInfo.Rows[0]["Program"]);
                languages = Convert.ToString(dtPersonalInfo.Rows[0]["languages"]);
                // costruct html for EducationalInfo
                if (dtEducationInfo.Rows.Count > 0)
                {
                    htmlStringEducationalInfo = ConstructHTMLforEducationInfo(dtEducationInfo, dtPersonalInfo);
                }
                using (StreamReader reader = new StreamReader(Path.GetFullPath(filepath)))
                {
                    body = reader.ReadToEnd();
                }
                htmlString = body.Replace("[[firstName]]", firstName).Replace("[[lastName]]", lastName).Replace("[[preferredName]]", preferredName).Replace("[[otherName]]", otherName).Replace("[[cusulbEmail]]", cusulbEmail).Replace("[[altEmail]]", altEmail).Replace("[[phoneNumber]]", phoneNumber).Replace("[[csulbCampusId]]", csulbCampusId).Replace("[[Semester]]", Semester).Replace("[[Program]]", Program).Replace("[[languages]]", languages).Replace("[[EducationInfo]]", htmlStringEducationalInfo);

                StringReader sr = new StringReader(htmlString.ToString());
                Document pdfDoc = new Document(PageSize.A4, 50f, 50f, 200f, 0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                byte[] htmlContent = null;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    pdfDoc.Open();

                    htmlparser.Parse(sr);
                    pdfDoc.Close();

                    htmlContent = memoryStream.ToArray();
                    memoryStream.Close();
                }
                string html2PDFFilePath = Path.Combine(fileStoringPath, "PersonalInfo" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf");
                File.WriteAllBytes(html2PDFFilePath, htmlContent);
                iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(html2PDFFilePath);
                //pdfReader.setUnethicalReading(true);
                copyprovider.AddDocument(pdfReader);
                pdfReader.Close();

            }

            //return htmlString;
        }
        private string ConstructHTMLforEducationInfo(DataTable dtEducationalInfo,DataTable dtPersonalInfo)
        {
            StringBuilder sb = new StringBuilder();
            string bachelorDegreeMajor = string.Empty;
            string institution = string.Empty;
            string highestDegreeEarned = string.Empty;
            bachelorDegreeMajor = Convert.ToString(dtPersonalInfo.Rows[0]["bachelorDegreeMajor"]);
            institution = Convert.ToString(dtPersonalInfo.Rows[0]["institution"]);
            highestDegreeEarned = Convert.ToString(dtPersonalInfo.Rows[0]["highestDegreeEarned"]);
            sb.Append("<table border='1' ><tr><td colspan='2'style='padding: 15px;'><br/></td></tr><tr><td colspan='2'>Education Information</td></tr><tr><td style='width: 10 %;'>Bachelor's Degree Major</td><td>" + bachelorDegreeMajor + "</td></tr><tr><td>Institution</td><td>"+ institution + "</td></tr><tr><td>Highest Degree Earned</td><td>"+ highestDegreeEarned + "</td></tr><tr><td colspan='2'><table><tr><td>College/University</td><td>Degree/Credential Earned</td><td>State</td><td>Dates Attended</td></tr>");
            for(int i = 0; i < dtEducationalInfo.Rows.Count; i++)
            {
                sb.Append("<tr>");
                sb.Append("<td>"+ Convert.ToString(dtEducationalInfo.Rows[i]["college"]) + "</td>");
                sb.Append("<td>" + Convert.ToString(dtEducationalInfo.Rows[i]["degree"]) + "</td>");
                sb.Append("<td>" + Convert.ToString(dtEducationalInfo.Rows[i]["state"]) + "</td>");
                sb.Append("<td>" + Convert.ToString(dtEducationalInfo.Rows[i]["dateFrom"]) + " to "+ Convert.ToString(dtEducationalInfo.Rows[i]["dateTo"]) + "</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table></td></tr></table>");
            return sb.ToString();
        }

        public BaseResponse AssignFormToReviewers(int programID, string termCode )
        {
            BaseResponse obj = new BaseResponse();
            SqlParameter[] parameters =
                                   { new SqlParameter("@ProgramId", SqlDbType.Int) { Value = programID } ,
                                    new SqlParameter("@TermCode", SqlDbType.NVarChar) { Value = termCode } 
        };

            int id = _helper.InsertTable("[dbo].[AssignFormToReviewers]", parameters);
            obj.IsSuccess = true;
            obj.Message = "Reviewers Assigned Successfully";
            return obj;
        }

        public BaseResponse UpdateInstructorFeedback(UpdateInstructorFeedbackRequest input)
        {
            BaseResponse response = new BaseResponse();
            if (input.FileContent != null && input.FileContent.Length > 0)
            {
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                string fileExtensionWord = string.Empty;
                string userFolderName = string.Empty;
                string savedFileName = string.Empty;
                bool isNotPDFExtension = false;
                var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];

                var workingFolderPath = Path.Combine(fileRepoPath, "WorkingFolder");

                // pull the saved file name format SP Below
                FormAttachmentFileNames fileNames = GetFormAttachmentFileName(input.FormID, input.DocumentID);
                if (input.FileName != string.Empty)
                {
                    AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.FileName);
                    //fileName = fileDetails.FileName;
                    fileExtension = fileDetails.FileExtension;
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
                    if (fileExtension.ToUpper() == "DOC" || fileExtension.ToUpper() == "DOCX")
                    {
                        isNotPDFExtension = true;
                        bool isFileSaved = SaveWordFileInTempFolder(input.FileContent, fileNames.FileName, fileExtension, workingFolderPath);
                        fileExtensionWord = fileExtension;
                        fileExtension = "pdf";
                    }

                }

                SqlParameter[] parameters =
                                         {
                                          new SqlParameter("@InstructionID", SqlDbType.BigInt) { Value = input.InstructionID },
                                          new SqlParameter("@InstructorUserID", SqlDbType.BigInt) { Value = input.InstructorID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@InstructionAttachmentID", SqlDbType.BigInt) { Value = input.InstructionAttachmentID },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 250) { Value = fileNames.FileName },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = fileNames.SavedFileName }
                                        };
                DataTable dtFormAttachment = _helper.GetDataTable("[dbo].[updateInstructorFeedback]", parameters);
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
                            // copy the file here 
                            if (isNotPDFExtension)
                            {
                                byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                            }
                            else
                            {
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(dirForm);
                            if (isNotPDFExtension)
                            {
                                byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                            }
                            else
                            {
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
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
                        if (isNotPDFExtension)
                        {
                            byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                        }
                        else
                        {
                            File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
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

        public BaseResponse UpdateInterviewerFeedback(UpdateInterviewerFeedbackRequest input)
        {
            BaseResponse response = new BaseResponse();
            if (input.FileContent != null && input.FileContent.Length > 0)
            {
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                string fileExtensionWord = string.Empty;
                string userFolderName = string.Empty;
                string savedFileName = string.Empty;
                bool isNotPDFExtension = false;
                var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];

                var workingFolderPath = Path.Combine(fileRepoPath, "WorkingFolder");

                // pull the saved file name format SP Below
                FormAttachmentFileNames fileNames = GetFormAttachmentFileName(input.FormID, 27);
                if (input.FileName != string.Empty)
                {
                    AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.FileName);
                    //fileName = fileDetails.FileName;
                    fileExtension = fileDetails.FileExtension;
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
                    if (fileExtension.ToUpper() == "DOC" || fileExtension.ToUpper() == "DOCX")
                    {
                        isNotPDFExtension = true;
                        bool isFileSaved = SaveWordFileInTempFolder(input.FileContent, fileNames.FileName, fileExtension, workingFolderPath);
                        fileExtensionWord = fileExtension;
                        fileExtension = "pdf";
                    }

                }

                SqlParameter[] parameters =
                                         {
                                          new SqlParameter("@InterviewID", SqlDbType.BigInt) { Value = input.InterviewID },
                                          new SqlParameter("@InterviewerUserID", SqlDbType.BigInt) { Value = input.InterviewerID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@InterviewAttachmentID", SqlDbType.BigInt) { Value = input.InterviewAttachmentID },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 250) { Value = fileNames.FileName },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = fileNames.SavedFileName }
                                        };
                DataTable dtFormAttachment = _helper.GetDataTable("[dbo].[updateInterviewerFeedback]", parameters);
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
                            // copy the file here 
                            if (isNotPDFExtension)
                            {
                                byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                            }
                            else
                            {
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(dirForm);
                            if (isNotPDFExtension)
                            {
                                byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                            }
                            else
                            {
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
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
                        if (isNotPDFExtension)
                        {
                            byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                        }
                        else
                        {
                            File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
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

        public BaseResponse AddInstructorToForm(AddInstructorRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@InstructorUserID", SqlDbType.BigInt) { Value = input.InstructorUserID },
                                          new SqlParameter("@isAssigned", SqlDbType.Bit) { Value = input.isAssigned }
                                        };

            int ID = _helper.InsertTable("[dbo].[AddInstructorToForm]", parameters);
            response.Message = "Instructor Added Successfully";
            response.IsSuccess = true;
            return response;
        }

        public BaseResponse AddInterviewerToForm(AddInterviewerRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@InterviewerUserID", SqlDbType.BigInt) { Value = input.InterviewerUserID },
                                          new SqlParameter("@isAssigned", SqlDbType.Bit) { Value = input.isAssigned }
                                        };

            int ID = _helper.InsertTable("[dbo].[AddInterviewerToForm]", parameters);
            response.Message = "Interviewer Added Successfully";
            response.IsSuccess = true;
            return response;
        }

        public BaseResponse AddReviewerToForm(AddReviewerRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@ReviewerID", SqlDbType.BigInt) { Value = input.ReviewerID }
                                        };

            int ID = _helper.InsertTable("[dbo].[AddReviewerToForm]", parameters);
            response.Message = "Reviewer Added Successfully";
            response.IsSuccess = true;
            return response;
        }

        public BaseResponse RemoveReviewerFromForm(AddReviewerRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@ReviewerID", SqlDbType.BigInt) { Value = input.ReviewerID }
                                        };

            int ID = _helper.InsertTable("[dbo].[RemoveReviewerFromForm]", parameters);
            response.Message = "Reviewer Removed Successfully";
            response.IsSuccess = true;
            return response;
        }

        public InstructorListResponse GetInstructorList(GetInstructorInterviewerListRequest input)
        {
            InstructorListResponse obj = new InstructorListResponse();
            SqlParameter[] parameters = {  
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = input.TermCode }
                                        };

            DataTable dtInstructors = _helper.GetDataTable("[dbo].[getInstructorList]", parameters);
            try
            {
                if (dtInstructors.Rows.Count > 0)
                {


                    obj.InstructorList = dtInstructors.AsEnumerable().Select(row =>
                                              new InstructorList
                                              {
                                                  InstructorUserID = Convert.ToInt32(row["InstructorUserID"]),
                                                  InstructorName = Convert.ToString(row["InstructorName"]),
                                                  isAssigned = Convert.ToBoolean(row["isAssigned"])
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

        public InterviewerListResponse GetInterviewerList(GetInstructorInterviewerListRequest input)
        {
            InterviewerListResponse obj = new InterviewerListResponse();
            SqlParameter[] parameters = {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = input.TermCode }
                                        };

            DataTable dtInterviewers = _helper.GetDataTable("[dbo].[getInterviewerList]", parameters);
            try
            {
                if (dtInterviewers.Rows.Count > 0)
                {


                    obj.InterviewerList = dtInterviewers.AsEnumerable().Select(row =>
                                              new InterviewerList
                                              {
                                                  InterviewerUserID = Convert.ToInt32(row["InterviewerUserID"]),
                                                  InterviewerName = Convert.ToString(row["InterviewerName"]),
                                                  isAssigned= Convert.ToBoolean(row["isAssigned"])
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

        public ReviewerListResponse GetReviewerList(GetReviewerListRequest input)
        {
            ReviewerListResponse obj = new ReviewerListResponse();
            SqlParameter[] parameters = {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = input.TermCode }
                                        };

            DataTable dtInterviewers = _helper.GetDataTable("[dbo].[getReviewerList]", parameters);
            try
            {
                if (dtInterviewers.Rows.Count > 0)
                {


                    obj.ReviewerList = dtInterviewers.AsEnumerable().Select(row =>
                                              new ReviewerList
                                              {
                                                  ReviewerID = Convert.ToInt32(row["ReviewerID"]),
                                                  ReviewerName = Convert.ToString(row["ReviewerName"]),
                                                  isAssigned= Convert.ToBoolean(row["isAssigned"])
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

        public StudentMessageBoardResponse GetFormStudentMessageBoard(int UserID, int FormID, int ProgramID, string TermCode)
        {
            StudentMessageBoardResponse obj = new StudentMessageBoardResponse();
            SqlParameter[] parameters = {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = TermCode }
                                        };

            DataTable dtMessageBoard = _helper.GetDataTable("[dbo].[getFormStudentMessageBoard]", parameters);
            try
            {
                if (dtMessageBoard.Rows.Count > 0)
                {


                    obj = dtMessageBoard.AsEnumerable().Select(row =>
                                              new StudentMessageBoardResponse
                                              {
                                                  FormID = Convert.ToInt32(row["ID"]),
                                                  UserID = Convert.ToInt32(row["UserID"]),
                                                  ProgramID = Convert.ToInt32(row["ProgramID"]),
                                                  TermCode = Convert.ToString(row["TermCode"]),
                                                  StudentMessageBoard = Convert.ToString(row["StudentMessageBoard"])
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

        public ProgramConfigurationHandlerResponse GetProgramConfigurationHandler(int UserID, int FormID, int ProgramID, string TermCode)
        {
            ProgramConfigurationHandlerResponse obj = new ProgramConfigurationHandlerResponse();
            SqlParameter[] parameters = {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = TermCode }
                                        };

            DataTable dtProgramConfiguration = _helper.GetDataTable("[Application].[GetProgramConfigurationHandler]", parameters);
            try
            {
                if (dtProgramConfiguration.Rows.Count > 0)
                {


                    obj = dtProgramConfiguration.AsEnumerable().Select(row =>
                                              new ProgramConfigurationHandlerResponse
                                              {
                                                  StateHandler = Convert.ToString(row["StateHandler"])

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

        public BaseResponse UpdateFormStudentMessageBoard(StudentMessageBoardRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@StudentMessageBoard", SqlDbType.NVarChar,-1) { Value = input.StudentMessageBoard }
                                        };

            int ID = _helper.InsertTable("[dbo].[updateFormStudentMessageBoard]", parameters);
            response.Message = "Message board updated Successfully";
            response.IsSuccess = true;
            return response;
        }

        public BaseResponse SaveFormGridNotes(FormSaveGridNotesRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@GridNotes", SqlDbType.NVarChar,-1) { Value = input.GridNotes }
                                        };

            int ID = _helper.InsertTable("[Application].[SaveFormGridNotes]", parameters);
            response.Message = "Notes updated Successfully";
            response.IsSuccess = true;
            return response;
        }
        public LatestWaitlistNumberResponse GetLatestWaitlistNumber(string TermCode, int ProgramID, int FormID)
        {
            LatestWaitlistNumberResponse obj = new LatestWaitlistNumberResponse();
            SqlParameter[] parameters = {
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = TermCode },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = ProgramID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID}
                                        };

            DataTable dtWaitlistNumber = _helper.GetDataTable("[dbo].[GetLatestWaitlistNumber]", parameters);
            try
            {
                if (dtWaitlistNumber.Rows.Count > 0)
                {
                    obj = dtWaitlistNumber.AsEnumerable().Select(row =>
                                              new LatestWaitlistNumberResponse
                                              {
                                                  WaitlistNumber = Convert.ToInt16(row["WaitlistNumber"])

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
        public BaseResponse BulkOfferNotOfferUpdateFormState(BulkNotOfferFormStatusUpdateRequest input)
        {
            BaseResponse response = new BaseResponse();
            string strFormID = string.Join(",", input.FormID);
            SqlParameter[] parameters =
                                   {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.VarChar , 500) { Value = strFormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@FormStateID", SqlDbType.Int) { Value = input.FormStateID }
                                        };
            int id = _helper.InsertTable("[dbo].[UpdateBulkDeny]", parameters);
            foreach (int formID in input.FormID)
            {
                SqlParameter[] parameters1 =
                {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value =input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode }
                };

                DataSet dsRec = _helper.GetDataSet("[dbo].[getFormDetailsByFormID]", parameters1);
                
                    if (dsRec.Tables[0].Rows.Count > 0 && dsRec.Tables[1].Rows.Count > 0)
                    {
                        int applicationTypeID = Convert.ToInt32(dsRec.Tables[1].Rows[0]["ApplicationTypeID"]);
                        if (applicationTypeID == 2 && input.FormStateID == 10)
                        {
                            //if (!(input.FormStateID == 10 && applicationTypeID == 2))
                            //{
                                // applicant mail
                                string logoText = "cid:myImageID";
                                string applicantsName = string.Empty;
                                string toMail = string.Empty;
                                string ccMail = string.Empty;
                                string subject = string.Empty;
                                string body = string.Empty;
                                string programName = string.Empty;
                                string decisionType = string.Empty;
                                string beforeBody = string.Empty;
                                string afterBody = string.Empty;
                                applicantsName = Convert.ToString(dsRec.Tables[0].Rows[0]["ApplicantName"]);
                                toMail = Convert.ToString(dsRec.Tables[0].Rows[0]["cusulbEmail"]);
                                ccMail = Convert.ToString(dsRec.Tables[0].Rows[0]["altEmail"]);
                                programName = Convert.ToString(dsRec.Tables[0].Rows[0]["programName"]);
                                if (input.FormStateID == 10)
                                {
                                    subject = "Application Offered";
                                    decisionType = "Offered";
                                    //body = GetMailBodyTemplate("Student_FormOffer_Confirmation.html");
                                }
                                else
                                {
                                    subject = "Application Not Offered";
                                    decisionType = "Not Offered";
                                    //body = GetMailBodyTemplate("Student_FormNotOffer_Confirmation.html");
                                }
                                SqlParameter[] parameters2 ={
                                                    new SqlParameter("@ProgramIdentifier", SqlDbType.NVarChar, 10) { Value = "" },
                                                    new SqlParameter("@OfferedCategories", SqlDbType.NVarChar , 50) { Value = "" },
                                                    new SqlParameter("@DecisionType", SqlDbType.NVarChar , 50) { Value = decisionType },
                                                    new SqlParameter("@ProgramId", SqlDbType.BigInt) { Value = input.ProgramID }
                                               };

                                DataSet dtDL = _helper.GetDataSet("[Application].[GetDecisionLetters]", parameters2);
                                if (dtDL.Tables[1].Rows.Count > 0)
                                {
                                    if (dtDL.Tables[1].Rows[0]["MailBody"] != DBNull.Value)
                                    {
                                        body = Convert.ToString(dtDL.Tables[1].Rows[0]["MailBody"]);
                                    }
                                }
                                beforeBody = "<html><body><div><img alt=\"logo\" src=[[logoPath]] style=\"width:300px; height:auto;\" /></div>";
                                afterBody = "</body></html>";
                                body = $"{beforeBody}{body}{afterBody}";

                                body = body.Replace("[[logoPath]]", logoText)
                                               .Replace("[[ApplicantName]]", applicantsName)
                                               .Replace("[[programName]]", programName);
                                byte[] inputStr = null;
                                _sendMail.SendEmail(toMail, ccMail, "COMMON", subject, body, inputStr);
                            //}
                        }
                    }
            }
            response.IsSuccess = true;
            response.Message = "Data updated successfully";
            return response;
        }
        public BaseResponse UpdateRecommendation(UpdateRecommendation input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@RecommendationID", SqlDbType.BigInt) { Value = input.RecommendationID },
                                          new SqlParameter("@RecommenderEmail", SqlDbType.NVarChar,200) {Value = input.RecommenderEmail},
                                          new SqlParameter("@RecommenderName", SqlDbType.NVarChar,200) {Value = input.RecommenderName}
                                        };

            int ID = _helper.InsertTable("[dbo].[UpdateRecommendation]", parameters);
            response.Message = "Recommendation Updated Successfully";
            response.IsSuccess = true;
            return response;
        }
        public BaseResponse DeleteRecommendation(DeleteRecommendations input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@RecommendationID", SqlDbType.BigInt) { Value = input.RecommendationID }
                                        };

            int ID = _helper.InsertTable("[dbo].[DeleteRecommendations]", parameters);
            response.Message = "Recommendation Deleted Successfully";
            response.IsSuccess = true;
            return response;
        }

        public AdhocMailLogResponse SendNotificationforPendingRecommendations(PendingRecommendationsRequest input)
        {
            AdhocMailLogResponse obj = new AdhocMailLogResponse();
            BaseResponse baseResponse = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@TermCode", SqlDbType.BigInt) { Value = input.TermCode},
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID}

                                        };

            DataTable dtRecommendationDetails = _helper.GetDataTable("[dbo].[GetListOfPendingRecommendation]", parameters);
            StringBuilder sbLogData = new StringBuilder();
            int totalFailure = 0;
            int count = 0;
            sbLogData.Append("<ol>");
            for (int i=0;i< dtRecommendationDetails.Rows.Count;i++)
            {
                
                try
                {
                    string recommenderName = Convert.ToString(dtRecommendationDetails.Rows[i]["RecommenderName"]);
                    string recommenderEmail = Convert.ToString(dtRecommendationDetails.Rows[i]["RecommenderEmail"]);
                    baseResponse = SendReminderToRecommender(Convert.ToInt32(dtRecommendationDetails.Rows[i]["RecommendationID"]));
                    count++;
                    string logSummary = $"{recommenderName} mail sent to {recommenderEmail} successfully.";
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
            totalFailure = dtRecommendationDetails.Rows.Count - count;
            obj = _fieldWorkService.GetAdocMailLogDetails(input.Type, input.Identifier, sbLogData.ToString(), count, totalFailure, input.UserID);
            obj.IsSuccess = true;
            return obj;

        }
        public BaseResponse UpdateProgramApplicationDates(UpdateProgramApplicationDatesRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar,10) { Value = input.TermCode },
                                          new SqlParameter("@ApplicationOpens", SqlDbType.DateTime) {Value = input.ApplicationOpens},
                                          new SqlParameter("@ApplicationDeadline", SqlDbType.DateTime) {Value = input.ApplicationDeadline},
                                          new SqlParameter("@ApplicationCloseDate", SqlDbType.DateTime) {Value = input.ApplicationCloseDate},
                                          new SqlParameter("@Status", SqlDbType.Bit) {Value = input.Status}
                                        };
            try
            {
                int ID = _helper.InsertTable("[dbo].[UpdateProgramApplicationDates]", parameters);
                response.Message = "Program application dates updated successfully";
                response.IsSuccess = true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response.Message = ex.Message;
                response.IsSuccess= false;
            }
            return response;
        }
        public ProgramApplicationDates GetProgramApplicationDates(int programID, string termCode)
        {
            ProgramApplicationDates obj = new ProgramApplicationDates();
            SqlParameter[] parameters = {
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = termCode }
                                        };

            DataTable dtApplicationDates = _helper.GetDataTable("[dbo].[GetProgramApplicationDates]", parameters);
            try
            {
                if (dtApplicationDates.Rows.Count > 0)
                {
                    obj = dtApplicationDates.AsEnumerable().Select(row =>
                                              new ProgramApplicationDates
                                              {
                                                  ApplicationOpens = Convert.ToDateTime(row["ApplicationOpens"]),
                                                  ApplicationDeadline = Convert.ToDateTime(row["ApplicationDeadline"]),
                                                  ApplicationCloseDate = Convert.ToDateTime(row["ApplicationCloseDate"]),
                                                  Status = Convert.ToBoolean(row["Status"])

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
        public ApplicationProgramResponse GetApplicationProgramsforDates(int userID, int applicationTypeID, string termCode)
        {
            ApplicationProgramResponse obj = new ApplicationProgramResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserId", SqlDbType.Int, 50) { Value = userID },
                                          new SqlParameter("@ApplicationTypeID", SqlDbType.Int, 50) { Value = applicationTypeID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = termCode }
                                        };

            DataSet dtApplicationPrograms = _helper.GetDataSet("[dbo].[GetApplicationProgramsforDates]", parameters);
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
                                                  TermCode = Convert.ToString(row["TermCode"]),
                                                  applicationOpens = Convert.ToDateTime(row["ApplicationOpens"]),
                                                  applicationCloseDate = Convert.ToDateTime(row["ApplicationCloseDate"]),
                                                  TotalCount = Convert.ToInt32(row["TotalCount"]),
                                                  AcceptedCount = Convert.ToInt32(row["AcceptedCount"]),
                                                  showApply = Convert.ToBoolean(row["showApply"]),
                                                  showView = Convert.ToBoolean(row["showView"]),
                                                  ProgramSetting = Convert.ToString(row["ProgramSetting"]),
                                                  SubmittedCount = Convert.ToInt32(row["SubmittedCount"])
                                              }).ToList();

                    obj.HeaderDetails = dtApplicationPrograms.Tables[1].AsEnumerable().Select(row =>
                                               new HeaderDetails
                                               {
                                                   semester = Convert.ToString(row["Semester"]),
                                                   TermCode = Convert.ToString(row["TermCode"]),
                                                   showApply = Convert.ToBoolean(row["showApply"]),
                                                   showView = Convert.ToBoolean(row["showView"]),
                                                   showAssignApplicationToReviewers = Convert.ToBoolean(row["showAssignApplicationToReviewers"]),
                                                   showSettings = Convert.ToBoolean(row["showSettings"])
                                               }).FirstOrDefault();

                    obj.Semesters = dtApplicationPrograms.Tables[1].AsEnumerable().Select(row =>
                                             new Semester
                                             {
                                                 TermCode = Convert.ToString(row["TermCode"]),
                                                 TermName = Convert.ToString(row["Semester"])
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
        public BaseResponse RevertBacktoPreviousState(int formID)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID }
                                        };

            int ID = _helper.InsertTable("[dbo].[RevertBacktoPreviousState]", parameters);
            response.Message = "Reverted Back to Previous State Successfully";
            response.IsSuccess = true;
            return response;
        }
        public BaseResponse MoveApplicationToSemester(MoveApplicationToSemesterRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = input.TermCode },
                                          new SqlParameter("@FormSchema", SqlDbType.NVarChar, -1) { Value = input.FormSchema }
                                        };
            DataTable dtResponse = _helper.GetDataTable("[dbo].[MoveApplicationToSemester]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "SUCCESS")
                {
                    response.Message = "Updated Semester Successfully";
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "FAILURE")
                {
                    response.Message = "Failed to update the semester";
                    response.IsSuccess = true;
                }
            }
            return response;
        }
        public ApplicationProgramsListResponse GetApplicationProgramList(int applicationId,string identifier)
        {
            ApplicationProgramsListResponse obj = new ApplicationProgramsListResponse();
            SqlParameter[] parameters ={
                                            new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt) { Value = applicationId },
                                       };

            DataTable dtApplicationPrograms = _helper.GetDataTable("[dbo].[GetProgramsByApplicationType]", parameters);
            try
            {
                if (dtApplicationPrograms.Rows.Count > 0)
                {
                    if (applicationId == 1 && identifier == "Evaluation")
                    {
                        obj.ApplicationProgramsList = dtApplicationPrograms.AsEnumerable().Where(row => row.Field<long>("ID") == 2 || row.Field<long>("ID") == 3 || row.Field<long>("ID") == 4).Select(row =>
                                              new ApplicationProgramsList
                                              {
                                                  ProgramID = Convert.ToInt32(row["ID"]),
                                                  ProgramName = Convert.ToString(row["Name"])
                                              }).ToList();
                    }
                    else if (applicationId == 2 && identifier == "Drop Down")
                    {
                        obj.ApplicationProgramsList = dtApplicationPrograms.AsEnumerable().Where(row => row.Field<long>("ID") == 12 || row.Field<long>("ID") == 20).Select(row =>
                                              new ApplicationProgramsList
                                              {
                                                  ProgramID = Convert.ToInt32(row["ID"]),
                                                  ProgramName = Convert.ToString(row["Name"])
                                              }).ToList();
                    }
                    else
                    {
                        obj.ApplicationProgramsList = dtApplicationPrograms.AsEnumerable().Select(row =>
                                                  new ApplicationProgramsList
                                                  {
                                                      ProgramID = Convert.ToInt32(row["ID"]),
                                                      ProgramName = Convert.ToString(row["Name"])
                                                  }).ToList();
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
        public RecommenderMailBodyResponse GetRecommenderMailBody(int applicationId, int programID,string identifier)
        {
            RecommenderMailBodyResponse obj = new RecommenderMailBodyResponse();
            SqlParameter[] parameters ={
                                            new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt) { Value = applicationId },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
                                            new SqlParameter("@Identifier", SqlDbType.NVarChar,50) { Value = identifier }
                                       };

            DataTable dtRecom = _helper.GetDataTable("[dbo].[GetRecommenderMailBody]", parameters);
            try
            {
                if (dtRecom.Rows.Count > 0)
                {
                    obj.recommenderResponse = dtRecom.AsEnumerable().Select(row =>
                                              new RecommenderBody
                                              {
                                                  ID = Convert.ToInt32(row["ID"]),
                                                  RecommenderMailBody = Convert.ToString(row["RecommenderMailBody"])
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
        public BaseResponse UpdateRecommenderMailBody(RecommenderBody input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@ID", SqlDbType.BigInt) { Value = input.ID },
                                          new SqlParameter("@RecommenderMailBody", SqlDbType.NVarChar, -1) { Value = input.RecommenderMailBody },
                                          new SqlParameter("@Identifier", SqlDbType.NVarChar,50) { Value = input.Identifier }
                                        };
            DataTable dtResponse = _helper.GetDataTable("[dbo].[UpdateRecommenderMailBody]", parameters);
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
        public BaseResponse UpsertDecisionLetters(DecisionLettersRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@DecisionLettersID", SqlDbType.BigInt) { Value = input.DecisionLettersID },
                                          new SqlParameter("@ProgramIdentifier", SqlDbType.NVarChar,10) { Value = input.ProgramIdentifier },
                                          new SqlParameter("@OfferedCategories", SqlDbType.NVarChar,50) { Value = input.OfferedCategories },
                                          new SqlParameter("@DecisionType", SqlDbType.NVarChar,50) { Value = input.DecisionType },
                                          new SqlParameter("@MailBody", SqlDbType.NVarChar, -1) { Value = input.MailBody },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID }
                                        };
            DataTable dtResponse = _helper.GetDataTable("[Application].[UpsertDecisionLetters]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "SUCCESS")
                {
                    response.Message = "Updated admit letter mail body successfully";
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "FAILURE")
                {
                    response.Message = "Failed to update the admit letter mail body";
                    response.IsSuccess = false;
                }
            }
            return response;
        }
        public DecisionLettersResponse GetDecisionLetters(string programIdentifier, string offeredCategories, string decisionType)
        {
            DecisionLettersResponse obj = new DecisionLettersResponse();
            SqlParameter[] parameters ={
                                            new SqlParameter("@ProgramIdentifier", SqlDbType.NVarChar, 10) { Value = programIdentifier },
                                            new SqlParameter("@OfferedCategories", SqlDbType.NVarChar , 50) { Value = offeredCategories },
                                            new SqlParameter("@DecisionType", SqlDbType.NVarChar , 50) { Value = decisionType }
                                       };

            DataSet dtDL = _helper.GetDataSet("[Application].[GetDecisionLetters]", parameters);
            try
            {
                if (dtDL.Tables[0].Rows.Count > 0)
                {
                    if (dtDL.Tables[0].Rows[0]["ID"] != DBNull.Value)
                    {
                        obj.finalDecision = dtDL.Tables[0].AsEnumerable().Select(row =>
                                                new FinalDecision
                                                {
                                                    ID = Convert.ToInt32(row["ID"]),
                                                    Decision = Convert.ToString(row["Decision"])
                                                }).ToList();
                    }
                    else
                    {
                        obj.finalDecision = null;
                    }
                    if (dtDL.Tables[1].Rows.Count > 0)
                    {
                        if (dtDL.Tables[1].Rows[0]["ProgramIdentifier"] != DBNull.Value)
                        {
                            obj.decisionLetters = dtDL.Tables[1].AsEnumerable().Select(row =>
                                                  new DecisionLetters
                                                  {
                                                      DecisionLettersID = Convert.ToInt32(row["DecisionLettersID"]),
                                                      ProgramIdentifier = Convert.ToString(row["ProgramIdentifier"]),
                                                      OfferedCategories = Convert.ToString(row["OfferedCategories"]),
                                                      DecisionType = Convert.ToString(row["DecisionType"]),
                                                      MailBody = Convert.ToString(row["MailBody"]),
                                                      CreatedByUserID = Convert.ToInt32(row["CreatedByUserID"]),
                                                      UpdatedDateTime = Convert.ToDateTime(row["UpdatedDateTime"])
                                                  }).FirstOrDefault();
                        }
                        else
                        {
                            obj.decisionLetters = null;
                        }
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
        public DropDownListResponse GetDropDownList(int programId, string controlLabel)
        {
            DropDownListResponse obj = new DropDownListResponse();
            SqlParameter[] parameters ={
                                            new SqlParameter("@ProgramId", SqlDbType.BigInt) { Value = programId },
                                            new SqlParameter("@ControlLabel", SqlDbType.VarChar,50) { Value = controlLabel }
                                       };

            DataTable dtDD = _helper.GetDataTable("[Master].[Get_Dropdowns_for_ICP_and_GRAD]", parameters);
            try
            {
                if (dtDD.Rows.Count > 0)
                {
                    if (programId > 6)
                    {
                        obj.DropDowns = dtDD.AsEnumerable().Where(row => row.Field<string>("ControlLabel") == "PersonalInformation").Select(row =>
                                              new DropDowns
                                              {
                                                  DropdownId = Convert.ToInt32(row["DropdownId"]),
                                                  ProgramID = Convert.ToInt32(row["ProgramID"]),
                                                  ControlLabel = "Confirm Program Option Applying to",
                                                  ControlValue = Convert.ToString(row["ControlValue"]),
                                                  Active = Convert.ToBoolean(row["Active"])
                                              }).ToList();
                    }
                    else
                    {
                        obj.DropDowns = dtDD.AsEnumerable().Select(row =>
                                                  new DropDowns
                                                  {
                                                      DropdownId = Convert.ToInt32(row["DropdownId"]),
                                                      ProgramID = Convert.ToInt32(row["ProgramID"]),
                                                      ControlLabel = Convert.ToString(row["ControlLabel"]),
                                                      ControlValue = Convert.ToString(row["ControlValue"]),
                                                      Active = Convert.ToBoolean(row["Active"])
                                                  }).ToList();
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
        public ControlLabelListResponse GetControlLabelList(int programId)
        {
            ControlLabelListResponse obj = new ControlLabelListResponse();
            SqlParameter[] parameters ={
                                            new SqlParameter("@ProgramId", SqlDbType.BigInt) { Value = programId }
                                       };

            DataTable dtCL = _helper.GetDataTable("[Master].[Get_Program_Term_for_ICP_and_GRAD]", parameters);
            try
            {
                if (dtCL.Rows.Count > 0)
                {
                    if (programId > 6)
                    {
                        obj.ConrolLabels = dtCL.AsEnumerable().Where(row=> row.Field<string>("ControlLabel") == "PersonalInformation").Select(row =>
                                              new ConrolLabel
                                              {
                                                  Label = "Confirm Program Option Applying to",
                                                  Value = Convert.ToString(row["ControlLabel"])
                                              }).ToList();
                    }
                    else
                    {
                        obj.ConrolLabels = dtCL.AsEnumerable().Select(row =>
                                                  new ConrolLabel
                                                  {
                                                      Label = Convert.ToString(row["ControlLabel"]),
                                                      Value = Convert.ToString(row["ControlLabel"])
                                                  }).ToList();
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
        public BaseResponse UpsertDropDown(DropDownRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@ProgramId", SqlDbType.BigInt) { Value = input.ProgramId },
                                          new SqlParameter("@ControlLabel", SqlDbType.NVarChar,50) { Value = input.ControlLabel },
                                          new SqlParameter("@ControlValue", SqlDbType.NVarChar,-1) { Value = input.ControlValue },
                                          new SqlParameter("@Active", SqlDbType.Bit) { Value = input.Active },
                                          new SqlParameter("@Action", SqlDbType.BigInt) { Value = input.Action},
                                          new SqlParameter("@DropdownId", SqlDbType.BigInt) { Value = input.DropdownId}
                                        };
            DataTable dtResponse = _helper.GetDataTable("[Master].[Upsert_Dropdowns_for_ICP_and_GRAD]", parameters);
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
        public EvaluatorMailBodyResponse GetEvaluatorMailBody(int applicationId, int programID, string identifier)
        {
            EvaluatorMailBodyResponse obj = new EvaluatorMailBodyResponse();
            SqlParameter[] parameters ={
                                            new SqlParameter("@ApplicationTypeID", SqlDbType.BigInt) { Value = applicationId },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
                                            new SqlParameter("@Identifier", SqlDbType.NVarChar,50) { Value = identifier }
                                       };

            DataTable dtEval = _helper.GetDataTable("[Application].[GetEvaluatorEmail]", parameters);
            try
            {
                if (dtEval.Rows.Count > 0)
                {
                    obj.evaluatorResponse = dtEval.AsEnumerable().Select(row =>
                                              new EvaluatorBody
                                              {
                                                  ID = Convert.ToInt32(row["ID"]),
                                                  EvaluatorMailBody = Convert.ToString(row["EvaluatorMailBody"])
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
        public BaseResponse UpdateEvaluatorMailBody(EvaluatorBody input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@ID", SqlDbType.BigInt) { Value = input.ID },
                                          new SqlParameter("@RecommenderMailBody", SqlDbType.NVarChar, -1) { Value = input.EvaluatorMailBody }
                                        };
            DataTable dtResponse = _helper.GetDataTable("[dbo].[UpdateRecommenderMailBody]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "SUCCESS")
                {
                    response.Message = "Updated clinical practice evaluation mail body successfully";
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "FAILURE")
                {
                    response.Message = "Failed to update the clinical practice evaluation mail body";
                    response.IsSuccess = false;
                }
            }
            return response;
        }
        public FormAttachmentDeatilsResponse GetFormAttachmentDeatils(int userID, int programID, int formID)
        {
            FormAttachmentDeatilsResponse obj = new FormAttachmentDeatilsResponse();
            SqlParameter[] parameters ={
                                            new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userID },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID },
                                            new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID }
                                       };

            DataTable dtAttachmentDetails = _helper.GetDataTable("[dbo].[GetFormAttachmentDeatils]", parameters);
            try
            {
                if (dtAttachmentDetails.Rows.Count > 0)
                {
                    obj.formAttachmentDeatils = dtAttachmentDetails.AsEnumerable().Select(row =>
                                              new FormAttachmentDeatils
                                              {
                                                  formID = Convert.ToInt32(row["FormID"]),
                                                  formAttachmentID= Convert.ToInt32(row["FormAttachmentID"] == DBNull.Value ? null : row["FormAttachmentID"]),
                                                  documentID = Convert.ToInt32(row["DocumentID"]),
                                                  programID = Convert.ToInt32(row["ProgramID"]),
                                                  attachmentTitle = Convert.ToString(row["AttachmentTitle"]),
                                                  fileName = Convert.ToString(row["FileName"] == DBNull.Value ? null : row["FileName"]),
                                                  fileExtn = Convert.ToString(row["FileExtn"] == DBNull.Value ? null : row["FileExtn"]),
                                                  IsOptional = Convert.ToBoolean(row["IsOptional"])
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
        public FormDocumentResponse GetFormAttachment(int userId, int formID, int programID)
        {
            FormDocumentResponse obj = new FormDocumentResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserId", SqlDbType.BigInt) { Value = userId },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = programID }
                                        };

            DataTable dsAttachments = _helper.GetDataTable("[Application].[GetFormDocuments]", parameters);
            try
            {
                if (dsAttachments.Rows.Count > 0)
                {
                    obj.formdocumentDeatils = dsAttachments.AsEnumerable().Select(row =>
                                              new FormDocumentDeatils
                                              {
                                                  formAttachmentID = Convert.ToInt32(row["FormAttachmentID"] == DBNull.Value ? null : row["FormAttachmentID"]),
                                                  DocumentID = Convert.ToInt32(row["DocumentID"]),
                                                  ProgramID = Convert.ToInt32(row["ProgramID"]),
                                                  FormID = Convert.ToInt32(row["FormID"]),
                                                  IsOptional = Convert.ToBoolean(row["IsOptional"]),
                                                  AttachmentTitle = Convert.ToString(row["AttachmentTitle"]),
                                                  FileName = Convert.ToString(row["FileName"]),
                                                  FileExtn= Convert.ToString(row["FileExtn"]),
                                                  IsApproved = Convert.ToBoolean(row["IsApproved"] == DBNull.Value ? null : row["IsApproved"]),
                                                  ApprovedBy = Convert.ToString(row["ApprovedBy"] == DBNull.Value ? null : row["ApprovedBy"]),
                                                  ValidatedDate = Convert.ToDateTime(row["ValidatedDate"] == DBNull.Value ? null : row["ValidatedDate"]),
                                                  ValidTill = Convert.ToDateTime(row["ValidTill"] == DBNull.Value ? null : row["ValidTill"]),
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
        public BaseResponse UpsertFormDocument(FormDocumentRequest input)
        {
            BaseResponse response = new BaseResponse();
            if (input.FileContent != null && input.FileContent.Length > 0)
            {
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                string fileExtensionWord = string.Empty;
                string userFolderName = string.Empty;
                string savedFileName = string.Empty;
                bool isNotPDFExtension = false;
                var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];

                var workingFolderPath = Path.Combine(fileRepoPath, "WorkingFolder");

                // pull the saved file name format SP Below
                FormAttachmentFileNames fileNames = GetFormAttachmentFileName(input.FormID, input.DocumentID);
                if (input.FileName != string.Empty)
                {
                    AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.FileName);
                    //fileName = fileDetails.FileName;
                    fileExtension = fileDetails.FileExtension;
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
                    if (fileExtension.ToUpper() == "DOC" || fileExtension.ToUpper() == "DOCX")
                    {
                        isNotPDFExtension = true;
                        bool isFileSaved = SaveWordFileInTempFolder(input.FileContent, fileNames.FileName, fileExtension, workingFolderPath);
                        fileExtensionWord = fileExtension;
                        fileExtension = "pdf";
                    }

                }

                SqlParameter[] parameters =
                                         {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@DocumentID", SqlDbType.BigInt) { Value = input.DocumentID },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 250) { Value = fileNames.FileName },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = fileNames.SavedFileName },
                                          new SqlParameter("@ValidTill", SqlDbType.DateTime) {Value=input.ValidTill},
                                          new SqlParameter("@Comments", SqlDbType.NVarChar,-1) {Value=input.Comments},
                                          new SqlParameter("@UploadedDate", SqlDbType.DateTime) {Value=input.UploadedDate}
                                        };
                DataTable dtFormAttachment = _helper.GetDataTable("[dbo].[UpsertFormDocument]", parameters);
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
                            // copy the file here 
                            if (isNotPDFExtension)
                            {
                                byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                            }
                            else
                            {
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(dirForm);
                            if (isNotPDFExtension)
                            {
                                byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                            }
                            else
                            {
                                File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
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
                        if (isNotPDFExtension)
                        {
                            byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileNames.FileName + "." + fileExtensionWord), Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension));
                        }
                        else
                        {
                            File.WriteAllBytes(Path.Combine(dirForm, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
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
        public BaseResponse UpdateFormDocumentValidation(FormDocumentValidationRequest input)
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
                                          new SqlParameter("@FormID", SqlDbType.BigInt, 50) { Value = input.FormID },
                                          new SqlParameter("@IsApproved", SqlDbType.Bit, 50) { Value = input.ApprovalStatus },
                                          new SqlParameter("@ApprovedBy", SqlDbType.BigInt, 50) { Value = input.ApproverUserId },
                                          new SqlParameter("@ValidatedDate", SqlDbType.DateTime, 50) { Value = validatedDate },
                                          new SqlParameter("@ValidTill", SqlDbType.DateTime, 50) { Value = (object)input.ValidTill??DBNull.Value },
                                          new SqlParameter("@RejectReason", SqlDbType.NVarChar, 255) { Value = (object)input.RejectedReason??DBNull.Value },
                                          new SqlParameter("@Comments", SqlDbType.NVarChar,-1) { Value = (object)input.Comments??DBNull.Value }
                                        };

                int identity = _helper.InsertTable("[dbo].[UpdateFormDocumentValidation]", parameters);

                response.Message = "Form Attachment review status updated successfully.";
            }
            catch (Exception ex)
            {
                if (response.IsSuccess == true)
                {
                    response.IsSuccess = false;
                    response.Message = "Form Attachment validation failed";
                }
            }

            return response;
        }
        public BaseResponse UpsertInterviewDate(FormUpsertAttachmentRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@FormSchema", SqlDbType.NVarChar,-1) { Value = input.FormSchema },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          
                                        };
            DataTable dtResponse = _helper.GetDataTable("[dbo].[UpsertFormInterviewDate]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "SUCCESS")
                {
                    response.Message = "Updated Interview Date with Faculty successfully";
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtResponse.Rows[0]["RESULT"]) == "FAILURE")
                {
                    response.Message = "Failed to update the Interview Date with Faculty";
                    response.IsSuccess = false;
                }
            }
            return response;
        }
    }


    public class FormAttachmentFileNames
    {
        public string SavedFileName { get; set; }
        public string FileName { get; set; }
        public string UserFolder { get; set; }
    }
    public class FormAttachmentEntity
    {
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string FolderName { get; set; }
    }
}
