using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Domain.Request.FieldWork;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.FieldWork;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class FieldWorkServiceImpl : IFieldWorkService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        public FieldWorkServiceImpl(ISqlDBUtility helper, IConfiguration configuration)
        {
            _helper = helper;
            _configuration = configuration;
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
            try
            {
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
                                                  Term = Convert.ToString(row["Term"]),
                                                  FieldWorkPrerequisiteStatus = Convert.ToInt32(row["FieldWorkPrerequisiteStatus"])
                                              }).FirstOrDefault();

                    obj.FieldWorkRoles = dsFieldWorkData.Tables[1].AsEnumerable().Select(row =>
                                              new FieldWorkRoles
                                              {
                                                  ID = Convert.ToInt32(row["ID"]),
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
                                                  DocumentName = Convert.ToString(row["DocumentName"]),
                                                  FileName = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                              // FileExtn = Convert.ToString(row["FileExtn"]),
                                              // FolderName = Convert.ToString(row["FolderName"]),
                                              IsApproved = Convert.ToBoolean(row["IsApproved"] == DBNull.Value ? null : row["IsApproved"]),
                                                  ApprovedBy = Convert.ToString(row["ApprovedBy"] == DBNull.Value ? null : row["ApprovedBy"]),
                                                  ValidatedDate = Convert.ToDateTime(row["ValidatedDate"] == DBNull.Value ? null : row["ValidatedDate"]),
                                              //CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                                              //CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                                              ValidTill = Convert.ToDateTime(row["ValidTill"] == DBNull.Value ? null : row["ValidTill"]),
                                              //FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(Path.Combine(row["FolderName"].ToString(), "FieldWork"), Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"])),
                                              RejectReason = Convert.ToString(row["RejectedReason"])
                                              }).ToList();

                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

                }
            }
            catch(Exception ex)
            {
                obj.IsSuccess = false;
                obj.Message = "Data Retrieval Failed , Please contact site admin ";
                obj.StackTrace = ex.Message;
            }
            return obj;
        }

        public byte[] GetFileContent(string userFolderPath,string fileName)
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
        public FieldWorkListResponse GetFieldWorkList(int userId)
        {
            FieldWorkListResponse objList = new FieldWorkListResponse();
            List<FieldWorkResponse> obj = new List<FieldWorkResponse>();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserId", SqlDbType.Int, 50) { Value = userId }
                                        };

            DataTable dtFieldWorkList= _helper.GetDataTable("[dbo].[GetFieldWorkData]", parameters);
            try
            {
                if (dtFieldWorkList.Rows.Count > 0)
                {
                    obj = dtFieldWorkList.AsEnumerable().Select(row =>
                                              new FieldWorkResponse
                                              {
                                                  FieldWorkId = Convert.ToInt32(row["ID"]),
                                                  StudentName = Convert.ToString(row["StudentName"]),
                                                  CourseTitle = Convert.ToString(row["CourseTitle"]),
                                                  CSULBCourseID = Convert.ToString(row["CSULBCourseID"]),
                                                  College = Convert.ToString(row["College"]),
                                                  Term = Convert.ToString(row["Term"]),
                                                  FieldWorkPrerequisiteStatus=Convert.ToInt32(row["FieldWorkPrerequisiteStatus"])

                                              }).ToList();

                    objList.FieldWorkResponse = obj;
                    objList.IsSuccess = true;
                    objList.Message = "Data Retrieved Successfully";

                }
            }
            catch(Exception ex)
            {
                objList.IsSuccess = false;
                objList.Message = "Data Retrievel Failed";
                objList.StackTrace =ex.Message;
            }
            return objList;
        }

        public BaseResponse UpdateFieldWorkValidation(FieldWorkValidationRequest input)
        {
            BaseResponse response = new BaseResponse();
            try
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
                response.IsSuccess = true;
                response.Message = "Fieldwork validation Updated successfully";

            }
            catch(Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Fieldwork validation failed";
            }

            return response;
        }

        public BaseResponse UpdateFieldWorkDocumentValidation(FieldWorkUploadDocumentsRequest input)
        {
            BaseResponse response = new BaseResponse();
            string fileName = string.Empty;
            string fileExtension = string.Empty;
            string userFolderName = string.Empty;
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            if (input.FileName != string.Empty)
            {
                string[] fileSplit = input.FileName.Split('.');
                fileName = input.FieldWorkAttachmentId+fileSplit[0].ToString()+DateTime.Now.ToString("MMddyyyyHHmmss");
                fileExtension= fileSplit[1].ToString();
            }
            // call the SP to save the save the file details in fieldwork.attachments table 
            SqlParameter[] parameters =
                                      {
                                          new SqlParameter("@UserId", SqlDbType.BigInt) { Value = input.UserID },
                                          new SqlParameter("@FieldWorkAttachmentID", SqlDbType.BigInt) { Value = input.FieldWorkAttachmentId },
                                          new SqlParameter("@FileName", SqlDbType.VarChar, 250) { Value = fileName },
                                          new SqlParameter("@FileExtn", SqlDbType.VarChar, 20) { Value = fileExtension }
                                        };
            DataTable dtFWDoc = _helper.GetDataTable("[dbo].[UpdateFieldWorkRequiredDocuments]", parameters);
            if (dtFWDoc.Rows.Count > 0)
            {
                // check if the userFolder exists and if it exists then check if if the FieldWorkFolder exists
                userFolderName = dtFWDoc.Rows[0]["FolderName"].ToString();
                string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
                if (Directory.Exists(dirUserFolderPath))
                {
                    string dirFieldWork = Path.Combine(dirUserFolderPath, "Fieldwork");
                    if (Directory.Exists(dirFieldWork))
                    {
                        // copy the file here 
                        File.WriteAllBytes(Path.Combine(dirFieldWork, fileName+"."+fileExtension), input.FileContent);
                    }
                    else
                    {
                        Directory.CreateDirectory(dirFieldWork);
                        File.WriteAllBytes(Path.Combine(dirFieldWork, fileName + "." + fileExtension), input.FileContent);
                    }
                }
                else
                {
                    string dirFieldWork = Path.Combine(dirUserFolderPath, "Fieldwork");
                    DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                    DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirFieldWork);
                    DirectorySecurity dSecurity = dirFieldWorkFolder.GetAccessControl();
                    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    dirFieldWorkFolder.SetAccessControl(dSecurity);

                    File.WriteAllBytes(Path.Combine(dirFieldWork, fileName + "." + fileExtension), input.FileContent);
                }
            }

            response.IsSuccess = true;
            response.Message = "Field Work Document Uploaded Successfully";
            return response;
        }

        public FieldWorkProfileAttachments DownloadRequiredDocuments(int userId, int fieldworkAttachmentId)
        {
            FieldWorkProfileAttachments obj = new FieldWorkProfileAttachments();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userId },
                                          new SqlParameter("@FieldWorkAttachmentID", SqlDbType.BigInt) { Value = fieldworkAttachmentId }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[dbo].[DownloadFieldWorkRequiredDocument]", parameters);

            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new FieldWorkProfileAttachments
                                          {
                                              FieldWorkAttachmentID = Convert.ToInt32(row["ID"]),
                                              UserID = Convert.ToInt32(row["UserID"]),
                                              FileName = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                              IsApproved = Convert.ToBoolean(row["IsApproved"] == DBNull.Value ? null : row["IsApproved"]),
                                              ApprovedBy = Convert.ToString(row["ApprovedBy"] == DBNull.Value ? null : row["ApprovedBy"]),
                                              ValidatedDate = Convert.ToDateTime(row["ValidatedDate"] == DBNull.Value ? null : row["ValidatedDate"]),
                                              ValidTill = Convert.ToDateTime(row["ValidTill"] == DBNull.Value ? null : row["ValidTill"]),
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(Path.Combine(row["FolderName"].ToString(), "FieldWork"), Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]))
                                          }).FirstOrDefault();

            return obj;
        }

        public FieldWorkActivityLogResponse GetFieldWorkActivityLog(int userId, int fieldworkId)
        {
            FieldWorkActivityLogResponse obj = new FieldWorkActivityLogResponse();

            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = userId },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = fieldworkId }
                                     };
            DataTable dtActivityLog = _helper.GetDataTable("[dbo].[GetFieldWorkActivityLog]", parameters);
            if (dtActivityLog.Rows.Count > 0)
            {
                obj = dtActivityLog.AsEnumerable().Select(row =>
                                              new FieldWorkActivityLogResponse
                                              {
                                                  FieldWorkId = Convert.ToInt32(row["FieldWorkID"]),
                                                  BaseSchema = Convert.ToString(row["BaseSchema"]),
                                                  ResponseSchema = Convert.ToString(row["ResponseSchema"])
                                              }).FirstOrDefault();
                obj.IsSuccess = true;
                obj.Message = "Data Retrieved Successfully";
            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No Data";
            }

            return obj;
        }

        public FieldWorkActivityLogResponse UpdateFieldWorkActivityLog(FieldWorkActivityLogRequest input)
        {
            FieldWorkActivityLogResponse obj = new FieldWorkActivityLogResponse();

            SqlParameter[] parameters =
                                    {
                                          new SqlParameter("@UserID", SqlDbType.BigInt) { Value = input.UserId },
                                          new SqlParameter("@FieldWorkID", SqlDbType.BigInt) { Value = input.FieldWorkId },
                                          new SqlParameter("@ResponseSchema", SqlDbType.NVarChar) { Value = input.ResponseSchema }
                                     };
            DataTable dtActivityLog = _helper.GetDataTable("[dbo].[UpdateFieldWorkActivityLog]", parameters);
            if (dtActivityLog.Rows.Count > 0)
            {
                obj = dtActivityLog.AsEnumerable().Select(row =>
                                              new FieldWorkActivityLogResponse
                                              {
                                                  FieldWorkId = Convert.ToInt32(row["FieldWorkID"]),
                                                  BaseSchema = Convert.ToString(row["BaseSchema"]),
                                                  ResponseSchema = Convert.ToString(row["ResponseSchema"])
                                              }).FirstOrDefault();
                obj.IsSuccess = true;
                obj.Message = "Data Saved Successfully";
            }
          

            return obj;
        }
    }
}
