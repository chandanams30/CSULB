namespace ThoughtFocus.Domain
{
    using System;

    #region Enumerations

    [Serializable]
    public enum ActionType
    {
        /// <summary>
        /// For View
        /// </summary>
        View,

        /// <summary>
        /// For Add
        /// </summary>
        Add,

        /// <summary>
        /// For Edit
        /// </summary>
        Edit
    }

    [Serializable]
    public enum AudtiTrailTableName
    {
        /// <summary>
        /// For Default
        /// </summary>
        Default,

        /// <summary>
        /// For Contacts
        /// </summary>
        Contacts,

        /// <summary>
        /// For ContactAddress
        /// </summary>
        ContactAddress,

        /// <summary>
        /// For ContactContactInfo
        /// </summary>
        ContactContactInfo,

        /// <summary>
        /// For ContactNotes
        /// </summary>
        ContactNotes,

        /// <summary>
        /// For ContactEmailAddress
        /// </summary>
        ContactEmailAddress,

        /// <summary>
        /// For ContactOrganization
        /// </summary>
        ContactOrganization,

        /// <summary>
        /// For Survey
        /// </summary>
        Surveys,

        /// <summary>
        /// For SchoolPersonInfo
        /// </summary>
        SchoolPersonInfo,

        /// <summary>
        /// For SiteVisit
        /// </summary>
        SiteVisit,

        /// <summary>
        /// For RSiteVisitsSchool
        /// </summary>
        RSiteVisitsSchool,

        /// <summary>
        /// For SiteVisitMemberRequest
        /// </summary>
        SiteVisitMemberRequest,

         /// <summary>
        /// For Organization
        /// </summary>
        SiteVisitReports,

        /// <summary>
        /// For Organization
        /// </summary>
        Organization,

        /// <summary>
        /// For OrganizationContactInfo
        /// </summary>
        OrganizationContactInfo,

        /// <summary>
        /// For OrganizationContactInfo
        /// </summary>
        UserPasswordResetInfo,

        /// <summary>
        /// For OrganizationContactInfo
        /// </summary>
        UserProfileLoginInfo,

        /// <summary>
        /// For OrganizationContactInfo
        /// </summary>
        UserProfileResetInfo,

        /// <summary>
        /// For ContactInvitationInfo
        /// </summary>
        ContactInvitationInfo,

         /// <summary>
        /// For ContactInvitationInfo
        /// </summary>
        UserAccountActivationInfo,

         /// <summary>
        /// For User
        /// </summary>
        Users,

        /// <summary>
        /// For RUserContact
        /// </summary>
        RUserContact,

        /// <summary>
        /// For RUserRole
        /// </summary>
        RUserRole,

        /// <summary>
        /// For UserSecurityQuestionInfo
        /// </summary>
        UserSecurityQuestionInfo,

        /// <summary>
        /// For AuditTrailPasswordHistory
        /// </summary>
        AuditTrailPasswordHistory,

        /// <summary>
        /// For AccountActivationQueue
        /// </summary>
        AccountActivationQueue,

        /// <summary>
        /// For PasswordResetQueue
        /// </summary>
        PasswordResetQueue,

        /// <summary>
        /// For ContactInvitationQueue
        /// </summary>
        ContactInvitationQueue,

        /// <summary>
        /// For SiteVisitCreateRemoveQueue
        /// </summary>
        SiteVisitCreateOrRemoveQueue,

        /// <summary>
        /// For SiteVisitPostPoneCancelQueue
        /// </summary>
        SiteVisitPostPoneOrCancelQueue,

        /// <summary>
        /// For PostUserAccountCreationQueue
        /// </summary>
        PostUserAccountCreationQueue,

        /// <summary>
        /// For ContactRating
        /// </summary>
        ContactRating,

        /// <summary>
        /// For ConflictOfInterest
        /// </summary>
        ConflictOfInterest,

        /// <summary>
        /// For NonAvailabilityDates
        /// </summary>
        NonAvailabilityDates,

        SiteVisitMemberInformation,

        Application,

        ApplicationSitevisit,

        /// <summary>
        /// For Email Teamplates
        /// </summary>
        Notification,

        /// <summary>
        /// For Invoices
        /// </summary>
        Invoices,

        /// <summary>
        /// For Feedbacks
        /// </summary>
        Feedbacks,

        /// <summary>
        /// For Programs
        /// </summary>
        Programs,

        /// <summary>
        /// For ProgramApplications
        /// </summary>
        ProgramApplications,
    }

    [Serializable]
    public enum ResponseStatus
    {
        /// <summary>
        /// For Default
        /// </summary>
        Default,

        /// <summary>
        /// For Success
        /// </summary>
        Success,

        /// <summary>
        /// For Fail
        /// </summary>
        Fail
    }

    [Serializable]
    public enum ContactInvitationStatus
    {
        /// <summary>
        /// For ACCEPT
        /// </summary>
        ACCEPT,

        /// <summary>
        /// For REJECT
        /// </summary>
        REJECT,

        /// <summary>
        /// For PENDING
        /// </summary>
        PENDING,
    }

    [Serializable]
    public enum ContactType
    {
        /// <summary>
        /// For Fax
        /// </summary>
        F,

        /// <summary>
        /// For Mobile
        /// </summary>
        M,

        /// <summary>
        /// For Phone
        /// </summary>
        P
    }

    [Serializable]
    public enum FunctionType
    {
        /// <summary>
        /// For Authentication
        /// </summary>
        Authentication,

        /// <summary>
        /// For Authorization
        /// </summary>
        Authorization
    }

    [Serializable]
    public enum IsMinorityMethodReturnType
    {
        /// <summary>
        /// For Error
        /// </summary>
        Yes,
        /// <summary>
        /// For TokenMismatch
        /// </summary>
        No,
    }

    [Serializable]
    public enum Module
    {
        /// <summary>
        /// For Login
        /// </summary>
        Login,

        /// <summary>
        /// For Contact
        /// </summary>
        Contact,

        /// <summary>
        /// For School
        /// </summary>
        School
    }

    [Serializable]
    public enum ProspectCreationInvitationStatus
    {
        /// <summary>
        /// For Error
        /// </summary>
        Error,

        /// <summary>
        /// For TokenIsUsed
        /// </summary>
        TokenIsUsed,

        /// <summary>
        /// For TokenMismatch
        /// </summary>
        TokenMismatch,

        /// <summary>
        /// For TokenMismatch
        /// </summary>
        Success,

        /// <summary>
        /// For OldTokenUsed
        /// </summary>
        OldTokenUsed,
    }

    [Serializable]
    public enum ResetPersonType
    {
        /// <summary>
        /// For S
        /// </summary>
        S,

        /// <summary>
        /// For A
        /// </summary>
        A,
    }

    [Serializable]
    public enum SchoolPersonTypes
    {
        /// <summary>
        /// For DEAN
        /// </summary>
        DEAN=1,

        /// <summary>
        /// For PRESIDENT
        /// </summary>
        PRESIDENT=2,

        /// <summary>
        /// For CONTACT
        /// </summary>
        CONTACT=3
    }

    [Serializable]
    public enum SiteVisitActionType
    {
        /// <summary>
        /// For CREATED
        /// </summary>
        CREATED,

        /// <summary>
        /// For CANCELLED
        /// </summary>
        CANCELLED,

        /// <summary>
        /// For POSTPONED
        /// </summary>
        POSTPONED,
        /// <summary>
        /// For REMOVED
        /// </summary>
        REMOVED,
    }

    [Serializable]
    public enum SiteVisitInvitationStatus
    {
        /// <summary>
        /// For Error
        /// </summary>
        Error,
        /// <summary>
        /// For TokenMismatch
        /// </summary>
        Success,
    }

    //[Serializable]
    //public enum SiteVisitReponseStatus
    //{
    //    /// <summary>
    //    /// For ACCEPT
    //    /// </summary>
    //    ACCEPT=2,

    //    /// <summary>
    //    /// For PENDING
    //    /// </summary>
    //    PENDING=3,

    //    /// <summary>
    //    /// For REJECT
    //    /// </summary>
    //    REJECT=1,

    //     /// <summary>
    //    /// For REMOVED
    //    /// </summary>
    //    REMOVED=4,

    //      /// <summary>
    //    /// For DRAFTREMOVED
    //    /// </summary>
    //    REMOVEDUPONREQUEST = 5,

    //     /// <summary>
    //    /// For DRAFTREMOVED
    //    /// </summary>
    //    DRAFTREMOVED = 6

    //    /// <summary>
    //    /// For NOT
    //    /// </summary>
    //    ACCEPT=2,

    //}

    [Serializable]
    public enum SiteVisitStatus
    {
        /// <summary>
        /// For CANCEL
        /// </summary>
        CANCEL=13
    }

    [Serializable]
    public enum SubModule
    {
        /// <summary>
        /// For Login
        /// </summary>
        Login,

        /// <summary>
        /// For Listing
        /// </summary>
        Listing,

        /// <summary>
        /// For Add
        /// </summary>
        Add,

        /// <summary>
        /// For Edit
        /// </summary>
        Edit,

        /// <summary>
        /// For View
        /// </summary>
        View
    }

    [Serializable]
    public enum UserAccountActivationStatus
    {
        /// <summary>
        /// For Error
        /// </summary>
        Error,

        /// <summary>
        /// For TokenIsUsed
        /// </summary>
        TokenIsUsed,

        /// <summary>
        /// For TokenExpired
        /// </summary>
        TokenExpired,

        /// <summary>
        /// For InvalidUserID
        /// </summary>
        InvalidUserID,

        /// <summary>
        /// For InvalidAccountActivation
        /// </summary>
        InvalidAccountActivation,

        /// <summary>
        /// For TokenIsValid
        /// </summary>
        TokenIsValid,

        /// <summary>
        /// For PasswordDoesnotMatch
        /// </summary>
        PasswordDoesnotMatch,

        /// <summary>
        /// For AccountActivationSuccess
        /// </summary>
        AccountActivationSuccess,

        /// <summary>
        /// For UpdationError
        /// </summary>
        UpdationError,

        ConatctDoesntExits
    }

    [Serializable]
    public enum UserForgotPasswordStatus
    {
        /// <summary>
        /// For Error
        /// </summary>
        Error,

        /// <summary>
        /// For InvalidUsername
        /// </summary>
        InvalidUsername,

        /// <summary>
        /// For InvalidPassword
        /// </summary>
        InvalidPassword,

        /// <summary>
        /// For AccountIsLockedOrInactive
        /// </summary>
        AccountIsLockedOrInactive,

        /// <summary>
        /// For SecurityQuestionAndAnswerIsNotSetup
        /// </summary>
        SecurityQuestionAndAnswerIsNotSetup,

        /// <summary>
        /// For SecurityQuestionAndOrAnswerDoesnotMatch
        /// </summary>
        SecurityQuestionAndOrAnswerDoesnotMatch,

        /// <summary>
        /// For ForgotPasswordSuccess
        /// </summary>
        ForgotPasswordSuccess
    }

    [Serializable]
    public enum UserLoginStatus
    {
        /// <summary>
        /// For Error
        /// </summary>
        Error,

        /// <summary>
        /// For CredentialVerificationSuccess
        /// </summary>
        CredentialVerificationSuccess,

        /// <summary>
        /// For CredentialVerificationFailed
        /// </summary>
        CredentialVerificationFailed,

        /// <summary>
        /// For AccountLocked
        /// </summary>
        AccountLocked,

        /// <summary>
        /// For AccountNotActivated
        /// </summary>
        AccountNotActivated,

        /// <summary>
        /// For AccountDoesnotExists
        /// </summary>
        AccountDoesnotExists,

        /// <summary>
        /// For AccountNotActive
        /// </summary>
        AccountNotActive
    }

    [Serializable]
    public enum UserResetPasswordStatus
    {
        /// <summary>
        /// For Error
        /// </summary>
        Error,

        /// <summary>
        /// For TokenIsUsed
        /// </summary>
        TokenIsUsed,

        /// <summary>
        /// For TokenExpired
        /// </summary>
        TokenExpired,

        /// <summary>
        /// For InvalidUserID
        /// </summary>
        InvalidUserID,

        /// <summary>
        /// For InvalidPasswordReset
        /// </summary>
        InvalidPasswordReset,

        /// <summary>
        /// For TokenIsValid
        /// </summary>
        TokenIsValid,

        /// <summary>
        /// For PasswordDoesnotMatch
        /// </summary>
        PasswordDoesnotMatch,

        /// <summary>
        /// For PasswordResetSuccess
        /// </summary>
        PasswordResetSuccess,

        /// <summary>
        /// For PasswordResetSuccess
        /// </summary>
        OldThreePasswordRepeated,
    }

    [Serializable]
    public enum UserChangePasswordStatus
    {
        /// <summary>
        /// For Error
        /// </summary>
        Error,

        /// <summary>
        /// For InvalidUserID
        /// </summary>
        InvalidUserID,

        /// <summary>
        /// For InvalidPasswordReset
        /// </summary>
        InvalidPasswordReset,

        /// <summary>
        /// For PasswordDoesnotMatch
        /// </summary>
        PasswordDoesnotMatch,

        /// <summary>
        /// For PasswordResetSuccess
        /// </summary>
        PasswordResetSuccess,

        /// <summary>
        /// For PasswordResetRepeated
        /// </summary>
        OldThreePasswordRepeated,

        /// <summary>
        /// For NewPasswordDoesnotMatch
        /// </summary>
        NewPasswordDoesnotMatch,
    }

    [Serializable]
    public enum EmailTemplateNameID
    {
        /// <summary>
        /// For CONTACTINVITATION
        /// </summary>
        CONTACTINVITATION = 1,

        /// <summary>
        /// For PROGRAMINVITATION
        /// </summary>
        PROGRAMINVITATION = 2,

        /// <summary>
        /// For STATECHANGE
        /// </summary>
        STATECHANGE = 3,

        /// <summary>
        /// For APPROVEDAPPLICATION
        /// </summary>
        APPROVEDAPPLICATION = 4,

         /// <summary>
        /// For POSTACCOUNTACTIVATION
        /// </summary>
        POSTACCOUNTACTIVATION = 5,

         /// <summary>
        /// For APPLICATIONSUBMITION
        /// </summary>
        APPLICATIONSUBMITION = 6,

         /// <summary>
        /// For REQUESTMOREINFO
        /// </summary>
        REQUESTMOREINFO = 7,

        /// <summary>
        /// For REQUESTCOMPLETED
        /// </summary>
        REQUESTCOMPLETED = 8,

        /// <summary>
        /// For ACCEPTED
        /// </summary>
        ACCEPTED = 9,

        /// <summary>
        /// For APPROVED
        /// </summary>
        APPROVED = 10,

        /// <summary>
        /// For REJECTED
        /// </summary>
        REJECTED = 11,

        /// <summary>
        /// For AGREEMENTSUBMITTED
        /// </summary>
        AGREEMENTSUBMITTED = 12,

        /// <summary>
        /// For AGREEMENTREJECTED
        /// </summary>
        AGREEMENTREJECTED = 13,

        /// <summary>
        /// For CFOAPPROVED
        /// </summary>
        CFOAPPROVED = 14,

        /// <summary>
        /// For ACCOUNTDISBURSED
        /// </summary>
        ACCOUNTDISBURSED = 15,
        
    }

    #endregion Enumerations
}