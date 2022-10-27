CREATE TABLE [Master].[ProgramConfigurations] (
    [ID]                         BIGINT         IDENTITY (1, 1) NOT NULL,
    [ProgramID]                  BIGINT         NOT NULL,
    [ApplicationTypeID]          BIGINT         NOT NULL,
    [CompletingYourApplication]  NVARCHAR (MAX) NULL,
    [RecommenderMailBody]        NVARCHAR (MAX) NULL,
    [AssignFormToReviewersCount] INT            CONSTRAINT [DF_ProgramConfigurations_AssignFormToReviewersCount] DEFAULT ((0)) NOT NULL
);

