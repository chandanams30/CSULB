CREATE TABLE [Application].[InterviewerAttachments] (
    [ID]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [InterviewID]  BIGINT         NOT NULL,
    [DocumentID]   BIGINT         NULL,
    [FileName]     NVARCHAR (200) NULL,
    [FileExtn]     NVARCHAR (10)  NULL,
    [FolderName]   NVARCHAR (200) NULL,
    [UploadedDate] DATETIME       NULL
);

