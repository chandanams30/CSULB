CREATE TABLE [WorkFlow].[WorkflowProcessInstance] (
    [WorkflowProcessInstanceID]      BIGINT           IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceID]              BIGINT           NOT NULL,
    [SchemeId]                       UNIQUEIDENTIFIER NOT NULL,
    [ActivityName]                   NVARCHAR (MAX)   NULL,
    [StateName]                      NVARCHAR (MAX)   NULL,
    [WorkflowDefinitionID]           BIGINT           NOT NULL,
    [PreviousActivity]               NVARCHAR (MAX)   NULL,
    [PreviousState]                  NVARCHAR (MAX)   NULL,
    [PreviousActivityForDirect]      NVARCHAR (MAX)   NULL,
    [PreviousStateForDirect]         NVARCHAR (MAX)   NULL,
    [PreviousActivityForReverse]     NVARCHAR (MAX)   NULL,
    [PreviousStateForReverse]        NVARCHAR (MAX)   NULL,
    [IsDeterminingParametersChanged] BIT              NOT NULL,
    CONSTRAINT [PK_WorkflowProcessInstance] PRIMARY KEY CLUSTERED ([WorkflowProcessInstanceID] ASC),
    CONSTRAINT [FK_WorkflowProcessInstance_WorkflowDefinition_WorkflowDefinitionID] FOREIGN KEY ([WorkflowDefinitionID]) REFERENCES [WorkFlow].[WorkflowDefinition] ([ID]) ON DELETE CASCADE
);

