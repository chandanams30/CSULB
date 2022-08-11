CREATE TABLE [Master].[CourseTerm] (
    [ID]       BIGINT       IDENTITY (1, 1) NOT NULL,
    [CourseID] BIGINT       NOT NULL,
    [TermCode] VARCHAR (10) NOT NULL
);

