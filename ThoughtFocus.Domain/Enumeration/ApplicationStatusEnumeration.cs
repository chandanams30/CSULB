using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ThoughtFocus.Domain.Enumeration
{
    [Serializable]
    public enum ApplicationStatusEnumeration
    {
          /// <summary>
        /// Initialized
        /// </summary>
        Initialized = 1,

        /// <summary>
        /// For Created
        /// </summary>
        Created = 2,

        /// <summary>
        /// For Drafted
        /// </summary>
        Drafted = 3,

         /// <summary>
        /// Submitted
        /// </summary>
        Submitted = 4,

        /// <summary>
        /// RequestedMoreInfo
        /// </summary>
        RequestedMoreInfo = 5,

        /// <summary>
        /// RequestCompleted
        /// </summary> 
        RequestCompleted = 6,

        /// <summary>
        /// Accepted
        /// </summary> 
        Accepted = 7,
        
        /// <summary>
        /// Approved
        /// </summary> 
        Approved = 8,
        
        /// <summary>
        /// Rejected
        /// </summary> 
        Rejected = 9,
        
        /// <summary>
        /// AgreementUploaded
        /// </summary> 
        AgreementUploaded = 10,
        
        /// <summary>
        /// AgreementAccepted
        /// </summary> 
        AgreementAccepted = 11,
        
        /// <summary>
        /// CFOApproved
        /// </summary> 
        CFOApproved = 12,

         /// <summary>
        /// AccountDisbursed
        /// </summary> 
        AccountDisbursed = 13,

        /// <summary>
        /// AgreementSubmitted
        /// </summary> 
        AgreementSubmitted = 14,
        
        /// <summary>
        /// AgreementRejected
        /// </summary> 
        AgreementRejected = 15,
    }
}
