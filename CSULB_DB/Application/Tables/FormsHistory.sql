CREATE TABLE [Application].[FormsHistory] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [FormsID]          INT            NOT NULL,
    [Form]             NVARCHAR (MAX) NULL,
    [FormStateID]      INT            NOT NULL,
    [ModifiedDateTime] DATETIME       NOT NULL,
    [ModifiedBy]       BIGINT         NOT NULL,
    [CreatedDateTime]  DATETIME       CONSTRAINT [DF_FormsHistory_CreatedDateTime] DEFAULT (getdate()) NOT NULL
);



