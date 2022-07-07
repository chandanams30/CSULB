CREATE TABLE [FieldWork].[FieldWorkUsers] (
    [ID]          BIGINT IDENTITY (1, 1) NOT NULL,
    [UserID]      BIGINT NOT NULL,
    [FieldWorkID] BIGINT NOT NULL,
    [RoleID]      BIGINT NOT NULL
);

