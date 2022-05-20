CREATE TABLE [Master].[Programs] (
    [ID]                 BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (100) NOT NULL,
    [Description]        VARCHAR (100)  NULL,
    [ApplicationTypesID] BIGINT         NOT NULL,
    [FormTemplate]       NVARCHAR (MAX) NULL,
    [Notes]              NVARCHAR (MAX) NULL,
    [CreatedDateTime]    DATETIME2 (7)  CONSTRAINT [DF_Programs_CreatedDateTime] DEFAULT (getdate()) NOT NULL,
    [CreatedByUserID]    BIGINT         NOT NULL,
    CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Application_ApplicationTypes] FOREIGN KEY ([ApplicationTypesID]) REFERENCES [Master].[ApplicationTypes] ([ID])
);

