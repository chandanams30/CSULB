CREATE TABLE [User].[Users] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [FirstName]            NVARCHAR (255) NOT NULL,
    [LastName]             NVARCHAR (255) NOT NULL,
    [Email]                NVARCHAR (255) NULL,
    [CreatedDateTime]      DATETIME2 (7)  NOT NULL,
    [CreatedByUserID]      BIGINT         NOT NULL,
    [AuthenticationTypeId] INT            NULL,
    [Status]               BIT            NULL,
    [CSULBID]              VARCHAR (50)   NULL,
    [FirstNamePref]        NVARCHAR (255) NULL,
    [LastNamePref]         NVARCHAR (255) NULL,
    [DisplayName]          NVARCHAR (255) NULL
);





