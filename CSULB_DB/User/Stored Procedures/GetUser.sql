-- =============================================
-- Author:		ThoughtFocus
-- Create date: 21-Feb-2023
-- Description:	Get user details by user ID
-- =============================================
CREATE PROCEDURE [User].[GetUser] 
	-- Add the parameters for the stored procedure here
	@UserID AS BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--SELECT(SELECT U.[ID] AS [UserID]
 --     ,[FirstName] AS [FirstName]
 --     ,[LastName] AS [LastName]
 --     ,[Email] AS [Email]
 --     ,[AuthenticationTypeId] AS [AuthenticationTypeId]
 --     ,[Status] AS [Status]
 --     ,[CSULBID] AS [CSULBID]
 --     ,[FirstNamePref] AS [FirstNamePre]
 --     ,[LastNamePref] AS [LastNamePref]
 --     ,[DisplayName] AS [DisplayName]
	--  ,Roles.[ID] AS [RoleID]
	--  ,Roles.[Name]  AS [RoleName]
	--  ,[Programs].[ID] AS [ProgramID]
	--  ,[Programs].[Name] AS [ProgramName]
 -- FROM [User].[Users] U
 -- INNER JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID]
 -- INNER JOIN [Master].[Role] Roles ON Roles.[ID] = UR.[RoleID]
 -- LEFT JOIN [Master].[ProgramUsers] PU ON PU.UserID = U.[ID] AND PU.[RoleID] = UR.[RoleID]
 -- LEFT JOIN [Master].[Programs] [Programs] ON [Programs].[ID] = PU.[ProgramID]
 -- WHERE U.[ID] = @UserID
 -- FOR JSON AUTO, WITHOUT_ARRAY_WRAPPER) AS  [UserDetail]

 --SELECT (
	--	SELECT U.[ID] AS [UserID]
	--		,[FirstName] AS [FirstName]
	--		,[LastName] AS [LastName]
	--		,[Email] AS [Email]
	--		,[AuthenticationTypeId] AS [AuthenticationTypeId]
	--		,[Status] AS [Status]
	--		,[CSULBID] AS [CSULBID]
	--		,[FirstNamePref] AS [FirstNamePref]
	--		,[LastNamePref] AS [LastNamePref]
	--		,[DisplayName] AS [DisplayName]
	--		,[Roles].[RoleID] AS [RoleID]
	--		,[Roles].[RoleName] AS [RoleName]
	--		,[Roles].[ProgramID] AS [ProgramID]
	--		,[Roles].[ProgramName] AS [ProgramName]
	--	FROM [User].[Users] U
	--	JOIN (
	--		SELECT [Roles].[ID] AS [RoleID]
	--			,[Roles].[Name] AS [RoleName]
	--			,[Programs].[ID] AS [ProgramID]
	--			,[Programs].[Name] AS [ProgramName]
	--			,UR.[UserID]
	--		FROM [User].[UserRoles] UR
	--		INNER JOIN [Master].[Role] Roles ON Roles.[ID] = UR.[RoleID]
	--		LEFT JOIN [Master].[ProgramUsers] PU ON PU.UserID = UR.[UserID] AND PU.[RoleID] = UR.[RoleID]
	--		LEFT JOIN [Master].[Programs] [Programs] ON [Programs].[ID] = PU.[ProgramID]
	--		WHERE UR.[UserID] = @UserID AND [Roles].[ID] NOT IN (2)
	--		) Roles ON Roles.UserID = U.[ID]
	--	WHERE U.[ID] = @UserID
	--	FOR JSON AUTO,WITHOUT_ARRAY_WRAPPER
	--	) AS [UserDetail]

	SELECT (
	SELECT U.[ID] AS [UserID]
			,[FirstName] AS [FirstName]
			,[LastName] AS [LastName]
			,[Email] AS [Email]
			,[AuthenticationTypeId] AS [AuthenticationTypeId]
			,[Status] AS [Status]
			,[CSULBID] AS [CSULBID]
			,[FirstNamePref] AS [FirstNamePref]
			,[LastNamePref] AS [LastNamePref]
			,[DisplayName] AS [DisplayName]
			,[Roles].[RoleID] AS [RoleID]
			,[Roles].[RoleName] AS [RoleName]
			,[Roles].[ProgramID] AS [ProgramID]
			,[Roles].[ProgramName] AS [ProgramName]
			,[Roles].[CommunityDistrictID]
			,[Roles].[CommunityDistrictName]
		FROM [User].[Users] U
		LEFT JOIN (
			SELECT [Roles].[ID] AS [RoleID]
				,[Roles].[Name] AS [RoleName]
				,[Programs].[ID] AS [ProgramID]
				,[Programs].[Name] AS [ProgramName]
				,UR.[UserID]
				,CD.[ID] AS [CommunityDistrictID]
				,CD.[Name] AS [CommunityDistrictName]
			FROM [User].[UserRoles] UR
			INNER JOIN [Master].[Role] Roles ON Roles.[ID] = UR.[RoleID]
			LEFT JOIN [Master].[ProgramUsers] PU ON PU.UserID = UR.[UserID] AND PU.[RoleID] = UR.[RoleID]
			LEFT JOIN [Master].[Programs] [Programs] ON [Programs].[ID] = PU.[ProgramID]
			LEFT JOIN [FieldWork].[CommunitySiteUsers] CSU ON CSU.[UserID] =  UR.[UserID] AND CSU.[RoleID] = UR.[RoleID]
			LEFT JOIN [FieldWork].[CommunityDistrict] CD ON CD.[ID] = CSU.[CommunityDistrictID]
			WHERE UR.[UserID] = @UserID AND [Roles].[ID] NOT IN (2)
			) Roles ON Roles.UserID = U.[ID]
		WHERE U.[ID] = @UserID FOR JSON AUTO,WITHOUT_ARRAY_WRAPPER,INCLUDE_NULL_VALUES
	) AS [UserDetail]

-- =============================================
-- Example to execute the stored procedure
-- =============================================
--EXECUTE [User].[GetUser]  @UserID = 304
--EXECUTE [User].[GetUser]  @UserID = 144
--EXECUTE [User].[GetUser]  @UserID = 9

/*
select [userid], count(*) from [User].[UserRoles] 
where [UserID] in (select [UserID] from [User].[UserRoles] where RoleID=4)
group by [userid]


SELECT * FROM [Master].[ProgramUsers] PU
SELECT * FROM [Master].[Programs] [Programs]
select * from [Master].Role


*/
END