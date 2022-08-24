CREATE TABLE [Master].[ProgramUsers] (
    [ID]        BIGINT IDENTITY (1, 1) NOT NULL,
    [UserID]    BIGINT NOT NULL,
    [ProgramID] BIGINT NOT NULL,
    [RoleID]    BIGINT NOT NULL
);

