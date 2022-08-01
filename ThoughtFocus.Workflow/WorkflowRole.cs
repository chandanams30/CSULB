using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using ThoughtFocus.Common.Exceptions;
using ThoughtFocus.Common.Workflow.Core.Runtime;
using Microsoft.Data.SqlClient;
using System.Data;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.Domain.Response.Users;

namespace ThoughtFocus.Workflow
{
    public class WorkflowRole : IWorkflowRoleProvider
    {
        private readonly ISqlDBUtility _helper;
 
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WorkflowRole));
       
        

        public WorkflowRole(ISqlDBUtility helper)
        {
            _helper = helper;
        }

        public bool IsInRole(long identityId, string roleName, long processID)
        {
            bool isInRole = false;
            long userID = 0;
           
            try
            {
                isInRole = true;
                DataTable dtRoleData = _helper.GetMasterTable("[Master].[Role]");

                if (dtRoleData.Rows.Count > 0)
                {
                    long roleID = dtRoleData.AsEnumerable().Where(x => Convert.ToString(x["Name"]) == roleName).Select(row => Convert.ToInt64(row["ID"])).FirstOrDefault();
                    
                    List<UsersResponse> listOfusers = this.GetUsersByRole(Convert.ToInt32(roleID));

                    if (listOfusers != null && listOfusers.Count > 0)
                    {
                        var user = listOfusers.Where(x => x.UserId == identityId).FirstOrDefault();

                        if (user == null)
                        {
                            isInRole = false;
                        }
                    }
                    else
                    {
                        isInRole = false;
                    }
                }
            }
            catch (RepositoryException ex)
            {
                //LoggerExtensions.LogMessage(Logger, ex);
                isInRole = false;
                return isInRole;
            }
            catch (Exception ex)
            {
                //LoggerExtensions.LogMessage(Logger, ex);
                isInRole = false;
                return isInRole;
                
            }
            return isInRole;

        }

        public IEnumerable<Guid> GetAllInRole(string roleId)
        {
            return new List<Guid>();
        }

        public List<UsersResponse> GetUsersByRole(int roleId)
        {
            List<UsersResponse> obj = new List<UsersResponse>();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@roleid", SqlDbType.Int, 50) { Value = roleId },
                                          new SqlParameter("@userId", SqlDbType.Int, 50) { Value = 0 },
                                        };

            DataTable dtUsers = _helper.GetDataTable("[dbo].[GetUsersByRole]", parameters);
            if (dtUsers.Rows.Count > 0)
            {
                obj = dtUsers.AsEnumerable().Select(row =>
                                         new UsersResponse
                                         {
                                             UserId = Convert.ToInt32(row["UserId"]),
                                             Name = Convert.ToString(row["Name"]),
                                             EmailId = Convert.ToString(row["Email"]),
                                             RoleId = Convert.ToInt32(row["RoleId"]),
                                             Role = Convert.ToString(row["Description"])
                                         }).ToList();
            }

            return obj;
        }
    }
}
