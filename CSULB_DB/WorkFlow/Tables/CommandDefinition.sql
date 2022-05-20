CREATE TABLE [WorkFlow].[CommandDefinition] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [CommandIconClass]     NVARCHAR (MAX) NULL,
    [WorkflowDefinitionID] BIGINT         NOT NULL,
    [Name]                 NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_CommandDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CommandDefinition_WorkflowDefinition_WorkflowDefinitionID] FOREIGN KEY ([WorkflowDefinitionID]) REFERENCES [WorkFlow].[WorkflowDefinition] ([ID]) ON DELETE CASCADE
);

