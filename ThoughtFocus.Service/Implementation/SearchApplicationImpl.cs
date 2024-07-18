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
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class SearchApplicationImpl : ISearchApplication
    {
        private readonly ISqlDBUtility _helper;
        private readonly IConfiguration _configuration;
        private readonly ISendMail _sendMail;
        public ILogger<SearchApplicationImpl> _logger;
        public SearchApplicationImpl(ISqlDBUtility helper
                                         , IConfiguration configuration
                                         , ISendMail sendMail
                                         , ILogger<SearchApplicationImpl> logger)
        {
            _helper = helper;
            _configuration = configuration;
            _sendMail = sendMail;
            _logger = logger;
        }
        public StudentSearchResponse GetStudentSearchData(string searchString)
        {
            StudentSearchResponse obj = new StudentSearchResponse();
            SqlParameter[] parameters =
                                     {
                                          new SqlParameter("@searchString", SqlDbType.NVarChar,100) { Value = searchString }
                                     };
            DataTable dtResponse = _helper.GetDataTable("[dbo].[SearchStudent]", parameters);
            if (dtResponse.Rows.Count > 0)
            {
                obj.studentSearch = dtResponse.AsEnumerable().Select(row =>
                                              new StudentSearch
                                              {
                                                  ID = Convert.ToInt32(row["ID"]),
                                                  FirstName = Convert.ToString(row["FirstName"]),
                                                  LastName = Convert.ToString(row["LastName"]),
                                                  EMAIL = Convert.ToString(row["EMAIL"]),
                                                  CSULBID = Convert.ToString(row["CSULBID"]),
                                                  Type = Convert.ToString(row["Type"]),
                                                  UserID = Convert.ToInt32(row["UserID"]),
                                                  TermCode = row["TermCode"]==DBNull.Value?"": Convert.ToString(row["TermCode"]),
                                                  ProgramID = row["ProgramID"]==DBNull.Value?(int?)null: Convert.ToInt32(row["ProgramID"]),
                                                  ApplicationTypeID = Convert.ToInt32(row["ApplicationTypeID"]),
                                                  ProgramName = Convert.ToString(row["ProgramName"]),
                                                  Term = Convert.ToString(row["Term"]),
                                                  Status = Convert.ToString(row["Status"])

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
