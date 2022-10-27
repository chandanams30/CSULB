CREATE TABLE [Master].[FieldWorkCoursesCategoryStandards] (
    [ID]              BIGINT        NOT NULL,
    [Standard]        VARCHAR (100) NULL,
    [Description]     VARCHAR (MAX) NULL,
    [CategoryID]      BIGINT        NOT NULL,
    [CreatedDateTime] DATETIME      DEFAULT (getdate()) NOT NULL,
    [CreatedByUserID] BIGINT        NOT NULL
);

