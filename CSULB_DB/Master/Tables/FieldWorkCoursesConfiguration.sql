CREATE TABLE [Master].[FieldWorkCoursesConfiguration] (
    [Name]              NVARCHAR (100) NOT NULL,
    [Description]       VARCHAR (100)  NULL,
    [Subject]           VARCHAR (250)  NULL,
    [CourseNumber]      VARCHAR (50)   NULL,
    [CategoryID]        BIGINT         NOT NULL,
    [RecordByDate]      BIT            DEFAULT ((0)) NOT NULL,
    [AutoCompute]       BIT            DEFAULT ((0)) NOT NULL,
    [EnableActivityLog] BIT            CONSTRAINT [DF_FieldWorkCoursesConfiguration_EnableActivityLog] DEFAULT ((0)) NOT NULL
);

