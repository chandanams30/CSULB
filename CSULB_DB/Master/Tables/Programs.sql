CREATE TABLE [Master].[Programs] (
    [ID]                    BIGINT         NOT NULL,
    [Name]                  NVARCHAR (100) NOT NULL,
    [Description]           VARCHAR (100)  NULL,
    [ShortName]             NVARCHAR (30)  NULL,
    [AliasNames]            NVARCHAR (MAX) NULL,
    [CreatedDateTime]       DATETIME       NOT NULL,
    [CreatedByUserID]       BIGINT         NOT NULL,
    [ProgramFormIdentifier] VARCHAR (10)   NULL,
    [AcademicPlanCode]      NVARCHAR (20)  NULL
);











