using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ThoughtFocus.DataAccess.DBHelper
{
    public interface ISqlDBUtility
    {
        DataTable GetDataTable(string procedureName, params SqlParameter[] commandParameters);
        int InsertTable(string procedureName, params SqlParameter[] commandParameters);
    }
}
