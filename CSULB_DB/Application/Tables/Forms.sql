CREATE TABLE [Application].[Forms] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserID]            BIGINT         NOT NULL,
    [ProgramID]         BIGINT         NOT NULL,
    [SemesterID]        BIGINT         NOT NULL,
    [Form]              NVARCHAR (MAX) NOT NULL,
    [CreatedDateTime]   DATETIME2 (7)  NOT NULL,
    [CreatedByUserID]   BIGINT         NOT NULL,
    [ModifiedDateTime]  DATETIME2 (7)  NOT NULL,
    [ModifiedBy]        BIGINT         NOT NULL,
    [State]             INT            NOT NULL,
    [ApplicationNumber] NVARCHAR (100) NULL,
    CONSTRAINT [PK_Forms] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Forms_Semester] FOREIGN KEY ([SemesterID]) REFERENCES [Master].[Semester] ([ID])
);





