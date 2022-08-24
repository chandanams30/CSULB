using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.FieldWork
{
    public class FieldWorkCommunitySites
    {
        public int CommunitySiteID { get; set; }
        public string CommunitySite { get; set; }
    }
    public class FieldWorkCommunitySitesResponse:BaseResponse
    {
        public List<FieldWorkCommunitySites> sites { get; set; }
    }
}
