using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ThoughtFocus.Domain.Enumeration
{
    [Serializable]
    public enum ApplicationStatusEnumeration
    {
        /// <summary>
        /// Open
        /// </summary>
        Open = 1,

        /// <summary>
        /// For Drafted
        /// </summary>
        Drafted = 2,

        /// <summary>
        /// For Submitted
        /// </summary>
        Submitted = 3,

        /// <summary>
        /// RequestedMoreInfo
        /// </summary>
        RequestedMoreInfo = 4,

        /// <summary>
        /// InReview
        /// </summary>
        InReview = 5,

        /// <summary>
        /// ReviewCompleted
        /// </summary> 
        ReviewCompleted = 6,

        /// <summary>
        /// ScheduleInterview
        /// </summary> 
        ScheduleInterview = 7,

        /// <summary>
        /// Waitlist
        /// </summary> 
        Waitlist = 8,

        /// <summary>
        /// Disqualified
        /// </summary> 
        Disqualified = 9,

        /// <summary>
        /// Offered
        /// </summary> 
        Offered = 10,

        /// <summary>
        /// OfferAccepted
        /// </summary> 
        OfferAccepted = 11,

        /// <summary>
        /// OfferDeclined
        /// </summary> 
        OfferDeclined = 12,

        /// <summary>
        /// NotOffered
        /// </summary> 
        NotOffered = 13,

    }
}
