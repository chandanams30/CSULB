using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ThoughtFocus.Common.Utilities.Interfaces;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Domain.Request.StudentProfile;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.Domain.Response.SearchApplication;
using ThoughtFocus.Domain.Response.StudentProfile;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class StudentProfileImpl : IStudentProfile
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        public ILogger<SearchApplicationImpl> _logger;

        public StudentProfileImpl(ISqlDBUtility helper
                                         , IConfiguration configuration
                                         , ISendMail sendMail
                                         , ILogger<SearchApplicationImpl> logger)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
        }
        public StudentProfileResponse GetStudentProfileData(string CsuldId)
        {
            StudentProfileResponse obj = new StudentProfileResponse();
            SqlParameter[] parameters = {
                                          new SqlParameter("@csulbid", SqlDbType.VarChar, 50) { Value = CsuldId }
                                        };

            DataTable dtStudentProfile = _helper.GetDataTable("[dbo].[GetStudentProfile]", parameters);
            try
            {
                if (dtStudentProfile.Rows.Count > 0)
                {
                    obj.studentProfile = dtStudentProfile.AsEnumerable().Select(row => new StudentProfile
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
                        AcademicPlan = Convert.ToString(row["AcademicPlan"]),
                        AcademicSubPlan = Convert.ToString(row["AcademicSubPlan"]),
                        AdditionalPlan = Convert.ToString(row["AdditionalPlan"]),
                        ProgramStatusDesc = Convert.ToString(row["ProgramStatusDesc"]),
                        GraduationFillingStatusDesc = Convert.ToString(row["GraduationFilingStatusDescr"]),
                        CurrentCsulbGpa = Convert.ToString(row["CurrentCSULBGPA"]),
                        CumulativeGpa = Convert.ToString(row["CumulativeGPA"]),
                        MajorGpa = Convert.ToString(row["MajorGPA"]),
                        AcademicStanding = Convert.ToString(row["AcademicStanding"]),
                        BachelorDegreeMajor = Convert.ToString(row["BachelorDegreeMajor"]),
                        AdmitTerm = Convert.ToString(row["AdmitTerm"]),
                        ActiveTerm = Convert.ToString(row["ActiveTerm"]),
                        GraduationFillingTerm = Convert.ToString(row["GraduationFillingTerm"]),
                        EducationalLeaveTerm = Convert.ToString(row["EducationalLeaveTerm"]),
                        Credential = Convert.ToString(row["Credential"]),
                        Certificate = Convert.ToString(row["Certificate"]),
                        DateOfBirth = Convert.ToDateTime(row["DateOfBirth"] == DBNull.Value ? null : row["DateOfBirth"]),
                        SSNNumber = Convert.ToString(row["SSNNumber"] == DBNull.Value ? null : row["SSNNumber"]),
                        AcademicIntegrityStatement = Convert.ToString(row["AcademicIntegrityStatement"] == DBNull.Value ? null : row["AcademicIntegrityStatement"]),
                        SubmittedDate = Convert.ToDateTime(row["SubmittedDate"] == DBNull.Value ? null : row["SubmittedDate"])
                    }).FirstOrDefault();
                    if (!string.IsNullOrEmpty(obj.studentProfile.SSNNumber) && obj.studentProfile.SSNNumber != null)
                    {
                        //IsApproved = Convert.ToBoolean(row["IsApproved"] == DBNull.Value ? null : row["IsApproved"]),
                        obj.studentProfile.SSNNumber = DecryptSSNNumber(obj.studentProfile.SSNNumber);
                    }
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
        public StudentProfileSearchResponse GetStudentProfileSearchData(string searchString)
        {
            StudentProfileSearchResponse obj = new StudentProfileSearchResponse();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@searchString", SqlDbType.NVarChar,100) { Value = searchString }
                                     };
            DataTable dtResponse = _helper.GetDataTable("[dbo].[SearchStudentProfile]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                obj.studentProfileSearch = dtResponse.AsEnumerable().Select(row =>
                                              new StudentProfileSearch
                                              {
                                                  ID = Convert.ToInt32(row["ID"]),
                                                  FirstName = Convert.ToString(row["FirstName"]),
                                                  LastName = Convert.ToString(row["LastName"]),
                                                  EMAIL = Convert.ToString(row["CSULBEmail"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),

                                              }).ToList();
                obj.IsSuccess = true;
                obj.Message = "Data retrieved succesfully ";

            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No data matching this search criteria";
            }
            return obj;

        }
        public StudentProfileMessageBoardResponse GetStudentProfileMessageBoard(string CsulbId, string MessageBoardIdentifier)
        {
            StudentProfileMessageBoardResponse obj = new StudentProfileMessageBoardResponse();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@CSULBID", SqlDbType.NVarChar,25) { Value = CsulbId },
                                          new SqlParameter("@MessageBoardIdentifier", SqlDbType.NVarChar,20) { Value = MessageBoardIdentifier}
                                     };
            DataTable dtResponse = _helper.GetDataTable("[dbo].[GetStudentProfileMessageBoard]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                obj.studentProfileMessageBoards = dtResponse.AsEnumerable().Select(row =>
                                              new StudentProfileMessageBoard
                                              {
                                                  MessageBoard = Convert.ToString(row["MessageBoard"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),

                                              }).FirstOrDefault();
                obj.IsSuccess = true;
                obj.Message = "Data retrieved succesfully ";

            }
            else
            {
                obj.IsSuccess = false;
                obj.Message = "No data matching this search criteria";
            }
            return obj;

        }
        public BaseResponse UpdateStudentProfileMessageBoard(UpdateStudentProfileMessageBoardRequest input)
        {
            BaseResponse response = new BaseResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@CSULBID", SqlDbType.NVarChar,25) { Value = input.CSULBID },
                                          new SqlParameter("@MessageBoardIdentifier", SqlDbType.NVarChar,20) { Value = input.MessageBoardIdentifier },
                                          new SqlParameter("@MessageBoard", SqlDbType.VarChar,-1) { Value = input.MessageBoard }
                                        };

            DataTable recomDetails = _helper.GetDataTable("[dbo].[UpdateStudentProfileMessageBoard]", parameters);
            response.Message = "Updated the student profile message board successfully";
            response.IsSuccess = true;
            return response;
        }
        public BaseResponse SaveStudentProfileData(SaveStudentProfileDataRequest input)
        {
            BaseResponse response = new BaseResponse();
            if (!string.IsNullOrEmpty(input.SSNNumber) && input.SSNNumber != null)
            {
                input.SSNNumber = EncryptSSNNumber(input.SSNNumber);
            }

            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@CSULBID", SqlDbType.NVarChar,25) { Value = input.CSULBID  },
                                          new SqlParameter("@DateOfBirth", SqlDbType.DateTime) { Value = input.DateOfBirth  },
                                          new SqlParameter("@SSNNumber", SqlDbType.VarChar,-1) { Value = input.SSNNumber },
                                          new SqlParameter("@AcademicIntegrityStatement", SqlDbType.VarChar,-1) { Value = input.AcademicIntegrityStatement },
                                          new SqlParameter("@SubmittedDate", SqlDbType.DateTime) { Value = input.SubmittedDate }
                                        };

            int id = _helper.InsertTable("[dbo].[SaveStudentProfile]", parameters);
            response.Message = "Student profile data saved successfully";
            response.IsSuccess = true;
            return response;
        }
        private string EncryptSSNNumber(string clearText)
        {
            string encryptionKey = _configuration["ApplicationKeys:EncryptionKey"];
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return clearText;
        }
        private string DecryptSSNNumber(string cipherText)
        {
            string encryptionKey = _configuration["ApplicationKeys:EncryptionKey"];
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
        }
    }
}
