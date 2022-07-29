CREATE TABLE [Master].[FieldWorkCourses] (
    [ID]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [CSULBCourseId]   VARCHAR (50)   NULL,
    [CourseTermID]    BIGINT         NOT NULL,
    [CreatedDateTime] DATETIME       NOT NULL,
    [CreatedByUserID] BIGINT         NOT NULL,
    [BaseSchema]      NVARCHAR (MAX) NULL
);





