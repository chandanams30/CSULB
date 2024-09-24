using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Request.Interviews;
using ThoughtFocus.Domain.Request.Rubrics;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Interviews;
using ThoughtFocus.Domain.Response.Milestones;
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
        public InterviewBasicDetailsResponse UpsertInterview(UpsertInterview input)
        {
            InterviewBasicDetailsResponse response = new InterviewBasicDetailsResponse();
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
                response.interviewBasicDetails = dtInterviews.AsEnumerable().Select(row=>
                                            new InterviewBasicDetails
                                            {
                                                InterviewId = Convert.ToInt32(row["Id"]),
                                                InterviewName = Convert.ToString(row["InterviewName"]),
                                                InterviewDescription = Convert.ToString(row["InterviewDescription"]),
                                                CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                                                SemesterTerm = Convert.ToString(row["TermCode"]),
                                                IsActive = Convert.ToBoolean(row["IsActive"]),
                                                ApplicationProgramID = Convert.ToInt32(row["ProgramId"]),
                                                InterviewFor = Convert.ToInt32(row["InterviewFor"]),
                                                Status = Convert.ToString(row["Status"]),
                                            }).FirstOrDefault();

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
        public InterviewDetails GetInterviewDetails(int InterviewId,int UserId)
        {
            InterviewDetails obj = new InterviewDetails();
            SqlParameter[] parameters = { 
                                           new SqlParameter("@InterviewId", SqlDbType.BigInt) { Value = InterviewId },
                                           new SqlParameter("@UserId", SqlDbType.BigInt) { Value = UserId }
                                        };
            DataSet dtInterviewDetails = _helper.GetDataSet("[Interview].[InterviewDetails]", parameters);
            try
            {
                if (dtInterviewDetails.Tables[0].Rows.Count > 0 && dtInterviewDetails.Tables[1].Rows.Count > 0)
                {
                    BasicDetails objBD = dtInterviewDetails.Tables[0].AsEnumerable().Select(row =>
                                           new BasicDetails
                                           {
                                               InterviewId = Convert.ToInt32(row["InterviewId"]),
                                               InterviewName = Convert.ToString(row["InterviewName"]),
                                               InterviewDescription = Convert.ToString(row["InterviewDescription"]),
                                               ProgramName = Convert.ToString(row["ProgramName"]),
                                               Semester = Convert.ToString(row["Semester"]),
                                               ApplicationProgramID = Convert.ToInt32(row["ProgramID"]),
                                               SemesterTerm = Convert.ToString(row["TermCode"]),
                                               ApplicationTypeID = Convert.ToInt32(row["ApplicationTypeID"]),
                                           }).FirstOrDefault();

                    obj.basicDetails = objBD;
                    if (dtInterviewDetails.Tables[0].Rows[0]["InterviewSlotId"] != DBNull.Value)
                    {
                        obj.slotsList = dtInterviewDetails.Tables[0].AsEnumerable().Select(row =>
                                            new SlotsList
                                            {
                                                InterviewSlotId = row["InterviewSlotId"] == DBNull.Value ? 0 : Convert.ToInt32(row["InterviewSlotId"]),
                                                InterviewDate = row["InterviewDate"] == DBNull.Value ? null : Convert.ToDateTime(row["InterviewDate"]).ToString("MM-dd-yyyy"),
                                                StartTime = Convert.ToString(row["StartTime"] == DBNull.Value ? null : row["StartTime"]),
                                                EndTime = Convert.ToString(row["EndTime"] == DBNull.Value ? null : row["EndTime"]),
                                                InterviewerName = Convert.ToString(row["InterviewerName"]),
                                                StudentName = Convert.ToString(row["StudentName"]),
                                                Status = Convert.ToString(row["Status"]),
                                                InterviewComments = Convert.ToString(row["InterviewerComments"]),
                                                InterviewLocation = Convert.ToString(row["InterviewLocation"]),
                                                InterviewLink = Convert.ToString(row["InterviewLink"]),
                                                Interviewer = row["Interviewers"] == DBNull.Value ? 0 : Convert.ToInt32(row["Interviewers"]),
                                                Student = row["Students"] == DBNull.Value ? 0 : Convert.ToInt32(row["Students"]),
                                            }).ToList();

                        obj.interviewSateHandler = dtInterviewDetails.Tables[1].AsEnumerable().Select(row =>
                                                   new InterviewSateHandler
                                                   {
                                                       StateHandler = Convert.ToString(row["InterviewStateHandler"])
                                                   }).FirstOrDefault();
                    }
                    else
                    {
                        obj.slotsList = null;
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
        public InterviewSlotsList UpsertInterviewSlots(UpsertInterviewSlots input)
        {
            InterviewSlotsList response = new InterviewSlotsList();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@InterviewId", SqlDbType.BigInt) { Value = input.InterviewId},
                                          new SqlParameter("@InterviewDate", SqlDbType.Date) { Value = input.InterviewDate },
                                          new SqlParameter("@StartTime", SqlDbType.Time) { Value = input.StartTime },
                                          new SqlParameter("@EndTime", SqlDbType.Time) { Value = input.EndTime },
                                          new SqlParameter("@CreatedBy", SqlDbType.BigInt) { Value = input.CreatedBy },
                                          new SqlParameter("@Interviewers", SqlDbType.BigInt) { Value = input.Interviewer },
                                          new SqlParameter("@Students", SqlDbType.BigInt) { Value = input.Student },
                                          new SqlParameter("@Status", SqlDbType.NVarChar ,50) { Value = input.Status },
                                          new SqlParameter("@InterviewComments", SqlDbType.NVarChar, -1) { Value = input.InterviewComments },
                                          new SqlParameter("@InterviewLocation", SqlDbType.NVarChar, 50) { Value = input.InterviewLocation },
                                          new SqlParameter("@InterviewLink", SqlDbType.NVarChar, -1) { Value = input.InterviewLink },
                                          new SqlParameter("@InterviewSlotId", SqlDbType.BigInt) { Value = input.InterviewSlotId }
                                       };

            DataSet dtInterviewsSlots = _helper.GetDataSet("[Interview].[InsertInterviewSlots]", parameters);
            if (dtInterviewsSlots.Tables[0].Rows.Count > 0 && dtInterviewsSlots.Tables[1].Rows.Count > 0)
            {
                response.interviewSlotsList = dtInterviewsSlots.Tables[1].AsEnumerable().Select(row =>
                                            new InterviewSlots
                                            {
                                                InterviewSlotId = row["Id"] == DBNull.Value ? 0 : Convert.ToInt32(row["Id"]),
                                                InterviewDate = row["InterviewDate"] == DBNull.Value ? null : Convert.ToDateTime(row["InterviewDate"]).ToString("MM-dd-yyyy"),
                                                StartTime = Convert.ToString(row["StartTime"] == DBNull.Value ? null : row["StartTime"]),
                                                EndTime = Convert.ToString(row["EndTime"] == DBNull.Value ? null : row["EndTime"]),
                                                Status = Convert.ToString(row["Status"]),
                                                InterviewComments = Convert.ToString(row["InterviewerComments"]),
                                                InterviewLocation = Convert.ToString(row["InterviewLocation"]),
                                                InterviewLink = Convert.ToString(row["InterviewLink"]),
                                                Interviewer = row["Interviewers"] == DBNull.Value ? 0 : Convert.ToInt32(row["Interviewers"]),
                                                Student = row["Students"] == DBNull.Value ? 0 : Convert.ToInt32(row["Students"]),
                                            }).ToList();

                //notify student
                string email = string.Empty;
                string interviewName = string.Empty;
                string programName = string.Empty;
                string termName = string.Empty;
                string studentName = string.Empty;
                string link = string.Empty;
                string interviewStartDate = string.Empty;
                string startTime = string.Empty;
                string endTime = string.Empty;
                string body = string.Empty;
                string logoText = "cid:myImageID";
                string interviewLocation = string.Empty;
                if (dtInterviewsSlots.Tables[0].Columns.Contains("Email"))
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dtInterviewsSlots.Tables[0].Rows[0]["Email"])))
                    {
                        email = Convert.ToString(dtInterviewsSlots.Tables[0].Rows[0]["Email"]);
                        interviewName = Convert.ToString(dtInterviewsSlots.Tables[0].Rows[0]["InterviewName"]);
                        studentName = Convert.ToString(dtInterviewsSlots.Tables[0].Rows[0]["StudentName"]);
                        programName = Convert.ToString(dtInterviewsSlots.Tables[0].Rows[0]["ProgramName"]);
                        termName = Convert.ToString(dtInterviewsSlots.Tables[0].Rows[0]["Semester"]);
                        DateTime interviewDate = (DateTime)dtInterviewsSlots.Tables[0].Rows[0]["InterviewDate"];
                        interviewStartDate = interviewDate.ToString("MM-dd-yyyy");
                        TimeSpan startTimeSpan = (TimeSpan)dtInterviewsSlots.Tables[0].Rows[0]["StartTime"];
                        TimeSpan endTimeSpan = (TimeSpan)dtInterviewsSlots.Tables[0].Rows[0]["EndTime"];
                        startTime = DateTime.Today.Add(startTimeSpan).ToString("h:mmtt").ToUpper();
                        endTime = DateTime.Today.Add(endTimeSpan).ToString("h:mmtt").ToUpper();
                        link = Convert.ToString(dtInterviewsSlots.Tables[0].Rows[0]["InterviewLink"]);
                        interviewLocation = Convert.ToString(dtInterviewsSlots.Tables[0].Rows[0]["InterviewLocation"]);
                        if (interviewLocation == "Online")
                        {
                            body = GetMailBodyTemplate("Online_Interview__MailTemplate.html");
                        }
                        else if (interviewLocation == "Offline")
                        {
                            body = GetMailBodyTemplate("Offline_Interview_MailTemplate.html");
                        }
                        body = body.Replace("[[logoPath]]", logoText)
                                   .Replace("[[interviewName]]", interviewName)
                                   .Replace("[[studentName]]", studentName)
                                   .Replace("[[programName]]", programName)
                                   .Replace("[[termName]]", termName)
                                   .Replace("[[link]]", link)
                                   .Replace("[[interviewStartDate]]", interviewStartDate)
                                   .Replace("[[interviewStartTime]]", startTime)
                                   .Replace("[[interviewEndTime]]", endTime);
                        string subject = "Interview Details";
                        _sendMail.SendEmail(email, "", "COMMON", subject, body, "");
                    }
                }

                if (Convert.ToString(dtInterviewsSlots.Tables[0].Rows[0]["RESULT"]) == "SUCCESS")
                {
                    response.Message = "Interview Created Successfully";
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtInterviewsSlots.Tables[0].Rows[0]["RESULT"]) == "FAILURE")
                {
                    response.Message = "Failed to Create Interview";
                    response.IsSuccess = true;
                }
                else
                {
                    response.AlertMessage = Convert.ToString(dtInterviewsSlots.Tables[0].Rows[0]["RESULT"]);
                    response.IsSuccess = true;
                }
            }
            else
            {
                response.IsSuccess = false;
                response.Message = "No Data Present";
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
                                               InterviewName = Convert.ToString(row["InterviewName"]),

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
        public InterviewersList GetInterviewerList()
        {
            InterviewersList obj = new InterviewersList();
            SqlParameter[] parameters = {};
            DataTable dtInterviewerList = _helper.GetDataTable("[Interview].[GetInterviewerList]", parameters);
            try
            {
                if (dtInterviewerList.Rows.Count > 0)
                {
                    obj.interviewersList = dtInterviewerList.AsEnumerable().Select(row =>
                                           new InterviewersListResponse
                                           {
                                               UserId = Convert.ToInt32(row["UserId"]),
                                               DisplayName = Convert.ToString(row["DisplayName"])
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
        public BaseResponse UpdateInterviewStudentAction(InterviewStudentAction input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@Status", SqlDbType.NVarChar, 50) { Value = input.Status },
                                          new SqlParameter("@InterviewSlotId", SqlDbType.BigInt) { Value = input.InterviewSlotId }
                                       };

            DataTable dtInterviewResponse = _helper.GetDataTable("[Interview].[InterviewStudentAction]", parameters);
            if (dtInterviewResponse.Rows.Count > 0)
            {
                if (Convert.ToString(dtInterviewResponse.Rows[0]["RESULT"]) == "SUCCESS")
                {
                    response.Message = "Interview " + input.Status + "ed" + " Successfully";
                    response.IsSuccess = true;
                }
                else if (Convert.ToString(dtInterviewResponse.Rows[0]["RESULT"]) == "FAILURE")
                {
                    response.Message = "Failed to" + input.Status + "Interview";
                    response.IsSuccess = true;
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

    }
}
