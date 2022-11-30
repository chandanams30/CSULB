--EXEC [dbo].[GetFieldWorkActivityLogByID] 1, 1
CREATE PROCEDURE [dbo].[GetFieldWorkActivityLogByID] 
	@UserID bigint,
	--@FieldWorkID bigint,
	@ActivityLogID bigint,
	@StartDate datetime = NULL

AS
BEGIN
	SET NOCOUNT ON

	SELECT FWAL.[ID]
      ,FWAL.[FieldWorkID]
	  ,FWAL.[CommunityDistrictID]
	  ,CD.[Name] AS [CommunityDistrict]
      ,FWAL.[CommunitySchoolID]
	  ,CS.[Name] AS [CommunitySchool]
      ,FWAL.[CommunitySiteUsersID]
	  ,U.[FirstName] + ' ' + U.LastName AS [CommunitySiteUser]
      ,FWAL.[ActivityStartDate]
      ,FWAL.[ActivityEndDate]
      ,FWAL.[Hours]
      ,FWAL.[Status]
  FROM [FieldWork].[FieldWorkActivityLog] FWAL
  JOIN [FieldWork].[CommunitySchool] CS ON CS.[ID] = FWAL.[CommunitySchoolID]
  JOIN [FieldWork].[CommunityDistrict] CD ON CD.[ID] = FWAL.[CommunityDistrictID]
  JOIN [User].[Users] U ON U.[ID]=FWAL.[CommunitySiteUsersID]
  WHERE 
  --FWAL.[FieldWorkID]=@FieldWorkID AND 
  FWAL.[ID] = isnull(@ActivityLogID,FWAL.[ID]) AND
  FWAL.[ActivityStartDate]=CASE WHEN @ActivityLogID IS NULL THEN @StartDate ELSE FWAL.ActivityStartDate END


  SELECT FWALS.[FieldWorkActivityLogID]
      ,FWALS.[FieldWorkCoursesCategoryStandardID]
	  ,FWALS.[FieldWorkCoursesCategorySchoolTypeID]
	  ,FWCCS.[Standard] AS [FieldWorkCoursesCategoryStandard]
	  ,FWCCST.[SchoolType] AS [FieldWorkCoursesCategorySchoolType]
      ,FWALS.[Details]
      ,FWALS.[Hours]
  FROM 
  [FieldWork].[FieldWorkActivityLog] FWAL
  JOIN [FieldWork].[FieldWorkActivityLogStandards] FWALS ON FWALS.[FieldWorkActivityLogID] = FWAL.[ID]
  JOIN [Master].[FieldWorkCoursesCategoryStandards] FWCCS ON FWCCS.[ID] = FWALS.[FieldWorkCoursesCategoryStandardID]
  JOIN [Master].[FieldWorkCoursesCategorySchoolTypes] FWCCST ON FWCCST.[ID] = FWALS.[FieldWorkCoursesCategorySchoolTypeID]
  WHERE 
  --FWAL.[FieldWorkID]=@FieldWorkID AND 
  FWAL.[ID] = isnull(@ActivityLogID,FWAL.[ID]) AND
  FWAL.[ActivityStartDate]=CASE WHEN @ActivityLogID IS NULL THEN @StartDate ELSE FWAL.ActivityStartDate END


  ----STATE HANDLER
DECLARE @FWALStatus AS VARCHAR(50), @isCommunityUser AS BIT
SELECT @FWALStatus = FWAL.[Status], 
@isCommunityUser = CASE WHEN FWAL.[Status] = 'Saved' AND FWAL.[CommunitySiteUsersID] = @UserID THEN 1 ELSE 0 END 
FROM [FieldWork].[FieldWorkActivityLog] FWAL WHERE FWAL.[ID] = isnull(@ActivityLogID,FWAL.[ID])

		--SELECT (
		--SELECT [showClose]
		--	  ,[showEdit]
		--	  ,[showSubmit]
		--	  ,[showRejectHours]
		--	  ,[showApproveHours]
		--  FROM [Master].[AcitivityLogHandler] ALH
		--  JOIN [User].[UserRoles] UR ON UR.UserID=@UserID AND ALH.RoleID = CASE WHEN @isCommunityUser=1 THEN 9 ELSE UR.RoleID END
		--	WHERE [Status] = @FWALStatus
		--	AND  UR.RoleID NOT IN (9,10) 
		--  ORDER BY ALH.[RoleID]
		--  FOR JSON AUTO) AS [AcitivityLogHandler];

		SELECT (
		SELECT [showClose]
			  ,[showEdit]
			  ,[showSubmit]
			  ,[showRejectHours]
			  ,[showApproveHours]
		  FROM [Master].[AcitivityLogHandler] ALH
		  JOIN [User].[UserRoles] UR ON UR.UserID=@UserID AND ALH.RoleID = UR.RoleID --CASE WHEN @isCommunityUser=1 THEN 9 ELSE UR.RoleID END
			WHERE [Status] = @FWALStatus
			--AND  UR.RoleID NOT IN (9,10) 
		  ORDER BY ALH.[RoleID]
		  FOR JSON AUTO) AS [AcitivityLogHandler];

END