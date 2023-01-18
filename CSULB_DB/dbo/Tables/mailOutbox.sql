CREATE TABLE [dbo].[mailOutbox] (
    [mailID]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [emailUserName]  NVARCHAR (200) NULL,
    [emailPassword]  NVARCHAR (200) NULL,
    [fromAddress]    NVARCHAR (200) NULL,
    [smtpSever]      NVARCHAR (200) NULL,
    [portNumber]     NVARCHAR (200) NULL,
    [enableSSL]      BIT            NULL,
    [toAddress]      NVARCHAR (500) NULL,
    [ccAddress]      NVARCHAR (500) NULL,
    [bccAddress]     NVARCHAR (500) NULL,
    [mailSubject]    NVARCHAR (200) NULL,
    [mailBody]       NVARCHAR (MAX) NULL,
    [createdON]      DATETIME       NULL,
    [isMailSent]     BIT            CONSTRAINT [DF_mailOutbox_isMailSent] DEFAULT ((0)) NULL,
    [malSentON]      DATETIME       NULL,
    [attachmentHTML] NVARCHAR (MAX) NULL
);

