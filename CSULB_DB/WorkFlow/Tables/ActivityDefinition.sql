CREATE TABLE [WorkFlow].[ActivityDefinition] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [State]                NVARCHAR (MAX) NULL,
    [IsInitial]            BIT            NOT NULL,
    [IsFinal]              BIT            NOT NULL,
    [IsForSetState]        BIT            NOT NULL,
    [IsAutoSchemeUpdate]   BIT            NOT NULL,
    [WorkflowDefinitionID] BIGINT         NOT NULL,
    [Name]                 NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ActivityDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ActivityDefinition_WorkflowDefinition_WorkflowDefinitionID] FOREIGN KEY ([WorkflowDefinitionID]) REFERENCES [WorkFlow].[WorkflowDefinition] ([ID]) ON DELETE CASCADE
);

