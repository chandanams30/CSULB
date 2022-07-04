using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Domain.Request.FieldWork;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class FieldWorkServiceImpl : IFieldWorkService
    {
        private readonly ISqlDBUtility _helper;
        public FieldWorkServiceImpl(ISqlDBUtility helper)
        {
            _helper = helper;
        }
        public FieldWorkDataResponse GetFieldWorkDetailsById(int userId,int fieldWorkId)
        {
            FieldWorkDataResponse obj = new FieldWorkDataResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserId", SqlDbType.Int, 50) { Value = userId },
                                          new SqlParameter("@FieldWorkId", SqlDbType.Int, 50) { Value = fieldWorkId }
                                        };

            DataSet dsFieldWorkData = _helper.GetDataSet("[dbo].[GetFieldWork]", parameters);
            if (dsFieldWorkData.Tables.Count > 0)
            {
                obj.FieldWork = dsFieldWorkData.Tables[0].AsEnumerable().Select(row =>
                                          new FieldWorkResponse
                                          {
                                              FieldWorkId = Convert.ToInt32(row["ID"]),
                                              StudentName = Convert.ToString(row["StudentName"]),
                                              CourseTitle = Convert.ToString(row["CourseTitle"]),
                                              CSULBCourseID = Convert.ToString(row["CSULBCourseID"]),
                                              College = Convert.ToString(row["College"]),
                                              Term = Convert.ToString(row["Term"])
                                          }).FirstOrDefault();

                obj.FieldWorkRoles = dsFieldWorkData.Tables[1].AsEnumerable().Select(row =>
                                          new FieldWorkRoles
                                          {
                                            ID= Convert.ToInt32(row["ID"]),
                                            UserID = Convert.ToInt32(row["UserID"]),
                                            FieldWorkID = Convert.ToInt32(row["FieldWorkID"]),
                                            RoleID = Convert.ToInt32(row["RoleID"]),
                                            Name = Convert.ToString(row["Name"]),
                                            Description = Convert.ToString(row["Description"])
                                          }).ToList();

                obj.FieldWorkAttachments = dsFieldWorkData.Tables[2].AsEnumerable().Select(row =>
                                          new FieldWorkProfileAttachments
                                          {
                                              FieldWorkAttachmentID = Convert.ToInt32(row["ID"]),
                                              UserID = Convert.ToInt32(row["UserID"]),
                                              DocumentID = Convert.ToInt32(row["DocumentID"]),
                                              FileName = Convert.ToString(row["FileName"]),
                                              FileExtn = Convert.ToString(row["FileExtn"]),
                                              FolderName = Convert.ToString(row["FolderName"]),
                                              IsApproved = Convert.ToBoolean(row["IsApproved"] == DBNull.Value ? null : row["IsApproved"]),
                                              ApprovedBy = Convert.ToInt32(row["ApprovedBy"] == DBNull.Value ? null : row["ApprovedBy"]),
                                              ValidatedDate = Convert.ToDateTime(row["ValidatedDate"] == DBNull.Value ? null : row["ValidatedDate"]),
                                              CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                                              CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                                              ValidTill = Convert.ToDateTime(row["ValidTill"] == DBNull.Value ? null : row["ValidTill"])
                                          }).ToList();
                
            }
            return obj;
        }

        public List<FieldWorkResponse> GetFieldWorkList(int userId)
        {
            List<FieldWorkResponse> obj = new List<FieldWorkResponse>();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserId", SqlDbType.Int, 50) { Value = userId }
                                        };

            DataTable dtFieldWorkList= _helper.GetDataTable("[dbo].[GetFieldWorkData]", parameters);
            if (dtFieldWorkList.Rows.Count > 0)
            {
                obj = dtFieldWorkList.AsEnumerable().Select(row =>
                                          new FieldWorkResponse
                                          {
                                              FieldWorkId = Convert.ToInt32(row["ID"]),
                                              StudentName=Convert.ToString(row["StudentName"]),
                                              CourseTitle = Convert.ToString(row["StudentName"]),
                                              CSULBCourseID = Convert.ToString(row["StudentName"]),
                                              College = Convert.ToString(row["StudentName"]),
                                              Term= Convert.ToString(row["StudentName"]),
                                              IsTBTest=false,
                                              IsCtcDone=false

                                          }).ToList();
            }
            return obj;
        }

        public string UpdateFieldWorkValidation(FieldWorkValidationRequest input)
        {
            string responseString = string.Empty;
            if (input.ApprovalStatus == true)
            {
                // set the validtill value and empty the rejectreason value
                input.RejectedReason = null;
            }
            else
            {
                // set the rejectreason value  and empty the validtill value
                input.ValidTill = null;
            }
            DateTime validatedDate = DateTime.Now;


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt, 50) { Value = input.FieldWorkAttachmentId },
                                          new SqlParameter("@IsApproved", SqlDbType.Bit, 50) { Value = input.ApprovalStatus },
                                          new SqlParameter("@ApprovedBy", SqlDbType.BigInt, 50) { Value = input.ApproverUserId },
                                          new SqlParameter("@ValidatedDate", SqlDbType.DateTime, 50) { Value = validatedDate },
                                          new SqlParameter("@ValidTill", SqlDbType.DateTime, 50) { Value = (object)input.ValidTill??DBNull.Value },
                                          new SqlParameter("@RejectReason", SqlDbType.NVarChar, 255) { Value = (object)input.RejectedReason??DBNull.Value },
                                        };

            int identity = _helper.InsertTable("[dbo].[UpdateFieldWorkValidation]", parameters);

            return responseString;
        }
    }
}
