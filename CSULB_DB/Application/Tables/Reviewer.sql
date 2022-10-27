CREATE TABLE [Application].[Reviewer] (
    [ID]                     BIGINT         IDENTITY (1, 1) NOT NULL,
    [FormID]                 BIGINT         NOT NULL,
    [ReviewerID]             BIGINT         NOT NULL,
    [ReviewerRecommendation] NVARCHAR (200) NULL,
    [ReviewerComments]       NVARCHAR (MAX) NULL,
    [ReviewedOn]             DATETIME       NULL,
    [CreatedBy]              BIGINT         NOT NULL,
    [CreatedDate]            DATETIME       NOT NULL
);

