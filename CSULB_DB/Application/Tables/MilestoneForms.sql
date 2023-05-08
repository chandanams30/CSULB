CREATE TABLE [Application].[MilestoneForms] (
    [ID]                  BIGINT         IDENTITY (1, 1) NOT NULL,
    [MilestoneID]         BIGINT         NOT NULL,
    [FormID]              BIGINT         NOT NULL,
    [MilestoneFilledForm] NVARCHAR (MAX) NULL,
    [Status]              INT            NULL,
    [CreatedBy]           BIGINT         NOT NULL,
    [CreatedDate]         DATETIME       NOT NULL
);

