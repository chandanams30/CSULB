CREATE TABLE [Master].[FieldWorkCourses] (
    [ID]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [CSULBCourseId]   VARCHAR (50)   NULL,
    [CourseTitle]     VARCHAR (50)   NULL,
    [TermCode]        VARCHAR (50)   NULL,
    [Subject]         VARCHAR (250)  NULL,
    [CourseNumber]    VARCHAR (50)   NULL,
    [ClassSection]    VARCHAR (50)   NULL,
    [ClassStatus]     BIT            NULL,
    [Division]        VARCHAR (50)   NULL,
    [Program]         VARCHAR (50)   NULL,
    [Type]            VARCHAR (50)   NULL,
    [College]         VARCHAR (50)   NULL,
    [CreatedDateTime] DATETIME2 (7)  NOT NULL,
    [CreatedByUserID] BIGINT         NOT NULL,
    [BaseSchema]      NVARCHAR (MAX) NULL
);



