CREATE TABLE [WorkFlow].[TransitionDefinition] (
    [ID]                     BIGINT         IDENTITY (1, 1) NOT NULL,
    [ConditionID]            BIGINT         NOT NULL,
    [TransitionClassifierID] INT            NOT NULL,
    [TriggerID]              BIGINT         NOT NULL,
    [WorkflowDefinitionID]   BIGINT         NOT NULL,
    [FromID]                 BIGINT         NULL,
    [ToID]                   BIGINT         NULL,
    [Name]                   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TransitionDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_TransitionDefinition_ActivityDefinition_FromID] FOREIGN KEY ([FromID]) REFERENCES [WorkFlow].[ActivityDefinition] ([ID]),
    CONSTRAINT [FK_TransitionDefinition_ActivityDefinition_ToID] FOREIGN KEY ([ToID]) REFERENCES [WorkFlow].[ActivityDefinition] ([ID]),
    CONSTRAINT [FK_TransitionDefinition_ConditionDefinition_ConditionID] FOREIGN KEY ([ConditionID]) REFERENCES [WorkFlow].[ConditionDefinition] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_TransitionDefinition_TransitionClassifier_TransitionClassifierID] FOREIGN KEY ([TransitionClassifierID]) REFERENCES [WorkFlow].[TransitionClassifier] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_TransitionDefinition_TriggerDefinition_TriggerID] FOREIGN KEY ([TriggerID]) REFERENCES [WorkFlow].[TriggerDefinition] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_TransitionDefinition_WorkflowDefinition_WorkflowDefinitionID] FOREIGN KEY ([WorkflowDefinitionID]) REFERENCES [WorkFlow].[WorkflowDefinition] ([ID]) ON DELETE CASCADE
);

