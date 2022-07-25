namespace ThoughtFocus.Domain.Params
{
    using System;
    using System.Collections.Generic;

    using ThoughtFocus.Domain.Response;

    [Serializable]
    public class CommonResponse
    {
        #region Fields

        private ResponseStatus commonCreationStatus;
        private string statusMessage;  
        private List<string> validationErrors;   

        #endregion Fields

        #region Constructors

        public CommonResponse(ResponseStatus commonCreationStatus, string statusMessage, List<string> validationErrors)
        {
            this.commonCreationStatus = commonCreationStatus;
            this.statusMessage = statusMessage;   
            this.validationErrors = validationErrors;          
        }

        #endregion Constructors

        #region Properties

        public ResponseStatus ResponseStatus
        {
            get
            {
                return this.commonCreationStatus;
            }
            set
            {
                this.commonCreationStatus = value;
            }
        }

        public string StatusMessage
        {
            get
            {
                return this.statusMessage;
            }
            set
            {
                this.statusMessage = value;
            }
        }

        public List<string> ValidationErrors
        {
            get
            {
                return this.validationErrors;
            }
            set
            {
                this.validationErrors = value;
            }
        }
        public long ID { get; set; }//Change this

        public List<Message> ValidationMessages { get; set; }

        #endregion Properties
    }
}