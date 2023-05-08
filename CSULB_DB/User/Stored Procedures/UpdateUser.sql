
-- =============================================
-- Author:		ThoughtFocus
-- Create date: 27-Feb-2023
-- Description:	Update user details
-- =============================================
CREATE PROCEDURE [User].[UpdateUser] 
	-- Add the parameters for the stored procedure here
	@UserID AS BIGINT
	,@FirstName [nvarchar](255)
	,@LastName [nvarchar](255)
	,@Email [nvarchar](255)
	,@AuthenticationTypeId [int]
	,@Status [bit]
	,@CSULBID [varchar](15)
	,@FirstNamePref [nvarchar](255)
	,@LastNamePref [nvarchar](255)
	,@DisplayName [nvarchar](255)
	,@Roles [User].[UDT_UserRoles] READONLY
	,@createdByUserID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;



	UPDATE [User].[Users]
	   SET [FirstName] = @FirstName
		  ,[LastName] = @LastName
		  ,[Email] = @Email
		  ,[CreatedByUserID] = @CreatedByUserID
		  ,[AuthenticationTypeId] = @AuthenticationTypeId
		  ,[Status] = @Status
		  ,[CSULBID] = @CSULBID
		  ,[FirstNamePref] = @FirstNamePref
		  ,[LastNamePref] = @LastNamePref
		  ,[DisplayName] = @DisplayName
	 WHERE [ID] = @UserID;
---------------------------------------
--DELETE ROLES
--SELECT distinct @UserID AS [UserID],[RoleID] FROM @Roles;
--SELECT * FROM @Roles;

DELETE
FROM [User].[UserRoles]
WHERE [ID] IN (
		SELECT ur.[ID]
		FROM [User].[UserRoles] UR
		LEFT JOIN @Roles R ON R.RoleID = ur.[RoleID]
		WHERE UR.[UserID] = @UserID
			AND UR.[RoleID] <> 2
			AND R.RoleID IS NULL
		)
	AND [UserID] = @UserID;
----
--INSERT INTO [User].[UserRoles]
--SELECT R.[RoleID], @UserID FROM @Roles R where R.RoleID NOT IN (SELECT RoleID FROM [User].[UserRoles] UR WHERE UR.[UserID] = @UserID AND UR.[RoleID] <> 2)

INSERT INTO [User].[UserRoles]
           ([UserID]
           ,[RoleID]
           ,[CreatedDateTime]
           ,[CreatedByUserID]
           ,[ApplicationTypeID])
           (SELECT DISTINCT @UserID, R.[RoleID], GETDATE(),@createdByUserID, 4 FROM @Roles R where (R.RoleID IS NOT NULL) AND R.RoleID NOT IN (SELECT RoleID FROM [User].[UserRoles] UR WHERE UR.[UserID] = @UserID AND UR.[RoleID] <> 2));
---------------------------------------
--DELETE  [Master].[ProgramUsers]

DELETE
FROM [Master].[ProgramUsers]
WHERE [ID] IN (SELECT PU.[ID] FROM [Master].[ProgramUsers] PU
LEFT JOIN @Roles R ON R.[RoleID] = PU.[RoleID] AND R.[ProgramID] = PU.[ProgramID]
WHERE PU.[UserID] = @UserID AND R.RoleID IS NULL) AND [UserID] = @UserID; 


--SELECT * FROM [Master].[ProgramUsers] PU WHERE PU.[UserID] = @UserID

--SELECT  * FROM @Roles R
--LEFT JOIN [Master].[ProgramUsers] PU  ON PU.[RoleID] = R.[RoleID] AND R.[ProgramID] = PU.[ProgramID] AND PU.[UserID] = @UserID
--WHERE PU.RoleID IS NULL AND R.[ProgramID] IS NOT NULL

INSERT INTO [Master].[ProgramUsers]
           ([UserID]
           ,[ProgramID]
           ,[RoleID])
     (SELECT DISTINCT @UserID, R.[ProgramID], R.[RoleID] FROM @Roles R
LEFT JOIN [Master].[ProgramUsers] PU  ON PU.[RoleID] = R.[RoleID] AND R.[ProgramID] = PU.[ProgramID] AND PU.[UserID] = @UserID
WHERE PU.RoleID IS NULL AND R.[ProgramID] IS NOT NULL);

--SELECT * FROM [Master].[ProgramUsers] PU
--right JOIN @Roles R ON R.[RoleID] = PU.[RoleID] AND R.[ProgramID] = PU.[ProgramID]
--WHERE PU.[UserID] = @UserID AND R.RoleID IS NULL

	--SELECT
	--	@UserID
	--	,[RoleID]
	--	,[RoleName]
	--	,[ProgramID]
	--	,[ProgramName]
	--FROM @Roles
---------------------------------------
--Delete [CommunitySiteUsers]
DELETE FROM [FieldWork].[CommunitySiteUsers]
WHERE [ID] IN (SELECT CU.[ID] FROM [FieldWork].[CommunitySiteUsers] CU
LEFT JOIN @Roles R ON R.[RoleID] = CU.[RoleID] AND R.[CommunityDistrictID] = CU.[CommunityDistrictID]
WHERE CU.[UserID] = @UserID AND R.RoleID IS NULL) AND [UserID] = @UserID; 

--INSERT [CommunitySiteUsers]
INSERT INTO [FieldWork].[CommunitySiteUsers]
			([UserID]
           ,[CommunityDistrictID]
           ,[RoleID])
		   (SELECT  DISTINCT @UserID, R.[CommunityDistrictID], R.[RoleID] 
			FROM @Roles R
			LEFT JOIN [FieldWork].[CommunitySiteUsers] CU  ON CU.[RoleID] = R.[RoleID] AND R.[CommunityDistrictID] = CU.[CommunityDistrictID] AND CU.[UserID] = @UserID
			WHERE CU.[CommunityDistrictID] IS NULL AND R.[CommunityDistrictID] IS NOT NULL);

--     (SELECT DISTINCT @UserID, R.[CommunityDistrictID], R.[RoleID] FROM @Roles R
--LEFT JOIN [FieldWork].[CommunitySiteUsers] CU  ON CU.[RoleID] = R.[RoleID] AND R.[CommunityDistrictID] = CU.[CommunityDistrictID] AND CU.[UserID] = @UserID
--WHERE --CU.RoleID IS NOT NULL AND 
--R.[CommunityDistrictID] IS NOT NULL);
---------------------------------------
	
	SELECT 1 AS [Status], '' AS [Message];

-- =============================================
-- Example to execute the stored procedure
-- =============================================



END