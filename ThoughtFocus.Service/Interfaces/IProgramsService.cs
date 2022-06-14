using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.Program;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IProgramsService
    {
        List<ProgramResponse> GetProgramList(int applicationTypeId, int semesterId, int stateId, int userId);
        ProgramResponse GetProgram(int programId, int semesterId, int userId);
    }
}
