using System;
using System.Collections.Generic;
using System.Text;
using ThoughtFocus.Domain.Response.Milestones;
using ThoughtFocus.Domain.Response.StudentTeachingMilestone;

namespace ThoughtFocus.Service.Interfaces
{
    public interface IStudentTeachingMilestoneService
    {
        GetLatestFormResponse GetLatestForm(string csulbid);
    }
}
