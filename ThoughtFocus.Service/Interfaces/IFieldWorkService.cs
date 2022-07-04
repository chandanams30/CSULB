using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Request.FieldWork;
using ThoughtFocus.Domain.Response.FieldWork;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IFieldWorkService
    {
        List<FieldWorkResponse> GetFieldWorkList(int userId);
        FieldWorkDataResponse GetFieldWorkDetailsById(int userId,int fieldWorkId);
        string UpdateFieldWorkValidation(FieldWorkValidationRequest input);
    }
}
