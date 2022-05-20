CREATE TABLE [User].[Users] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [FirstName]            NVARCHAR (255) NOT NULL,
    [LastName]             NVARCHAR (255) NOT NULL,
    [Email]                NVARCHAR (255) NULL,
    [CreatedDateTime]      DATETIME2 (7)  NOT NULL,
    [CreatedByUserID]      BIGINT         NOT NULL,
    [AuthenticationTypeId] INT            NULL,
    [Status]               BIT            NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([ID] ASC)
);



