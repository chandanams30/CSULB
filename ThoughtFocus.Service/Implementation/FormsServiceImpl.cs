using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ThoughtFocus.DataAccess.DBHelper;
using ThoughtFocus.DataAccess.Models;
using ThoughtFocus.Domain.Enumeration;
using ThoughtFocus.Domain.Request.Form;
using ThoughtFocus.Domain.Response.Form;
using ThoughtFocus.Domain.Response.Program;
using ThoughtFocus.Domain.Response.Program.TemplateResponse;
using ThoughtFocus.Domain.TemplateModels;
using ThoughtFocus.Service.Interfaces;

namespace ThoughtFocus.Service.Implementation
{
    public class FormsServiceImpl : IFormsService
    {
        private readonly CSULB_DBContext _context;
        private readonly ISqlDBUtility _helper;
        public FormsServiceImpl(CSULB_DBContext context
                               ,ISqlDBUtility helper)
        {
            _context = context;
            _helper = helper;
        }
        public FormResponse GetForm(int formId,int applicationId,int programId)
        {
            #region Linq statement
            //var obj = _context.Forms.Where(a => a.Id==formId).Select(data => new FormResponse
            //{
            //    FormId = Convert.ToInt32(data.Id),
            //    ApplicantId = Convert.ToInt32(data.UserId),
            //    ApplicantName = "",
            //    ProgramId = Convert.ToInt32(data.ApplicationId),
            //    ProgramName = "",
            //    SemesterId = Convert.ToInt32(data.SemesterId),
            //    Semester = "",
            //    Form = data.Form1
            //}).FirstOrDefault();
            #endregion

            FormResponse obj = new FormResponse();
            SqlParameter[] parameters =
                                 {
                                          new SqlParameter("@FormId", SqlDbType.Int, 50) { Value = formId }
                                        };

            DataTable dtForm = _helper.GetDataTable("[dbo].[GetFormDetails]", parameters);
            if (dtForm.Rows.Count > 0)
            {
                obj = dtForm.AsEnumerable().Select(row =>
                                         new FormResponse
                                         {
                                             FormId = Convert.ToInt32(row["ID"]),
                                             //Form = Convert.ToString(row["Form"])//,
                                             Form = GetDeserializedTemplate(Convert.ToString(row["Form"]),applicationId, programId)
                                         }).FirstOrDefault();
            }
            return obj;
        }

        public List<FormResponse> GetFormList(int programId, int semesterId,int stateId)
        {

            #region Linq Statement 
            //var obj = _context.Forms.Where(a => a.SemesterId == semesterId && a.ProgramId==programId).Select(data => new FormResponse
            //{
            //  FormId=Convert.ToInt32(data.Id),
            //  ApplicantId= Convert.ToInt32(data.UserId),
            //  ApplicantName="",
            //  ProgramId= Convert.ToInt32(data.ProgramId),
            //  ProgramName="",
            //  SemesterId= Convert.ToInt32(data.SemesterId),
            //  Semester="",
            //  Form=data.Form1


            //}).ToList();

            #endregion

            List<FormResponse> obj = new List<FormResponse>();
            SqlParameter[] parameters =
                                 {
                                          new SqlParameter("@ProgramID", SqlDbType.Int, 50) { Value = programId },
                                          new SqlParameter("@SemesterID", SqlDbType.Int, 50) { Value = semesterId },
                                          new SqlParameter("@State", SqlDbType.Int, 50) { Value = stateId }
                                 };

            DataTable dtForm = _helper.GetDataTable("[dbo].[GetFormsList]", parameters);
            if (dtForm.Rows.Count > 0)
            {
                obj = dtForm.AsEnumerable().Select(row =>
                                         new FormResponse
                                         {
                                            FormId=Convert.ToInt32(row["ID"]),
                                            ApplicantId= Convert.ToInt32(row["UserID"]),
                                            ApplicantName=Convert.ToString(row["ApplicantName"]),
                                            ProgramId= Convert.ToInt32(row["ProgramID"]),
                                            ProgramName= Convert.ToString(row["ProgramName"]),
                                            SemesterId= Convert.ToInt32(row["SemesterID"]),
                                            Semester= Convert.ToString(row["SemesterName"]),
                                            Form="",
                                            ApplicationNumber= Convert.ToString(row["ApplicationNumber"]),
                                            ReviewerId=0,
                                            ReviewerName= "",
                                            ReviewerRecommendation=""
                                         }).ToList();
            }


            return obj;
        }

        public string SaveForm(FormAddRequest request)
        {
            string responseString = string.Empty;
            string jsonString = string.Empty;
            if (request != null)
            {
                string applicationNumber = GetApplicationNumber(request.ProgramId, request.SemesterId);
                jsonString  = SetJsonString(request.Form, request.ProgramId);
                SqlParameter[] parameters =
                                      {
                                          new SqlParameter("@FormId", SqlDbType.Int, 50) { Value = Convert.ToInt32(request.FormId) },
                                          new SqlParameter("@UserId", SqlDbType.Int, 50) { Value = Convert.ToInt32(request.UserId) },
                                          new SqlParameter("@ProgramId", SqlDbType.Int, 50) { Value = Convert.ToInt32(request.ProgramId) },
                                          new SqlParameter("@SemesterId", SqlDbType.Int, 50) { Value = Convert.ToInt32(request.SemesterId) },
                                          new SqlParameter("@Form", SqlDbType.NVarChar, 3999) { Value = Convert.ToString(request.Form) },
                                          new SqlParameter("@State", SqlDbType.Int, 50) { Value = Convert.ToInt32(request.State) },
                                          new SqlParameter("@ApplicationNumber", SqlDbType.NVarChar, 100) { Value = Convert.ToString(applicationNumber) }
                                        };

                int identity = _helper.InsertTable("[dbo].[SaveForm]", parameters);
            }

            return responseString;
        }

        private string GetApplicationNumber(int programId,int semesterId)
        {
            string ApplicationNumber = string.Empty;

            SqlParameter[] parameters =
                                {
                                          new SqlParameter("@ProgramID", SqlDbType.Int, 50) { Value = programId },
                                          new SqlParameter("@SemesterID", SqlDbType.Int, 50) { Value = semesterId }
                                 };

            ApplicationNumber = _helper.ExecuteSPWithOutputVariable("[dbo].[GetApplicationNumber]", parameters);
            return ApplicationNumber;
        }

        private string SetJsonString(object obj,int programId)
        {
            string jsonString = string.Empty;
            object templateClassParameter = GetFormInstance(programId);
            Type formTemplateType = templateClassParameter.GetType(); // gets the form template type
            Type templateStoreType = typeof(TemplateStore<>); // gets the template store type
            var genericType = templateStoreType.MakeGenericType(formTemplateType);
            object templateStoreInstance = Activator.CreateInstance(genericType); // creating instance of the template store 
            var serializedMethod = templateStoreInstance.GetType().GetMethod("SetSerializedJSONString"); // get the generic method to serialize
            object retObj = serializedMethod.Invoke(templateStoreInstance, new object[] { obj }); // invoke the generic method and return the object 

            return retObj.ToString();
        }
        public dynamic Cast(dynamic source, Type dest)
        {
            return Convert.ChangeType(source, dest);
        }
        private object GetDeserializedTemplate(string jsonString,int applicationId, int programId)
        {
            object _obj = new object();
            if (applicationId == (int)ApplicationTypes.InitialCredentialPrograms)
            {
                // CTE PROGRAM IS MISSING TEMPLATE// ADD HERE ONCE CONFIRMED 
                if (programId == (int)Programs.MultipleSubjectCredentialProgram)
                {
                    TemplateStore<FormMSCP> store = new TemplateStore<FormMSCP>();
                    _obj = store.GetDeserializedTemplate(jsonString);
                }
                if (programId == (int)Programs.SingleSubjectCredentialProgram)
                {
                    TemplateStore<FormSSCP> store = new TemplateStore<FormSSCP>();
                    _obj = store.GetDeserializedTemplate(jsonString);
                }
                if (programId == (int)Programs.EducationSpecialistCredentialProgram)
                {
                    TemplateStore<FormESCP> store = new TemplateStore<FormESCP>();
                    _obj = store.GetDeserializedTemplate(jsonString);
                }
                if (programId == (int)Programs.UrbanDualCredentialProgram)
                {
                    TemplateStore<FormUDCP> store = new TemplateStore<FormUDCP>();
                    _obj = store.GetDeserializedTemplate(jsonString);
                }
            }
            if(applicationId==(int)ApplicationTypes.GraduatePrograms)
            {
                TemplateStore<FormGraduatePrograms> store = new TemplateStore<FormGraduatePrograms>();
                _obj = store.GetDeserializedTemplate(jsonString);
            }
            if (applicationId == (int)ApplicationTypes.DoctoralPrograms)
            {

            }

            return _obj;
        }

        private object GetFormInstance(int programId)
        {

            object obj = new object();
            if (programId == (int)Programs.MultipleSubjectCredentialProgram)
                obj = new ThoughtFocus.Domain.Response.Program.TemplateResponse.FormMSCP();
            if (programId == (int)Programs.SingleSubjectCredentialProgram)
                obj = new ThoughtFocus.Domain.Response.Program.TemplateResponse.FormSSCP();
            if (programId == (int)Programs.EducationSpecialistCredentialProgram)
                obj = new ThoughtFocus.Domain.Response.Program.TemplateResponse.FormESCP();
            if (programId == (int)Programs.UrbanDualCredentialProgram)
                obj = new ThoughtFocus.Domain.Response.Program.TemplateResponse.FormUDCP();

            return obj;
        }
    }
}
