using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Request.GraduateProgram;
using ThoughtFocus.Domain.Request.Milestones;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.Milestones;
using ThoughtFocus.Domain.Response.StudentProfile;
using ThoughtFocus.Service.Interfaces;
using iTextSharp.text;
using ThoughtFocus.Domain.Enumeration;
using ThoughtFocus.Domain.Response.Form;
using ThoughtFocus.Domain.Response.Guests;

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
                                          new SqlParameter("@ActivityControlLabel", SqlDbType.NVarChar,20) { Value = input.ActivityControlLabel },
                                          new SqlParameter("@ExternalApprovers", SqlDbType.NVarChar,-1) { Value = input.ExternalApprovers },
                                          new SqlParameter("@IsExternalApprover", SqlDbType.Bit) { Value = input.IsExternalApprover }

                                        };

            DataSet dtMilestones = _helper.GetDataSet("[dbo].[UpsertMilestoneFilledForm]", parameters);
            if (dtMilestones.Tables[0].Rows.Count > 0)
            {
                //send mail to approver
                if (input.ActivityDefinitionID == 3 || input.ActivityDefinitionID == 6)
                {
                    if (dtMilestones.Tables[1].Rows.Count > 0)
                    {
                        string approverName = string.Empty;
                        string approverEmail = string.Empty;
                        string studentName = string.Empty;
                        string programName = string.Empty;
                        string milestoneName = string.Empty;
                        int milestoneApproverType = 0;
                        string body = string.Empty;
                        string approverIdentifier = string.Empty;
                        if (!string.IsNullOrEmpty(Convert.ToString(dtMilestones.Tables[1].Rows[0]["ApproverEmail"])))
                        {
                            approverEmail = Convert.ToString(dtMilestones.Tables[1].Rows[0]["ApproverEmail"]);
                            approverName = Convert.ToString(dtMilestones.Tables[1].Rows[0]["ApproverName"]);
                            studentName = Convert.ToString(dtMilestones.Tables[1].Rows[0]["StudentName"]);
                            programName = Convert.ToString(dtMilestones.Tables[1].Rows[0]["ProgramName"]);
                            milestoneName = Convert.ToString(dtMilestones.Tables[1].Rows[0]["MilestoneName"]);
                            milestoneApproverType = Convert.ToInt32(dtMilestones.Tables[1].Rows[0]["MilestoneApproverTypeID"]);
                            approverIdentifier = Convert.ToString(dtMilestones.Tables[1].Rows[0]["ExternalApprovalIdentifier"]);
                            string link = _configuration["ApplicationKeys:ExternalApproverBaseURL"] + approverIdentifier;
                            if (milestoneApproverType == 1)
                            {
                                body = GetMailBodyTemplate("MilestoneApprove.html");
                            }
                            else
                            {
                                body = GetMailBodyTemplate("MilestoneExternalApprover.html");
                            }
                            string logoText = "cid:myImageID";
                            body = body.Replace("[[logoPath]]", logoText)
                                       .Replace("[[approverName]]", approverName)
                                       .Replace("[[studentName]]", studentName)
                                       .Replace("[[programName]]", programName)
                                       .Replace("[[milestoneName]]", milestoneName)
                                       .Replace("[[link]]", link);
                            string subject = "Approve Milestone Form";
                            _sendMail.SendEmail(approverEmail, "", "COMMON", subject, body, "");
                        }
                    }
                }
                
                if (Convert.ToInt32(dtMilestones.Tables[0].Rows[0]["Status"]) == 1)
                {
                    response.Message = "Milestone "+ input.ActivityControlLabel + " Successfully";
                    response.IsSuccess = true;
                }
                else
                {
                    response.Message = Convert.ToString(dtMilestones.Tables[0].Rows[0]["Message"]);
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
                                                  MilestonePublishedFormID = Convert.ToInt32(row["MilestonePublishedFormID"]),
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

        public GetMilestoneApplicationFormResponse GetMilestoneApplicationForm(int UserID, int MilestoneFormID, int FormID, int MilestonePublishedFormID, bool IsReApply,bool IsExternalApprover)
        {
            GetMilestoneApplicationFormResponse obj = new GetMilestoneApplicationFormResponse();
            if (MilestoneFormID == 0)
            {
                MilestoneFormID = GetMilestoneFormID(UserID, FormID, MilestoneFormID, MilestonePublishedFormID,IsReApply);
            }
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                            new SqlParameter("@MilestoneFormID", SqlDbType.BigInt) { Value = MilestoneFormID },
                                            new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID },
                                            new SqlParameter("@IsExternalApprover", SqlDbType.Bit) { Value = IsExternalApprover}
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
                                                  MilestonePublishedFormID = Convert.ToInt32(row["MilestonePublishedFormID"]),
                                                  FormID = Convert.ToInt32(row["FormID"]),
                                                  MilestoneFilledForm = Convert.ToString(row["MilestoneFilledForm"]),
                                                  Status = Convert.ToBoolean(row["Status"]),
                                                  MilestoneForm = Convert.ToString(row["MilestoneForm"]),
                                                  MilestoneName = Convert.ToString(row["MilestoneName"]),
                                                  isEditable = Convert.ToBoolean(row["isEditable"]),
                                                  MilestoneDescription = Convert.ToString(row["MilestoneDescription"]),
                                                  StatusName = Convert.ToString(row["StatusName"]),
                                                  SubmittedDate = Convert.ToDateTime(row["SubmittedDate"] == DBNull.Value ? null : row["SubmittedDate"])
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

                    StudentDetails objSD = dtMilestones.Tables[3].AsEnumerable().Select(row =>
                                              new StudentDetails
                                              {
                                                  ID = Convert.ToInt32(row["ID"]),
                                                  CSULBID = Convert.ToInt32(row["CSULBID"]),
                                                  FirstName = Convert.ToString(row["FirstName"]),
                                                  LastName = Convert.ToString(row["LastName"]),
                                                  Email = Convert.ToString(row["Email"])
                                              }).FirstOrDefault();
                    obj.StudentDetails = objSD;
                    MileStoneFilledFormExternalApprovers objMFFEA = dtMilestones.Tables[4].AsEnumerable().Select(row =>
                                              new MileStoneFilledFormExternalApprovers
                                              {
                                                    ExternalApprovers = Convert.ToString(row["MileStoneFilledFormExternalApprovers"])
                                              }).FirstOrDefault();
                    obj.MileStoneFilledFormExternalApprovers = objMFFEA;

                    
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
        private int GetMilestoneFormID(int UserID, int FormID, int MilestoneFormID, int MilestonePublishedFormID, bool IsReApply)
        {
            //get the MilestoneFormID
            MilestoneApplicationFormsListResponse response = new MilestoneApplicationFormsListResponse();
            SqlParameter[] parameters1 = {
                                            new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID },
                                            new SqlParameter("@FormID", SqlDbType.BigInt) { Value = FormID },
                                            new SqlParameter("@ProgramID", SqlDbType.BigInt) { Value = 0 },
                                            new SqlParameter("@TermCode", SqlDbType.VarChar, 10) { Value = ""},
                                            new SqlParameter("@MilestonePublishedFormId", SqlDbType.BigInt) { Value = MilestonePublishedFormID},
                                            new SqlParameter("@IsReApply", SqlDbType.Bit ) { Value = IsReApply }
                                        };
            DataTable dtFilledFormList = _helper.GetDataTable("[dbo].[GetMilestoneFilledFormsList]", parameters1);
            if (dtFilledFormList.Rows.Count > 0)
            {

                response.MilestoneApplicationFormsList = dtFilledFormList.AsEnumerable().Where(row => row.Field<long>("MilestonePublishedFormID") == MilestonePublishedFormID).Select(row =>
                                          new MilestoneApplicationFormsList
                                          {
                                              MilestoneFormsID = Convert.ToInt32(row["MilestoneFormsID"]),

                                          }).ToList();
                MilestoneFormID = response.MilestoneApplicationFormsList.Select(form => form.MilestoneFormsID).FirstOrDefault();

            }
            return MilestoneFormID;
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
                if (dtMilestones.Rows.Count > 0)
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
                                          new SqlParameter("@MilestoneTypeID", SqlDbType.BigInt) { Value = input.MilestoneTypeID },
                                          new SqlParameter("@MilestoneRequirement", SqlDbType.NVarChar,500) { Value = input.MilestoneRequirement },
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
        public GetMilestoneUsersListResponse GetMilestoneUsersList()
        {
            GetMilestoneUsersListResponse obj = new GetMilestoneUsersListResponse();
            SqlParameter[] parameters = {

                                        };

            DataTable dtMilestoneUserList = _helper.GetDataTable("[Milestone].[GetMilestoneUsersList]", parameters);
            try
            {
                if (dtMilestoneUserList.Rows.Count > 0)
                {

                    obj.MilestoneUsersList = Convert.ToString(dtMilestoneUserList.Rows[0]["GetMilestoneUsersList"]);
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
        public GetMilestoneFilledFormByUserListResponse GetMilestoneFilledFormByUserList(int UserID)
        {
            GetMilestoneFilledFormByUserListResponse obj = new GetMilestoneFilledFormByUserListResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID }
                                        };

            DataTable dtMilestoneFilledForm = _helper.GetDataTable("[Milestone].[GetMilestoneFilledFormByUserList]", parameters);
            try
            {
                if (dtMilestoneFilledForm.Rows.Count > 0)
                {

                    obj.MilestoneFilledFormByUserList = Convert.ToString(dtMilestoneFilledForm.Rows[0]["GetMilestoneFilledFormByUserList"]);
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

        public GetMilestoneFilledFormByPublishedFormListResponse GetMilestoneFilledFormByPublishedFormList(int MilestonePublishedFormID, int UserID)
        {
            GetMilestoneFilledFormByPublishedFormListResponse obj = new GetMilestoneFilledFormByPublishedFormListResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@MilestonePublishedFormID", SqlDbType.BigInt) { Value = MilestonePublishedFormID },
                                            new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID }
                                        };

            DataTable dtMilestonePublishedForm = _helper.GetDataTable("[Milestone].[GetMilestoneFilledFormByPublishedFormList]", parameters);
            try
            {
                if (dtMilestonePublishedForm.Rows.Count > 0)
                {

                    obj.MilestonePublishedFormsList = Convert.ToString(dtMilestonePublishedForm.Rows[0]["MilestonePublishedFormsList"]);
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
        public GetMilestoneRequirementListResponse GetMilestoneRequirementList()
        {
            GetMilestoneRequirementListResponse obj = new GetMilestoneRequirementListResponse();
            SqlParameter[] parameters = { };

            DataTable dtMilestoneRequirementList = _helper.GetDataTable("[Milestone].[GetMilestoneRequirementList]", parameters);
            try
            {
                if (dtMilestoneRequirementList.Rows.Count > 0)
                {

                    obj.MilestoneRequirementList = Convert.ToString(dtMilestoneRequirementList.Rows[0]["MilestonePublishedFormsList"]);
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
        public GetMilestoneSubmittedFormsListResponse GetMilestoneSubmittedFormsList(int RoleID, int ApproverUserID)
        {
            GetMilestoneSubmittedFormsListResponse obj = new GetMilestoneSubmittedFormsListResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@RoleId", SqlDbType.BigInt) { Value = RoleID },
                                            new SqlParameter("@ApproverUserID", SqlDbType.BigInt) { Value = ApproverUserID }
                                        };

            DataTable dtMilestoneFormsList = _helper.GetDataTable("[Milestone].[GetMilestoneSubmittedFormsList]", parameters);
            try
            {
                if (dtMilestoneFormsList.Rows.Count > 0)
                {

                    obj.milestoneSubmittedFormsList = dtMilestoneFormsList.AsEnumerable().Select(row =>
                                               new GetMilestoneSubmittedFormsResponse
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
                                                   MilestonePublishedFormID = Convert.ToInt32(row["MilestonePublishedFormID"])
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
        public GetMilestoneWorkflowProcessTransitionHistoryResponse GetMilestoneWorkflowProcessTransitionHistory(int MilestoneFormID, int RoleID)
        {
            GetMilestoneWorkflowProcessTransitionHistoryResponse obj = new GetMilestoneWorkflowProcessTransitionHistoryResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@MilestoneFormID", SqlDbType.BigInt) { Value = MilestoneFormID },
                                            new SqlParameter("@RoleID", SqlDbType.BigInt) { Value = RoleID }
                                        };

            DataTable dtMilestonePublishedForm = _helper.GetDataTable("[Milestone].[GetWorkflowProcessTransitionHistory]", parameters);
            try
            {
                if (dtMilestonePublishedForm.Rows.Count > 0)
                {

                    obj.WorkflowTransitionHistory = Convert.ToString(dtMilestonePublishedForm.Rows[0]["WorkflowTransitionHistory"]);
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
        public ApplicationProgramsResponse GetApplicationProgramsByTermCode(string termCode)
        {
            ApplicationProgramsResponse obj = new ApplicationProgramsResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@TermCode", SqlDbType.VarChar,10) { Value = termCode }
                                        };

            DataTable dtProgramList = _helper.GetDataTable("[dbo].[GetApplicationProgramsByTermCode]", parameters);
            try
            {
                if (dtProgramList.Rows.Count > 0)
                {
                    obj.ProgramsList = dtProgramList.AsEnumerable().Select(row =>
                                              new ApplicationProgram
                                              {
                                                  ProgramID = Convert.ToInt32(row["ID"]),
                                                  ProgramName = Convert.ToString(row["ProgramName"])
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
        public SemesterListResponse GetDistinctSemesterList()
        {
            SemesterListResponse obj = new SemesterListResponse();
            SqlParameter[] parameters = { };
            DataTable dtSemesters = _helper.GetDataTable("[dbo].[GetDistinctSemesterList]", parameters);
            try
            {
                if (dtSemesters.Rows.Count > 0)
                {
                    obj.Semesters = dtSemesters.AsEnumerable().Select(row =>
                                              new Domain.Response.GraduateProgram.Semester
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
        public MilestoneFormAttachmentResponse UpsertMilestoneFormAttachment(UpsertMilestoneFormAttachment input)
        {
            MilestoneFormAttachmentResponse response = new MilestoneFormAttachmentResponse();
            if (input.FileContent != null && input.FileContent.Length > 0)
            {
                string fileName = string.Empty;
                string fileExtension = string.Empty;
                string userFolderName = string.Empty;
                int userID = 0;
                var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];

                AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(input.GUID);
                input.GUID = fileDetails.FileName;

                //split Filename using '~'
                string[] splitFileName = input.GUID.ToString().Split('~');
                string uploadControlName = Convert.ToString(splitFileName[0]);
                int formID = Convert.ToInt32(splitFileName[1]);
                int milestonePublishedFormID = Convert.ToInt32(splitFileName[2]);
                string GUID = Convert.ToString(splitFileName[3]);
                fileName = "DOC" + "_" + DateTime.Now.ToString("MMddyyyyHHmmss");

                // get userID
                SqlParameter[] parameters =
                                 {
                                    new SqlParameter("@FormId", SqlDbType.Int) { Value = formID }
                                 };

                DataTable dtForm = _helper.GetDataTable("[dbo].[GetFormDetails]", parameters);
                if (dtForm.Rows.Count > 0)
                {
                    userID = Convert.ToInt32(dtForm.Rows[0]["UserID"]);
                }
                string folderName = userID + "~" + "Milestone" + "_" + fileName;
                if (input.GUID != string.Empty)
                {

                    fileExtension = fileDetails.FileExtension;
                    if (fileExtension.ToUpper() == "PNG" || fileExtension.ToUpper() == "JPG" || fileExtension.ToUpper() == "JPEG")
                    {
                        // logic to convert png to pdf 
                        byte[] imageContent = null;
                        imageContent = GetImageFilecontent(input.FileContent);
                        input.FileContent = null;
                        input.FileContent = imageContent;
                        fileExtension = "pdf";
                    }
                }

                SqlParameter[] parameters1 =
                                         {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userID},
                                          new SqlParameter("@FormID", SqlDbType.BigInt) { Value = formID },
                                          new SqlParameter("@MilestonePublishedFormID", SqlDbType.BigInt) { Value = milestonePublishedFormID },
                                          new SqlParameter("@GUID", SqlDbType.UniqueIdentifier, 250) { Value = new Guid(GUID) },
                                          new SqlParameter("@FileName", SqlDbType.NVarChar, 250) { Value = fileName },
                                          new SqlParameter("@FileExtn", SqlDbType.NVarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@FolderName", SqlDbType.VarChar, 100) { Value = folderName },
                                        };
                DataTable dtFormAttachment = _helper.GetDataTable("[dbo].[UpsertMilestoneFormAttachment]", parameters1);
                string[] folderSplit = folderName.ToString().Split('~');
                userFolderName = folderSplit[0].ToString();
                string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
                if (Directory.Exists(dirUserFolderPath))
                {
                    string dirForm = Path.Combine(dirUserFolderPath, "Milestone");
                    if (Directory.Exists(dirForm))
                    {
                        {
                            File.WriteAllBytes(Path.Combine(dirForm, fileName + "." + fileExtension), input.FileContent);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(dirForm);
                        {
                            File.WriteAllBytes(Path.Combine(dirForm, fileName + "." + fileExtension), input.FileContent);
                        }
                    }
                }
                else
                {
                    string dirForm = Path.Combine(dirUserFolderPath, "Milestone");
                    DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                    DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirForm);
                    DirectorySecurity dSecurity = dirFieldWorkFolder.GetAccessControl();
                    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    dirFieldWorkFolder.SetAccessControl(dSecurity);
                    {
                        File.WriteAllBytes(Path.Combine(dirForm, fileName + "." + fileExtension), input.FileContent);
                    }
                }
                response.fileName = fileName;
                response.IsSuccess = true;
                response.Message = "Form attachment Uploaded Successfully";
                return response;
            }
            else
            {
                response.IsSuccess = true;
                response.Message = "No Attachment to upload";
                return response;
            }
        }
        public FormAttachments DownloadMilestoneFormAttachments(string FileName)
        {
            FormAttachments obj = new FormAttachments();
            AttachmentFileDetails fileDetails = GetAttachedFileSplitValues(FileName);
            FileName = fileDetails.FileName;

            //split Filename using '~'
            string[] splitFileName = FileName.ToString().Split('~');
            string GUID = Convert.ToString(splitFileName[3]);
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@GUID", SqlDbType.UniqueIdentifier) { Value = new Guid(GUID) }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[dbo].[GetMilestoneFormAttachment]", parameters);

            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new FormAttachments
                                          {
                                              Filename = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtension"]),
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(Path.Combine(GetAttachmentsFolderName(row["FolderName"].ToString()), "Milestone"), row["FileName"].ToString() + "." + Convert.ToString(row["FileExtension"]))
                                          }).FirstOrDefault();

            return obj;
        }
        public PublishedMilestoneDetailsResponse GetPublishedMilestoneDetails(int MilestoneTemplateID)
        {
            PublishedMilestoneDetailsResponse obj = new PublishedMilestoneDetailsResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@MilestoneTemplateID", SqlDbType.BigInt) { Value = MilestoneTemplateID }
                                        };

            DataSet dtMilestoneTemplate = _helper.GetDataSet("[Milestone].[GetPublishedMilestoneDetails]", parameters);
            try
            {
                if (obj.publishedMilestoneDetails == null)
                {
                    obj.publishedMilestoneDetails = new PublishedMilestoneDetails();
                }
                if (dtMilestoneTemplate.Tables[0].Rows.Count > 0 && dtMilestoneTemplate.Tables[1].Rows.Count > 0 && dtMilestoneTemplate.Tables[2].Rows.Count > 0)
                {

                    var milestoneDetail = dtMilestoneTemplate.Tables[0].AsEnumerable().Select(row =>
                                            new PublishedMilestoneDetails
                                            {
                                                MilestoneName = Convert.ToString(row["MilestoneName"]),
                                                MilestoneDescription = Convert.ToString(row["MilestoneDescription"]),
                                                ProgramName = Convert.ToString(row["ProgramName"]),
                                                TermName = Convert.ToString(row["TermName"])
                                            }).FirstOrDefault();

                    if (milestoneDetail != null)
                    {
                        obj.publishedMilestoneDetails.MilestoneName = milestoneDetail.MilestoneName;
                        obj.publishedMilestoneDetails.MilestoneDescription = milestoneDetail.MilestoneDescription;
                        obj.publishedMilestoneDetails.ProgramName = milestoneDetail.ProgramName;
                        obj.publishedMilestoneDetails.TermName = milestoneDetail.TermName;
                    }
                    var approvers = dtMilestoneTemplate.Tables[1].AsEnumerable().Select(row =>
                                           new PublishedMilestoneDetails
                                           {
                                               Approvers = Convert.ToString(row["ApproversName"])

                                           }).FirstOrDefault();
                    if(approvers != null)
                    {
                        obj.publishedMilestoneDetails.Approvers = approvers.Approvers;
                    }

                    var students = dtMilestoneTemplate.Tables[2].AsEnumerable().Select(row =>
                                         new PublishedMilestoneDetails
                                         {
                                             Students = Convert.ToString(row["StudentsName"])

                                         }).FirstOrDefault();
                    if (students != null)
                    {
                        obj.publishedMilestoneDetails.Students = students.Students;
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
        public StudentMilestoneListResponse GetStudentsMilestone(int UserID)
        {
            StudentMilestoneListResponse obj = new StudentMilestoneListResponse();
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserID", SqlDbType.BigInt) { Value = UserID }
                                        };

            DataTable dtMilestoneList = _helper.GetDataTable("[Milestone].[ApplyStudentMilestone]", parameters);
            try
            {
                if (dtMilestoneList.Rows.Count > 0)
                {
                    obj.studentsMilestone = dtMilestoneList.AsEnumerable().Select(row =>
                                              new StudentMilestoneList
                                              {
                                                  MilestoneFormID = Convert.ToInt32(row["MilestoneFormID"]),
                                                  MilestonePublishedFormID = Convert.ToInt32(row["MilestonePublishedFormID"]),
                                                  ApproverName = Convert.ToString(row["ApproverName"]),
                                                  StudentName = Convert.ToString(row["StudentName"]),
                                                  CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                                                  State = Convert.ToString(row["State"]),
                                                  CSULBID = Convert.ToInt32(row["CSULBID"]),
                                                  ProgramName = Convert.ToString(row["ProgramName"]),
                                                  MilestoneName = Convert.ToString(row["MilestoneName"]),
                                                  ProgramID = Convert.ToInt32(row["ProgramID"]),
                                                  TermCode = Convert.ToString(row["TermCode"]),
                                                  FormId = Convert.ToInt32(row["FormId"]),
                                                  TermName = Convert.ToString(row["TermName"])
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
        public BaseResponse SendRemainderToApprover(string Identifier, int MilestoneFormID)
        {
            BaseResponse obj = new BaseResponse();

            SqlParameter[] parameters =
                              {
                                          new SqlParameter("@ExternalApprovalIdentifier", SqlDbType.UniqueIdentifier) { Value = new Guid(Identifier) },
                                          new SqlParameter("@MilestoneFormID", SqlDbType.BigInt) { Value = MilestoneFormID }
                               };

            DataTable dtMilestones = _helper.GetDataTable("[Milestone].[GetMilestoneSubmittedFormsListForExternalApprovers]", parameters);
            if (dtMilestones.Rows.Count > 0)
            {
                try
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dtMilestones.Rows[0]["ApproverEmail"])))
                    {
                        string approverEmail = Convert.ToString(dtMilestones.Rows[0]["ApproverEmail"]);
                        string approverName = Convert.ToString(dtMilestones.Rows[0]["ApproverName"]);
                        string studentName = Convert.ToString(dtMilestones.Rows[0]["StudentName"]);
                        string programName = Convert.ToString(dtMilestones.Rows[0]["ProgramName"]);
                        string milestoneName = Convert.ToString(dtMilestones.Rows[0]["MilestoneName"]);
                        string link = _configuration["ApplicationKeys:ExternalApproverBaseURL"] + Identifier;
                        string body = GetMailBodyTemplate("MilestoneExternalApprover.html");
                        string logoText = "cid:myImageID";
                        body = body.Replace("[[logoPath]]", logoText)
                                   .Replace("[[approverName]]", approverName)
                                   .Replace("[[studentName]]", studentName)
                                   .Replace("[[programName]]", programName)
                                   .Replace("[[milestoneName]]", milestoneName)
                                   .Replace("[[link]]", link);
                        string subject = "Approve Milestone Form";
                        _sendMail.SendEmail(approverEmail, "", "COMMON", subject, body, "");
                    }
                    obj.IsSuccess = true;
                    obj.Message = "Mail sent successfully !";
                }
                catch (Exception ee)
                {
                    obj.IsSuccess = false;
                    obj.Message = "Failure sending mail.";
                }
            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data .";
            }


            return obj;
        }
        private AttachmentFileDetails GetAttachedFileSplitValues(string filename)
        {
            AttachmentFileDetails fileObject = new AttachmentFileDetails();
            string uploadedFileName = filename;
            string[] splitter = uploadedFileName.Split('.');
            StringBuilder fileNameAppender = new StringBuilder();
            string fileExtension = uploadedFileName.Split('.').Last();
            int length = splitter.Length;
            for (int i = 0; i < splitter.Length; i++)
            {
                if (i == length - 2 && length > 2)
                    fileNameAppender.Append(splitter[i]);
                else if (length == 2)
                {
                    fileNameAppender.Append(splitter[i]);
                    break;
                }
                else
                {
                    if (i != length - 1)
                        fileNameAppender.Append(splitter[i] + ".");
                }
            }
            fileObject.FileName = fileNameAppender.ToString();
            fileObject.FileExtension = fileExtension;
            return fileObject;
        }
        private byte[] GetImageFilecontent(byte[] fileContent)
        {
            byte[] inputStream = null;
            string documentName = string.Empty;
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                //Initialize the PDF document object.
                using (iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4, 10f, 10f, 10f, 10f))
                {
                    PdfWriter.GetInstance(pdfDoc, stream).SetFullCompression();
                    pdfDoc.Open();

                    //Add the Image file to the PDF document object.
                    iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(fileContent);

                    //Scaling the image
                    if (pic.Height > pic.Width)
                    {
                        float percentage = 0.0f;
                        percentage = 700 / pic.Height;
                        pic.ScalePercent(percentage * 100);
                    }
                    else
                    {
                        float percentage = 0.0f;
                        percentage = 540 / pic.Width;
                        pic.ScalePercent(percentage * 100);
                    }
                    pdfDoc.Add(pic);
                    pdfDoc.Close();
                    inputStream = stream.ToArray();
                }
            }
            return inputStream;
        }
        public byte[] GetFileContent(string userFolderPath, string fileName)
        {
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            string filepath = Path.Combine(fileRepoPath, Path.Combine(userFolderPath, fileName));
            byte[] fileContent = null;
            System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            long byteLength = new System.IO.FileInfo(filepath).Length;
            fileContent = binaryReader.ReadBytes((Int32)byteLength);
            fs.Close();
            fs.Dispose();
            binaryReader.Close();
            return fileContent;
        }
        private string GetAttachmentsFolderName(string combinedString)
        {
            string[] folderSplit = combinedString.ToString().Split('~');
            string userFolderName = folderSplit[0].ToString();
            return userFolderName;
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
