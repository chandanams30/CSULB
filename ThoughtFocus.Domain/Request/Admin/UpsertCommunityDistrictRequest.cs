using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Request.Admin
{
    public class UpsertCommunityDistrictRequest
    {
        public int CommunityDistrictID { get; set; }
        public string CommunityDistrictName { get; set; }
        public int createdByUserID { get; set; }
    }
}
