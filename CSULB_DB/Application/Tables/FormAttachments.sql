CREATE TABLE [Application].[FormAttachments] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [FormID]            BIGINT         NOT NULL,
    [ProgramDocumentID] BIGINT         NOT NULL,
    [FileName]          NVARCHAR (200) NULL,
    [FileExtn]          NVARCHAR (10)  NULL,
    [FolderName]        NVARCHAR (200) NULL,
    [CreatedBy]         BIGINT         NOT NULL,
    [CreatedDate]       DATETIME       NOT NULL
);

