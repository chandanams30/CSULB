CREATE TABLE [Application].[StudentDocuments] (
    [ID]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserID]          BIGINT         NOT NULL,
    [DocumentID]      BIGINT         NOT NULL,
    [FileName]        NVARCHAR (200) NOT NULL,
    [FileExtn]        NVARCHAR (10)  NOT NULL,
    [FolderName]      NVARCHAR (200) NOT NULL,
    [CreatedDateTime] DATETIME2 (7)  NOT NULL,
    [CreatedByUserID] BIGINT         NOT NULL,
    CONSTRAINT [PK_StudentDocuments] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_StudentDocuments_Documents] FOREIGN KEY ([DocumentID]) REFERENCES [Master].[Documents] ([ID])
);



