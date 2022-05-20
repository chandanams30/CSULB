CREATE TABLE [WorkFlow].[ActorDefinitionIsIdentity] (
    [ID]                   BIGINT           IDENTITY (1, 1) NOT NULL,
    [IdentityId]           UNIQUEIDENTIFIER NOT NULL,
    [WorkflowDefinitionID] BIGINT           NOT NULL,
    [Name]                 NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_ActorDefinitionIsIdentity] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ActorDefinitionIsIdentity_WorkflowDefinition_WorkflowDefinitionID] FOREIGN KEY ([WorkflowDefinitionID]) REFERENCES [WorkFlow].[WorkflowDefinition] ([ID]) ON DELETE CASCADE
);

