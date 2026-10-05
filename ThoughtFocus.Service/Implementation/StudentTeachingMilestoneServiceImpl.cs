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
    }
}
