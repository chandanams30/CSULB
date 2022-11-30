using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
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
        public InitialCredentialProgramService(ISqlDBUtility helper
                                         , IConfiguration configuration
                                         , ISendMail sendMail
                                         , ILogger<InitialCredentialProgramService> logger)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
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
    }
}
