CREATE TABLE [WorkFlow].[ConditionDefinition] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]                 NVARCHAR (MAX) NULL,
    [ConditionTypeID]      INT            NOT NULL,
    [ResultOnPreExecution] BIT            NULL,
    [Action_ID]            BIGINT         NULL,
    CONSTRAINT [PK_ConditionDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ConditionDefinition_ActionDefinition_Action_ID] FOREIGN KEY ([Action_ID]) REFERENCES [WorkFlow].[ActionDefinition] ([ID]),
    CONSTRAINT [FK_ConditionDefinition_ConditionType_ConditionTypeID] FOREIGN KEY ([ConditionTypeID]) REFERENCES [WorkFlow].[ConditionType] ([Id]) ON DELETE CASCADE
);

