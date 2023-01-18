CREATE TABLE [Master].[FormStateHandler] (
    [ID]                              BIGINT IDENTITY (1, 1) NOT NULL,
    [RoleID]                          BIGINT NOT NULL,
    [FormStateID]                     BIGINT NOT NULL,
    [showCancel]                      BIT    NOT NULL,
    [showSave]                        BIT    NOT NULL,
    [showEdit]                        BIT    NOT NULL,
    [showSubmit]                      BIT    NOT NULL,
    [showDecline]                     BIT    NOT NULL,
    [showAccept]                      BIT    NOT NULL,
    [showRequestformoreInfo]          BIT    NOT NULL,
    [showDisqualify]                  BIT    NOT NULL,
    [showWaitlist]                    BIT    NOT NULL,
    [showOffer]                       BIT    NOT NULL,
    [showNotOffer]                    BIT    NOT NULL,
    [showMergeandDownloadAttachments] BIT    NOT NULL,
    [showCompletingYourApplication]   BIT    CONSTRAINT [DF_FormStateHandler_showCompletingYourApplication] DEFAULT ((0)) NOT NULL,
    [showReviewerSection]             BIT    CONSTRAINT [DF_FormStateHandler_showReviewerSection] DEFAULT ((0)) NOT NULL,
    [editReviewerSection]             BIT    CONSTRAINT [DF_FormStateHandler_editReviewerSection] DEFAULT ((0)) NOT NULL,
    [showAddInterviewer]              BIT    CONSTRAINT [DF_Table_Test_showAddInterviewer] DEFAULT ((0)) NOT NULL,
    [showAddInstructor]               BIT    CONSTRAINT [DF_Table_Test_showAddInstructor] DEFAULT ((0)) NOT NULL,
    [showInReview]                    BIT    CONSTRAINT [DF_Table_Test_showInReview] DEFAULT ((0)) NOT NULL
);



