using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
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

        public StudentTeachingMilestonesResponse GetStudentTeachingMilestoneDetails(string CsuldId, int UserID, int formID, int roleID)
        {
            StudentTeachingMilestonesResponse obj = new StudentTeachingMilestonesResponse();
            
            SqlParameter[] parameters = {
                                          new SqlParameter("@csulbid", SqlDbType.VarChar, 50) { Value = CsuldId },
                                          new SqlParameter("@UserID", SqlDbType.VarChar, 50) { Value = UserID },
                                          new SqlParameter("@FormID", SqlDbType.NVarChar, -1) { Value = formID },
                                          new SqlParameter("@LoggedInRole", SqlDbType.NVarChar, -1) { Value = roleID }
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
                        ProgramStatusDesc = Convert.ToString(row["ProgramStatusDesc"])

                    }).FirstOrDefault();

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

    }
}
