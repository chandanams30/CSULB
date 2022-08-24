using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.Program;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IProgramsService
    {
        List<ProgramListResponse> GetProgramList();
        ProgramResponse GetProgram(int programId, int semesterId, int userId);
        List<ProgramApplicationListResponse> GetProgramApplications(int programId, int semesterId, int stateId, int userId);
        ProgramProgramOpenFormCollectionResponse GetUserAppliedForms(int userId);

    }
}
