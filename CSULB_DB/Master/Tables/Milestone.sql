CREATE TABLE [Master].[Milestone] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [MilestoneName]        NVARCHAR (50)  NOT NULL,
    [MilestoneDescription] NVARCHAR (250) NOT NULL,
    [MilestoneForm]        NVARCHAR (MAX) NULL,
    [isPublished]          BIT            CONSTRAINT [DF_Milestone_isPublished] DEFAULT ((0)) NULL,
    [isMandatory]          BIT            NULL,
    [CreatedBy]            BIGINT         NOT NULL,
    [CreatedDate]          DATETIME       NOT NULL
);

