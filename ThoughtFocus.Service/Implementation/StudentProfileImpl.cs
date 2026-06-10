using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.xmp.impl;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Domain.Request.InitialCredentialProgram;
using ThoughtFocus.Domain.Request.StudentProfile;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Application;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;
using ThoughtFocus.Domain.Response.SearchApplication;
using ThoughtFocus.Domain.Response.StudentProfile;
using ThoughtFocus.Service.Interfaces;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;


namespace ThoughtFocus.Service.Implementation
{
    public class StudentProfileImpl : IStudentProfile
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        public ILogger<SearchApplicationImpl> _logger;
        private readonly ICommonUtils _utils;
        public StudentProfileImpl(ISqlDBUtility helper
                                         , IConfiguration configuration
                                         , ISendMail sendMail
                                         , ILogger<SearchApplicationImpl> logger
                                         , ICommonUtils utils)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
            _utils = utils;
        }
       

public StudentProfileResponse GetStudentProfileData(string CsuldId, int UserID,int formID)
{
    StudentProfileResponse obj = new StudentProfileResponse();
    string csulbIDsStaff = String.Empty;
    var jsonObj = JObject.Parse(File.ReadAllText(@"SupportFiles/MycedConfigurations/CSULBCEDConfig.json"));
    csulbIDsStaff = jsonObj["CSULBIDStaffSP"]?.ToString();

    SqlParameter[] parameters = {
                                          new SqlParameter("@csulbid", SqlDbType.VarChar, 50) { Value = CsuldId },
                                          new SqlParameter("@UserID", SqlDbType.VarChar, 50) { Value = UserID },
                                          new SqlParameter("@CSULBIDStaffSP", SqlDbType.NVarChar, -1) { Value = csulbIDsStaff },
                                          new SqlParameter("@FormID", SqlDbType.NVarChar, -1) { Value = formID }
                                        };

    DataSet dtStudentProfile = _helper.GetDataSet("[dbo].[GetStudentProfile]", parameters);
    try
    {
        if (dtStudentProfile.Tables.Count > 0)
        {
            obj.studentProfile = dtStudentProfile.Tables[0].AsEnumerable().Select(row => new StudentProfile
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
                AcademicPlan = Convert.ToString(row["AcademicPlan"]),
                AcademicSubPlan = Convert.ToString(row["AcademicSubPlan"]),
                AdditionalPlan = Convert.ToString(row["AdditionalPlan"]),
                ProgramStatusDesc = Convert.ToString(row["ProgramStatusDesc"]),
                GraduationFillingStatusDesc = Convert.ToString(row["GraduationFilingStatusDescr"]),
                CurrentCsulbGpa = Convert.ToString(row["CurrentCSULBGPA"]),
                CumulativeGpa = Convert.ToString(row["CumulativeGPA"]),
                MajorGpa = Convert.ToString(row["MajorGPA"]),
                AcademicStanding = Convert.ToString(row["AcademicStanding"]),
                BachelorDegreeMajor = Convert.ToString(row["BachelorDegreeMajor"]),
                AdmitTerm = Convert.ToString(row["AdmitTerm"]),
                ActiveTerm = Convert.ToString(row["ActiveTerm"]),
                GraduationFillingTerm = Convert.ToString(row["GraduationFillingTerm"]),
                EducationalLeaveTerm = Convert.ToString(row["EducationalLeaveTerm"]),
                Credential = Convert.ToString(row["Credential"]),
                Certificate = Convert.ToString(row["Certificate"]),
                DateOfBirth = Convert.ToDateTime(row["DateOfBirth"] == DBNull.Value ? null : row["DateOfBirth"]),
                SSNNumber = Convert.ToString(row["SSNNumber"] == DBNull.Value ? null : row["SSNNumber"]),
                AcademicIntegrityStatement = Convert.ToString(row["AcademicIntegrityStatement"] == DBNull.Value ? null : row["AcademicIntegrityStatement"]),
                SubmittedDate = Convert.ToDateTime(row["SubmittedDate"] == DBNull.Value ? null : row["SubmittedDate"]),
                IsAgreed = Convert.ToBoolean(row["IsAgreed"] == DBNull.Value ? null : row["IsAgreed"]),
                AgreedDate = row["AgreedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["AgreedDate"]) : null,
                BachelorDegreeMajorSP = Convert.ToString(row["BachelorDegreeMajorSP"]),
                ConsolidatedAddress = Convert.ToString(row["ConsolidatedAddress"]),
                UserID = Convert.ToInt32(row["UserID"]),
                isStaff = Convert.ToBoolean(row["isStaff"]),
                CredentialProgram = Convert.ToString(row["CredentialProgram"]),
                CredentialPathway = Convert.ToString(row["CredentialPathway"]),
                TermCode = Convert.ToString(row["TermCode"])

            }).FirstOrDefault();
               

                    obj.FormSubSectionResponseSMC = dtStudentProfile.Tables.Count > 1 && dtStudentProfile.Tables[1] != null
    ? dtStudentProfile.Tables[1].AsEnumerable().Select(row =>
        new FormSubSectionResponseSMC
        {
            FormSubSectionID = row["FormSubSectionID"] != DBNull.Value
                ? Convert.ToInt32(row["FormSubSectionID"])
                : 0,

            FormID = row["FormID"] != DBNull.Value
                ? Convert.ToInt32(row["FormID"])
                : 0,

            SubSectionIdentifiers = row["SubSectionIdentifiers"] != DBNull.Value
                ? Convert.ToString(row["SubSectionIdentifiers"])
                : string.Empty,

            SubSectionForm = row["SubSectionForm"] != DBNull.Value
                ? Convert.ToString(row["SubSectionForm"])
                : string.Empty,

                TermCode = row["LatestTermCode"] != DBNull.Value
                ? Convert.ToString(row["LatestTermCode"])
                : string.Empty,
        }).FirstOrDefault()
    : null;

                    obj.FormSectionAttachmentListSMC = dtStudentProfile.Tables.Count > 2 && dtStudentProfile.Tables[2] != null
                        ? dtStudentProfile.Tables[2].AsEnumerable().Select(row =>
                            new FormSectionAttachmentListSMC
                            {
                                FormSubSectionAttachmentID = row["FormSubSectionAttachmentID"] != DBNull.Value
                                    ? Convert.ToInt32(row["FormSubSectionAttachmentID"])
                                    : 0,

                                FormID = row["FormID"] != DBNull.Value
                                    ? Convert.ToInt32(row["FormID"])
                                    : 0,

                                FormSubSectionID = row["FormSubSectionID"] != DBNull.Value
                                    ? Convert.ToInt32(row["FormSubSectionID"])
                                    : 0,

                                SubSectionIdentifiers = row["SubSectionIdentifiers"] != DBNull.Value
                                    ? Convert.ToString(row["SubSectionIdentifiers"])
                                    : string.Empty,

                                FileName = row["FileName"] != DBNull.Value
                                    ? Convert.ToString(row["FileName"])
                                    : string.Empty,

                                FileExtn = row["FileExtn"] != DBNull.Value
                                    ? Convert.ToString(row["FileExtn"])
                                    : string.Empty
                            }).ToList()
                        : new List<FormSectionAttachmentListSMC>();

                    obj.FormSubSectionResponseGPA = dtStudentProfile.Tables.Count > 3 && dtStudentProfile.Tables[3] != null
                        ? dtStudentProfile.Tables[3].AsEnumerable().Select(row =>
                            new FormSubSectionResponseGPA
                            {
                                FormSubSectionIDGPA = row["FormSubSectionIDGPA"] != DBNull.Value
                                    ? Convert.ToInt32(row["FormSubSectionIDGPA"])
                                    : 0,

                                FormIDGPA = row["FormIDGPA"] != DBNull.Value
                                    ? Convert.ToInt32(row["FormIDGPA"])
                                    : 0,

                                SubSectionIdentifiersGPA = row["SubSectionIdentifiersGPA"] != DBNull.Value
                                    ? Convert.ToString(row["SubSectionIdentifiersGPA"])
                                    : string.Empty,

                                SubSectionFormGPA = row["SubSectionFormGPA"] != DBNull.Value
                                    ? Convert.ToString(row["SubSectionFormGPA"])
                                    : string.Empty
                            }).FirstOrDefault()
                        : null;

                    obj.admitDecision = dtStudentProfile.Tables.Count > 4 && dtStudentProfile.Tables[4] != null
                        ? dtStudentProfile.Tables[4].AsEnumerable().Select(row =>
                            new AdmitDecision
                            {
                                FinalDecisionDate = row["FinalDecisionDate"] != DBNull.Value
                                    ? Convert.ToDateTime(row["FinalDecisionDate"])
                                    : DateTime.MinValue,

                                FinalDecision = row["FinalDecision"] != DBNull.Value
                                    ? Convert.ToString(row["FinalDecision"])
                                    : string.Empty,

                                LatestApplicationTypeID = row["LatestApplicationTypeID"] != DBNull.Value
                                    ? Convert.ToInt32(row["LatestApplicationTypeID"])
                                    : 0,

                                LatestProgramID = row["LatestProgramID"] != DBNull.Value
                                    ? Convert.ToInt32(row["LatestProgramID"])
                                    : 0
                            }).FirstOrDefault()
                        : null;
                   
                    obj.StudentProfileRoleHandler = dtStudentProfile.Tables.Count > 5
                   && dtStudentProfile.Tables[5] != null
                   && dtStudentProfile.Tables[5].Rows.Count > 0
                   ? new StudentProfileRoleHandler
                   {
                       StudentProfileControls =
                           dtStudentProfile.Tables[5].Rows[0]["StudentProfileRoleHandler"] != DBNull.Value
                           ? Newtonsoft.Json.JsonConvert.DeserializeObject(
                               Convert.ToString(
                                   dtStudentProfile.Tables[5].Rows[0]["StudentProfileRoleHandler"]))
                           : new { }

                   }
                   : null;
                    obj.StudentProfileStateHandler = dtStudentProfile.Tables[6].AsEnumerable().Select(row =>
                                                       new StudentProfileStateHandler
                                                       {
                                                           StateHandler = Convert.ToString(row["StateHandler"])

                                                       }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(obj.studentProfile.SSNNumber) && obj.studentProfile.SSNNumber != null)
                    {
                        //IsApproved = Convert.ToBoolean(row["IsApproved"] == DBNull.Value ? null : row["IsApproved"]),
                        obj.studentProfile.SSNNumber = DecryptSSNNumber(obj.studentProfile.SSNNumber);
                    }
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
public StudentProfileSearchResponse GetStudentProfileSearchData(string searchString)
        {
            StudentProfileSearchResponse obj = new StudentProfileSearchResponse();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@searchString", SqlDbType.NVarChar,100) { Value = searchString }
                                     };
            DataTable dtResponse = _helper.GetDataTable("[dbo].[SearchStudentProfile]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                obj.studentProfileSearch = dtResponse.AsEnumerable().Select(row =>
                                              new StudentProfileSearch
                                              {
                                                  ID = Convert.ToInt32(row["ID"]),
                                                  FirstName = Convert.ToString(row["FirstName"]),
                                                  LastName = Convert.ToString(row["LastName"]),
                                                  EMAIL = Convert.ToString(row["EMAIL"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),
                                                  Type = Convert.ToString(row["Type"]),
                                                  UserID = Convert.ToInt32(row["UserID"]),
                                                  TermCode = row["TermCode"] == DBNull.Value ? "" : Convert.ToString(row["TermCode"]),
                                                  ProgramID = row["ProgramID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ProgramID"]),
                                                  ApplicationTypeID = Convert.ToInt32(row["ApplicationTypeID"]),
                                                  ProgramName = Convert.ToString(row["ProgramName"]),
                                                  Term = Convert.ToString(row["Term"]),
                                                  Status = Convert.ToString(row["Status"]),
                                                  ProgramPlannerState = row["ProgramPlannerState"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ProgramPlannerState"])
                                              }).ToList();
                obj.IsSuccess = true;
                obj.Message = "Data retrieved succesfully ";

            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No data matching this search criteria";
            }
            return obj;

        }
        public StudentProfileMessageBoardResponse GetStudentProfileMessageBoard(string CsulbId, string MessageBoardIdentifier)
        {
            StudentProfileMessageBoardResponse obj = new StudentProfileMessageBoardResponse();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@CSULBID", SqlDbType.NVarChar,25) { Value = CsulbId },
                                          new SqlParameter("@MessageBoardIdentifier", SqlDbType.NVarChar,20) { Value = MessageBoardIdentifier}
                                     };
            DataTable dtResponse = _helper.GetDataTable("[dbo].[GetStudentProfileMessageBoard]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                obj.studentProfileMessageBoards = dtResponse.AsEnumerable().Select(row =>
                                              new StudentProfileMessageBoard
                                              {
                                                  MessageBoard = Convert.ToString(row["MessageBoard"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),

                                              }).FirstOrDefault();
                obj.IsSuccess = true;
                obj.Message = "Data retrieved succesfully ";

            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No data matching this search criteria";
            }
            return obj;

        }
        public BaseResponse UpdateStudentProfileMessageBoard(UpdateStudentProfileMessageBoardRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@CSULBID", SqlDbType.NVarChar,25) { Value = input.CSULBID },
                                          new SqlParameter("@MessageBoardIdentifier", SqlDbType.NVarChar,20) { Value = input.MessageBoardIdentifier },
                                          new SqlParameter("@MessageBoard", SqlDbType.VarChar,-1) { Value = input.MessageBoard }
                                        };

            DataTable recomDetails = _helper.GetDataTable("[dbo].[UpdateStudentProfileMessageBoard]", parameters);
            response.Message = "Updated the student profile message board successfully";
            response.IsSuccess = true;
            return response;
        }
        public BaseResponse SaveStudentProfileData(SaveStudentProfileDataRequest input)
        {
            BaseResponse response = new BaseResponse();
            if (!string.IsNullOrEmpty(input.SSNNumber) && input.SSNNumber != null)
            {
                input.SSNNumber = EncryptSSNNumber(input.SSNNumber);
            }

            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@CSULBID", SqlDbType.NVarChar,25) { Value = input.CSULBID  },
                                          new SqlParameter("@DateOfBirth", SqlDbType.DateTime) { Value = input.DateOfBirth  },
                                          new SqlParameter("@SSNNumber", SqlDbType.VarChar,-1) { Value = input.SSNNumber },
                                          new SqlParameter("@AcademicIntegrityStatement", SqlDbType.VarChar,-1) { Value = input.AcademicIntegrityStatement },
                                          new SqlParameter("@SubmittedDate", SqlDbType.DateTime) { Value = input.SubmittedDate }
                                        };

            int id = _helper.InsertTable("[dbo].[SaveStudentProfile]", parameters);
            response.Message = "Student profile data saved successfully";
            response.IsSuccess = true;
            return response;
        }
        public List<ApplicationList> GetApplications(int userId,string identifier)
        {
            // gets the list of applications
            List<ApplicationList> obj = new List<ApplicationList>();

            SqlParameter[] parameters =
                                  {
                                    new SqlParameter("@UserID", SqlDbType.NVarChar, 255) { Value = userId}
                                  };

            DataTable dtApplications = _helper.GetDataTable("[dbo].[GetApplications]", parameters);
            if (dtApplications.Rows.Count > 0)
            {
                if (identifier == "Drop Down")
                {
                    //get only ICP/GPA/Field Work applications
                    obj = dtApplications.AsEnumerable().Where(row => row.Field<long>("ID") == 1 || row.Field<long>("ID") == 2 || row.Field<long>("ID") == 4)
                                                       .Select(row =>
                                                             new ApplicationList
                                                             {
                                                                 ApplicationId = Convert.ToInt32(row["ID"]),
                                                                 ApplicationName = Convert.ToString(row["Name"])
                                                             }).ToList();
                }
                else
                {
                    //get only ICP/GPA/Doctoral applications
                    obj = dtApplications.AsEnumerable().Where(row => row.Field<long>("ID") == 1 || row.Field<long>("ID") == 2 || row.Field<long>("ID") == 3)
                                                       .Select(row =>
                                                             new ApplicationList
                                                             {
                                                                 ApplicationId = Convert.ToInt32(row["ID"]),
                                                                 ApplicationName = Convert.ToString(row["Name"])
                                                             }).ToList();
                }
            }

            return obj;
        }
        public SemesterTermListResponse GetSemesterList(int applicationId)
        {
            SemesterTermListResponse obj = new SemesterTermListResponse();


            SqlParameter[] parameters = { };

            DataTable dtSemesters = _helper.GetDataTable("[dbo].[GetSemesterList]", parameters);
            try
            {
                if (dtSemesters.Rows.Count > 0)
                {


                    obj.SemesterTerms = dtSemesters.AsEnumerable().Where(row => row.Field<long>("ApplicationTypeID") == applicationId)
                                                                  .Select(row =>
                                                                              new SemesterTerm
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
        public ApplicationProgramListResponse GetApplicationProgramList(int userID, int applicationTypeID, string termCode)
        {
            ApplicationProgramListResponse obj = new ApplicationProgramListResponse();


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


                    obj.ApplicationProgramList = dtApplicationPrograms.Tables[0].AsEnumerable().Select(row =>
                                              new ApplicationProgramList
                                              {
                                                  programID = Convert.ToInt32(row["ID"]),
                                                  programName = Convert.ToString(row["Name"]),
                                                  semester = Convert.ToString(row["Semester"]),
                                                  TermCode = Convert.ToString(row["TermCode"])
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
        public StudentAppliedFormsByProgramsResponse GetStudentAppliedFormsByPrograms(int programID, string termCode, string CSULBID,string identifier)
        {
            StudentAppliedFormsByProgramsResponse obj = new StudentAppliedFormsByProgramsResponse();
            

            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@ProgramID", SqlDbType.Int, 50) { Value = programID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar, 10) { Value = termCode },
                                          new SqlParameter("@CSULBID", SqlDbType.NVarChar, 25) { Value = CSULBID },
                                          new SqlParameter("@Identifier", SqlDbType.NVarChar, 25) { Value = identifier }
                                        };

            DataTable dsStudentAppliedFormsByProgram = _helper.GetDataTable("[dbo].[StudentProfileSearch]", parameters);
            try
            { 

                    if (dsStudentAppliedFormsByProgram.Rows.Count > 0)
                    {
                        obj.StudentAppliedFormsByPrograms = dsStudentAppliedFormsByProgram.AsEnumerable().Select(row =>
                                              new StudentAppliedFormsByPrograms
                                              {
                                                  FormID = Convert.ToInt32(row["ID"]),
                                                  StudentFirstName = Convert.ToString(row["FirstName"]),
                                                  StudentLastName = Convert.ToString(row["LastName"]),
                                                  Email = Convert.ToString(row["EMAIL"]),
                                                  UserID = Convert.ToInt32(row["UserID"]),
                                                  ProgramID = Convert.ToInt32(row["ProgramID"]),
                                                  ProgramName = Convert.ToString(row["ProgramName"]),
                                                  Semester = Convert.ToString(row["Term"]),
                                                  TermCode = Convert.ToString(row["TermCode"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),
                                                  ApplicationTypeName = Convert.ToString(row["Type"]),
                                                  ApplicantTypeID = Convert.ToInt32(row["ApplicationTypeID"]),
                                                  Status = Convert.ToString(row["Status"]),
                                                  DOB = Convert.ToDateTime(row["DOB"] == DBNull.Value ? null : row["DOB"]),
                                                  SSN = Convert.ToString(row["SSN"] == DBNull.Value ? null : row["SSN"])
                                              }).ToList();
                        foreach (var studentForm in obj.StudentAppliedFormsByPrograms)
                        {
                            if (!string.IsNullOrEmpty(studentForm.SSN) && studentForm.SSN != null)
                            {
                                studentForm.SSN = DecryptSSNNumber(studentForm.SSN);
                            }
                        }
                        int SSNtimeoutInSeconds = Convert.ToInt32(_configuration["ApplicationKeys:SSNTimeout"]);
                        int SSNtimeoutInMilliseconds = SSNtimeoutInSeconds * 1000;
                        obj.SSNSessionTimeOut = SSNtimeoutInMilliseconds;
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
        public BaseResponse SaveStudentAggrement(SaveStudentAggrementRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@csulbId", SqlDbType.VarChar,9) { Value = input.CSULBID  },
                                          new SqlParameter("@IsAgreed", SqlDbType.Bit) { Value = input.IsAgreed },
                                          new SqlParameter("@AgreedDate", SqlDbType.DateTime) { Value = input.AgreedDate }
                                        };

            int id = _helper.InsertTable("[dbo].[SaveStudentAggrement]", parameters);
            response.Message = "Student Aggrement saved successfully";
            response.IsSuccess = true;
            return response;
        }
        public UpsertProfileAttachmentResponse UpsertProfileAttachment(UpsertProfileDocumentRequest input)
        {
            UpsertProfileAttachmentResponse response = new UpsertProfileAttachmentResponse();
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
                    AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.FileName);
                    fileExtension = fileDetails.FileExtension;
                    fileName = input.FileName;
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
                    //if (fileExtension.ToUpper() == "DOC" || fileExtension.ToUpper() == "DOCX")
                    //{
                    //    isNotPDFExtension = true;
                    //    bool isFileSaved = SaveWordFileInTempFolder(input.FileContent, fileName, fileExtension, workingFolderPath);
                    //    fileExtensionWord = fileExtension;
                    //    fileExtension = "pdf";
                    //}


                }

                SqlParameter[] parameters =
                                         {
                                          new SqlParameter("@UniqueID", SqlDbType.UniqueIdentifier) { Value = input.UniqueID },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 200) { Value = fileName },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@CSULBID", SqlDbType.NVarChar, 20) { Value = input.CSULBID},
                                          new SqlParameter("@CreatedBy", SqlDbType.BigInt) { Value = input.CreatedBy }
                                          //new SqlParameter("@ProfileAttachmentComments", SqlDbType.NVarChar, -1) { Value = input.ProfileAttachmentComments}
                                        };
                DataTable dtFormAttachment = _helper.GetDataTable("[dbo].[UpsertProfileAttachment]", parameters);
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
                            if (isNotPDFExtension)
                            {
                                byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileName + "." + fileExtensionWord), Path.Combine(dirForm, fileName + "." + fileExtension));
                            }
                            else
                            {
                                File.WriteAllBytes(Path.Combine(dirForm, fileName + "." + fileExtension), input.FileContent);
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(dirForm);
                            if (isNotPDFExtension)
                            {
                                byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileName + "." + fileExtensionWord), Path.Combine(dirForm, fileName + "." + fileExtension));
                            }
                            else
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
                        if (isNotPDFExtension)
                        {
                            byte[] inputStr = word2PDF(Path.Combine(workingFolderPath, fileName + "." + fileExtensionWord), Path.Combine(dirForm, fileName + "." + fileExtension));
                        }
                        else
                        {
                            File.WriteAllBytes(Path.Combine(dirForm, fileName + "." + fileExtension), input.FileContent);
                        }
                    }
                    response.FileName = Convert.ToString(dtFormAttachment.Rows[0]["FileName"]);
                    response.FileExtn = Convert.ToString(dtFormAttachment.Rows[0]["FileExtn"]);
                    response.UniqueID = (Guid)(dtFormAttachment.Rows[0]["UniqueID"]);
                    response.ProfileDocumentID = Convert.ToInt32(dtFormAttachment.Rows[0]["ProfileDocumentID"]);
                    response.UserID = Convert.ToInt32(dtFormAttachment.Rows[0]["UserID"]);
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
        public DownloadProfileAttachmentResponse DownloadProfileAttachment(Guid UniqueID)
        {
            DownloadProfileAttachmentResponse obj = new DownloadProfileAttachmentResponse();

            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@UniqueID", SqlDbType.UniqueIdentifier) { Value = UniqueID }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[dbo].[GetProfileAttachment]", parameters);
            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new DownloadProfileAttachmentResponse
                                          {
                                              ProfileDocumentID = Convert.ToInt32(row["ProfileDocumentID"]),
                                              UserID = Convert.ToInt32(row["UserID"]),
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
        public ProfileAttachmentDetailsResponse GetProfileAttachmentDetails(string CSULBID)
        {
            ProfileAttachmentDetailsResponse obj = new ProfileAttachmentDetailsResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@CSULBID", SqlDbType.VarChar,20) { Value = CSULBID }
                                     };
            DataTable dtProfileAttachmentDetails = _helper.GetDataTable("[dbo].[GetProfileAttachmentDetails]", parameters);
            try
            {
                if (dtProfileAttachmentDetails.Rows.Count > 0)
                {
                    obj.ProfileAttachmentDetails = dtProfileAttachmentDetails.AsEnumerable().Select(row =>
                                              new ProfileAttachmentDetails
                                              {
                                                  ProfileDocumentID = Convert.ToInt32(row["ProfileDocumentID"]),
                                                  UserID = Convert.ToInt32(row["UserID"]),
                                                  FileName = Convert.ToString(row["FileName"] == DBNull.Value ? null : row["FileName"]),
                                                  FileExtn = Convert.ToString(row["FileExtn"] == DBNull.Value ? null : row["FileExtn"]),
                                                  FolderName = Convert.ToString(row["FolderName"] == DBNull.Value ? null : row["FolderName"]),
                                                  CreatedBy = Convert.ToInt32(row["CreatedBy"] == DBNull.Value ? null : row["CreatedBy"]),
                                                  CreatedDate = Convert.ToDateTime(row["CreatedDate"] == DBNull.Value ? null : row["CreatedDate"]),
                                                  CanView = Convert.ToString(row["CanView"]),
                                                  UniqueID = Guid.Parse(row["UniqueID"].ToString()),
                                                  CreatedByUsername = Convert.ToString(row["CreatedByUsername"])
                                              }).ToList();


                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                obj.IsSuccess = true;
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }
        public BaseResponse DeleteProfileAttachment(DeleteProfileAttachmentRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                    {
                                           new SqlParameter("@UniqueID", SqlDbType.UniqueIdentifier) { Value = input.UniqueID }
                                     };
            try
            {
                DataTable dtAttachment = _helper.GetDataTable("[dbo].[DeleteProfileAttachment]", parameters);
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
        private bool SaveWordFileInTempFolder(byte[] fileContent, string fileName, string fileExtension, string workingFolderPath)
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
        private string EncryptSSNNumber(string clearText)
        {
            string encryptionKey = _configuration["ApplicationKeys:EncryptionKey"];
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return clearText;
        }
        private string DecryptSSNNumber(string cipherText)
        {
            string encryptionKey = _configuration["ApplicationKeys:EncryptionKey"];
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
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
        public BaseResponse SaveStudentProfilePersonalInfoData(SaveStudentProfilePersonalInfoDataRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@CSULBID", SqlDbType.VarChar,9) { Value = input.CSULBID  },
                                          new SqlParameter("@BachelorDegreeMajorSP", SqlDbType.NVarChar,255) { Value = input.BachelorDegreeMajorSP  },
                                          new SqlParameter("@CredentialProgram", SqlDbType.NVarChar,255) { Value = input.CredentialProgram },
                                          new SqlParameter("@CredentialPathway", SqlDbType.NVarChar,255) { Value = input.CredentialPathway },
                                          new SqlParameter("@Certificate", SqlDbType.NVarChar,255) { Value = input.Certificate },
                                        };

            int id = _helper.InsertTable("[dbo].[SaveStudentProfilePersonalInfo]", parameters);
            response.Message = "Student profile data saved successfully";
            response.IsSuccess = true;
            return response;
        }
        public ProgramPlannerCourseListResponse GetProgramPlannerCourseList(string CSULBID, int ProgramID, string TermCode)
        {
            ProgramPlannerCourseListResponse obj = new ProgramPlannerCourseListResponse();
            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@StudentID", SqlDbType.VarChar,20) { Value = CSULBID },
                                          new SqlParameter("@ProgramID", SqlDbType.Int) { Value = ProgramID },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar,20) { Value = TermCode }
                                     };
            DataSet dtProgramPlannerCourseList = _helper.GetDataSet("[dbo].[GetProgramPlannerCourses]", parameters);
            try
            {
                if (dtProgramPlannerCourseList.Tables.Count > 0)
                {
                    obj.ProgramPlannerCourseList =
                    dtProgramPlannerCourseList.Tables.Count > 0
                    ? dtProgramPlannerCourseList.Tables[0]
                        .AsEnumerable()
                        .Select(row => new ProgramPlannerCourseList
                        {
                            MasterID = row["MasterID"] == DBNull.Value ? 0 : Convert.ToInt32(row["MasterID"]),
                            DetailID = row["DetailID"] == DBNull.Value ? 0 : Convert.ToInt32(row["DetailID"]),
                            CourseName = row["CourseName"] == DBNull.Value ? null : Convert.ToString(row["CourseName"]),
                            TermCode = row["TermCode"] == DBNull.Value ? null : Convert.ToString(row["TermCode"]),
                            Term = row["Term"] == DBNull.Value ? null : Convert.ToString(row["Term"]),
                            Year = row["Year"] == DBNull.Value ? null : Convert.ToString(row["Year"]),
                            Notes = row["Notes"] == DBNull.Value ? null : Convert.ToString(row["Notes"]),
                            CSULBID = row["StudentID"] == DBNull.Value ? null : Convert.ToString(row["StudentID"])
                        })
                        .ToList()
                    : new List<ProgramPlannerCourseList>();


                    obj.StateHandler =
                        dtProgramPlannerCourseList.Tables.Count > 1 &&
                        dtProgramPlannerCourseList.Tables[1].Rows.Count > 0 &&
                        dtProgramPlannerCourseList.Tables[1].Columns.Contains("StateID")
                        ? dtProgramPlannerCourseList.Tables[1]
                            .AsEnumerable()
                            .Select(row => new ProgramPlannerStateHandler
                            {
                                StateID = row["StateID"] == DBNull.Value
                                            ? 0
                                            : Convert.ToInt32(row["StateID"])
                            })
                            .FirstOrDefault()
                        : new ProgramPlannerStateHandler
                        {
                            StateID = 0
                        };

                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
                obj.IsSuccess = true;
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public BaseResponse UpsertCourseDetails(string JSONString)
        {
            BaseResponse obj = new BaseResponse();
            try
            { 
                SqlParameter[] parameters =
                                   {
                                          new SqlParameter("@Json", SqlDbType.NVarChar, -1) { Value = JSONString }
                                   };
                DataTable SPCDID = _helper.GetDataTable("[dbo].[SaveStudentProfileCourseDetails]", parameters);

                if (SPCDID.Rows.Count > 0)
                {
                    obj.IsSuccess = true;
                    obj.Message = "Data updated Successfully";
                }
                else
                {
                    obj.IsSuccess = false;
                    obj.Message = "Failed to save data";
                }
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data update Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
                _logger.LogInformation("Error Message : " + ex.Message + " Stack Trace : " + ex.StackTrace);
            }

            return obj;
        }
    }
}
