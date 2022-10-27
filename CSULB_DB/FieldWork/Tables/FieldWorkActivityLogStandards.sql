CREATE TABLE [FieldWork].[FieldWorkActivityLogStandards] (
    [FieldWorkActivityLogID]               BIGINT         NULL,
    [FieldWorkCoursesCategoryStandardID]   BIGINT         NULL,
    [Details]                              VARCHAR (MAX)  NULL,
    [Hours]                                DECIMAL (4, 2) CONSTRAINT [DF__FieldWork__Hours__062DE679] DEFAULT ((0)) NOT NULL,
    [FieldWorkCoursesCategorySchoolTypeID] BIGINT         NULL
);

