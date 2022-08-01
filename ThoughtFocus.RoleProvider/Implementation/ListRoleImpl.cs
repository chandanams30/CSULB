//namespace ThoughtFocus.ABA.Accreditation.RoleProvider.Impl
//{
    
//    using System;
//    using System.Collections.Generic;
//    using System.Reflection;
//    using System.Linq;
//    using ThoughtFocus.ABA.Accreditation.DataAccess.Models;
//    using ThoughtFocus.ABA.Accreditation.Domain.CustomView;
//    using ThoughtFocus.ABA.Accreditation.Repository.Interfaces.Master;
//    using ThoughtFocus.ABA.Accreditation.Repository.Interfaces.SiteVisit;
//    using ThoughtFocus.ABA.Accreditation.Repository.Interfaces.User;
//    using ThoughtFocus.ABA.Accreditation.Repository.Interfaces.School;
//    using ThoughtFocus.Common.Exceptions;
//    using ThoughtFocus.ABA.Accreditation.RoleProvider.Interfaces;
//    using ThoughtFocus.ABA.Accreditation.RoleProvider.Interface;
//    using log4net;

//    public class ListRoleImpl : IListRole
//    {
//        #region Fields

//        private IRolePermissionRepository rolePermissionRepository;
//        private IRoleStatePermissionRepository roleStatePermissionRepository;
//        private ISiteVisitRepository siteVisitRepository;
//        private IRUserRoleRepository rUserRoleRepository;
//        private ISchoolPersonInfoRepository schoolPersonInfoRepository;
//        private ISiteVisitMemberRoleRepository siteVisitMemberRoleRepository;
//        private IRoleRepository _roleRepository;
//        private IRoleEntityPermissionRepository _roleEntityPermissionRepository;
//        private IRoleForStateRepository roleForStateRepository;
//        private IUserRepository userRepository;
//        private static readonly ILog Logger = LogManager.GetLogger(typeof(ListRoleImpl));

//        #endregion Fields

//        #region Constructors

//        public ListRoleImpl(IRolePermissionRepository _rolePermissionRepository, ISiteVisitRepository _siteVisitRepository, IRUserRoleRepository _rUserRoleRepository, IRoleStatePermissionRepository _roleStatePermissionRepository,
//            ISchoolPersonInfoRepository _schoolPersonInfoRepository, ISiteVisitMemberRoleRepository _siteVisitMemberRoleRepository, IRoleRepository roleRepository,
//            IRoleEntityPermissionRepository roleEntityPermissionRepository, IRoleForStateRepository _roleForStateRepository,IUserRepository _userRepository)
//        {
//            this.rolePermissionRepository = _rolePermissionRepository;
//            this.siteVisitRepository = _siteVisitRepository;
//            this.rUserRoleRepository = _rUserRoleRepository;
//            this.roleStatePermissionRepository = _roleStatePermissionRepository;
//            this.schoolPersonInfoRepository = _schoolPersonInfoRepository;
//            this.siteVisitMemberRoleRepository = _siteVisitMemberRoleRepository;
//            this._roleRepository = roleRepository;
//            this._roleEntityPermissionRepository = roleEntityPermissionRepository;
//            this.roleForStateRepository = _roleForStateRepository;
//            this.userRepository = _userRepository;
          
//        }

//        #endregion Constructors

//        #region Methods

//        public List<RoleListingViewEntity> GetRoleList()
//        {
//            try
//            {
//                List<RoleListingViewEntity> listRoleListingViewEntity = null;
//                List<Role> roleList = this._roleRepository.FindBy(a => a.IsActive == true && a.IsLoginRole == true).ToList();
//                if (roleList != null)
//                {
//                    listRoleListingViewEntity = new List<RoleListingViewEntity>();
//                    foreach (var item in roleList)
//                    {

//                        listRoleListingViewEntity.Add(new RoleListingViewEntity
//                        {
//                            RoleID = item.RoleID,
//                            RoleName = item.RoleName,
//                            RoleDescription = item.RoleDescription

//                        });
//                    }
//                }
//                return listRoleListingViewEntity;
//            }
//            catch (Exception ex)
//            {
//                Logger.Error("Error encountered at public List<RoleListingViewEntity> GetRoleList()>>", ex);
//                throw;
//            }

//        }

//        public List<long> GetSiteVisitRoles(long userID, SiteVisit siteVisit)
//        {
//            List<long> roles = new List<long>();
//            if (userID == 0)
//            {
//                Logger.Error("UserID is 0 in GetSiteVisitRoles");
//                return null;
//            }
//            if (siteVisit == null)
//            {
//                Logger.Error("SiteVisit is null in GetSiteVisitRoles");
//                return null;
//            }
//            try
//            {
//                var type = typeof(IRoleProvider);

//                var siteVisitRoleProviders =
//                                            Assembly.GetExecutingAssembly()
//                                            .GetTypes()
//                                            .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);

//                foreach (var siteVisitRoleProvider in siteVisitRoleProviders.Select(
//                           roleProvider => (IRoleProvider)Activator.CreateInstance(roleProvider, rolePermissionRepository, siteVisitRepository, rUserRoleRepository, roleStatePermissionRepository, schoolPersonInfoRepository, siteVisitMemberRoleRepository, roleForStateRepository)))
//                {
//                    roles.AddRange(siteVisitRoleProvider.FetchRoles(userID, siteVisit));
//                }
//            }
//            catch (RepositoryException ex)
//            {
//                throw new BusinessException("", ex);
//            }
//            catch (TargetInvocationException ex)
//            {
//                throw new BusinessException("", ex);
//            }
//            catch (BusinessException ex)
//            {
//                throw ex;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//            return roles;
//        }

//        public List<RolePermission> GetGlobalRolePermissions(Int64 userId)
//        {
//            List<RolePermission> rolePermissions = new List<RolePermission>();
//            //List<RolePermissionViewEntity> rolePermissionViewEntity = new List<RolePermissionViewEntity>();
//            List<long> roles = new List<long>();

//            //GetGlobal Roles
//            roles = this.rUserRoleRepository.GetGlobalRoles(userId);

//            //Get RolePermissions
//            rolePermissions = this.rolePermissionRepository.GetGlobalRolePermissions(roles);

//            return rolePermissions;
//        }

//        public List<RoleStatePermission> GetSiteVisitRolePermissions(Int64 userID, Int64 siteVisitID)
//        {
//            List<RoleStatePermission> roleStatePermissions = new List<RoleStatePermission>();
//            List<AccessPermissionViewEntity> roleStatePermissionViewEntity = new List<AccessPermissionViewEntity>();
//            List<long> roles = new List<long>();

//            if (siteVisitID == 0)
//            {
//                //GetGlobal Roles
//                roles = this.rUserRoleRepository.GetGlobalRoles(userID);

//                SiteVisitState siteVisitState = this.siteVisitRepository.GetInitialSiteVisitState();
//                roleStatePermissions = this.roleStatePermissionRepository.GetSiteVisitRolePermissions(roles, siteVisitState.SiteVisitStateID);
//            }
//            else
//            {
//                //Get Site Visit by sitevisitid
//                SiteVisit siteVisit = this.siteVisitRepository.GetSiteVisitBySiteVisitID(siteVisitID);

//                roles = GetSiteVisitRoles(userID, siteVisit);

//                //Get SiteVisitRoleFunctions
//                roleStatePermissions = this.roleStatePermissionRepository.GetSiteVisitRolePermissions(roles, siteVisit.SiteVisitStateID);
//            }

//            return roleStatePermissions;
//        }

//        public long GetRoleIDByRoleName(string roleName)
//        {
//            long roleID = 0;
//            Role role = this._roleRepository.FirstOrDefault(a => a.RoleName == roleName);
//            if (role != null)
//            {
//                roleID = role.RoleID;
//            }
//            return roleID;
//        }

//        public List<RoleEntityPermission> GetRoleEntityPermission(long userID, long SiteVisitID)
//        {
//            List<RoleEntityPermission> roleEntityPermissions = new List<RoleEntityPermission>();
//            List<long> roles = new List<long>();

//            //Get Site Visit by sitevisitid
//            SiteVisit siteVisit = this.siteVisitRepository.GetSiteVisitBySiteVisitID(SiteVisitID);

//            roles = GetSiteVisitRoles(userID, siteVisit);
//            if (roles != null)
//            {

//                foreach (var role in roles)
//                {
//                    if (role > 0)
//                    {
//                        roleEntityPermissions.AddRange(this._roleEntityPermissionRepository.FindBy(a => a.RoleID == role && a.SiteVisitID == SiteVisitID).ToList());
//                    }
//                }
//            }
//            return roleEntityPermissions;

//        }

//        //Here List of Roles is passed to reduce function call stack
//        public List<long> GetSiteVisitUsersByRoleID(List<long> roleIDs,long siteVisistID)
//        {
//            List<long> userIDs = new List<long>();
//            try
//            {
//                var type = typeof(IRoleResolver);
//                var siteVisitRoleResolvers =
//                                            Assembly.GetExecutingAssembly()
//                                            .GetTypes()
//                                            .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);

//                foreach (var siteVisitRoleResolver in siteVisitRoleResolvers.Select(
//                           roleProvider => (IRoleResolver)Activator.CreateInstance(roleProvider, rUserRoleRepository, siteVisitRepository, userRepository)))
//                {
//                    if (siteVisitRoleResolver != null && siteVisitRoleResolver.CanGetUserByRoleID(roleIDs))
//                    {
//                        var response = siteVisitRoleResolver.GetSiteVisitUsersByRoleID(siteVisistID);
//                        if(response!=null)
//                        {
//                            userIDs.AddRange(response);
//                        }
//                    }
//                }
//            }
//            catch (RepositoryException ex)
//            {
//                throw new BusinessException(String.Format("{0} at GetSiteVisitUsersByRoleID", ex.Message), ex);
//            }
//            catch (TargetInvocationException ex)
//            {
//                throw new BusinessException(String.Format("{0} at GetSiteVisitUsersByRoleID", ex.Message), ex);
//            }
//            catch (BusinessException ex)
//            {
//                throw ex;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//            return userIDs;

//        }


//        #endregion Methods


//    }
//}