CREATE TABLE [WorkFlow].[WorkflowProcessInstanceStatus] (
    [WorkflowProcessInstanceStatusID] BIGINT           IDENTITY (1, 1) NOT NULL,
    [Lock]                            UNIQUEIDENTIFIER NOT NULL,
    [Status]                          INT              NOT NULL,
    [ProcessInstanceID]               BIGINT           NOT NULL,
    [WorkFlowDefinationID]            BIGINT           NOT NULL,
    CONSTRAINT [PK_WorkflowProcessInstanceStatus] PRIMARY KEY CLUSTERED ([WorkflowProcessInstanceStatusID] ASC)
);

