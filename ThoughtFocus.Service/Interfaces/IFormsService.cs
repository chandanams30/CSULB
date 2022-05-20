using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.Form;
using ThoughtFocus.Domain.Response.Form;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IFormsService
    {
        List<FormResponse> GetFormList(int programId,int semesterId);
        FormResponse GetForm(int formId,int programId);

        string SaveForm(FormAddRequest request);
    }
}
