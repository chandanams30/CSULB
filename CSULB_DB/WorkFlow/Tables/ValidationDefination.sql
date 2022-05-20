CREATE TABLE [WorkFlow].[ValidationDefination] (
    [ValidationDefinationID]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [ValidationDefinationName]         NVARCHAR (MAX) NULL,
    [ValidationDefinationDescription]  NVARCHAR (MAX) NULL,
    [IsEnabled]                        BIT            NOT NULL,
    [ValidationDefinationErrorMessage] NVARCHAR (MAX) NULL,
    [ValidationTypeID]                 BIGINT         NOT NULL,
    CONSTRAINT [PK_ValidationDefination] PRIMARY KEY CLUSTERED ([ValidationDefinationID] ASC),
    CONSTRAINT [FK_ValidationDefination_ValidationType_ValidationTypeID] FOREIGN KEY ([ValidationTypeID]) REFERENCES [WorkFlow].[ValidationType] ([ValidationTypeID]) ON DELETE CASCADE
);

