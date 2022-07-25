using System;
using System.Collections.Generic;


namespace ThoughtFocus.Domain.Params
{
    public class ApplicationRequest
    {
        #region Properties
       
        public string CommandName {get;set;}
        public long ApplicationID​​​​​​​​ {get; set;}
        public long ApplicationStatusID {get; set;}

        #endregion Properties
    }
}
