using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Service.Interfaces;
using System.Linq;
using ThoughtFocus.Domain.Response.Program;
using ThoughtFocus.Domain.Enumeration;
using ThoughtFocus.Domain.TemplateModels;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.DataAccess.DBHelper;
using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Reflection;

namespace ThoughtFocus.Service.Implementation
{
    public class ProgramServiceImpl : IProgramsService
    {
        private readonly CSULB_DBContext _context;
        private readonly ISqlDBUtility _helper;


        public ProgramServiceImpl(CSULB_DBContext context
                                 ,ISqlDBUtility helper)
        {
            _context = context;
            _helper = helper;
        }
        public ProgramResponse GetProgram(int programId, int userId)
        {
            #region LINQ statement
            //var obj = _context.Programs.Where(pr=>pr.Id==programId)
            //           .Join(_context.ApplicationTypes,
            //           p => p.ApplicationTypesId,
            //           at => at.Id,
            //           (p, at) => new ProgramResponse
            //           {
            //               ProgramId = Convert.ToInt32(p.Id),
            //               ProgramName = p.Name,
            //               ApplicationTypeId = Convert.ToInt32(p.ApplicationTypesId),
            //               ApplicationName = at.Name,
            //               FormTemplate = p.FormTemplate,
            //               FormNotes = p.Notes
            //           }).FirstOrDefault();
            #endregion

            ProgramResponse obj = new ProgramResponse();
            SqlParameter[] parameters =
                                       {
                                          new SqlParameter("@ProgramId", SqlDbType.Int, 50) { Value = programId }
                                        };

            DataTable dtProgram = _helper.GetDataTable("[dbo].[GetProgram]", parameters);
            if (dtProgram.Rows.Count > 0)
            {
                obj = dtProgram.AsEnumerable().Select(row =>
                                         new ProgramResponse
                                         {
                                             ProgramId = Convert.ToInt32(row["ID"]),
                                             ProgramName = Convert.ToString(row["Name"]),
                                             Template= GetDeserializedTemplate(Convert.ToString(row["FormTemplate"]),programId)
                                         }).FirstOrDefault();
            }

                return obj;
        }

        public List<ProgramResponse> GetProgramList(int applicationTypeId,int semesterId,int stateId,int userId)
        {
            List<ProgramResponse> obj = new List<ProgramResponse>();

            #region LINQ statement 
            //var obj = _context.Forms
            //   .Join(_context.Programs, forms => forms.ApplicationId, prog => prog.Id, (forms, prog) => new { forms, prog })//.DefaultIfEmpty()
            //   .Join(_context.ApplicationTypes, pro => pro.prog.ApplicationTypesId, at => at.Id, (pro, at) => new { pro, at })//.DefaultIfEmpty()
            //   .Where(ati=>ati.pro.prog.ApplicationTypesId==applicationTypeId)
            //   .GroupBy(g => new { g.pro.prog.Id, g.pro.prog.Description ,g.pro.prog.ApplicationTypesId})
            //   .Select(data => new ProgramResponse
            //   {
            //       ProgramId = Convert.ToInt32(data.Key.Id),
            //       ProgramName = data.Key.Description,
            //       ApplicationTypeId=Convert.ToInt32(data.Key.ApplicationTypesId),
            //       Count = data.Count()
            //   }).ToList();
            #endregion

            
            SqlParameter[] parameters =
                                        {
                                          new SqlParameter("@ApplicationTypeId", SqlDbType.Int, 50) { Value = applicationTypeId },
                                          new SqlParameter("@SemesterId", SqlDbType.Int, 50) { Value = semesterId },
                                          new SqlParameter("@StateId", SqlDbType.Int, 50) { Value = stateId },
                                        };

            DataTable dtProgramsList = _helper.GetDataTable("[dbo].[GetProgramList]", parameters);
            if (dtProgramsList.Rows.Count > 0)
            {
                obj= dtProgramsList.AsEnumerable().Select(row =>
                                         new ProgramResponse
                                         {
                                             ProgramId = Convert.ToInt32(row["ProgramID"]),
                                             ProgramName = Convert.ToString(row["Name"]),
                                             ApplicationCount= Convert.ToInt32(row["ApplicationCount"]),
                                             OfferedCount = Convert.ToInt32(row["OfferedCount"])
                                         }).ToList();
            }
            return obj;
        }

        private object GetDeserializedTemplate(string jsonString,int programId)
        {
            object _obj = new object();
            _obj = GetTemplateName(programId); // gets the class instance based on the Program ID
            Type formTemplateType = _obj.GetType(); // gets the form template type
            Type templateStoreType = typeof(TemplateStore<>); // gets the template store type
            var genericType = templateStoreType.MakeGenericType(formTemplateType);
            object templateStoreInstance = Activator.CreateInstance(genericType); // creating instance of the template store 
            var deserializedMethod = templateStoreInstance.GetType().GetMethod("GetDeserializedTemplate"); // get the generic method to deserialize

            object obj = deserializedMethod.Invoke(templateStoreInstance, new object[] { jsonString }); // invoke the generic method and return the object 

            #region Repetative trigger to Template Store 
            //if (programId == (int)Programs.MultipleSubjectCredentialProgram)
            //{
            //    TemplateStore<FormTemplate> store = new TemplateStore<FormTemplate>();
            //    _obj = store.GetDeserializedTemplate(jsonString);
            //}
            //if (programId == (int)Programs.SingleSubjectCredentialProgram)
            //{
            //    TemplateStore<FormTemplateSSCP> store = new TemplateStore<FormTemplateSSCP>();
            //    _obj = store.GetDeserializedTemplate(jsonString);
            //}
            //if (programId == (int)Programs.EducationSpecialistCredentialProgram)
            //{
            //    TemplateStore<FormTemplateESCP> store = new TemplateStore<FormTemplateESCP>();
            //    _obj = store.GetDeserializedTemplate(jsonString);
            //}
            //if (programId == (int)Programs.UrbanDualCredentialProgram)
            //{
            //    TemplateStore<FormTemplateUDCP> store = new TemplateStore<FormTemplateUDCP>();
            //    _obj = store.GetDeserializedTemplate(jsonString);
            //}
            #endregion

            return obj;
        }

        private object GetTemplateName(int programId)
        {
            
            object obj = new object();
            if (programId == (int)Programs.MultipleSubjectCredentialProgram)
                obj = new ThoughtFocus.Domain.Response.Program.FormTemplate();
            if (programId == (int)Programs.SingleSubjectCredentialProgram)
                obj = new ThoughtFocus.Domain.Response.Program.FormTemplateSSCP();
            if (programId == (int)Programs.EducationSpecialistCredentialProgram)
                obj = new ThoughtFocus.Domain.Response.Program.FormTemplateESCP();
            if (programId == (int)Programs.UrbanDualCredentialProgram)
                obj = new ThoughtFocus.Domain.Response.Program.FormTemplateUDCP();

            return obj;
        }

    }
}
