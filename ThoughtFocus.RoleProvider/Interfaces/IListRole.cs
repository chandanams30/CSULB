//namespace ThoughtFocus.ABA.Accreditation.RoleProvider.Interfaces
//{
//    using System;
//    using System.Collections.Generic;
//    using ThoughtFocus.ABA.Accreditation.Domain.CustomView;
//    using ThoughtFocus.ABA.Accreditation.DataAccess.Models;

//    public interface IListRole
//    {
//        #region Methods

//        List<RoleListingViewEntity> GetRoleList();

//        List<RolePermission> GetGlobalRolePermissions(Int64 userId);

//        //List<AccessPermissionViewEntity> GetAccessControlRolePermissions(Int64 userId);

//        List<long> GetSiteVisitRoles(Int64 userID, SiteVisit siteVisit);

//        List<RoleStatePermission> GetSiteVisitRolePermissions(long userID, long siteVisitID);

//        long GetRoleIDByRoleName(string roleName);

//        List<RoleEntityPermission> GetRoleEntityPermission(long userID, long SiteVisitID);

//        List<long> GetSiteVisitUsersByRoleID(List<long> roleID, long siteVisistID);
      


//        #endregion Methods
//    }
//}