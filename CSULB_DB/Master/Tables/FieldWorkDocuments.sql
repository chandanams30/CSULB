CREATE TABLE [Master].[FieldWorkDocuments] (
    [ID]              BIGINT        IDENTITY (1, 1) NOT NULL,
    [DocumentID]      BIGINT        NOT NULL,
    [CreatedDateTime] DATETIME2 (7) NOT NULL,
    [CreatedByUserID] BIGINT        NOT NULL,
    [CourseID]        BIGINT        NULL,
    [IsRestricted]    BIT           DEFAULT ((0)) NOT NULL
);





