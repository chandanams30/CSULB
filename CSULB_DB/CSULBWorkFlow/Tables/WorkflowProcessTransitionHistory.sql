CREATE TABLE [CSULBWorkFlow].[WorkflowProcessTransitionHistory] (
    [WorkflowProcessTransitionHistoryID] BIGINT   IDENTITY (1, 1) NOT NULL,
    [WorkFlowDefinationID]               BIGINT   NOT NULL,
    [FormID]                             BIGINT   NOT NULL,
    [FromActivityDefinitionID]           BIGINT   NOT NULL,
    [ToActivityDefinitionID]             BIGINT   NOT NULL,
    [TransitionDateTime]                 DATETIME NOT NULL
);

