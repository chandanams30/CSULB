CREATE TABLE [WorkFlow].[WorkflowProcessTimer] (
    [WorkflowProcessTimerID] UNIQUEIDENTIFIER NOT NULL,
    [Name]                   NVARCHAR (MAX)   NULL,
    [NextExecutionDateTime]  DATETIME2 (7)    NOT NULL,
    [ProcessInstanceID]      BIGINT           NOT NULL,
    [WorkFlowDefinationID]   BIGINT           NOT NULL,
    [Ignore]                 BIT              NOT NULL,
    CONSTRAINT [PK_WorkflowProcessTimer] PRIMARY KEY CLUSTERED ([WorkflowProcessTimerID] ASC)
);

