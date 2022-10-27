CREATE TABLE [Master].[FieldWorkCoursesCategory] (
    [ID]              BIGINT        NOT NULL,
    [Category]        VARCHAR (100) NULL,
    [Description]     VARCHAR (100) NULL,
    [CreatedDateTime] DATETIME      DEFAULT (getdate()) NOT NULL,
    [CreatedByUserID] BIGINT        NOT NULL
);

