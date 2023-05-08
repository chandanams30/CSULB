-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2023-Mar-17
-- Description:	Community District List
-- =============================================
CREATE PROCEDURE [FieldWork].[UpsertCommunitySchool]
@CommunitySchoolID BIGINT
,@CommunitySchoolName varchar(100)
,@CommunityDistrictID BIGINT
,@createdByUserID bigint 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
IF ((@CommunitySchoolID IS NULL) OR (@CommunitySchoolID = 0))
	BEGIN
		INSERT INTO [FieldWork].[CommunitySchool]
				   ([Name]
				   ,[Description]
				   ,[CommunityDistrictID]
				   ,[CreatedBy]
				   ,[CreatedDate])
			 VALUES
				   (@CommunitySchoolName
				   ,@CommunitySchoolName
				   ,@CommunityDistrictID
				   ,@createdByUserID
				   ,GETDATE())
	END
ELSE
	BEGIN
		UPDATE [FieldWork].[CommunitySchool]
		   SET [Name] = @CommunitySchoolName
		   ,[Description] = @CommunitySchoolName
		   ,[CommunityDistrictID] = @CommunityDistrictID
		 WHERE [ID] = @CommunitySchoolID
	END
END