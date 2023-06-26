using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Travel
{
    public class UpsertFacultySupervisorRequest
    {
        public int TravelCSULBFacultySupervisorUserID { get; set; }
        public string Phone { get; set; }
        public string MailingAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int ModifiedByUserID { get; set; }

    }
}
