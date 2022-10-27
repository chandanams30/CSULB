CREATE TABLE [FieldWork].[FieldWorkActivityLog] (
    [ID]                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [FieldWorkID]          BIGINT         NULL,
    [CommunityDistrictID]  BIGINT         NULL,
    [CommunitySchoolID]    BIGINT         NULL,
    [CommunitySiteUsersID] BIGINT         NULL,
    [ActivityStartDate]    DATETIME       NULL,
    [ActivityEndDate]      DATETIME       NULL,
    [Hours]                DECIMAL (4, 2) CONSTRAINT [DF__FieldWork__Hours__04459E07] DEFAULT ((0)) NOT NULL,
    [Status]               VARCHAR (50)   CONSTRAINT [DF_FieldWorkActivityLog_Status] DEFAULT ('Saved') NULL,
    [ActivityLogFnCSchema] NVARCHAR (MAX) NULL,
    [ModifiedDateTime]     DATETIME       NULL,
    [ModifiedBy]           BIGINT         NULL
);


GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER [FieldWork].[TR_FieldWorkActivityLog_Update] 
   ON  [FieldWork].[FieldWorkActivityLog]
   AFTER UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
		INSERT INTO [FieldWork].[FieldWorkActivityLogHistory]
           ([FieldWorkActivityLogID]
           ,[FieldWorkID]
           ,[CommunityDistrictID]
           ,[CommunitySchoolID]
           ,[CommunitySiteUsersID]
           ,[ActivityStartDate]
           ,[ActivityEndDate]
           ,[Hours]
           ,[Status]
           ,[ModifiedDateTime]
           ,[ModifiedBy])
			(SELECT [ID]
				,[FieldWorkID]
				,[CommunityDistrictID]
				,[CommunitySchoolID]
				,[CommunitySiteUsersID]
				,[ActivityStartDate]
				,[ActivityEndDate]
				,[Hours]
				,[Status]
				,[ModifiedDateTime]
				,[ModifiedBy]
			FROM deleted)
END
GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Activity Log Feedback and Comments Schema', @level0type = N'SCHEMA', @level0name = N'FieldWork', @level1type = N'TABLE', @level1name = N'FieldWorkActivityLog', @level2type = N'COLUMN', @level2name = N'ActivityLogFnCSchema';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Saved,Submitted,Approved,Not-Approved', @level0type = N'SCHEMA', @level0name = N'FieldWork', @level1type = N'TABLE', @level1name = N'FieldWorkActivityLog', @level2type = N'COLUMN', @level2name = N'Status';

