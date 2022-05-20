CREATE TABLE [WorkFlow].[ActorDefinitionIsInRole] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [RoleId]               NVARCHAR (MAX) NULL,
    [WorkflowDefinitionID] BIGINT         NOT NULL,
    [Name]                 NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ActorDefinitionIsInRole] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ActorDefinitionIsInRole_WorkflowDefinition_WorkflowDefinitionID] FOREIGN KEY ([WorkflowDefinitionID]) REFERENCES [WorkFlow].[WorkflowDefinition] ([ID]) ON DELETE CASCADE
);

