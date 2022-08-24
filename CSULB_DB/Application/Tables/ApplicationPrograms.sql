CREATE TABLE [Application].[ApplicationPrograms] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [ApplicationTypeID] BIGINT         NOT NULL,
    [ProgramID]         BIGINT         NOT NULL,
    [BaseSchema]        NVARCHAR (MAX) NULL
);

