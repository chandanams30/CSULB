CREATE TABLE [User].[UserRegistration] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [FirstName]            NVARCHAR (255) NOT NULL,
    [LastName]             NVARCHAR (255) NOT NULL,
    [Email]                NVARCHAR (255) NULL,
    [AuthenticationTypeId] INT            NULL,
    [CSULBID]              VARCHAR (50)   NULL,
    [Password]             VARCHAR (50)   NULL,
    [isUserCreated]        BIT            DEFAULT ((0)) NOT NULL,
    [CreatedDateTime]      DATETIME2 (7)  NOT NULL,
    [CreatedByUserID]      BIGINT         NOT NULL
);

