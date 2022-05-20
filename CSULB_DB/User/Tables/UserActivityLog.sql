CREATE TABLE [User].[UserActivityLog] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [UserId]        BIGINT        NOT NULL,
    [LoginDateTime] DATETIME      NOT NULL,
    [IPAddress]     NVARCHAR (50) NULL
);

