using System.Collections.Generic;
using static ThoughtFocus.Domain.Request.GraduateProgram.DispositionMSCPFiledata;

namespace ThoughtFocus.Domain.Request.GraduateProgram
{
    public class DispositionMSCPList
    {
        public List<Professional> professionals { get; set; }
        public List<Attendance> attendance { get; set; }
        public List<Communication> communication { get; set; }
        public List<Rating> ratings { get; set; }
    }
}
