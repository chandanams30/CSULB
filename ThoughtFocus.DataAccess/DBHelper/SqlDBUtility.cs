using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ThoughtFocus.DataAccess.DBHelper
{
    public class SqlDBUtility: ISqlDBUtility
    {
        
        private readonly string _connectionString;
        private readonly ILogger<SqlDBUtility> _logger;
        private readonly IConfiguration _config;
        public SqlDBUtility(ILogger<SqlDBUtility> logger
                           ,IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _connectionString = _config["ConnectionStrings:AppDBConnection"];
        }

        public DataTable GetDataTable(string procedureName, params SqlParameter[] commandParameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = procedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Clear();
                    if (commandParameters != null)
                    {
                        cmd.Parameters.AddRange(commandParameters);
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                string exceptionmessage = ex.Message;
                _logger.LogError(ex, exceptionmessage);
                dt = null;
            }
            return dt;
        }

        public DataSet GetDataSet(string ProcedureName, SqlParameter[] commandParameters)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = ProcedureName;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    if (commandParameters != null)
                    {
                        cmd.Parameters.AddRange(commandParameters);
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                string exceptionmessage = ex.Message;
                _logger.LogError(ex, exceptionmessage);
                ds = null;
            }
            return ds;
        }

        public int InsertTable(string procedureName, params SqlParameter[] commandParameters)
        {
            int result = -1;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = procedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    if (commandParameters != null)
                    {
                        cmd.Parameters.AddRange(commandParameters);
                    }
                    cmd.ExecuteNonQuery();
                   // result = Convert.ToInt32(cmd.Parameters["@new_identity"].Value);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                _logger.LogError(ex, msg);
                result = -1;
            }
            return result;
        }

        public string ExecuteSPWithOutputVariable(string procedureName, params SqlParameter[] commandParameters)
        {
            string result = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = procedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    if (commandParameters != null)
                    {
                        cmd.Parameters.Add("@Message", SqlDbType.Char, 500);
                        cmd.Parameters["@Message"].Direction = ParameterDirection.Output;
                        cmd.Parameters.AddRange(commandParameters);
                    }
                    cmd.ExecuteNonQuery();
                    result = Convert.ToString(cmd.Parameters["@Message"].Value);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                _logger.LogError(ex, msg);
                result = "Error";
            }
            return result;
        }
        public int UpsertData(string procedureName, params SqlParameter[] commandParameters)
        {
            int id = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = procedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    if (commandParameters != null)
                    {
                        cmd.Parameters.AddRange(commandParameters);
                    }
                    var outputParam = new SqlParameter("@Id", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);
                    cmd.ExecuteNonQuery();
                    // result = Convert.ToInt32(cmd.Parameters["@new_identity"].Value);
                    id = (int)outputParam.Value;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                //_logger.LogError(ex, msg);
                id = -1;
                throw;
            }
            return id;
        }


    }
}
