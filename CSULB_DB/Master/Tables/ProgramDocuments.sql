CREATE TABLE [Master].[ProgramDocuments] (
    [ID]              BIGINT        IDENTITY (1, 1) NOT NULL,
    [ProgramID]       BIGINT        NOT NULL,
    [DocumentID]      BIGINT        NOT NULL,
    [CreatedDateTime] DATETIME2 (7) NOT NULL,
    [CreatedByUserID] BIGINT        NOT NULL,
    CONSTRAINT [PK_ApplicationDocuments] PRIMARY KEY CLUSTERED ([ID] ASC)
);

