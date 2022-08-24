CREATE TABLE [FieldWork].[CommunitySites] (
    [ID]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (100) NULL,
    [Description] VARCHAR (100) NULL,
    [CreatedBy]   BIGINT        NOT NULL,
    [CreatedDate] DATETIME      NOT NULL
);

