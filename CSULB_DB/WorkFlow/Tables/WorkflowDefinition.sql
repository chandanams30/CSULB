CREATE TABLE [WorkFlow].[WorkflowDefinition] (
    [ID]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [DesignerModel] NVARCHAR (MAX) NULL,
    [Name]          NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_WorkflowDefinition] PRIMARY KEY CLUSTERED ([ID] ASC)
);

