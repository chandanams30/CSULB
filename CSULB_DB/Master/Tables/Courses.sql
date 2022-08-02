CREATE TABLE [Master].[Courses] (
    [ID]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (100) NOT NULL,
    [Description]     VARCHAR (100)  NULL,
    [Subject]         VARCHAR (250)  NULL,
    [CourseNumber]    VARCHAR (50)   NULL,
    [ClassSection]    VARCHAR (50)   NULL,
    [Division]        VARCHAR (50)   NULL,
    [Program]         VARCHAR (50)   NULL,
    [Type]            VARCHAR (50)   NULL,
    [College]         VARCHAR (50)   NULL,
    [AliasNames]      NVARCHAR (MAX) NULL,
    [CreatedDateTime] DATETIME       NOT NULL,
    [CreatedByUserID] BIGINT         NOT NULL,
    [FieldWorkHours]  INT            NULL
);



