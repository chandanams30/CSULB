CREATE TABLE [WorkFlow].[LocalizeType] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR (100) NULL,
    CONSTRAINT [PK_LocalizeType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

