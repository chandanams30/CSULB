-- =============================================
-- Author:		ThoughtFocus
-- Create date: 03/07/2023
-- Description:	get Users by Program Role
-- =============================================
CREATE PROCEDURE [User].[GetUsersByProgramRole]
@ProgramID BIGINT,
@RoleID BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

--SELECT
--(SELECT DISTINCT PU.[ProgramID]
--	,PU.[ProgramName]
--	,PU.[RoleID]
--	,PU.[RoleName]
--	,[Users].[UserID]
--	,[Users].[UserFirstName]
--	,[Users].[UserLastName]
--	,[Users].[UserCSULBID]
--	,[Users].[UserEmail]
--FROM (
--SELECT P.[ID] AS [ProgramID]
--		,P.[Name] AS [ProgramName]
--		,R.[ID] AS [RoleID]
--		,R.[Name] AS [RoleName]
--		,PUS.[UserID]
--	FROM [Master].[Programs] P 
--	CROSS  JOIN [Master].[Role] R 
--	LEFT JOIN [CSULB_DB].[Master].[ProgramUsers] PUS ON PUS.[ProgramID] = @ProgramID AND  PUS.[RoleID] = @RoleID
--	WHERE P.[ID] = @ProgramID AND R.[ID] = @RoleID
--	--SELECT P.[ID] AS [ProgramID]
--	--	,P.[Name] AS [ProgramName]
--	--	,R.[ID] AS [RoleID]
--	--	,R.[Name] AS [RoleName]
--	--	,PUS.[UserID]
--	--FROM [CSULB_DB].[Master].[ProgramUsers] PUS
--	--JOIN [Master].[Programs] P ON P.[ID] = PUS.[ProgramID]
--	--JOIN [Master].[Role] R ON R.[ID] = PUS.[RoleID]
--	--WHERE PUS.[ProgramID] = @ProgramID AND PUS.[RoleID] = @RoleID
--	) AS PU
--LEFT JOIN (
--	SELECT U.[ID] AS [UserID]
--		,U.[FirstName] AS [UserFirstName]
--		,U.[LastName] AS [UserLastName]
--		,U.[CSULBID] AS [UserCSULBID]
--		,U.[Email] AS [UserEmail]
--	FROM [User].[Users] U
--	) AS [Users] ON [Users].[UserID] = PU.[UserID]
--FOR JSON AUTO
--	,WITHOUT_ARRAY_WRAPPER) AS [UsersByProgramRole]

DECLARE @JSONResult AS NVARCHAR(MAX)
SELECT @JSONResult =
(SELECT DISTINCT PU.[ProgramID]
	,PU.[ProgramName]
	,PU.[RoleID]
	,PU.[RoleName]
	,[Users].[UserID]
	,[Users].[UserFirstName]
	,[Users].[UserLastName]
	,[Users].[UserCSULBID]
	,[Users].[UserEmail]
FROM (
SELECT P.[ID] AS [ProgramID]
		,P.[Name] AS [ProgramName]
		,R.[ID] AS [RoleID]
		,R.[Name] AS [RoleName]
		,PUS.[UserID]
	FROM [Master].[Programs] P 
	CROSS  JOIN [Master].[Role] R 
	LEFT JOIN [Master].[ProgramUsers] PUS ON PUS.[ProgramID] = @ProgramID AND  PUS.[RoleID] = @RoleID
	WHERE P.[ID] = @ProgramID AND R.[ID] = @RoleID
	) AS PU
LEFT JOIN (
	SELECT U.[ID] AS [UserID]
		,U.[FirstName] AS [UserFirstName]
		,U.[LastName] AS [UserLastName]
		,U.[CSULBID] AS [UserCSULBID]
		,U.[Email] AS [UserEmail]
	FROM [User].[Users] U
	) AS [Users] ON [Users].[UserID] = PU.[UserID]
FOR JSON AUTO
	,WITHOUT_ARRAY_WRAPPER) --AS [UsersByProgramRole]

	SELECT REPLACE(@JSONResult,'[{}]', '[]') AS [UsersByProgramRole];


--execute [User].[GetUsersByProgramRole] 6,6
--execute [User].[GetUsersByProgramRole]  @ProgramID = 4, @RoleID = 7
END