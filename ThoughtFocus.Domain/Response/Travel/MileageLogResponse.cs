using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Travel
{
    public class MileageLogResponse:BaseResponse
    {
        public int BusinessMileageLogID { get; set; }
        public int TravelCSULBFacultySupervisorID { get; set; }
        public DateTime TravelDateTime { get; set; }
        public string StartingLocation { get; set; }
        public string DestinationAddress { get; set; }
        public string BusinessPurpose { get; set; }
        public decimal Miles { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public decimal Rate { get; set; }
        public Boolean ShowEdit { get; set; }
        public Boolean ShowApprove { get; set; }

    }
}
