using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Program
{
    public class ProgramProgramOpenFormCollectionResponse:BaseResponse
    {
       public List<ProgramOpenFormCollection> response { get; set; }
    }
    public class ProgramOpenFormCollection
    {
        public int FormID { get; set; }
        public string ProgramName { get; set; }
        public string Term { get; set; }
        public int FormState { get; set; }
        public string Status { get; set; }
    }
}
