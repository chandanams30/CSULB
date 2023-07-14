using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.Milestones
{
    public class MilestoneUsersListResponse:BaseResponse
    {
        public string MilestoneUsersList { get; set; }
    }
    
    public class MilestoneProgramTermListResponse : BaseResponse
    {
        public string MilestoneProgramTermList { get; set; }
    }

    public class MilestonePublishedFormsListResponse : BaseResponse
    {
        public string MilestonePublishedFormsList { get; set; }
    }

}
