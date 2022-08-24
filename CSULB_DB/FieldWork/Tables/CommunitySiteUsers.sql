CREATE TABLE [FieldWork].[CommunitySiteUsers] (
    [ID]              BIGINT IDENTITY (1, 1) NOT NULL,
    [UserID]          BIGINT NOT NULL,
    [CommunitySiteID] BIGINT NOT NULL,
    [RoleID]          BIGINT NOT NULL
);

