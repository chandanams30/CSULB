CREATE TABLE [WorkFlow].[ValidationType] (
    [ValidationTypeID]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [ValidationTypeName]        NVARCHAR (MAX) NULL,
    [ValidationTypeDescription] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ValidationType] PRIMARY KEY CLUSTERED ([ValidationTypeID] ASC)
);

