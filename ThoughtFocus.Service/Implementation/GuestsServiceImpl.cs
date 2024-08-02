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
using ThoughtFocus.Domain.Response.Guests;
using ThoughtFocus.Domain.Response.SearchApplication;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class GuestsServiceImpl : IGuestsService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        public ILogger<GuestsServiceImpl> _logger;
        public GuestsServiceImpl(ISqlDBUtility helper
                                         , IConfiguration configuration
                                         , ISendMail sendMail
                                         , ILogger<GuestsServiceImpl> logger)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
        }
        public GetMilestoneSubmittedFormsList GetMilestoneSubmittedFormsList(int UserID, int FormID, string Identifier)
        {
            GetMilestoneSubmittedFormsList obj = new GetMilestoneSubmittedFormsList();
            DataTable dtMilestoneFormsList = new DataTable();
            if (!string.IsNullOrEmpty(Identifier))
            {
                SqlParameter[] parameters = {
                                            new SqlParameter("@ExternalApprovalIdentifier", SqlDbType.UniqueIdentifier) { Value = new Guid(Identifier) }
                                            };

               dtMilestoneFormsList = _helper.GetDataTable("[Milestone].[GetMilestoneSubmittedFormsListForExternalApprovers]", parameters);
            }
            else if (UserID != 0 && FormID != 0)
            {
                SqlParameter[] parameters = {
                                                new SqlParameter("@UserId", SqlDbType.BigInt) { Value = UserID },
                                                new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID }
                                            };
                dtMilestoneFormsList = _helper.GetDataTable("[Milestone].[GetStudentMilestonesforLbad]", parameters);
            }
            try
            {
                if (dtMilestoneFormsList.Rows.Count > 0)
                {
                    var columnExists = dtMilestoneFormsList.Columns.Contains("MileStoneFormApproverExternalId");

                    obj.milestoneSubmittedFormsList = dtMilestoneFormsList.AsEnumerable().Select(row =>
                                               new GetMilestoneSubmittedForms
                                               {
                                                   MilestoneFormID = Convert.ToInt32(row["MilestoneFormID"]),
                                                   ApproverName = Convert.ToString(row["ApproverName"]),
                                                   CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                                                   State = Convert.ToString(row["State"]),
                                                   CSULBID = Convert.ToInt32(row["CSULBID"]),
                                                   StudentName = Convert.ToString(row["StudentName"]),
                                                   ProgramName = Convert.ToString(row["ProgramName"]),
                                                   MilestoneName = Convert.ToString(row["MilestoneName"]),
                                                   FormID = Convert.ToInt32(row["FormId"]),
                                                   TermName = Convert.ToString(row["TermName"]),
                                                   MilestonePublishedFormID = Convert.ToInt32(row["MilestonePublishedFormID"]),
                                                   MileStoneFormApproverExternalId = columnExists && row["MileStoneFormApproverExternalId"] != DBNull.Value && !string.IsNullOrEmpty(row["MileStoneFormApproverExternalId"].ToString())? Convert.ToInt32(row["MileStoneFormApproverExternalId"]): default(int)
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
