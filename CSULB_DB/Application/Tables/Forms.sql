CREATE TABLE [Application].[Forms] (
    [ID]                    BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserID]                BIGINT         NOT NULL,
    [ApplicationProgramsID] BIGINT         NOT NULL,
    [TermCode]              VARCHAR (10)   NULL,
    [Form]                  NVARCHAR (MAX) NULL,
    [FormStateID]           INT            NOT NULL,
    [ApplicationNumber]     NVARCHAR (100) NULL,
    [ResponseSchema]        NVARCHAR (MAX) NULL,
    [CreatedDateTime]       DATETIME       NOT NULL,
    [CreatedByUserID]       BIGINT         NOT NULL
);









