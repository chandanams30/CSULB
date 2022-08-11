CREATE TABLE [User].[UserRoles] (
    [ID]                BIGINT   IDENTITY (1, 1) NOT NULL,
    [UserID]            BIGINT   NOT NULL,
    [RoleID]            BIGINT   NOT NULL,
    [CreatedDateTime]   DATETIME NOT NULL,
    [CreatedByUserID]   BIGINT   NOT NULL,
    [ApplicationTypeID] BIGINT   NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([ID] ASC)
);





