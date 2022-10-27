CREATE TABLE [Master].[FieldWorkCoursesCategorySchoolTypes] (
    [ID]              BIGINT        NOT NULL,
    [SchoolType]      VARCHAR (100) NULL,
    [Description]     VARCHAR (MAX) NULL,
    [CategoryID]      BIGINT        NOT NULL,
    [CreatedDateTime] DATETIME      NOT NULL,
    [CreatedByUserID] BIGINT        NOT NULL
);

