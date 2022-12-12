--EXEC [dbo].[GetFieldWorkActivityLogByID] 1, 1
--EXEC [dbo].[GetFieldWorkActivityLogforAdd] 346, 737
CREATE PROCEDURE [dbo].[GetFieldWorkActivityLogforAdd] 
	@UserID bigint,
	@FieldWorkID bigint
	--@ActivityLogID bigint,
	--@StartDate datetime = NULL

AS
BEGIN
	SET NOCOUNT ON

	DECLARE @ActivityLogID bigint;
	SELECT @ActivityLogID = FWAL.[ID] FROM [FieldWork].[FieldWorkActivityLog] FWAL WHERE FWAL.[FieldWorkID] = @FieldWorkID ORDER BY FWAL.[ActivityStartDate], FWAL.[ID];


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
	  --,CASE WHEN FWAL.[Status] = 'Saved' THEN 'Submitted' ELSE FWAL.[Status] END AS [Status] 
  FROM [FieldWork].[FieldWorkActivityLog] FWAL
  JOIN [FieldWork].[CommunitySchool] CS ON CS.[ID] = FWAL.[CommunitySchoolID]
  JOIN [FieldWork].[CommunityDistrict] CD ON CD.[ID] = FWAL.[CommunityDistrictID]
  JOIN [User].[Users] U ON U.[ID]=FWAL.[CommunitySiteUsersID]
  WHERE 
  --FWAL.[FieldWorkID]=@FieldWorkID AND 
  --FWAL.[ID] = isnull(@ActivityLogID,FWAL.[ID]) 
  FWAL.[ID] = @ActivityLogID
  --AND FWAL.[ActivityStartDate]=CASE WHEN @ActivityLogID IS NULL THEN @StartDate ELSE FWAL.ActivityStartDate END

  IF EXISTS (SELECT * FROM [FieldWork].[FieldWorkActivityLog] FWAL WHERE FWAL.[ID] = @ActivityLogID)
	  BEGIN
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
		  FWAL.[ID] = @ActivityLogID
	  END
  ELSE
	  BEGIN
		  SELECT NULL AS [FieldWorkActivityLogID]
			  ,NULL AS [FieldWorkCoursesCategoryStandardID]
			  ,NULL AS [FieldWorkCoursesCategorySchoolTypeID]
			  ,NULL AS [FieldWorkCoursesCategoryStandard]
			  ,NULL AS [FieldWorkCoursesCategorySchoolType]
			  ,NULL AS [Details]
			  ,NULL AS [Hours]
	  END


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