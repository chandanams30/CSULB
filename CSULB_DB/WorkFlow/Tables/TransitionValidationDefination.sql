CREATE TABLE [WorkFlow].[TransitionValidationDefination] (
    [TransitionValidationDefinationID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [TransitionValidationName]         NVARCHAR (MAX) NULL,
    [IsEnabled]                        BIT            NOT NULL,
    [ValidationDefinationID]           BIGINT         NOT NULL,
    [TransitionDefinitionID]           BIGINT         NOT NULL,
    CONSTRAINT [PK_TransitionValidationDefination] PRIMARY KEY CLUSTERED ([TransitionValidationDefinationID] ASC),
    CONSTRAINT [FK_TransitionValidationDefination_TransitionDefinition_TransitionDefinitionID] FOREIGN KEY ([TransitionDefinitionID]) REFERENCES [WorkFlow].[TransitionDefinition] ([ID]) ON DELETE CASCADE,
    CONSTRAINT [FK_TransitionValidationDefination_ValidationDefination_ValidationDefinationID] FOREIGN KEY ([ValidationDefinationID]) REFERENCES [WorkFlow].[ValidationDefination] ([ValidationDefinationID]) ON DELETE CASCADE
);

