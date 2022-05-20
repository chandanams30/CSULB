CREATE TABLE [Master].[ApplicationTypes] (
    [ID]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (250) NOT NULL,
    [Description]     VARCHAR (100)  NULL,
    [CreatedDateTime] DATETIME2 (7)  CONSTRAINT [DF_ApplicationTypes_CreatedDateTime] DEFAULT (getdate()) NOT NULL,
    [CreatedByUserID] BIGINT         NOT NULL,
    CONSTRAINT [PK_ApplicationTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);

