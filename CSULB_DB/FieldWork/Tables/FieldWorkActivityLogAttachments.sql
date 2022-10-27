CREATE TABLE [FieldWork].[FieldWorkActivityLogAttachments] (
    [ID]                     BIGINT         IDENTITY (1, 1) NOT NULL,
    [FieldWorkID]            BIGINT         NULL,
    [FieldWorkActivityLogID] BIGINT         DEFAULT ((0)) NULL,
    [FileName]               NVARCHAR (200) NULL,
    [FileExtn]               NVARCHAR (10)  NULL,
    [FolderName]             NVARCHAR (200) NULL,
    [CreatedBy]              BIGINT         NOT NULL,
    [CreatedDate]            DATETIME       NOT NULL
);

