using System;

namespace ThoughtFocus.Domain.Enumeration
{
    #region Enumerations

    [Serializable]
    public enum WorkflowNotificationType
    {
        /// <summary>
        /// For BindToActivity
        /// </summary>
        BindToActivity = 1,

        /// <summary>
        /// For NotBindToActivity
        /// </summary>
        NotBindToActivity = 2,

        /// <summary>
        /// For Other
        /// </summary>
        Other = 3,

        /// <summary>
        /// For Reminder
        /// </summary>
        Reminder = 4,

    }

  
    #endregion Enumerations
}
