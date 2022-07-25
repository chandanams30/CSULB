CREATE TABLE [FieldWork].[FieldWork] (
    [ID]              BIGINT         IDENTITY (1, 1) NOT NULL,
    [CourseID]        BIGINT         NULL,
    [UserID]          BIGINT         NULL,
    [Status]          BIT            NULL,
    [EnrollmentId]    VARCHAR (255)  NULL,
    [CreatedDateTime] DATETIME2 (7)  NOT NULL,
    [CreatedByUserID] BIGINT         NOT NULL,
    [ResponseSchema]  NVARCHAR (MAX) NULL
);



