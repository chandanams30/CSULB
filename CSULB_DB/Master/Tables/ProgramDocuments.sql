CREATE TABLE [Master].[ProgramDocuments] (
    [ID]                 BIGINT        IDENTITY (1, 1) NOT NULL,
    [ProgramID]          BIGINT        NOT NULL,
    [DocumentID]         BIGINT        NOT NULL,
    [CreatedDateTime]    DATETIME2 (7) NOT NULL,
    [CreatedByUserID]    BIGINT        NOT NULL,
    [IsOptional]         BIT           CONSTRAINT [DF_ProgramDocuments_IsOptional] DEFAULT ((0)) NOT NULL,
    [AllowMultiple]      BIT           NULL,
    [SortingOrder]       BIGINT        NULL,
    [isRequiredForMerge] BIT           NULL,
    CONSTRAINT [PK_ApplicationDocuments] PRIMARY KEY CLUSTERED ([ID] ASC)
);





