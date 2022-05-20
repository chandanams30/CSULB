CREATE TABLE [WorkFlow].[ValidationFieldDefination] (
    [ValidationFieldDefinationID]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [ValidationFieldDefinationName] NVARCHAR (MAX) NULL,
    [ValidationFieldValue]          NVARCHAR (MAX) NULL,
    [ValidationDefinationID]        BIGINT         NOT NULL,
    CONSTRAINT [PK_ValidationFieldDefination] PRIMARY KEY CLUSTERED ([ValidationFieldDefinationID] ASC),
    CONSTRAINT [FK_ValidationFieldDefination_ValidationDefination_ValidationDefinationID] FOREIGN KEY ([ValidationDefinationID]) REFERENCES [WorkFlow].[ValidationDefination] ([ValidationDefinationID]) ON DELETE CASCADE
);

