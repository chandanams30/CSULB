CREATE TABLE [WorkFlow].[TriggerDefinition] (
    [ID]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (MAX) NULL,
    [CommandID] BIGINT         NULL,
    [TypeId]    INT            NULL,
    CONSTRAINT [PK_TriggerDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_TriggerDefinition_CommandDefinition_CommandID] FOREIGN KEY ([CommandID]) REFERENCES [WorkFlow].[CommandDefinition] ([ID]),
    CONSTRAINT [FK_TriggerDefinition_TriggerType_TypeId] FOREIGN KEY ([TypeId]) REFERENCES [WorkFlow].[TriggerType] ([Id])
);

