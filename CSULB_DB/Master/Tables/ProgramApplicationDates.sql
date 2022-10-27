CREATE TABLE [Master].[ProgramApplicationDates] (
    [ApplicationTypeID]    BIGINT       NOT NULL,
    [ProgramID]            BIGINT       NOT NULL,
    [TermCode]             VARCHAR (10) NULL,
    [ApplicationOpens]     DATETIME     NOT NULL,
    [ApplicationDeadline]  DATETIME     NOT NULL,
    [ApplicationCloseDate] DATETIME     NOT NULL,
    [Status]               BIT          CONSTRAINT [DF_ProgramApplicationDates_Status] DEFAULT ((0)) NOT NULL
);

