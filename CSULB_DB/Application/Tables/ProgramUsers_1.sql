CREATE TABLE [Application].[ProgramUsers] (
    [ID]        INT IDENTITY (1, 1) NOT NULL,
    [UserId]    INT NOT NULL,
    [ProgramId] INT NOT NULL,
    [RoleId]    INT NOT NULL
);

