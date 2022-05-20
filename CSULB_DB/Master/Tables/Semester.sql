CREATE TABLE [Master].[Semester] (
    [ID]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (100) NOT NULL,
    [Description]     VARCHAR (100)  NULL,
    [StartDate]       DATETIME2 (7)  NOT NULL,
    [EndDate]         DATETIME2 (7)  NOT NULL,
    [CreatedByUserID] BIGINT         NOT NULL,
    [CreatedDateTime] DATETIME2 (7)  CONSTRAINT [DF_Semester_CreatedDateTime] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Semester] PRIMARY KEY CLUSTERED ([ID] ASC)
);

