CREATE TABLE [FieldWork].[FieldWork] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [FieldWorkCourseID] BIGINT         NULL,
    [UserID]            BIGINT         NULL,
    [Status]            BIT            NULL,
    [EnrollmentId]      VARCHAR (255)  NULL,
    [CreatedDateTime]   DATETIME2 (7)  NOT NULL,
    [CreatedByUserID]   BIGINT         NOT NULL,
    [ResponseSchema]    NVARCHAR (MAX) NULL,
    [SummaryFnCSchema]  NVARCHAR (MAX) NULL,
    [SAFnCSchema]       NVARCHAR (MAX) NULL,
    [CPAFnCSchema]      NVARCHAR (MAX) NULL
);








GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Summary Tab Feedback and Commands Schema', @level0type = N'SCHEMA', @level0name = N'FieldWork', @level1type = N'TABLE', @level1name = N'FieldWork', @level2type = N'COLUMN', @level2name = N'SummaryFnCSchema';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Supervisor Assessment Feedback and Comments Schema', @level0type = N'SCHEMA', @level0name = N'FieldWork', @level1type = N'TABLE', @level1name = N'FieldWork', @level2type = N'COLUMN', @level2name = N'SAFnCSchema';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Community Partner Assessment Feedback and Schema', @level0type = N'SCHEMA', @level0name = N'FieldWork', @level1type = N'TABLE', @level1name = N'FieldWork', @level2type = N'COLUMN', @level2name = N'CPAFnCSchema';

