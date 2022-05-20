CREATE TABLE [WorkFlow].[ActionDefinition] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [TypeAsString]         NVARCHAR (MAX) NULL,
    [FullTypeName]         NVARCHAR (MAX) NULL,
    [MethodName]           NVARCHAR (MAX) NULL,
    [WorkflowDefinitionID] BIGINT         NOT NULL,
    [Name]                 NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ActionDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ActionDefinition_WorkflowDefinition_WorkflowDefinitionID] FOREIGN KEY ([WorkflowDefinitionID]) REFERENCES [WorkFlow].[WorkflowDefinition] ([ID]) ON DELETE CASCADE
);

