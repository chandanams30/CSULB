CREATE TABLE [User].[UserCred] (
    [ID]       INT           IDENTITY (1, 1) NOT NULL,
    [UserId]   INT           NOT NULL,
    [UserName] NVARCHAR (50) NOT NULL,
    [Password] NVARCHAR (50) NOT NULL
);

