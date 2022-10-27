CREATE TABLE [FieldWork].[FieldWorkActivityLogHistory] (
    [FieldWorkActivityLogID] BIGINT         NOT NULL,
    [FieldWorkID]            BIGINT         NULL,
    [CommunityDistrictID]    BIGINT         NULL,
    [CommunitySchoolID]      BIGINT         NULL,
    [CommunitySiteUsersID]   BIGINT         NULL,
    [ActivityStartDate]      DATETIME       NULL,
    [ActivityEndDate]        DATETIME       NULL,
    [Hours]                  DECIMAL (4, 2) NOT NULL,
    [Status]                 VARCHAR (50)   NULL,
    [ModifiedDateTime]       DATETIME       NULL,
    [ModifiedBy]             BIGINT         NULL
);

