CREATE TABLE [WorkFlow].[WorkflowProcessTransitionHistory] (
    [WorkflowProcessTransitionHistoryID] UNIQUEIDENTIFIER NOT NULL,
    [ProcessInstanceID]                  BIGINT           NOT NULL,
    [WorkFlowDefinationID]               BIGINT           NOT NULL,
    [ActorIdentityId]                    NVARCHAR (MAX)   NULL,
    [ExecutorIdentityId]                 NVARCHAR (MAX)   NULL,
    [IsFinalised]                        BIT              NOT NULL,
    [FromActivityName]                   NVARCHAR (MAX)   NULL,
    [FromStateName]                      NVARCHAR (MAX)   NULL,
    [ToActivityName]                     NVARCHAR (MAX)   NULL,
    [ToStateName]                        NVARCHAR (MAX)   NULL,
    [TransitionClassifier]               NVARCHAR (MAX)   NULL,
    [TransitionTime]                     DATETIME2 (7)    NOT NULL,
    [TriggerName]                        NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_WorkflowProcessTransitionHistory] PRIMARY KEY CLUSTERED ([WorkflowProcessTransitionHistoryID] ASC)
);

