CREATE TABLE [WorkFlow].[WorkflowProcessInstancePersistence] (
    [WorkflowProcessInstancePersistenceID] UNIQUEIDENTIFIER NOT NULL,
    [ParameterName]                        NVARCHAR (MAX)   NULL,
    [ProcessInstanceID]                    BIGINT           NOT NULL,
    [WorkFlowDefinationID]                 BIGINT           NOT NULL,
    [Value]                                NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_WorkflowProcessInstancePersistence] PRIMARY KEY CLUSTERED ([WorkflowProcessInstancePersistenceID] ASC)
);

