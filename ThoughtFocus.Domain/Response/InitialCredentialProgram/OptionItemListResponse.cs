using System;
using System.Collections.Generic;
using System.Text;

namespace ThoughtFocus.Domain.Response.InitialCredentialProgram
{
    public class OptionItemListResponse:BaseResponse
    {
        public OptionItemList OptionItemList { get; set; }
    }
    public class OptionItemList
    {
        public string OptionItemsList { get; set; }
    }
}
