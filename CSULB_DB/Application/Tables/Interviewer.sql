CREATE TABLE [Application].[Interviewer] (
    [ID]                BIGINT IDENTITY (1, 1) NOT NULL,
    [FormID]            BIGINT NOT NULL,
    [InterviewerUserID] BIGINT NOT NULL,
    [isAssigned]        BIT    CONSTRAINT [DF_Interviewer_isAssigned] DEFAULT ((1)) NOT NULL
);



