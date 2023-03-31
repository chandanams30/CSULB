using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Admin
{
    public class UpsertCommunitySchoolRequest
    {
        public int CommunitySchoolID { get; set; }
        public string CommunitySchoolName { get; set; }
        public int CommunityDistrictID { get; set; }
        public int createdByUserID { get; set; }
    }
}
