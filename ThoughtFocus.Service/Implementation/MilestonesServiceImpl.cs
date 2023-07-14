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
                                          new SqlParameter("@MileStoneApprovers", SqlDbType.Structured) { Value = approvers },
                                          new SqlParameter("@MilestoneTypeID", SqlDbType.BigInt) { Value = input.MilestoneTypeID }
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
                                                                     CreatedByName = Convert.ToString(row["CreatedByName"]),
                                                                     MilestoneTypeID = Convert.ToInt32(row["MilestoneTypeID"]),
                                                                     MilestoneTypeName = Convert.ToString(row["MilestoneTypeName"])

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

        public MilestoneTypesResponse GetMilestoneTypes()
        {
            MilestoneTypesResponse obj = new MilestoneTypesResponse();
            SqlParameter[] parameters = {

                                        };

            DataTable dtMilestoneTypes = _helper.GetDataTable("[dbo].[GetMilestoneTypes]", parameters);
            try
            {
                if (dtMilestoneTypes.Rows.Count > 0)
                {



                    obj.MilestoneTypes = dtMilestoneTypes.AsEnumerable().Select(row =>
                                           new MilestoneTypes
                                           {
                                               ID = Convert.ToInt32(row["ID"]),
                                               Name = Convert.ToString(row["Name"])

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

        public BaseResponse UpsertMilestoneTemplate(UpsertMilestoneTemplateRequest input)
        {
           // DataTable approvers = _utils.ToDataTable(input.MileStoneApprovers);
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@MilestoneTemplateID", SqlDbType.BigInt) { Value = input.MilestoneTemplateID },
                                          new SqlParameter("@MilestoneName", SqlDbType.NVarChar,50) { Value = input.MilestoneName },
                                          new SqlParameter("@MilestoneDescription", SqlDbType.NVarChar,500) { Value = input.MilestoneDescription },
                                          new SqlParameter("@MilestoneForm", SqlDbType.NVarChar,-1) { Value = input.MilestoneForm },
                                          new SqlParameter("@isReadyToPublish", SqlDbType.Bit) { Value = input.isReadyToPublish },
                                          new SqlParameter("@createdByUserID", SqlDbType.BigInt) { Value = input.createdByUserID }
                                         
                                        };
            int ID = _helper.InsertTable("[Milestone].[UpsertMilestoneTemplate]", parameters);
            response.Message = "Milestone Saved Successfully";
            response.IsSuccess = true;
            return response;
        }

        public MilestoneTemplateListResponse GetMilestoneTemplateList(bool isReadyToPublish)
        {
            MilestoneTemplateListResponse obj = new MilestoneTemplateListResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@isReadyToPublish", SqlDbType.Bit) { Value = isReadyToPublish }
                                        };

            DataTable dtMilestoneTemplateList = _helper.GetDataTable("[Milestone].[GetMilestoneTemplateList]", parameters);
            try
            {
                if (dtMilestoneTemplateList.Rows.Count > 0)
                {



                    obj.MilestoneTemplateList = dtMilestoneTemplateList.AsEnumerable().Select(row =>
                                           new MilestoneTemplateList
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

        public MilestoneTemplateByIDResponse GetMilestoneTemplateByID(int MilestoneTemplateID)
        {
            MilestoneTemplateByIDResponse obj = new MilestoneTemplateByIDResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@MilestoneTemplateID", SqlDbType.BigInt) { Value = MilestoneTemplateID }
                                        };

            DataTable dtMilestoneTemplate = _helper.GetDataTable("[Milestone].[GetMilestoneTemplateByID]", parameters);
            try
            {
                if (dtMilestoneTemplate.Rows.Count > 0)
                {

                    obj.MilestoneTemplate = Convert.ToString(dtMilestoneTemplate.Rows[0]["MilestoneTempate"]);

                    //obj.MilestoneTemplateByID = dtMilestoneTemplate.AsEnumerable().Select(row =>
                    //                       new MilestoneTemplateByID
                    //                       {
                    //                           MilestoneTemplate = Convert.ToString(row["MilestoneTempate"])

                    //                       }).FirstOrDefault();

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

        public MilestonesApproverTypesResponse GetMilestoneApproverTypes()
        {
            MilestonesApproverTypesResponse obj = new MilestonesApproverTypesResponse();
            SqlParameter[] parameters = {

                                        };

            DataTable dtMilestoneApproverTypes = _helper.GetDataTable("[dbo].[GetMilestoneApproverTypes]", parameters);
            try
            {
                if (dtMilestoneApproverTypes.Rows.Count > 0)
                {



                    obj.ApproverTypes = dtMilestoneApproverTypes.AsEnumerable().Select(row =>
                                           new MilestonesApproverTypes
                                           {
                                               ApproverTypeID = Convert.ToInt32(row["ID"]),
                                               Name = Convert.ToString(row["Name"])

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

        public BaseResponse PublishMilestoneForm(PublishMilestoneFormRequest input)
        {
            DataTable approvers = _utils.ToDataTable(input.MilestonePublishedFormApprovers);
            DataTable FormUsers = _utils.ToDataTable(input.MilestonePublishedFormUsers);
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {

                                          new SqlParameter("@MilestoneTemplateID", SqlDbType.BigInt) { Value = input.MilestoneTemplateID },
                                          new SqlParameter("@MilestoneName", SqlDbType.NVarChar,50) { Value = input.MilestoneName },
                                          new SqlParameter("@MilestoneDescription", SqlDbType.NVarChar,500) { Value = input.MilestoneDescription },
                                          new SqlParameter("@MilestoneForm", SqlDbType.NVarChar,-1) { Value = input.MilestoneForm },
                                          new SqlParameter("@createdByUserID", SqlDbType.BigInt) { Value = input.createdByUserID },
                                          new SqlParameter("@isMandatory", SqlDbType.Bit) { Value = input.isMandatory },
                                          new SqlParameter("@MilestonePublishedFormApprovers", SqlDbType.Structured) { Value = approvers },
                                          new SqlParameter("@MilestonePublishedFormUsers", SqlDbType.Structured) { Value = FormUsers },
                                          new SqlParameter("@MilestoneTypeID", SqlDbType.BigInt) { Value = input.MilestoneTypeID }
                                        };

            int ID = _helper.InsertTable("[Milestone].[PublishMilestoneForm]", parameters);
            response.Message = "Milestone Published Successfully";
            response.IsSuccess = true;
            return response;
        }

        public MilestoneUsersListResponse GetMilestoneUsersList(int RoleID, int ProgramID, string TermCode)
        {
            MilestoneUsersListResponse obj = new MilestoneUsersListResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = RoleID },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = ProgramID },
                                            new SqlParameter("@TermCode", SqlDbType.VarChar,10) { Value = TermCode }
                                        };

            DataTable dtMilestoneTemplate = _helper.GetDataTable("[Milestone].[GetMilestoneUsersList]", parameters);
            try
            {
                if (dtMilestoneTemplate.Rows.Count > 0)
                {

                    obj.MilestoneUsersList = Convert.ToString(dtMilestoneTemplate.Rows[0]["MilestoneUsersList"]);
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

        public MilestoneProgramTermListResponse GetMilestoneProgramTermList()
        {
            MilestoneProgramTermListResponse obj = new MilestoneProgramTermListResponse();
            SqlParameter[] parameters = {
                                          
                                        };

            DataTable dtMilestoneTemplate = _helper.GetDataTable("[Milestone].[GetMilestoneProgramTermList]", parameters);
            try
            {
                if (dtMilestoneTemplate.Rows.Count > 0)
                {

                    obj.MilestoneProgramTermList = Convert.ToString(dtMilestoneTemplate.Rows[0]["MilestoneProgramTermList"]);
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

        public MilestonePublishedFormsListResponse GetMilestonePublishedFormsList(int UserID)
        {
            MilestonePublishedFormsListResponse obj = new MilestonePublishedFormsListResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID }
                                        };

            DataTable dtMilestoneTemplate = _helper.GetDataTable("[Milestone].[GetMilestonePublishedFormsList]", parameters);
            try
            {
                if (dtMilestoneTemplate.Rows.Count > 0)
                {

                    obj.MilestonePublishedFormsList = Convert.ToString(dtMilestoneTemplate.Rows[0]["MilestonePublishedFormsList"]);
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
