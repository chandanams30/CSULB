CREATE TABLE [WorkFlow].[ParameterDefinition] (
    [ID]                     BIGINT         IDENTITY (1, 1) NOT NULL,
    [TypeAsString]           NVARCHAR (MAX) NULL,
    [PurposeID]              INT            NOT NULL,
    [SerializedDefaultValue] NVARCHAR (MAX) NULL,
    [WorkflowDefinitionID]   BIGINT         NOT NULL,
    [Name]                   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ParameterDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ParameterDefinition_ParameterPurpose_PurposeID] FOREIGN KEY ([PurposeID]) REFERENCES [WorkFlow].[ParameterPurpose] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ParameterDefinition_WorkflowDefinition_WorkflowDefinitionID] FOREIGN KEY ([WorkflowDefinitionID]) REFERENCES [WorkFlow].[WorkflowDefinition] ([ID]) ON DELETE CASCADE
);

