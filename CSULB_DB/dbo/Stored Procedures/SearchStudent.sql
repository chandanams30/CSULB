-- =============================================
-- Author:		ThoughtFocus
-- Create date: 12-Dec-2022
-- Description:	Search Students
-- =============================================

CREATE PROCEDURE [dbo].[SearchStudent] 
	@searchString nvarchar(100)
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

SELECT F.[ID], U.[FirstName], U.[LastName], U.[EMAIL], U.[CSULBID], APT.[Name]  AS [Type], U.[ID] AS [UserID], F.[TermCode], F.[ProgramID], PAT.[ApplicationTypeID]
INTO #TempUsers
FROM [User].[Users] U  
JOIN [Application].[Forms] F ON F.[UserID] = U.[ID] 
JOIN [Master].[ProgramApplicationType] PAT ON PAT.[ProgramID] = F.[ProgramID]
JOIN [Master].[ApplicationTypes] APT ON APT.[ID] = PAT.[ApplicationTypeID]
WHERE U.[FirstName] LIKE @FirstName OR U.[LastName] LIKE @LastName OR U.[Email] LIKE @Email OR U.[CSULBID] LIKE @CSULBID
UNION 
SELECT F.[ID], U.[FirstName], U.[LastName], U.[EMAIL], U.[CSULBID], 'FieldWork'  AS [Type], U.[ID] AS [UserID], NULL AS [TermCode], NULL AS [ProgramID],  4 AS [ApplicationTypeID]
FROM [User].[Users] U
JOIN [FieldWork].[FieldWork] F ON F.[UserID] = U.[ID] -- 3679
WHERE U.[FirstName] LIKE @FirstName OR U.[LastName] LIKE @LastName OR U.[Email] LIKE @Email OR U.[CSULBID] LIKE @CSULBID

----
--SELECT * FROM #TempUsers

SELECT [ID], [FirstName], [LastName], [EMAIL], [CSULBID], isnull([Type],'-') AS [Type] , [UserID], [TermCode], [ProgramID], [ApplicationTypeID] 
FROM (
	SELECT [ID], [FirstName], [LastName], [EMAIL], [CSULBID], [Type] , [UserID], [TermCode], [ProgramID], [ApplicationTypeID]  
	FROM #TempUsers
	UNION 
	SELECT NULL AS [ID], U.[FirstName], U.[LastName], U.[EMAIL], U.[CSULBID], NULL  AS [Type], U.[ID] AS [UserID], NULL AS [TermCode], NULL AS [ProgramID], NULL AS [ApplicationTypeID] 
	FROM [User].[Users] U 
	JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID] 
	WHERE UR.RoleID=3 AND U.[ID] NOT IN (SELECT [UserID] FROM #TempUsers)
	AND (U.[FirstName] LIKE @FirstName OR U.[LastName] LIKE @LastName OR U.[Email] LIKE @Email OR U.[CSULBID] LIKE @CSULBID)
	) T ORDER BY [FirstName], [LastName], [CSULBID]

DROP TABLE #TempUsers
-- =============================================
-- Example to execute the stored procedure
-- =============================================
--EXECUTE [dbo].[SearchStudent] @searchString = venk
--exec [dbo].[SearchStudent] @searchString=N'venka'

END