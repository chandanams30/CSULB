CREATE TABLE [WorkFlow].[ActionDefinitionForActivity] (
    [ID]                   BIGINT IDENTITY (1, 1) NOT NULL,
    [ActionDefinitionID]   BIGINT NULL,
    [ActivityDefinitionID] BIGINT NULL,
    [IsPostExecution]      BIT    NOT NULL,
    [Order]                INT    NOT NULL,
    CONSTRAINT [PK_ActionDefinitionForActivity] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ActionDefinitionForActivity_ActionDefinition_ActionDefinitionID] FOREIGN KEY ([ActionDefinitionID]) REFERENCES [WorkFlow].[ActionDefinition] ([ID]),
    CONSTRAINT [FK_ActionDefinitionForActivity_ActivityDefinition_ActivityDefinitionID] FOREIGN KEY ([ActivityDefinitionID]) REFERENCES [WorkFlow].[ActivityDefinition] ([ID])
);

