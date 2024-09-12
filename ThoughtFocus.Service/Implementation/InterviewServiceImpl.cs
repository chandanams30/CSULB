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
using ThoughtFocus.Domain.Request.Interviews;
using ThoughtFocus.Domain.Request.Rubrics;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Interviews;
using ThoughtFocus.Domain.Response.Rubrics;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class InterviewServiceImpl : IInterviewService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        public ILogger<InterviewServiceImpl> _logger;
        private readonly ICommonUtils _utils;
        public InterviewServiceImpl(ISqlDBUtility helper
                                         , IConfiguration configuration
                                         , ISendMail sendMail
                                         , ILogger<InterviewServiceImpl> logger
                                         , ICommonUtils utils)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
            _utils = utils;
        }
        public BaseResponse UpsertInterview(UpsertInterview input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@InterviewName", SqlDbType.NVarChar, 50) { Value = input.InterviewName },
                                          new SqlParameter("@InterviewDescription", SqlDbType.NVarChar, -1) { Value = input.InterviewDescription },
                                          new SqlParameter("@CreatedBy", SqlDbType.BigInt) { Value = input.CreatedBy },
                                          new SqlParameter("@IsActive", SqlDbType.Bit) { Value = input.IsActive },
                                          new SqlParameter("@ProgramId", SqlDbType.BigInt) { Value = input.ProgramId },
                                          new SqlParameter("@TermCode", SqlDbType.VarChar,10) { Value = input.TermCode },
                                          new SqlParameter("@InterviewFor", SqlDbType.BigInt) { Value = input.InterviewFor },
                                          new SqlParameter("@InterviewId", SqlDbType.BigInt) { Value = input.InterviewId },
                                          new SqlParameter("@Status", SqlDbType.NVarChar, 50) { Value = input.Status }
                                       };

            DataTable dtInterviews = _helper.GetDataTable("[Interview].[InsertInterview]", parameters);
            if (dtInterviews.Rows.Count > 0)
            {

                if (Convert.ToString(dtInterviews.Rows[0]["RESULT"]) == "SUCCESS")
                {
                    response.Message = "Interview Created Successfully";
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtInterviews.Rows[0]["RESULT"]) == "FAILURE")
                {
                    response.Message = "Failed to Create Interview";
                    response.IsSuccess = true;
                }
            }
            return response;
        }
        public InterviewDetails GetInterviewDetails(int InterviewId)
        {
            InterviewDetails obj = new InterviewDetails();
            SqlParameter[] parameters = { 
                                           new SqlParameter("@InterviewId", SqlDbType.BigInt) { Value = InterviewId }
                                        };
            DataTable dtInterviewDetails = _helper.GetDataTable("[Interview].[InterviewDetails]", parameters);
            try
            {
                if (dtInterviewDetails.Rows.Count > 0)
                {
                    obj.interviewDetailsResponse = dtInterviewDetails.AsEnumerable().Select(row =>
                                           new InterviewDetailsResponse
                                           {
                                               InterviewId = Convert.ToInt32(row["InterviewId"]),
                                               InterviewName = Convert.ToString(row["InterviewName"]),
                                               InterviewDescription = Convert.ToString(row["InterviewDescription"]),
                                               ProgramName = Convert.ToString(row["ProgramName"]),
                                               Semester = Convert.ToString(row["Semester"]),
                                               InterviewStart = Convert.ToDateTime(row["InterviewStart"] == DBNull.Value ? null : row["InterviewStart"]),
                                               InterviewEnd = Convert.ToDateTime(row["InterviewEnd"] == DBNull.Value ? null : row["InterviewEnd"]),
                                               InterviewerName = Convert.ToString(row["InterviewerName"]),
                                               StudentName = Convert.ToString(row["StudentName"]),
                                               Status = Convert.ToString(row["Status"]),
                                               InterviewerComments = Convert.ToString(row["InterviewerComments"]),
                                               InterviewLocation = Convert.ToString(row["InterviewLocation"]),
                                               InterviewLink = Convert.ToString(row["InterviewLink"]),

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
        public BaseResponse UpsertInterviewSlots(UpsertInterviewSlots input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@InterviewId", SqlDbType.BigInt) { Value = input.InterviewId},
                                          new SqlParameter("@InterviewStart", SqlDbType.DateTime) { Value = input.InterviewStart },
                                          new SqlParameter("@InterviewEnd", SqlDbType.DateTime) { Value = input.InterviewEnd },
                                          new SqlParameter("@CreatedBy", SqlDbType.BigInt) { Value = input.CreatedBy },
                                          new SqlParameter("@Interviewers", SqlDbType.BigInt) { Value = input.Interviewers },
                                          new SqlParameter("@Students", SqlDbType.BigInt) { Value = input.Students },
                                          new SqlParameter("@Status", SqlDbType.NVarChar ,50) { Value = input.Status },
                                          new SqlParameter("@InterviewComments", SqlDbType.NVarChar, -1) { Value = input.InterviewComments },
                                          new SqlParameter("@InterviewLocation", SqlDbType.NVarChar, 50) { Value = input.InterviewLocation },
                                          new SqlParameter("@InterviewLink", SqlDbType.NVarChar, -1) { Value = input.InterviewLink },
                                          new SqlParameter("@InterviewSlotId", SqlDbType.BigInt) { Value = input.InterviewSlotId }
                                       };

            DataTable dtInterviewsSlots = _helper.GetDataTable("[Interview].[InsertInterviewSlots]", parameters);
            if (dtInterviewsSlots.Rows.Count > 0)
            {

                if (Convert.ToString(dtInterviewsSlots.Rows[0]["RESULT"]) == "SUCCESS")
                {
                    response.Message = "Interview Created Successfully";
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtInterviewsSlots.Rows[0]["RESULT"]) == "FAILURE")
                {
                    response.Message = "Failed to Create Interview";
                    response.IsSuccess = true;
                }
            }
            return response;
        }
        public InterviewList GetInterviewList(int UserId)
        {
            InterviewList obj = new InterviewList   ();
            SqlParameter[] parameters = {
                                           new SqlParameter("@UserId", SqlDbType.BigInt) { Value = UserId }
                                        };
            DataTable dtInterviewList = _helper.GetDataTable("[Interview].[InterviewList]", parameters);
            try
            {
                if (dtInterviewList.Rows.Count > 0)
                {
                    obj.interviewListResponse = dtInterviewList.AsEnumerable().Select(row =>
                                           new InterviewListResponse
                                           {
                                               InterviewId = Convert.ToInt32(row["InterviewId"]),
                                               InterviewDescription = Convert.ToString(row["InterviewDescription"]),
                                               ProgramName = Convert.ToString(row["ProgramName"]),
                                               Semester = Convert.ToString(row["Semester"]),
                                               InterviewName = Convert.ToString(row["DisplayName"]),

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
