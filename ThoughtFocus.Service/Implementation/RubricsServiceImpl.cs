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
                else if (Convert.ToString(dtRubrics.Rows[0]["RESULT"]) == "FAILURE")
                {
                    response.Message = "Failed to Publish Rubrics Template";
                    response.IsSuccess = true;
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

    }
}
