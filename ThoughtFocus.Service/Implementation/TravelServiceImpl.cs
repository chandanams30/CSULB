using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
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
using ThoughtFocus.Domain.Request.Travel;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.Travel;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class TravelServiceImpl : ITravelService
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        private readonly ICommonUtils _util;
        public ILogger<TravelServiceImpl> _logger;
        public TravelServiceImpl(ISqlDBUtility helper
                                         , IConfiguration configuration
                                         , ISendMail sendMail
                                         , ILogger<TravelServiceImpl> logger
                                         , ICommonUtils util)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
            _util = util;
        }

        public FacultySupervisorListResponse GetFacultySupervisorList(int UserID)
        {
            FacultySupervisorListResponse obj = new FacultySupervisorListResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@UserID", SqlDbType.Int,50) { Value = UserID }
                                        };

            DataTable dtFacultySupervisorList = _helper.GetDataTable("[Travel].[GetBusinessMileageSupervisorList]", parameters);
            try
            {
                if (dtFacultySupervisorList.Rows.Count > 0)
                {


                    obj.FacultySupervisorList = dtFacultySupervisorList.AsEnumerable().Select(row =>
                                              new FacultySupervisor
                                              {
                                                  BusinessMileageSupervisorUserID = Convert.ToInt32(row["BusinessMileageSupervisorUserID"]),
                                                  BusinessMileageSupervisorName = Convert.ToString(row["BusinessMileageSupervisorName"]),
                                                  Email = Convert.ToString(row["Email"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),
                                                  Phone = Convert.ToString(row["Phone"]),
                                                  MailingAddress = Convert.ToString(row["MailingAddress"]),
                                                  City = Convert.ToString(row["City"]),
                                                  State = Convert.ToString(row["State"]),
                                                  Zip = Convert.ToString(row["Zip"]),
                                                  BusinessMileageSupervisorID = row["BusinessMileageSupervisorID"] ==DBNull.Value?(int?)null:Convert.ToInt32(row["BusinessMileageSupervisorID"]),
                                                  isPersonalInfoCompleted = Convert.ToBoolean(row["isPersonalInfoCompleted"]),
                                                  IsTravelPrerequisitesCompleted = Convert.ToBoolean(row["IsTravelPrerequisitesCompleted"])
                                              }).ToList();

                
                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

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

        public BaseResponse UpsertBusinessMileageSupervisorList(UpsertFacultySupervisorRequest input)
        {
            BaseResponse obj = new BaseResponse();

            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@BusinessMileageSupervisorUserID", SqlDbType.BigInt, 50) { Value = input.TravelCSULBFacultySupervisorUserID },
                                          new SqlParameter("@Phone", SqlDbType.NVarChar, 20) { Value = input.Phone },
                                          new SqlParameter("@MailingAddress", SqlDbType.NVarChar, 255) { Value = input.MailingAddress },
                                          new SqlParameter("@City", SqlDbType.NVarChar, 25) { Value = input.City },
                                          new SqlParameter("@State", SqlDbType.NVarChar, 25) { Value = input.State },
                                          new SqlParameter("@Zip", SqlDbType.NVarChar, 25) { Value = input.Zip },
                                          new SqlParameter("@ModifiedByUserID", SqlDbType.BigInt, 50) { Value = input.ModifiedByUserID }
                                        };

            int identity = _helper.InsertTable("[Travel].[UpsertBusinessMileageSupervisorUser]", parameters);
            obj.IsSuccess = true;
            obj.Message = "Data Saved Successfully";

            return obj;
        }

        public MileageLogListResponse GetBusinessMileageLogList(int BusinessMileageSupervisorID, int BusinessMileageSupervisorUserID)    
        {
            MileageLogListResponse obj = new MileageLogListResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@BusinessMileageSupervisorID", SqlDbType.Int,50) { Value = BusinessMileageSupervisorID },
                                          new SqlParameter("@BusinessMileageSupervisorUserID", SqlDbType.Int,50) { Value = BusinessMileageSupervisorUserID }
                                        };

            DataSet dsMileageLogList = _helper.GetDataSet("[Travel].[GetBusinessMileageLogList]", parameters);
            try
            {
                if (dsMileageLogList.Tables[0].Rows.Count > 0 && dsMileageLogList !=null || dsMileageLogList.Tables[1].Rows.Count > 0)
                {


                    obj.PersonalInfo = dsMileageLogList.Tables[0].AsEnumerable().Select(row =>
                                              new SupervisorPersonalInfo
                                              {
                                                  BusinessMileageSupervisorUserID = Convert.ToInt32(row["BusinessMileageSupervisorUserID"]),
                                                  BusinessMileageSupervisorName = Convert.ToString(row["BusinessMileageSupervisorName"]),
                                                  Email = Convert.ToString(row["Email"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),
                                                  Phone = Convert.ToString(row["Phone"]),
                                                  MailingAddress = Convert.ToString(row["MailingAddress"]),
                                                  City = Convert.ToString(row["City"]),
                                                  State = Convert.ToString(row["State"]),
                                                  Zip = Convert.ToString(row["Zip"]),
                                                  BusinessMileageSupervisorID = Convert.ToInt32(row["BusinessMileageSupervisorID"]),
                                                  isPersonalInfoCompleted = Convert.ToBoolean(row["isPersonalInfoCompleted"]),
                                                  IsTravelPrerequisitesCompleted = Convert.ToBoolean(row["IsTravelPrerequisitesCompleted"])


                                              }).FirstOrDefault();

                    obj.TravelPrerequisites = dsMileageLogList.Tables[1].AsEnumerable().Select(row =>
                                           new TravelPrerequisites
                                           {
                                               TravelAttachmentID = Convert.ToInt32(row["ID"]),
                                               BusinessMileageSupervisorUserID = Convert.ToInt32(row["BusinessMileageSupervisorUserID"]),
                                               DocumentID = Convert.ToInt32(row["DocumentID"]),
                                               DocumentName = Convert.ToString(row["DocumentName"]),
                                               DocumentInfo = Convert.ToString(row["Instruction"]),
                                               FileName = Convert.ToString(row["FileName"]),
                                               FileExtn = Convert.ToString(row["FileExtn"]),
                                               IsApproved = Convert.ToBoolean(row["IsApproved"] == DBNull.Value ? null : row["IsApproved"]),
                                               ApprovedBy = Convert.ToString(row["ApprovedBy"] == DBNull.Value ? null : row["ApprovedBy"]),
                                               ValidatedDate = Convert.ToDateTime(row["ValidatedDate"] == DBNull.Value ? null : row["ValidatedDate"]),
                                               ValidTill = Convert.ToDateTime(row["ValidTill"] == DBNull.Value ? null : row["ValidTill"]),
                                               RejectReason = Convert.ToString(row["RejectedReason"]),
                                               Comments = Convert.ToString(row["Comments"]),
                                               ModifiedBy = Convert.ToInt32(row["ModifiedBy"]),
                                               ModifiedDateTime = Convert.ToDateTime(row["ModifiedDateTime"] == DBNull.Value ? null : row["ModifiedDateTime"]),
                                               PrerequisiteStatus = Convert.ToString(row["PrerequisiteStatus"]),
                                               ShowUpload = Convert.ToBoolean(row["ShowUpload"] == DBNull.Value ? null : row["ShowUpload"]),
                                               ShowReview = Convert.ToBoolean(row["ShowReview"] == DBNull.Value ? null : row["ShowReview"])

                                           }).ToList();

                    obj.MileageLogDetailsList = dsMileageLogList.Tables[2].AsEnumerable().Select(row =>
                                           new MileageLogDetailsList
                                           {
                                               BusinessMileageLogID = Convert.ToInt32(row["BusinessMileageLogID"]),
                                               BusinessMileageSupervisorID = Convert.ToInt32(row["BusinessMileageSupervisorID"]),
                                               TravelDateTime = Convert.ToDateTime(row["TravelDateTime"]),
                                               StartingLocation = Convert.ToString(row["StartingLocation"]),
                                               DestinationAddress = Convert.ToString(row["DestinationAddress"]),
                                               BusinessPurpose = Convert.ToString(row["BusinessPurpose"]),
                                               Miles = Convert.ToDecimal(row["Miles"]),
                                               ModifiedBy = Convert.ToInt32(row["ModifiedBy"]),
                                               ModifiedDateTime = Convert.ToDateTime(row["ModifiedDateTime"]),
                                               Rate = Convert.ToDecimal(row["Rate"] == DBNull.Value ? null : row["Rate"])

                                           }).ToList();


                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

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
        public MileageLogResponse GetBusinessMileageLog(int BusinessMileageLogID)
        {
            MileageLogResponse obj = new MileageLogResponse();


            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@BusinessMileageLogID", SqlDbType.Int,50) { Value = BusinessMileageLogID }
                                        };

            DataTable dtMileageLog = _helper.GetDataTable("[Travel].[GetBusinessMileageLog]", parameters);
            try
            {
                if (dtMileageLog.Rows.Count > 0)
                {


                    obj = dtMileageLog.AsEnumerable().Select(row =>
                                              new MileageLogResponse
                                              {
                                                  BusinessMileageLogID = Convert.ToInt32(row["BusinessMileageLogID"]),
                                                  TravelCSULBFacultySupervisorID = Convert.ToInt32(row["BusinessMileageSupervisorID"]),
                                                  TravelDateTime = Convert.ToDateTime(row["TravelDateTime"]),
                                                  StartingLocation = Convert.ToString(row["StartingLocation"]),
                                                  DestinationAddress = Convert.ToString(row["DestinationAddress"]),
                                                  BusinessPurpose = Convert.ToString(row["BusinessPurpose"]),
                                                  Miles = Convert.ToDecimal(row["Miles"]),
                                                  ModifiedBy = Convert.ToInt32(row["ModifiedBy"]),
                                                  ModifiedDateTime = Convert.ToDateTime(row["ModifiedDateTime"]),
                                                  Rate = Convert.ToDecimal(row["Rate"])

                                              }).FirstOrDefault();


                    obj.IsSuccess = true;
                    obj.Message = "Data Retrieved Successfully";

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

        public BaseResponse UpsertBusinessMileageLog(UpsertBusinessMileageLogRequest input)
        {
            BaseResponse obj = new BaseResponse();
            string mapFileName = string.Empty;
            // take the filecontent and save it in Travel/MileageMaps folder
            if(input.DirectionsMapFileContent != null && input.DirectionsMapFileContent.Length>0)
            {
                mapFileName=SaveImageInFileSystem(input.DirectionsMapFileContent,input.BusinessMileageSupervisorUserID);
            }
            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@BusinessMileageLogID", SqlDbType.BigInt, 50) { Value = input.BusinessMileageLogID },
                                          new SqlParameter("@BusinessMileageSupervisorID", SqlDbType.BigInt, 50) { Value = input.BusinessMileageSupervisorID },
                                          new SqlParameter("@BusinessMileageSupervisorUserID", SqlDbType.BigInt, 50) { Value = input.BusinessMileageSupervisorUserID },
                                          new SqlParameter("@TravelDateTime", SqlDbType.DateTime) { Value = input.TravelDateTime },
                                          new SqlParameter("@StartingLocation", SqlDbType.NVarChar, 255) { Value = input.StartingLocation },
                                          new SqlParameter("@DestinationAddress", SqlDbType.NVarChar, 255) { Value = input.DestinationAddress },
                                          new SqlParameter("@BusinessPurpose", SqlDbType.NVarChar, 25) { Value = input.BusinessPurpose },
                                          new SqlParameter("@Miles", SqlDbType.Decimal) { Value = input.Miles },
                                          new SqlParameter("@ModifiedByUserID", SqlDbType.BigInt, 50) { Value = input.ModifiedByUserID },
                                          new SqlParameter("@DirectionsJSON", SqlDbType.NVarChar, -1) { Value = input.DirectionsJSON },
                                          new SqlParameter("@DirectionsMapFileName", SqlDbType.NVarChar, 255) { Value = mapFileName }
                                        };

            int identity = _helper.InsertTable("[Travel].[UpsertBusinessMileageLog]", parameters);
            obj.IsSuccess = true;
            obj.Message = "Data Saved Successfully";

            return obj;
        }
        private string SaveImageInFileSystem(byte[] fileContent,int userID)
        {
            string fileName = string.Empty;
            var fileRepoPath = _configuration["ApplicationKeys:TravelMapFileRepository"];
            string userFolderName = userID.ToString() + "/Travel";
            string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
            fileName = "MileageLogMap" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".png";
            if (Directory.Exists(dirUserFolderPath))
            {
                string dirForm = Path.Combine(dirUserFolderPath, "MileageLogMaps");
                if (Directory.Exists(dirForm))
                {
                 
                    File.WriteAllBytes(Path.Combine(dirForm, fileName), fileContent);
                  
                }
                else
                {
                    Directory.CreateDirectory(dirForm);
             
                    File.WriteAllBytes(Path.Combine(dirForm, fileName), fileContent);

                }
            }
            else
            {
                string dirForm = Path.Combine(dirUserFolderPath, "MileageLogMaps");
                DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirForm);
                DirectorySecurity dSecurity = dirFieldWorkFolder.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dirFieldWorkFolder.SetAccessControl(dSecurity);
            
                    File.WriteAllBytes(Path.Combine(dirForm, fileName), fileContent);
                
            }
            return fileName;
        }

        public BaseResponse UpdatePrerequisitesDocument(UpdatePrerequisitesDocumentRequest input,Boolean isValidation)
        {
            BaseResponse response = new BaseResponse();
            string fileName = string.Empty;
            string fileExtension = string.Empty;
            string userFolderName = string.Empty;
            string savedFileName = string.Empty;
            var fileRepoPath = _configuration["ApplicationKeys:FileRepository"];
            FormAttachmentFileNames fileNames = GetPrerequisitesDocumentFileName(input.TravelPrerequisiteID, input.DocumentID);
            if (input.FileName != string.Empty)
            {
                ThoughtFocus.Common.Utilities.Implementation.AttachmentFileDetails fileDetails = _util.GetAttachedFileSplitValues(input.FileName);
                // fileName = fileDetails.FileName;
                fileExtension = fileDetails.FileExtension;
                // savedFileName = input.FieldWorkAttachmentId+fileDetails.FileName.ToString()+DateTime.Now.ToString("MMddyyyyHHmmss");
            }
            // call the current file name from DB  and delete the file from the file system and then run the below ``
            // first time upload , the filename will be null 
            // call the SP to save the save the file details in fieldwork.attachments table 
            SqlParameter[] parameters =
                                      {
                                          new SqlParameter("@TravelPrerequisiteID", SqlDbType.BigInt) { Value = input.TravelPrerequisiteID },
                                          new SqlParameter("@BusinessMileageSupervisorUserID", SqlDbType.BigInt) { Value = input.BusinessMileageSupervisorUserID },
                                          new SqlParameter("@FileName", SqlDbType.VarChar, 250) { Value = fileNames.FileName },
                                          new SqlParameter("@FileExtn", SqlDbType.VarChar, 20) { Value = fileExtension },
                                          new SqlParameter("@SavedFileName", SqlDbType.VarChar, 100) { Value = isValidation==true?string.Empty:fileNames.SavedFileName },
                                          new SqlParameter("@ValidTill", SqlDbType.DateTime) { Value = input.ValidTill },
                                          new SqlParameter("@Comments", SqlDbType.VarChar, -1) { Value = input.Comments },
                                          new SqlParameter("@IsApproved", SqlDbType.Bit) { Value = input.IsApproved },
                                          new SqlParameter("@ApprovedBy", SqlDbType.BigInt) { Value = input.ApprovedBy },
                                          new SqlParameter("@RejectedReason", SqlDbType.VarChar, 500) { Value = input.RejectedReason },

                                        };
            DataTable dtFWDoc = _helper.GetDataTable("[Travel].[UpdatePrerequisitesDocument]", parameters);
            if (dtFWDoc.Rows.Count > 0 && input.FileName != string.Empty && !isValidation)
            {
                // check if the userFolder exists and if it exists then check if if the FieldWork Folder exists
                string[] folderSplit = dtFWDoc.Rows[0]["FolderName"].ToString().Split('~');
                userFolderName = folderSplit[0].ToString();
                string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
                if (Directory.Exists(dirUserFolderPath))
                {
                    string dirFieldWork = Path.Combine(dirUserFolderPath, "Travel");
                    if (Directory.Exists(dirFieldWork))
                    {
                        // copy the file here 
                        File.WriteAllBytes(Path.Combine(dirFieldWork, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                    }
                    else
                    {
                        Directory.CreateDirectory(dirFieldWork);
                        File.WriteAllBytes(Path.Combine(dirFieldWork, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                    }
                }
                else
                {
                    string dirFieldWork = Path.Combine(dirUserFolderPath, "Travel");
                    DirectoryInfo dirUserFolder = System.IO.Directory.CreateDirectory(dirUserFolderPath);
                    DirectoryInfo dirFieldWorkFolder = System.IO.Directory.CreateDirectory(dirFieldWork);
                    DirectorySecurity dSecurity = dirFieldWorkFolder.GetAccessControl();
                    dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                    dirFieldWorkFolder.SetAccessControl(dSecurity);

                    File.WriteAllBytes(Path.Combine(dirFieldWork, fileNames.SavedFileName + "." + fileExtension), input.FileContent);
                }
                // now delete the old file based on the file name return from DB call above 
            }

            response.IsSuccess = true;
            response.Message = "Document Uploaded Successfully";
            return response;
        }
        private FormAttachmentFileNames GetPrerequisitesDocumentFileName(int TravelPrerequisiteID, int documentID)
        {
            FormAttachmentFileNames fileNames = new FormAttachmentFileNames();
            string savedFileName = string.Empty;
            SqlParameter[] parameters =
                                    {

                                          new SqlParameter("@TravelPrerequisiteID", SqlDbType.BigInt) { Value = TravelPrerequisiteID },
                                          new SqlParameter("@DocumentID", SqlDbType.BigInt) { Value = documentID }
                                    };
            DataTable dtName = _helper.GetDataTable("[Travel].[GetPrerequisitesDocumentFileName]", parameters);

            if (dtName.Rows.Count > 0)
            {
                fileNames.SavedFileName = Convert.ToString(dtName.Rows[0]["SavedFileName"]);
                fileNames.FileName = Convert.ToString(dtName.Rows[0]["FileName"]);
                fileNames.UserFolder = Convert.ToString(dtName.Rows[0]["UserFolder"]);
            }

            return fileNames;
        }

        public TravelPrerequisiteAttachments DownloadPrerequisitesDocument(int TravelPrerequisiteID, int BusinessMileageSupervisorUserID)
        {
            TravelPrerequisiteAttachments obj = new TravelPrerequisiteAttachments();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@TravelPrerequisiteID", SqlDbType.BigInt) { Value = TravelPrerequisiteID },
                                          new SqlParameter("@BusinessMileageSupervisorUserID", SqlDbType.BigInt) { Value = BusinessMileageSupervisorUserID }
                                     };
            DataTable dtAttachments = _helper.GetDataTable("[Travel].[DownloadPrerequisitesDocument]", parameters);

            obj = dtAttachments.AsEnumerable().Select(row =>
                                          new TravelPrerequisiteAttachments
                                          {
                                              TravelAttachmentID = Convert.ToInt32(row["ID"]),
                                              BusinessMileageSupervisorUserID = Convert.ToInt32(row["BusinessMileageSupervisorUserID"]),
                                              DocumentID = Convert.ToInt32(row["DocumentID"]),
                                              FileName = Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]),
                                              IsApproved = Convert.ToBoolean(row["IsApproved"] == DBNull.Value ? null : row["IsApproved"]),
                                              ApprovedBy = Convert.ToString(row["ApprovedBy"] == DBNull.Value ? null : row["ApprovedBy"]),
                                              ValidatedDate = Convert.ToDateTime(row["ValidatedDate"] == DBNull.Value ? null : row["ValidatedDate"]),
                                              ValidTill = Convert.ToDateTime(row["ValidTill"] == DBNull.Value ? null : row["ValidTill"]),
                                              //FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(Path.Combine(row["FolderName"].ToString(), "FieldWork"), Convert.ToString(row["FileName"]) + "." + Convert.ToString(row["FileExtn"]))
                                              FileContent = row["FileName"] == DBNull.Value || Convert.ToString(row["FileName"]) == string.Empty ? null : GetFileContent(Path.Combine(_util.GetAttachmentsFolderName(row["FolderName"].ToString()), "Travel"), _util.GetAttachmentsSavedFileName(row["FolderName"].ToString()) + "." + Convert.ToString(row["FileExtn"]))
                                          }).FirstOrDefault();

            return obj;
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

        public ReportDataResponse GetReportData(ReportDataRequest input)
        {
            ReportDataResponse obj = new ReportDataResponse();

            string reportingDates = string.Empty;
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@BusinessMileageSupervisorID", SqlDbType.BigInt) { Value = input.BusinessMileageSupervisorID },
                                          new SqlParameter("@ReportStartMonth", SqlDbType.NVarChar,10) { Value = input.ReportStartMonth },
                                          new SqlParameter("@ReportStartYear", SqlDbType.NVarChar,10) { Value = input.ReportStartYear },
                                          new SqlParameter("@ReportEndMonth", SqlDbType.NVarChar,10) { Value = input.ReportEndMonth },
                                          new SqlParameter("@ReportEndYear", SqlDbType.NVarChar,10) { Value = input.ReportEndYear }
                                     };
            DataSet dsReportData = _helper.GetDataSet("[Travel].[GetReportData]", parameters);
            DataTable dtLogs=dsReportData.Tables[0].Copy();
            DataTable dtDirections=dsReportData.Tables[1].Copy();
            DataTable dtPersonalInfo = dsReportData.Tables[2].Copy();
            //reportingDates = ReportStartDate.ToString("MMM-dd-yyyy")+" - "+ ReportEndDate.ToString("MMM-dd-yyyy");
            obj.FileName = "Mileage-Report-" + DateTime.Now.ToString("MMddyyyyHHmmss") + ".pdf";
            obj.FileContent = GetReportDataContent(input.BusinessMileageSupervisorID, dtLogs, dtDirections,dtPersonalInfo, reportingDates);
            return obj;
        }

        private byte[] GetReportDataContent(int BusinessMileageSupervisorID, DataTable dtLogs,DataTable dtDirections,DataTable dtPersonalInfo,string reportingDates)
        {
            byte[] inputStream = null;
            StringBuilder sbReportData = new StringBuilder();
            StringBuilder sbLogData = new StringBuilder();
            // get personal info related data 
            string CSULBID=string.Empty;
            string employeeName = string.Empty;
            string email = string.Empty;
            string mailingAddress = string.Empty;
            string city = string.Empty;
            string state = string.Empty;
            string zip = string.Empty;
            string phone = string.Empty;
            if (dtPersonalInfo.Rows.Count>0)
            {
                CSULBID = Convert.ToString(dtPersonalInfo.Rows[0]["CSULBID"]);
                employeeName = Convert.ToString(dtPersonalInfo.Rows[0]["BusinessMileageSupervisorName"]);
                email = Convert.ToString(dtPersonalInfo.Rows[0]["Email"]);
                mailingAddress = Convert.ToString(dtPersonalInfo.Rows[0]["MailingAddress"]);
                city = Convert.ToString(dtPersonalInfo.Rows[0]["City"]);
                state = Convert.ToString(dtPersonalInfo.Rows[0]["State"]);
                zip = Convert.ToString(dtPersonalInfo.Rows[0]["Zip"]);
                phone = Convert.ToString(dtPersonalInfo.Rows[0]["Phone"]);
                reportingDates = Convert.ToString(dtPersonalInfo.Rows[0]["ReportingDates"]);
            }

            // loop through all the mileage logs
            double finalAmount=0.00;
            for (int y = 0; y < dtLogs.Rows.Count; y++)
            {
                string travelDateTime = string.Empty;
                string startingAddress = string.Empty;
                string destinationAddress = string.Empty;
                string businessPurpose=string.Empty;
                double miles;
                double rate;
                double amount;

                startingAddress = Convert.ToString(dtLogs.Rows[y]["StartingLocation"]);
                destinationAddress = Convert.ToString(dtLogs.Rows[y]["DestinationAddress"]);
                travelDateTime = Convert.ToDateTime(dtLogs.Rows[y]["TravelDateTime"]).ToString("MMM-dd-yyyy");
                businessPurpose = Convert.ToString(dtLogs.Rows[y]["BusinessPurpose"]);
                miles = Math.Round(Convert.ToDouble(dtLogs.Rows[y]["Miles"]),1);
                rate = Convert.ToDouble(dtLogs.Rows[y]["Rate"]);
                amount = Math.Round(Convert.ToDouble(dtLogs.Rows[y]["Amount"]), 2);
                finalAmount = finalAmount + amount;
                string strLogs=ConstructMileageLogRows(travelDateTime,startingAddress,destinationAddress,businessPurpose,miles.ToString(),rate.ToString(),amount.ToString());
                sbLogData.Append(strLogs);
            }
            // now append the total below log data 
            sbLogData.Append("<tr><td colspan='5'>&nbsp;</td><td bgcolor='#FFFFE0'><b>Total</b></td><td bgcolor='#FFFFE0'><b>$" + Math.Round(finalAmount,2).ToString() +"</b></td></tr>");
            // loop through the directions datatable
            for (int i=0;i<dtDirections.Rows.Count; i++)
            {
                string startingAddress = string.Empty;
                string destinationAddress=string.Empty;
                string directionsJSON=string.Empty;
                string directionsMapFileName=string.Empty;
                string UserID = string.Empty;

                startingAddress = Convert.ToString(dtDirections.Rows[i]["StartingLocation"]);
                destinationAddress = Convert.ToString(dtDirections.Rows[i]["DestinationAddress"]);
                directionsJSON = Convert.ToString(dtDirections.Rows[i]["DirectionsJSON"]);
                directionsMapFileName = Convert.ToString(dtDirections.Rows[i]["DirectionsMapFileName"]);
                UserID = Convert.ToString(dtDirections.Rows[i]["UserID"]);

                string strDirection=ConstructDirectionsTableData(directionsJSON,startingAddress,destinationAddress,directionsMapFileName);
                sbReportData.Append("<tr><td style='text-align: center;'><img src='SupportFiles/Img/STARTING_ADDRESS.png' height='20px' width='20px' /></td><td style='font-size: 10px;'> Starting Location : " + startingAddress + "</td></tr>");
                sbReportData.Append(strDirection);
                sbReportData.Append("<tr><td align='center'><img src='SupportFiles/Img/DESTINATION_ADDRESS.png' height='20px' width='20px' /></td><td style='font-size: 10px;'> Destination : " + destinationAddress + "</td></tr>");
                sbReportData.Append("<tr><td colspan='2'><img src='"+ GenerateImgURL(UserID,directionsMapFileName)+ "' /></td></tr>");
                sbReportData.Append("<tr><td colspan='2'> &nbsp; </td > </tr>");
                

            }
            // push the data into template 
            string template = GetMileageReportTemplate("MileageReportTemplate.html");
            template = template.Replace("[[dataLogs]]", sbLogData.ToString())
                               .Replace("[[dataRows]]", sbReportData.ToString())
                               .Replace("[[reportingDates]]", reportingDates.ToString())
                               .Replace("[[CSULBID]]", CSULBID.ToString())
                               .Replace("[[employeeName]]", employeeName.ToString())
                               .Replace("[[Email]]", email.ToString())
                               .Replace("[[phone]]", phone.ToString())
                               .Replace("[[MailingAddress]]", mailingAddress.ToString())
                               .Replace("[[City]]", city.ToString())
                               .Replace("[[State]]", state.ToString())
                               .Replace("[[Zip]]", zip.ToString());
            inputStream = GetPDFFileContent(template);
            return inputStream;
        }

        private string GenerateImgURL(string UserID,string fileName)
        {
            string imgURL = string.Empty;
            var fileRepoPath = _configuration["ApplicationKeys:TravelMapFileRepository"];
            string userFolderName = UserID.ToString() + "/Travel";
            string dirUserFolderPath = Path.Combine(fileRepoPath, userFolderName);
            string filePath = "MileageLogMaps/" + fileName;
            imgURL= Path.Combine(dirUserFolderPath, filePath);
            return imgURL;
        }
        private byte[] GetPDFFileContent(string htmlFormBody)
        {
            byte[] fileContent = null;
            StringReader sr = new StringReader(htmlFormBody); // workable code uncomment after testing 
            //TextReader sr = new StringReader(htmlFormBody);
            //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            //Document pdfDoc = new Document(PageSize.A4, 16, 16, 25, 20); //portrait mode
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 25, 25, 16, 16);//landscape mode

            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                iTextSharp.text.pdf.PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();

                htmlparser.Parse(sr);
                pdfDoc.Close();

                fileContent = memoryStream.ToArray();
                memoryStream.Close();
            }
            return fileContent;
        }
        private string GetMileageReportTemplate(string templateName)
        {
            string body = string.Empty;
            string filepath = Path.Combine("SupportFiles/DocumentTemplates", templateName);
            using (StreamReader reader = new StreamReader(Path.GetFullPath(filepath)))
            {
                body = reader.ReadToEnd();
            }
            return body;
        }
        private string ConstructDirectionsTableData(string directionsJSON,string directionsMapFileName,string startingAddress,string destinationAddress)
        {
            StringBuilder sbDirections = new StringBuilder();
            JArray fields = JArray.Parse(directionsJSON);
            foreach (JObject jObject in fields)
            {
                string tableData = string.Empty;
                string mapImage = string.Empty;
                string narrative = (string)jObject["narrative"];
                int turnType = (int)jObject["turnType"];
                // get the start and end destination

                // get the sub template and bind data - only directions from JSON
                tableData = BindUniqueDirections(turnType, narrative);
                // bind the image after the directions
                sbDirections.Append(tableData);

            }
            return sbDirections.ToString();
        }
        private string ConstructMileageLogRows(string travelDateTime,string startingAddress,string destinationAddress,string businessPurpose,string miles,string rate,string amount)
        {
            StringBuilder sbRows=new StringBuilder();
            sbRows.Append("<tr>");
            sbRows.Append("<td width='10%' style='font-size: 10px;'>" + travelDateTime+"</td>");
            sbRows.Append("<td width='27%' style='font-size: 10px;'>" + startingAddress + "</td>");
            sbRows.Append("<td width='27%' style='font-size: 10px;'>" + destinationAddress + "</td>");
            sbRows.Append("<td width='8%'  style='font-size: 10px;'>" + businessPurpose + "</td>");
            sbRows.Append("<td style='font-size: 10px;'>" + miles+"</td>");
            sbRows.Append("<td style='font-size: 10px;'>" + rate + "</td>");
            sbRows.Append("<td width='10%' style='font-size: 10px;'>" + amount + "</td>");
            sbRows.Append("</tr>");
            return sbRows.ToString();
        }
        private string BindUniqueDirections(int turnType, string narrative)
        {
            string tableData = string.Empty;
            string imagePath = string.Empty;
            imagePath = GetTurnTypeImage(turnType);
            tableData = DirectionsTableData(imagePath, narrative);
            return tableData;
        }
        private string GetTurnTypeImage(int turnType)
        {
            string imgPath = string.Empty;
            switch (turnType)
            {
                case 0:
                    imgPath = "STRAIGHT.png";// straight
                    break;
                case 1:
                    imgPath = "SLIGHT_RIGHT.png";// slight right
                    break;
                case 2:
                    imgPath = "TURN_RIGHT.png";// right
                    break;
                case 3:
                    imgPath = "SHARP_RIGHT.png";// sharp right
                    break;
                case 6:
                    imgPath = "TURN_LEFT.png";// left
                    break;
                case 7:
                    imgPath = "SLIGHT_LEFT.png";// slight left
                    break;
                case 8:
                    imgPath = "U_TURN_RIGHT.png";// right U turn
                    break;
                case 9:
                    imgPath = "U_TURN_LEFT.png";// left U turn
                    break;
                case 10:
                    imgPath = "MERGE.png";// right merge
                    break;
                case 11:
                    imgPath = "MERGE.png";// left merge
                    break;
                case 12:
                    imgPath = "RAMP_RIGHT.png";// right on ramp
                    break;
                case 13:
                    imgPath = "RAMP_LEFT.png";// left on ramp
                    break;
                case 16:
                    imgPath = "FORK_RIGHT.png";// right fork
                    break;
                case 17:
                    imgPath = "FORK_LEFT.png";// left fork
                    break;
                default:
                    imgPath = "STRAIGHT.png";// straight
                    break;
            }
            string filepath = Path.Combine("SupportFiles/Img", imgPath);
            return filepath;

        }
        private string DirectionsTableData(string imagePath, string narrative)
        {
            StringBuilder strTableData = new StringBuilder();
            strTableData.Append("<tr>");
            strTableData.Append("<td width='10%' text-align='center'><img src=" + imagePath + " height='20px' width='20px'/></td>");
            strTableData.Append("<td style='font-size: 10px;'>" + narrative + "</td>");
            strTableData.Append("</tr>");
            return strTableData.ToString();
        }
    }
}
