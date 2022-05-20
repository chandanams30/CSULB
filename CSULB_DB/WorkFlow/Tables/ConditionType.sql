CREATE TABLE [WorkFlow].[ConditionType] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR (100) NULL,
    CONSTRAINT [PK_ConditionType] PRIMARY KEY CLUSTERED ([Id] ASC)
);

