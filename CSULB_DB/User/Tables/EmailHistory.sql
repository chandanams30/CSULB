CREATE TABLE [User].[EmailHistory] (
    [ID]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserID]      BIGINT         NULL,
    [Subject]     NVARCHAR (200) NULL,
    [Body]        NVARCHAR (MAX) NULL,
    [Email_to]    VARCHAR (500)  NULL,
    [Email_from]  VARCHAR (500)  NULL,
    [email_date]  DATETIME       NULL,
    [cc]          NVARCHAR (500) NULL,
    [bcc]         NVARCHAR (500) NULL,
    [notes]       NVARCHAR (500) NULL,
    [isEmailSent] BIT            NULL
);

