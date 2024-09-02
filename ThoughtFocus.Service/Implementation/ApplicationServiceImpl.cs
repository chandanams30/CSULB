using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.Application;
using ThoughtFocus.Repository.Interfaces;
using ThoughtFocus.Repository.Interfaces.User;
using ThoughtFocus.Service.Interfaces;
using ThoughtFocus.Domain.Response;
using ThoughtFocus.DataAccess.Models;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Data;
using ThoughtFocus.DataAccess.DBHelper;

namespace ThoughtFocus.Service.Implementation
{
    public class ApplicationServiceImpl : IApplicationService
    {
        private readonly CSULB_DBContext _context;
        private readonly ISqlDBUtility _helper;
        public ApplicationServiceImpl(CSULB_DBContext context
                                     , ISqlDBUtility helper)
        {
            _context = context;
            _helper = helper;
        }
        public List<ApplicationListResponse> GetApplications(int userId)
        {
            #region Linq Statement
            //var obj = _context.Forms
            //    .Join(_context.Programs, forms => forms.ProgramId, prog => prog.Id, (forms, prog) => new { forms, prog })//.DefaultIfEmpty()
            //    .Join(_context.ApplicationTypes, pro => pro.prog.ApplicationTypesId, at => at.Id, (pro, at) => new { pro, at })//.DefaultIfEmpty()
            //    .GroupBy(g=> new { g.at.Id,g.at.Description})
            //    .Select(data => new ApplicationListResponse
            //    {
            //        ApplicationId = Convert.ToInt32(data.Key.Id),
            //        ApplicationName=data.Key.Description,
            //        Count = data.Count()
            //    }).ToList();
            #endregion

            // gets the list of applications
            List<ApplicationListResponse> obj = new List<ApplicationListResponse>();

            SqlParameter[] parameters =
                                  {
                                    new SqlParameter("@UserID", SqlDbType.NVarChar, 255) { Value = userId}
                                  };

            DataTable dtApplications = _helper.GetDataTable("[dbo].[GetApplications]", parameters);
            if (dtApplications.Rows.Count > 0)
            {
                obj = dtApplications.AsEnumerable().Select(row =>
                                         new ApplicationListResponse
                                         {
                                             ApplicationId = Convert.ToInt32(row["ID"]),
                                             ApplicationName= Convert.ToString(row["Name"])
                                         }).ToList();
            }

            return obj;
        }

        public StudentNotificationResponse GetStudentNotification(string CSULBID)
        {
            StudentNotificationResponse obj = new StudentNotificationResponse();

            SqlParameter[] parameters =
                                  {
                                    new SqlParameter("@csulbid", SqlDbType.NVarChar, 255) { Value = CSULBID}
                                  };

            DataTable dtNotification = _helper.GetDataTable("[User].[GetStudentNotification]", parameters);
            if (dtNotification.Rows.Count > 0)
            {
                obj = dtNotification.AsEnumerable().Select(row =>
                                         new StudentNotificationResponse
                                         {
                                             ShowNotification = Convert.ToBoolean(row["ShowNotification"]),
                                             Message = Convert.ToString(row["Message"]),
                                             ShowAgreement = Convert.ToBoolean(row["ShowAgreement"])
                                         }).FirstOrDefault();
            }

            return obj;
        }
    }
}
