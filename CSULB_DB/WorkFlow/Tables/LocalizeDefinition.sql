CREATE TABLE [WorkFlow].[LocalizeDefinition] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [LocalizeTypeID]       INT            NOT NULL,
    [IsDefault]            BIT            NOT NULL,
    [ObjectName]           NVARCHAR (MAX) NULL,
    [Culture]              NVARCHAR (MAX) NULL,
    [Value]                NVARCHAR (MAX) NULL,
    [WorkflowDefinitionID] BIGINT         NOT NULL,
    CONSTRAINT [PK_LocalizeDefinition] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_LocalizeDefinition_LocalizeType_LocalizeTypeID] FOREIGN KEY ([LocalizeTypeID]) REFERENCES [WorkFlow].[LocalizeType] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_LocalizeDefinition_WorkflowDefinition_WorkflowDefinitionID] FOREIGN KEY ([WorkflowDefinitionID]) REFERENCES [WorkFlow].[WorkflowDefinition] ([ID]) ON DELETE CASCADE
);

