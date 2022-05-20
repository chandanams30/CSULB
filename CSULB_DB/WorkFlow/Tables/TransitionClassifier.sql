CREATE TABLE [WorkFlow].[TransitionClassifier] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR (100) NULL,
    CONSTRAINT [PK_TransitionClassifier] PRIMARY KEY CLUSTERED ([Id] ASC)
);

