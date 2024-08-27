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
using ThoughtFocus.Domain.Request.Milestones;
using ThoughtFocus.Domain.Request.Rubrics;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Milestones;
using ThoughtFocus.Domain.Response.Rubrics;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class RubricsServiceImpl : IRubricsService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        public ILogger<RubricsServiceImpl> _logger;
        private readonly ICommonUtils _utils;
        public RubricsServiceImpl(ISqlDBUtility helper
                                         , IConfiguration configuration
                                         , ISendMail sendMail
                                         , ILogger<RubricsServiceImpl> logger
                                         , ICommonUtils utils)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
            _utils = utils;
        }
        public RubricsTemplateListResponse GetRubricsTemplateList()
        {
            RubricsTemplateListResponse obj = new RubricsTemplateListResponse();
            SqlParameter[] parameters = { };
            DataTable dtRubricsTemplateList = _helper.GetDataTable("[Rubrics].[GetRubricsTemplates]", parameters);
            try
            {
                if (dtRubricsTemplateList.Rows.Count > 0)
                {
                    obj.RubricsTemplateList = dtRubricsTemplateList.AsEnumerable().Select(row =>
                                           new RubricsTemplateList
                                           {
                                               ID = Convert.ToInt32(row["ID"]),
                                               TemplateName = Convert.ToString(row["TemplateName"]),
                                               TemplateDescription = Convert.ToString(row["TemplateDescription"]),
                                               IsReadytoPublish = Convert.ToBoolean(row["IsReadytoPublish"]),
                                               TotalPoints = Convert.ToInt32(row["TotalPoints"]),
                                               CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                                               CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                                               IsPublished = Convert.ToBoolean(row["IsPublished"])
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
        public BaseResponse UpsertRubricsTemplate(UpsertRubricsTemplateRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@ID", SqlDbType.BigInt) { Value = input.ID },
                                          new SqlParameter("@TemplateName", SqlDbType.NVarChar, 50) { Value = input.TemplateName },
                                          new SqlParameter("@TemplateDescription", SqlDbType.NVarChar, -1) { Value = input.TemplateDescription },
                                          new SqlParameter("@TemplateForm", SqlDbType.NVarChar, -1) { Value = input.TemplateForm },
                                          new SqlParameter("@IsReadytoPublish", SqlDbType.Bit) { Value = input.IsReadytoPublish },
                                          new SqlParameter("@TotalPoints", SqlDbType.Int) { Value = input.TotalPoints },
                                          new SqlParameter("@CreatedBy", SqlDbType.BigInt) { Value = input.CreatedBy }
                                       };

            DataTable dtRubrics = _helper.GetDataTable("[Rubrics].[InsertRubricsTemplates]", parameters);
            if (dtRubrics.Rows.Count > 0)
            {

                if (Convert.ToString(dtRubrics.Rows[0]["RESULT"]) == "SUCCESS")
                {
                    response.Message = "Rubrics Template Added Successfully";
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtRubrics.Rows[0]["RESULT"]) == "FAILURE")
                {
                    response.Message = "Failed to Add Rubrics Template";
                    response.IsSuccess = true;
                }
            }
            return response;
        }
        public BaseResponse PublishRubricsForm(PublishRubricsFormRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {

                                          new SqlParameter("@TemplateID", SqlDbType.BigInt) { Value = input.TemplateID },
                                          new SqlParameter("TermCode", SqlDbType.VarChar,20) { Value = input.TermCode },
                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@RubricTypeID", SqlDbType.Int) { Value = input.RubricTypeID },
                                          new SqlParameter("@ReviewerTypeID", SqlDbType.Int) { Value = input.ReviewerTypeID },
                                          new SqlParameter("@CreatedBy", SqlDbType.BigInt) { Value = input.CreatedBy },
                                          new SqlParameter("@IsActive", SqlDbType.Bit) { Value = input.IsActive }
                                        };
            DataTable dtRubrics = _helper.GetDataTable("[Rubrics].[PublishRubricsTemplates]", parameters);
            if (dtRubrics.Rows.Count > 0)
            {
                if (Convert.ToString(dtRubrics.Rows[0]["RESULT"]) == "SUCCESS")
                {
                    response.Message = "Rubrics Template Published Successfully";
                    response.IsSuccess = true;
                }
               else 
                {
                    response.Message = Convert.ToString(dtRubrics.Rows[0]["RESULT"]);
                    response.IsSuccess = false;
                }
            }
            return response;
        }
        public RubricsTemplateByIDResponse GetRubricsTemplateByID(int TemplateID)
        {
            RubricsTemplateByIDResponse obj = new RubricsTemplateByIDResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@TemplateID", SqlDbType.BigInt) { Value = TemplateID }
                                        };

            DataTable dtRubricsTemplate = _helper.GetDataTable("[Rubrics].[GetRubricsTemplateByID]", parameters);
            try
            {
                if (dtRubricsTemplate.Rows.Count > 0)
                {

                    obj.RubricsTemplate = Convert.ToString(dtRubricsTemplate.Rows[0]["RubricsTemplate"]);
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
        public GetRubricSubmittedFormsListResponse GetRubricSubmittedFormsList(int UserID, int ProgramID, string TermCode)
        {
            GetRubricSubmittedFormsListResponse obj = new GetRubricSubmittedFormsListResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = ProgramID },
                                            new SqlParameter("@Termcode", SqlDbType.NChar, 50) { Value = TermCode }
                                        };

            DataTable dtRubricFormsList = _helper.GetDataTable("[Rubrics].[GetRubricSubmittedFormsList]", parameters);
            try
            {
                if (dtRubricFormsList.Rows.Count > 0)
                {
                    obj.rubricSubmittedFormsList = dtRubricFormsList.AsEnumerable().Select(row =>
                                               new GetRubricSubmittedFormsResponse
                                               {
                                                   FilledRubricID = Convert.ToInt32(row["FilledRubricID"]),
                                                   PublishRubricID = Convert.ToInt32(row["PublishRubricID"]),
                                                   CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                                                   TemplateID = Convert.ToInt32(row["TemplateID"]),
                                                   CSULBID = Convert.ToInt32(row["CSULBID"]),
                                                   StudentName = Convert.ToString(row["StudentName"]),
                                                   ProgramName = Convert.ToString(row["ProgramName"]),
                                                   TemplateName = Convert.ToString(row["TemplateName"]),
                                                   FormID = Convert.ToInt32(row["FormId"]),
                                                   TermName = Convert.ToString(row["TermName"]),
                                                   ReviewerName = Convert.ToString(row["ReviewerName"]),
                                                   ReviewerID = Convert.ToInt32(row["ReviewerID"]),
                                                   TermCode = Convert.ToString(row["TermCode"]),
                                                   State = Convert.ToString(row["Status"])
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
        public PublishedRubricsDetailsResponse GetPublishedRubricsDetails(int TemplateID)
        {
            PublishedRubricsDetailsResponse obj = new PublishedRubricsDetailsResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@ID", SqlDbType.BigInt) { Value = TemplateID }
                                        };

            DataTable dtRubricFormDetails = _helper.GetDataTable("[Rubrics].[GetPublishedRubricsDetails]", parameters);
            try
            {
                if (dtRubricFormDetails.Rows.Count > 0)
                {
                              RubricsDetails objRD = dtRubricFormDetails.AsEnumerable().Select(row =>
                                               new RubricsDetails
                                               {
                                                   TemplateName = Convert.ToString(row["TemplateName"]),
                                                   TemplateDescription = Convert.ToString(row["TemplateDescription"]),
                                                   TermName = Convert.ToString(row["TermName"]),
                                                   ProgramName = Convert.ToString(row["ProgramName"]),
                                                   TemplateForm = Convert.ToString(row["TemplateForm"]),
                                                   TotalPoints = Convert.ToInt32(row["TotalPoints"]),
                                               }).FirstOrDefault();
                               obj.rubricsDetails = objRD;
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
        public BaseResponse UpsertRubricsFilledForm(UpsertRubricsFilledFormRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {

                                          new SqlParameter("@PublishedRubricsID", SqlDbType.BigInt) { Value = input.PublishedRubricsID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@RubricForm", SqlDbType.NVarChar, -1) { Value = input.RubricForm },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@Status", SqlDbType.Int) { Value = input.Status },
                                          new SqlParameter("@filledRubricID", SqlDbType.BigInt) { Value = input.FilledRubricID }
                                        };
            DataTable dtRubrics = _helper.GetDataTable("[Rubrics].[InsertFilledRubrics]", parameters);
            if (dtRubrics.Rows.Count > 0)
            {
                if (Convert.ToString(dtRubrics.Rows[0]["RESULT"]) == "SUCCESS")
                {
                    response.Message = "Rubrics Submitted Successfully";
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtRubrics.Rows[0]["RESULT"]) == "FAILURE")
                {
                    response.Message = "Failed to Submit Rubrics";
                    response.IsSuccess = true;
                }
            }
            return response;
        }
        public RubricsApplicationFormResponse GetRubricsApplicationForm(RubricsApplicationFormRequest input)
        {
            RubricsApplicationFormResponse obj = new RubricsApplicationFormResponse();
            SqlParameter[] parameters = {
                                          new SqlParameter("@FilledRubricID", SqlDbType.BigInt) { Value = input.FilledRubricID },
                                          new SqlParameter("@PublishedRubricsID", SqlDbType.BigInt) { Value = input.PublishedRubricsID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@TemplateID", SqlDbType.BigInt) { Value = input.TemplateID },
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@CreatedBy", SqlDbType.BigInt) { Value = input.CreatedBy }
                                        };

            DataSet dsRubricFormDetails = _helper.GetDataSet("[Rubrics].[GetFilledRubricsDetails]", parameters);
            try
            {
                if (dsRubricFormDetails.Tables[0].Rows.Count > 0 && dsRubricFormDetails.Tables[1].Rows.Count > 0)
                {
                    RubricsApplicationForm objRAF = dsRubricFormDetails.Tables[0].AsEnumerable().Select(row =>
                                     new RubricsApplicationForm
                                     {
                                         FilledRubricID = Convert.ToInt32(row["ID"]),
                                         PublishedRubricID = Convert.ToInt32(row["PublishedRubricID"]),
                                         FormID = Convert.ToInt32(row["FormID"]),
                                         RubricForm = Convert.ToString(row["RubricForm"]),
                                         UserID = Convert.ToInt32(row["UserID"]),
                                         Status = Convert.ToInt32(row["Status"]),
                                         ProgramID = Convert.ToInt32(row["ProgramID"]),
                                         TermCode = Convert.ToString(row["TermCode"]),
                                         ApplicationTypeID = Convert.ToInt32(row["ApplicationTypeID"]),
                                         TemplateName = Convert.ToString(row["TemplateName"]),
                                         TemplateDescription = Convert.ToString(row["TemplateDescription"]),
                                         TotalPoints = Convert.ToInt32(row["TotalPoints"]),
                                     }).FirstOrDefault();
                    obj.rubricsApplicationForm = objRAF;

                    RubricsFormActivityHandler objRFAH = dsRubricFormDetails.Tables[1].AsEnumerable().Select(row =>
                                            new RubricsFormActivityHandler
                                            {
                                                ActivityHandler = Convert.ToString(row["RubricsFormActivityHandler"])
                                            }).FirstOrDefault();

                    obj.rubricsFormActivityHandler = objRFAH;
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
