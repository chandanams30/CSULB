CREATE TABLE [WorkFlow].[ActorDefinitionExecuteRule] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [RuleName]             NVARCHAR (MAX) NULL,
    [WorkflowDefinitionID] BIGINT         NOT NULL,
    [Name]                 NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ActorDefinitionExecuteRule] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ActorDefinitionExecuteRule_WorkflowDefinition_WorkflowDefinitionID] FOREIGN KEY ([WorkflowDefinitionID]) REFERENCES [WorkFlow].[WorkflowDefinition] ([ID]) ON DELETE CASCADE
);

