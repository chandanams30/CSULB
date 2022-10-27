CREATE TYPE [FieldWork].[FieldWorkActivityLogStandardsType] AS TABLE (
    [FieldWorkCoursesCategoryStandardID]   BIGINT         NULL,
    [FieldWorkCoursesCategorySchoolTypeID] BIGINT         NULL,
    [Details]                              VARCHAR (MAX)  NULL,
    [Hours]                                DECIMAL (4, 2) NULL);

