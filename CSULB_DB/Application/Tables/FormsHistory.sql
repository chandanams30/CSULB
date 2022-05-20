CREATE TABLE [Application].[FormsHistory] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [FormsID]          INT            NOT NULL,
    [Form]             NVARCHAR (MAX) NOT NULL,
    [VersionNumber]    NVARCHAR (50)  NOT NULL,
    [ModifiedDateTime] DATETIME2 (7)  NOT NULL,
    [ModifiedBy]       BIGINT         NOT NULL
);

