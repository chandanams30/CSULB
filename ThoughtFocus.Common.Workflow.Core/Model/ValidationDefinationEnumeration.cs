using System;

namespace ThoughtFocus.Common.Workflow.Core.Model
{
    [Serializable]
    public enum ValidationDefinationEnumeration
    {
        /// <summary>
        /// For CommentRequiredValidation
        /// </summary>
         CommentRequiredValidation,

         /// <summary>
         /// For ConfirmationOnCancel
         /// </summary>
         ConfirmationOnCancel,

         RejectionInformation,

         ConfirmationOnSubmit,


         FeedbackInformationOnRejection,

         FundUtilizationValidation,

         AgreementValidation
         
    }
}
