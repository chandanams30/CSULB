CREATE TABLE [CSULBWorkFlow].[TransitionDefinition] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [WorkflowDefinitionID] BIGINT         NOT NULL,
    [FromID]               BIGINT         NULL,
    [ToID]                 BIGINT         NULL,
    [Name]                 NVARCHAR (MAX) NULL
);

