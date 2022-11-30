using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.GraduateProgram;
using ThoughtFocus.Domain.Response.InitialCredentialProgram;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IInitialCredentialProgramService
    {
        ApplicationProgramResponse GetApplicationPrograms(int userID, int applicationTypeID, string termCode);
        AppliedFormsByProgramsResponse GetAppliedFormsByPrograms(int userID, int programID, string termcode, int formStateID);
        AppliedFormsResponse GetAppliedForms(int userID, int applicationTypeID);
        FormStatesResponse GetFormStates(int userID);
        SemesterListResponse GetSemesterList();
        OptionItemListResponse GetIntialCreditialOptionItemsList(int programID);


    }
}
