CREATE TABLE [Master].[Term] (
    [TermCode]        VARCHAR (10)   NULL,
    [Name]            NVARCHAR (100) NOT NULL,
    [Description]     VARCHAR (100)  NULL,
    [StartDate]       DATETIME       NOT NULL,
    [EndDate]         DATETIME       NOT NULL,
    [CreatedDateTime] DATETIME       NOT NULL,
    [CreatedByUserID] BIGINT         NOT NULL
);

