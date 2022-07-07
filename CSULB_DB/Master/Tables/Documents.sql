CREATE TABLE [Master].[Documents] (
    [ID]              BIGINT        IDENTITY (1, 1) NOT NULL,
    [Description]     VARCHAR (100) NULL,
    [CreatedDateTime] DATETIME2 (7) NOT NULL,
    [CreatedByUserID] BIGINT        NOT NULL,
    [IsOptional]      BIT           NULL,
    [Name]            VARCHAR (100) NULL,
    CONSTRAINT [PK_Documents] PRIMARY KEY CLUSTERED ([ID] ASC)
);



