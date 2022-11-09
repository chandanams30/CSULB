--EXEC [dbo].[GetFieldWorkActivityLogList] 361,213
CREATE PROCEDURE [dbo].[GetFieldWorkActivityLogList] 
	@FieldWorkID bigint,
	@UserID bigint
AS
BEGIN
	SET NOCOUNT ON
	   
	--DECLARE @CommunityPartner bigint = 0
	--SELECT @CommunityPartner =[UserID] FROM [FieldWork].[CommunitySiteUsers] WHERE [UserID] = @UserID AND [RoleID]=9 
	----SELECT @CommunityPartner

	--DECLARE @CommunitySite bigint = 0
	--SELECT @CommunitySite =[CommunityDistrictID] FROM [FieldWork].[CommunitySiteUsers] WHERE [UserID] = @UserID AND [RoleID]=10 
	----SELECT @CommunitySite

	DECLARE @CommunityPartner bigint = 0
	DECLARE @CommunitySite bigint = 0

	IF NOT EXISTS (SELECT * FROM [User].[UserRoles] UR WHERE UserID = @UserID AND [RoleID] IN (1,4,5))
	BEGIN
		SELECT @CommunityPartner =[UserID] FROM [FieldWork].[CommunitySiteUsers] WHERE [UserID] = @UserID AND [RoleID]=9;
		SELECT @CommunitySite =[CommunityDistrictID] FROM [FieldWork].[CommunitySiteUsers] WHERE [UserID] = @UserID AND [RoleID]=10;
	END

	DECLARE @canApproveHours as bit = 0
	IF EXISTS (SELECT * FROM [User].[UserRoles] UR WHERE UserID = @UserID AND [RoleID] IN (1,4,5))
	BEGIN
		set @canApproveHours = 1
	END

	
	SELECT 
		FAL.[ID]
		,FORMAT(CAST(FAL.[ID] AS INT), 'ID0000000000') 'DisplayID'
		,FAL.[FieldWorkID]
		,FAL.[CommunitySchoolID] AS  [CommunitySiteID]
		,CS.[Name] 'Site'
		,FAL.[ActivityStartDate]
		,FAL.[ActivityEndDate]
		,FAL.[Hours]
		,FAL.[Status] 
		,CASE WHEN FAL.[Status] = 'Saved' AND (FAL.[CommunitySiteUsersID] = @UserID OR @canApproveHours = 1) THEN 1
		ELSE 0 END AS [ShowCheckbox]
		--1 AS [ShowCheckbox]
	FROM
		[FieldWork].[FieldWork] FW 
		JOIN [FieldWork].[FieldWorkActivityLog] FAL ON FAL.[FieldWorkID] = FW.[ID]
		JOIN [Fieldwork].[CommunitySchool] CS ON CS.ID = FAL.CommunitySchoolID
	WHERE 
		[FieldWorkID] = @FieldWorkID 
		AND FAL.[CommunitySiteUsersID]= CASE WHEN @CommunityPartner > 0 THEN  @CommunityPartner ELSE FAL.[CommunitySiteUsersID] END
		AND FAL.[CommunityDistrictID]= CASE WHEN @CommunitySite > 0 THEN  @CommunitySite ELSE FAL.[CommunityDistrictID] END
		--AND (FAL.[Status] = CASE WHEN @CommunitySite > 0 OR @CommunityPartner > 0 THEN  'Submitted' ELSE FAL.[Status] END
		--OR
		--FAL.[Status] = CASE WHEN @CommunitySite > 0 OR @CommunityPartner > 0 THEN  'Approved' ELSE FAL.[Status] END
		--OR
		--FAL.[Status] = CASE WHEN @CommunitySite > 0 OR @CommunityPartner > 0 THEN  'Not-Approved' ELSE FAL.[Status] END)
		--@UserID
	ORDER BY
		[ActivityStartDate], FAL.[ID]

		  ----STATE HANDLER
--DECLARE @FWALStatus AS VARCHAR(50)
--SELECT @FWALStatus = FWAL.[Status] FROM [FieldWork].[FieldWorkActivityLog] FWAL WHERE FWAL.[ID] = isnull(@ActivityLogID,FWAL.[ID])
	
		SELECT (
		SELECT [showClose]
			  ,[showEdit]
			  ,[showSubmit]
			  ,[showRejectHours]
			  ,[showApproveHours]
		   FROM [Master].[AcitivityLogHandler] ALH
		  JOIN [User].[UserRoles] UR ON ALH.[RoleID] = ur.[RoleID] 
			WHERE [Status] = 'Saved'
			AND UR.RoleID IN (1,4,5,9,10)
			AND UR.UserID=@UserID
		  ORDER BY ALH.[RoleID]
		  FOR JSON AUTO) AS [AcitivityLogHandler];
END