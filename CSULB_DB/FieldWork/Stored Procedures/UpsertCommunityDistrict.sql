
-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2023-Mar-17
-- Description:	Community District List
-- =============================================
CREATE PROCEDURE [FieldWork].[UpsertCommunityDistrict]
@CommunityDistrictID BIGINT
,@CommunityDistrictName varchar(100)
,@createdByUserID bigint 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

IF ((@CommunityDistrictID IS NULL) OR (@CommunityDistrictID=0))
	BEGIN
		INSERT INTO [FieldWork].[CommunityDistrict]
				   ([Name]
				   ,[Description]
				   ,[CreatedBy]
				   ,[CreatedDate])
			 VALUES
				   (@CommunityDistrictName
				   ,@CommunityDistrictName
				   ,@createdByUserID
				   ,GETDATE())
	END
ELSE
	BEGIN
		UPDATE [FieldWork].[CommunityDistrict]
		   SET [Name] = @CommunityDistrictName
		   ,[Description] = @CommunityDistrictName
		 WHERE [ID] = @CommunityDistrictID
	END

END