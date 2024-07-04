-- =============================================
-- Author:		ThoughtFocus
-- Create date: 16-Feb-2023
-- Description:	get users list
-- =============================================
CREATE PROCEDURE [User].[GetUsersList] 
	@searchString nvarchar(100) = NULL
	,@RoleID BIGINT = NULL
AS
BEGIN
	DECLARE @FirstName AS nvarchar(255) = @searchString,
	@LastName AS nvarchar(255) = @searchString,
	@Email AS nvarchar(255) = @searchString,
	@CSULBID AS varchar(50) = @searchString

SELECT @FirstName = REPLACE('%' + ISNULL(@FirstName, '%') + '%', '%%%','%')
, @LastName = REPLACE('%' + ISNULL(@LastName, '%') + '%', '%%%','%')
, @Email = REPLACE('%' + ISNULL(@Email, '%') + '%', '%%%','%')
, @CSULBID = REPLACE('%' +ISNULL(@CSULBID, '%') + '%', '%%%','%')

--SELECT @FirstName, @LastName , @Email, @CSULBID;

SELECT U.[ID] AS [UserID], U.[FirstName], U.[LastName], U.[EMAIL], U.[CSULBID]
,STRING_AGG(R.[Name], ', ') WITHIN GROUP (ORDER BY R.[ID]) AS [UserRoles]
FROM [User].[Users] U  
LEFT JOIN [User].[UserRoles] UR ON U.[ID] = UR.[UserID]
LEFT JOIN [Master].[Role] R ON R.[ID] = UR.[RoleID]
--WHERE R.[ID] <> 2
WHERE R.[ID] = ISNULL(@RoleID, R.[ID]) AND
(U.[FirstName] LIKE @FirstName OR U.[LastName] LIKE @LastName OR U.[Email] LIKE @Email OR U.[CSULBID] LIKE @CSULBID)
GROUP BY U.[ID], U.[FirstName], U.[LastName], U.[EMAIL], U.[CSULBID]
ORDER BY  U.[ID];

-- =============================================
-- Example to execute the stored procedure
-- =============================================
--EXECUTE [User].[GetUsersList] 
--EXECUTE [User].[GetUsersList] 'A'
--EXECUTE [User].[GetUsersList] '000009618'
--EXECUTE [User].[GetUsersList] 'A', 4
--EXECUTE [User].[GetUsersList] 'A', 3
--EXECUTE [User].[GetUsersList] 'A', 5
--EXECUTE [User].[GetUsersList] 'A', 6
--
END