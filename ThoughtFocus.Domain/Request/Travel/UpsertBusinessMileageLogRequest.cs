using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Travel
{
    public class UpsertBusinessMileageLogRequest
    {
        public int BusinessMileageLogID { get; set; }
        public int BusinessMileageSupervisorID { get; set; }
        public int BusinessMileageSupervisorUserID { get; set; }
        public DateTime TravelDateTime { get; set; }
        public string StartingLocation { get; set; }
        public string DestinationAddress { get; set; }
        public string BusinessPurpose { get; set; }
        public decimal Miles { get; set; }
        public int ModifiedByUserID { get; set; }
        public string DirectionsJSON { get; set; }
        public byte[] DirectionsMapFileContent { get; set; }
        public string DirectionsMapFileName { get; set; }
    }
}
