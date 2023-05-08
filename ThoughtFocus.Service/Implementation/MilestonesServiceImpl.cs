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
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Milestones;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class MilestonesServiceImpl : IMilestonesService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        public ILogger<MilestonesServiceImpl> _logger;
        private readonly ICommonUtils _utils;
        public MilestonesServiceImpl(ISqlDBUtility helper
                                         , IConfiguration configuration
                                         , ISendMail sendMail
                                         , ILogger<MilestonesServiceImpl> logger
                                         , ICommonUtils utils)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
            _utils = utils;
        }
        public MilestonesListResponse GetMilestoneList()
        {
            MilestonesListResponse obj = new MilestonesListResponse();
            SqlParameter[] parameters = {
                                        };

            DataTable dtMilestones = _helper.GetDataTable("[dbo].[GetMilestoneList]", parameters);
            try
            {
                if (dtMilestones.Rows.Count > 0)
                {


                    obj.Milestones = dtMilestones.AsEnumerable().Select(row =>
                                              new Milestones
                                              {
                                                  MilestoneList = Convert.ToString(row["MilestoneList"])
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

        public BaseResponse UpsertMilestone(UpsertMilestoneRequest input)
        {
            DataTable approvers = _utils.ToDataTable(input.MileStoneApprovers);
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {

                                          new SqlParameter("@MilestoneID", SqlDbType.BigInt) { Value = input.MilestoneID },
                                          new SqlParameter("@MilestoneName", SqlDbType.NVarChar,50) { Value = input.MilestoneName },
                                          new SqlParameter("@MilestoneDescription", SqlDbType.NVarChar,50) { Value = input.MilestoneDescription },
                                          new SqlParameter("@MilestoneForm", SqlDbType.NVarChar,-1) { Value = input.MilestoneForm },
                                          new SqlParameter("@isPublished", SqlDbType.Bit) { Value = input.isPublished },
                                          new SqlParameter("@isMandatory", SqlDbType.Bit) { Value = input.isMandatory },
                                          new SqlParameter("@createdByUserID", SqlDbType.BigInt) { Value = input.createdByUserID },
                                          new SqlParameter("@MileStoneApprovers", SqlDbType.Structured) { Value = approvers }
                                        };

            int ID = _helper.InsertTable("[dbo].[UpsertMilestone]", parameters);
            response.Message = "Milestone Added Successfully";
            response.IsSuccess = true;
            return response;
        }

        public BaseResponse UpsertMilestoneFilledForm(UpsertMilestoneFilledFormRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {

                                          new SqlParameter("@MilestoneFormID", SqlDbType.BigInt) { Value = input.MilestoneFormID },
                                          new SqlParameter("@MilestoneID", SqlDbType.BigInt) { Value = input.MilestoneID },
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = input.FormID },
                                          new SqlParameter("@MilestoneFilledForm", SqlDbType.NVarChar,-1) { Value = input.MilestoneFilledForm },
                                          new SqlParameter("@createdByUserID", SqlDbType.BigInt) { Value = input.createdByUserID },
                                          new SqlParameter("@ApproverUserID", SqlDbType.BigInt) { Value = input.ApproverID },
                                          new SqlParameter("@ApproverComments", SqlDbType.NVarChar,-1) { Value = input.ApproverComments },
                                          new SqlParameter("@ActivityDefinitionID", SqlDbType.BigInt) { Value = input.ActivityDefinitionID },
                                          new SqlParameter("@ActivityDefinitionState", SqlDbType.NVarChar,-1) { Value = input.ActivityDefinitionState },
                                          new SqlParameter("@ActivityControlLabel", SqlDbType.NVarChar,20) { Value = input.ActivityControlLabel }

                                        };

            DataTable dtResult = _helper.GetDataTable("[dbo].[UpsertMilestoneFilledForm]", parameters);
            if(dtResult.Rows.Count>0)
            {
                if(Convert.ToInt32(dtResult.Rows[0]["Status"])==1)
                {
                    response.Message = "Milestone Added Successfully";
                    response.IsSuccess = true;
                }
                else
                {
                    response.Message = Convert.ToString(dtResult.Rows[0]["Message"]);
                    response.IsSuccess = true;
                }
            }
            
            return response;
        }

        public BaseResponse AssignMilestoneProgram(AssignMilestoneToProgram input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {

                                          new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = input.ProgramID },
                                          new SqlParameter("@MilestoneID", SqlDbType.BigInt) { Value = input.MilestoneID },
                                          new SqlParameter("@TermCode", SqlDbType.NVarChar,10) { Value = input.TermCode }
                                        };

            int ID = _helper.InsertTable("[dbo].[AssignMilestoneProgram]", parameters);
            response.Message = "Milestone Assigned Successfully";
            response.IsSuccess = true;
            return response;
        }

        public PublishedMilestonesListResponse GetPublishedMilestoneList(int ProgramID, string TermCode)
        {
            PublishedMilestonesListResponse obj = new PublishedMilestonesListResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = ProgramID },
                                            new SqlParameter("@TermCode", SqlDbType.NVarChar,10) { Value = TermCode }
                                        };

            DataTable dtMilestones = _helper.GetDataTable("[dbo].[GetMilestonesByProgramTerm]", parameters);
            try
            {
                if (dtMilestones.Rows.Count > 0)
                {


                    obj.MilestonesByProgramTerm = dtMilestones.AsEnumerable().Select(row =>
                                              new PublishedMilestonesList
                                              {
                                                  MilestonesByProgramTerm = Convert.ToString(row["MilestonesByProgramTerm"])
                                             
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

        public MilestoneApplicationFormsListResponse GetMilestoneApplicationFormsList(int UserID, int FormID, int ProgramID, string TermCode)
        {
            MilestoneApplicationFormsListResponse obj = new MilestoneApplicationFormsListResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                            new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = ProgramID },
                                            new SqlParameter("@TermCode", SqlDbType.NVarChar,10) { Value = TermCode }
                                        };

            DataTable dtMilestones = _helper.GetDataTable("[dbo].[GetMilestoneFilledFormsList]", parameters);
            try
            {
                if (dtMilestones.Rows.Count > 0)
                {


                    obj.MilestoneApplicationFormsList = dtMilestones.AsEnumerable().Select(row =>
                                              new MilestoneApplicationFormsList
                                              {
                                                  MilestoneFormsID = Convert.ToInt32(row["MilestoneFormsID"]),
                                                  MilestoneID = Convert.ToInt32(row["MilestoneID"]),
                                                  FormID = Convert.ToInt32(row["FormID"]),
                                                  Status = Convert.ToBoolean(row["Status"]),
                                                  MilestoneName = Convert.ToString(row["MilestoneName"])

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

        public GetMilestoneApplicationFormResponse GetMilestoneApplicationForm(int UserID, int MilestoneFormID, int FormID)
        {
            GetMilestoneApplicationFormResponse obj = new GetMilestoneApplicationFormResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                            new SqlParameter("@MilestoneFormID", SqlDbType.BigInt) { Value = MilestoneFormID },
                                            new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID }
                                        };

            DataSet dtMilestones = _helper.GetDataSet("[dbo].[GetMilestoneFilledForm]", parameters);
            try
            {
                if (dtMilestones.Tables[0].Rows.Count > 0 && dtMilestones.Tables[1].Rows.Count > 0)
                {


                    MilestoneApplicationForm objMAF = dtMilestones.Tables[0].AsEnumerable().Select(row =>
                                              new MilestoneApplicationForm
                                              {
                                                  MilestoneFormID = Convert.ToInt32(row["MilestoneFormID"]),
                                                  MilestoneID = Convert.ToInt32(row["MilestoneID"]),
                                                  FormID = Convert.ToInt32(row["FormID"]),
                                                  MilestoneFilledForm = Convert.ToString(row["MilestoneFilledForm"]),
                                                  Status = Convert.ToBoolean(row["Status"]),
                                                  MilestoneForm = Convert.ToString(row["MilestoneForm"]),
                                                  MilestoneName = Convert.ToString(row["MilestoneName"]),
                                                  isEditable = Convert.ToBoolean(row["isEditable"])

                                              }).FirstOrDefault();

                    obj.MilestoneApplicationForm = objMAF;

                    MilestoneFormActivityHandler objMAH = dtMilestones.Tables[1].AsEnumerable().Select(row =>
                                           new MilestoneFormActivityHandler
                                           {
                                               MilestoneActivityHandler = Convert.ToString(row["MilestoneFormActivityHandler"])

                                           }).FirstOrDefault();

                    obj.MilestoneFormActivityHandler = objMAH;

                    MileStoneFilledFormApprovers objMFFA = dtMilestones.Tables[2].AsEnumerable().Select(row =>
                                         new MileStoneFilledFormApprovers
                                         {
                                             MileStoneFilledFormApproversDetails = Convert.ToString(row["MileStoneFilledFormApprovers"])

                                         }).FirstOrDefault();

                    obj.MileStoneFilledFormApprovers = objMFFA;

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

        public GetMilestoneResponse GetMilestone(int MilestoneID)
        {
            GetMilestoneResponse obj = new GetMilestoneResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@MilestoneID", SqlDbType.BigInt) { Value = MilestoneID }
                                        };

            DataSet dtMilestones = _helper.GetDataSet("[dbo].[GetMilestone]", parameters);
            try
            {
                if (dtMilestones.Tables[0].Rows.Count > 0 && dtMilestones.Tables[1].Rows.Count > 0)
                {

                    MilestoneDetails objMAF = dtMilestones.Tables[0].AsEnumerable().Select(row =>
                                                                 new MilestoneDetails
                                                                 {
                                                                     MilestoneID = Convert.ToInt32(row["MilestoneID"]),
                                                                     MilestoneName = Convert.ToString(row["MilestoneName"]),
                                                                     MilestoneDescription = Convert.ToString(row["MilestoneDescription"]),
                                                                     MilestoneForm = Convert.ToString(row["MilestoneForm"]),
                                                                     isPublished = Convert.ToBoolean(row["isPublished"]),
                                                                     isMandatory = Convert.ToBoolean(row["isMandatory"]),
                                                                     CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                                                                     CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                                                                     CreatedByName = Convert.ToString(row["CreatedByName"])

                                                                 }).FirstOrDefault();

                    obj.MilestoneDetails = objMAF;

                    obj.ApproverDetails = dtMilestones.Tables[1].AsEnumerable().Select(row =>
                                           new ApproverDetails
                                           {
                                               ApproverUserID = Convert.ToInt32(row["ApproverUserID"]),
                                               Sequence = Convert.ToInt32(row["Sequence"]),
                                               ApproverUserName = Convert.ToString(row["ApproverUserName"])

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

        public MilestoneApproverUserListResponse GetMilestoneApproverUserList()
        {
            MilestoneApproverUserListResponse obj = new MilestoneApproverUserListResponse();
            SqlParameter[] parameters = {
                                           
                                        };

            DataTable dtMilestones = _helper.GetDataTable("[dbo].[GetMilestoneApproverUserList]", parameters);
            try
            {
                if (dtMilestones.Rows.Count>0)
                {

                 

                    obj.MilestoneApproverUserList = dtMilestones.AsEnumerable().Select(row =>
                                           new MilestoneApproverUserList
                                           {
                                               ApproverUserID = Convert.ToInt32(row["ApproverUserID"]),
                                               ApproverUserName = Convert.ToString(row["ApproverUserName"])

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
