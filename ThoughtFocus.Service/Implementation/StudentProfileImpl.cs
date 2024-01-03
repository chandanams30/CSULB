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
                        Phone = Convert.ToString(row["Phone"]),
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

    }
}
