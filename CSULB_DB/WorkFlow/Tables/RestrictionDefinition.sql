CREATE TABLE [WorkFlow].[RestrictionDefinition] (
    [ID]                            BIGINT IDENTITY (1, 1) NOT NULL,
    [RestrictionType_Id]            INT    NULL,
    [Transition_ID]                 BIGINT NULL,
    [ActorDefinitionExecuteRule_ID] BIGINT NULL,
    [ActorDefinitionIsIdentity_ID]  BIGINT NULL,
    [ActorDefinitionIsInRole_ID]    BIGINT NULL,
    CONSTRAINT [PK_RestrictionDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_RestrictionDefinition_ActorDefinitionExecuteRule_ActorDefinitionExecuteRule_ID] FOREIGN KEY ([ActorDefinitionExecuteRule_ID]) REFERENCES [WorkFlow].[ActorDefinitionExecuteRule] ([ID]),
    CONSTRAINT [FK_RestrictionDefinition_ActorDefinitionIsIdentity_ActorDefinitionIsIdentity_ID] FOREIGN KEY ([ActorDefinitionIsIdentity_ID]) REFERENCES [WorkFlow].[ActorDefinitionIsIdentity] ([ID]),
    CONSTRAINT [FK_RestrictionDefinition_ActorDefinitionIsInRole_ActorDefinitionIsInRole_ID] FOREIGN KEY ([ActorDefinitionIsInRole_ID]) REFERENCES [WorkFlow].[ActorDefinitionIsInRole] ([ID]),
    CONSTRAINT [FK_RestrictionDefinition_RestrictionType_RestrictionType_Id] FOREIGN KEY ([RestrictionType_Id]) REFERENCES [WorkFlow].[RestrictionType] ([Id]),
    CONSTRAINT [FK_RestrictionDefinition_TransitionDefinition_Transition_ID] FOREIGN KEY ([Transition_ID]) REFERENCES [WorkFlow].[TransitionDefinition] ([ID])
);

