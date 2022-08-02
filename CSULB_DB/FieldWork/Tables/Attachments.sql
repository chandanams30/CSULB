CREATE TABLE [FieldWork].[Attachments] (
    [ID]             BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserID]         BIGINT         NOT NULL,
    [DocumentID]     BIGINT         NOT NULL,
    [FileName]       NVARCHAR (200) NULL,
    [FileExtn]       NVARCHAR (10)  NULL,
    [FolderName]     NVARCHAR (200) NULL,
    [IsApproved]     BIT            NULL,
    [ApprovedBy]     BIGINT         NULL,
    [ValidatedDate]  DATETIME       NULL,
    [CreatedBy]      BIGINT         NOT NULL,
    [CreatedDate]    DATETIME       NOT NULL,
    [ValidTill]      DATETIME       NULL,
    [RejectedReason] NVARCHAR (255) NULL,
    [Comments]       NVARCHAR (MAX) NULL
);



