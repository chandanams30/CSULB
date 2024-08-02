using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.Guests;
using ThoughtFocus.Domain.Response.SearchApplication;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IGuestsService
    {
        GetMilestoneSubmittedFormsList GetMilestoneSubmittedFormsList(int UserID, int FormID, string ExternalApprovalIdentifier);
    }
}
