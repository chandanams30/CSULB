CREATE TABLE [FieldWork].[CommunitySchool] (
    [ID]                  BIGINT        IDENTITY (1, 1) NOT NULL,
    [Name]                VARCHAR (100) NULL,
    [Description]         VARCHAR (100) NULL,
    [CommunityDistrictID] BIGINT        NULL,
    [CreatedBy]           BIGINT        NOT NULL,
    [CreatedDate]         DATETIME      NOT NULL
);

