using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using ThoughtFocus.Common.Exceptions;
using ThoughtFocus.Common.Workflow.Core.Runtime;
//using ThoughtFocus.RoleProvider.Interfaces;
//using ThoughtFocus.Repository.Interfaces.User;
//using ThoughtFocus.DataAccess.Models.User;

namespace ThoughtFocus.Workflow
{
    public class WorkflowRole : IWorkflowRoleProvider
    {
        //private IListRole _listRole;
        //private IUserRepository _iUserRepository;

        private static readonly ILog Logger = LogManager.GetLogger(typeof(WorkflowRole));
       
        

        public WorkflowRole(
            //IListRole listRole, IUserRepository iUserRepository
            )
        {
            //_listRole =listRole;
            //_iUserRepository = iUserRepository;
        }

        public bool IsInRole(long identityId, string roleName, long processID)
        {
            bool isInRole = false;
            long userID = 0;
           
            try
            {
                isInRole = true;
                    //long roleID = this._listRole.GetRoleIDByRoleName(roleName);

                    //User user = this._iUserRepository.FirstOrDefault(item => item.IdentityID == identityId && item.IsActive == true);
                    //if (user != null)
                    //{
                    //    userID = user.UserID;
                    //}

                    // List<long> listOfRoles = this._listRole.GetRolesByID(userID);
                    //if (listOfRoles != null && listOfRoles.Count > 0)
                    //{
                    //    if (listOfRoles.Contains(roleID))
                    //    {
                    //        isInRole = true;
                    //    }
                    //}
                    //else
                    //{
                    //    isInRole = false;
                    //}
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
    }
}
