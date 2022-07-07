CREATE TABLE [Master].[Role] (
    [ID]                   BIGINT        IDENTITY (1, 1) NOT NULL,
    [Description]          VARCHAR (100) NULL,
    [CreatedDateTime]      DATETIME2 (7) NOT NULL,
    [CreatedByUserID]      BIGINT        NOT NULL,
    [LastModifiedDateTime] DATETIME2 (7) NOT NULL,
    [LastModifiedByUserID] BIGINT        NOT NULL,
    [Name]                 VARCHAR (100) NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([ID] ASC)
);



