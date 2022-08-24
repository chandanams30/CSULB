CREATE TABLE [Master].[FormState] (
    [ID]              BIGINT         NOT NULL,
    [Name]            NVARCHAR (250) NOT NULL,
    [Description]     VARCHAR (100)  NULL,
    [CreatedDateTime] DATETIME2 (7)  NOT NULL,
    [CreatedByUserID] BIGINT         NOT NULL
);

