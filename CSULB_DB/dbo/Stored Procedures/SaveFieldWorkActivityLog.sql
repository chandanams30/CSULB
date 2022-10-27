
CREATE PROCEDURE [dbo].[SaveFieldWorkActivityLog] 
(	@UserID BIGINT = null,
	@FieldWorkActivityLogID bigint, 
	@FieldWorkID bigint,
	@CommunityDistrictID bigint,
	@CommunitySchoolID  bigint,
	@CommunitySiteUsersID bigint,
	@ActivityStartDate DateTime,
	@ActivityEndDate DateTime,
	@Hours decimal(4, 2),
	@Status	varchar(50),
	@LogStandards Fieldwork.FieldWorkActivityLogStandardsType READONLY
) AS
BEGIN
	SET NOCOUNT ON

	DECLARE @AcLogID bigint = NULL
	IF (@ActivityStartDate IS NOT NULL) SET @ActivityStartDate = CONVERT(DATE, @ActivityStartDate)
	IF (@ActivityEndDate IS NOT NULL) SET @ActivityEndDate = CONVERT(VARCHAR, CONVERT(DATE, @ActivityEndDate)) + ' 23:59:59' 

	IF NOT EXISTS(SELECT ID FROM [FieldWork].[FieldWorkActivityLog] WHERE ID=@FieldWorkActivityLogID)
	BEGIN
		--Proeed to insert as new data
		INSERT INTO [FieldWork].[FieldWorkActivityLog]
		(
			FieldWorkID,
			CommunityDistrictID,
			CommunitySchoolID,
			CommunitySiteUsersID,
			ActivityStartDate,
			ActivityEndDate,
			[Hours],
			[Status],
			[ModifiedBy],
			[ModifiedDateTime]
		)
		VALUES
		(
			@FieldWorkID,
			@CommunityDistrictID,
			@CommunitySchoolID,
			@CommunitySiteUsersID,
			@ActivityStartDate,
			@ActivityEndDate,
			@Hours,
			@Status,
			@UserID,
			GETDATE()
		)
		IF (@@ERROR = 0)
		BEGIN
			SELECT @AcLogID = @@IDENTITY; 
		END
	END
	ELSE
	BEGIN
		--Proeed to update as existing data
		UPDATE [FieldWork].[FieldWorkActivityLog] SET
			CommunityDistrictID = @CommunityDistrictID,
			CommunitySchoolID = @CommunitySchoolID,
			CommunitySiteUsersID = @CommunitySiteUsersID,
			ActivityStartDate = @ActivityStartDate,
			ActivityEndDate = @ActivityEndDate,
			[Hours] = @Hours,
			[Status] = @Status,
			[ModifiedBy] = @UserID,
			[ModifiedDateTime] = GETDATE()
		WHERE
			ID = @FieldWorkActivityLogID

		IF (@@ERROR = 0)
		BEGIN
			SELECT @AcLogID = @FieldWorkActivityLogID; 
		END
	END		
	
	IF(@AcLogID IS NOT NULL)
	BEGIN
		--Insert [FieldWorkActivityLogStandards] data
		DELETE FROM [FieldWork].[FieldWorkActivityLogStandards] WHERE FieldWorkActivityLogID = @AcLogID

		INSERT INTO [FieldWork].[FieldWorkActivityLogStandards]
		(
			FieldWorkActivityLogID,
			FieldWorkCoursesCategoryStandardID,
			FieldWorkCoursesCategorySchoolTypeID,
			Details, 
			[Hours]
		)
		SELECT 
			@AcLogID,
			FieldWorkCoursesCategoryStandardID,
			FieldWorkCoursesCategorySchoolTypeID,
			Details, 
			[Hours]
		FROM @LogStandards
	END
	
	SELECT @AcLogID 'FieldWorkActivityLogID'
END