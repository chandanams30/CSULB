CREATE TABLE [dbo].[mailOutboxAttachments] (
    [mailAttachmentID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [mailID]           BIGINT         NULL,
    [attachmentHTML]   NVARCHAR (MAX) NULL
);

