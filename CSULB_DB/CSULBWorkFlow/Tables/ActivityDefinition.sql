CREATE TABLE [CSULBWorkFlow].[ActivityDefinition] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [State]                NVARCHAR (MAX) NULL,
    [IsInitial]            BIT            NOT NULL,
    [IsFinal]              BIT            NOT NULL,
    [IsForSetState]        BIT            NOT NULL,
    [WorkflowDefinitionID] BIGINT         NOT NULL,
    [Name]                 NVARCHAR (MAX) NULL,
    [ActivityControlLabel] NVARCHAR (20)  NULL
);

