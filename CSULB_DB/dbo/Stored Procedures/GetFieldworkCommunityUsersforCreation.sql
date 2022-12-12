-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [dbo].[GetFieldworkCommunityUsersforCreation]
CREATE PROCEDURE [dbo].[GetFieldworkCommunityUsersforCreation]

AS
BEGIN
/*
SELECT * FROM [User].[Users] U;
SELECT * FROM [User].[UserRoles] UR;
SELECT * FROM [User].[UserCred] UC;
SELECT * FROM [Master].[Role] R;
*/
SELECT 
	U.[ID]
	,U.[FirstName]
	,U.[LastName]
	,'asif.khan@thoughtfocus.com' AS [Email]
	--,U.[Email]
	,R.[Name] AS [RoleName]
	,UC.[UserId]
	,UC.[UserName]
FROM [User].[Users] U
JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID]
JOIN [Master].[Role] R ON R.[ID] = UR.[RoleID]
LEFT JOIN [User].[UserCred] UC ON UC.[UserId] = U.[ID]
WHERE UR.[RoleID] IN (9,10)
AND UC.[UserName] IS NULL
ORDER BY U.[ID]

--WHERE U.[CSULBID] IS NULL
--AND U.[AuthenticationTypeId] =2
--AND UR.[RoleID] IN (9,10)
--AND UC.[UserName] IS NULL
--ORDER BY U.[ID]

--DELETE FROM [User].[UserCred] WHERE UserId IN (2215,2253,2254,2255,2256,2257,40)
--SELECT * FROM [User].[UserCred] WHERE UserId IN (2215,2253,2254,2255,2256,2257,40)

END