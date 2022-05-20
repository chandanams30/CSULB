CREATE TABLE [WorkFlow].[ParameterDefinitionForAction] (
    [ID]                    BIGINT IDENTITY (1, 1) NOT NULL,
    [IsInputParameter]      BIT    NOT NULL,
    [ParameterDefinitionID] BIGINT NULL,
    [Order]                 INT    NOT NULL,
    [ActionDefinitionID]    BIGINT NULL,
    CONSTRAINT [PK_ParameterDefinitionForAction] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ParameterDefinitionForAction_ActionDefinition_ActionDefinitionID] FOREIGN KEY ([ActionDefinitionID]) REFERENCES [WorkFlow].[ActionDefinition] ([ID]),
    CONSTRAINT [FK_ParameterDefinitionForAction_ParameterDefinition_ParameterDefinitionID] FOREIGN KEY ([ParameterDefinitionID]) REFERENCES [WorkFlow].[ParameterDefinition] ([ID])
);

