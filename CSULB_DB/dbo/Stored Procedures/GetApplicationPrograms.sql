--exec [GetApplicationPrograms] 1,2
CREATE PROCEDURE [dbo].[GetApplicationPrograms]
@Userid bigint,
@ApplicationTypeID bigint,
@TermCode VarChar(10) = NULL

AS
BEGIN

DECLARE @RoleID AS BIGINT

SELECT @RoleID = [RoleID] FROM [User].[UserRoles] UR WHERE UR.[UserID]=@Userid AND UR.[RoleID] <>2 order by [RoleID] desc

SELECT P.[ID]
		,P.[Name]
		,T.[Name] AS [Semester]
		,PAD.[ApplicationOpens]
		,PAD.[ApplicationCloseDate]
		,ISNULL(TC.[TotalCount],0) AS [TotalCount]
		,ISNULL(TC.[AcceptedCount], 0) AS [AcceptedCount]
		, CASE WHEN @RoleID=3 THEN 1 ELSE 0 END AS [showApply]
		, CASE WHEN @RoleID<>3 THEN 1 ELSE 0 END AS [showView] into #tempApplicationPrograms
	FROM [Master].[ProgramApplicationDates] PAD
		LEFT JOIN [Master].[Programs] P ON P.[ID] = PAD.[ProgramID]
		JOIN [Master].[Term] T ON T.[TermCode] = PAD.[TermCode]
		LEFT JOIN [Application].[Forms] F ON F.[ProgramID] = PAD.[ProgramID] AND F.[TermCode] = PAD.[TermCode] AND F.[UserID] = @Userid
		LEFT JOIN [User].[UserRoles] UR ON UR.UserID = F.[UserID]
		LEFT JOIN (SELECT F.[ProgramID], F.[TermCode], COUNT(*) AS [TotalCount],
SUM(CASE WHEN F.FormStateID=12 THEN 1 ELSE 0 END) AS [AcceptedCount] FROM [Application].[Forms] F GROUP BY F.[ProgramID], F.[TermCode]) TC ON TC.[ProgramID] = PAD.[ProgramID] AND TC.[TermCode] = PAD.[TermCode]
	WHERE 
		F.[ProgramID] IS NULL
		AND F.[TermCode] IS NULL
		AND PAD.[TermCode] = isnull(@TermCode, PAD.[TermCode])
		AND PAD.[ApplicationOpens] < CASE WHEN @TermCode IS NULL THEN GETDATE() + 30 ELSE '3000-01-01' END
		AND PAD.[ApplicationCloseDate] > CASE WHEN @TermCode IS NULL THEN GETDATE() ELSE '1900-01-01' END
		AND PAD.[ApplicationTypeID] = @ApplicationTypeID
		AND PAD.[Status]=1

		SELECT DISTINCT
			T.[ID]
			,T.[Name]
			,T.[Semester]
			,T.[ApplicationOpens]
			,T.[ApplicationCloseDate]
			,T.[TotalCount]
			,T.[AcceptedCount]
			,T.[showApply]
			,T.[showView]
		FROM #tempApplicationPrograms T 
			JOIN [Master].[ProgramUsers] PU ON PU.ProgramID = T.[ID]
		WHERE PU.UserID = (CASE WHEN @RoleID IN (1,3) THEN PU.UserID ELSE @Userid END)
		
		--drop table #tempApplicationPrograms
-------------------------------------------------------------------------------------------------------------------------------
SELECT DISTINCT T.[Name] AS [Semester], T.[TermCode]
		, CASE WHEN @RoleID=3 THEN 1 ELSE 0 END AS [showApply]
		, CASE WHEN @RoleID<>3 THEN 1 ELSE 0 END AS [showView]
		, CASE WHEN @RoleID in (1,4) THEN 1 ELSE 0 END AS [showAssignApplicationToReviewers]
	FROM [Master].[ProgramApplicationDates] PAD
		LEFT JOIN [Master].[Term] T ON T.[TermCode] = PAD.[TermCode]
	WHERE 
		PAD.[TermCode] = isnull(@TermCode, PAD.[TermCode])
		AND PAD.[ApplicationOpens] < CASE WHEN @TermCode IS NULL THEN GETDATE() + 30 ELSE '3000-01-01' END
		AND PAD.[ApplicationCloseDate] > CASE WHEN @TermCode IS NULL THEN GETDATE() ELSE '1900-01-01' END
		AND PAD.[ApplicationTypeID] = @ApplicationTypeID

		
END

/*
exec [GetApplicationPrograms] 1,2, 2234
exec [GetApplicationPrograms] 1,2
exec [GetApplicationPrograms] 1,2, 2242
exec [GetApplicationPrograms] 1,2, 2264

exec [GetApplicationPrograms] 1,2, 2234
exec [GetApplicationPrograms] 1,2, 2242
exec [GetApplicationPrograms] 1,2, 2244
exec [GetApplicationPrograms] 1,2, 2252
exec [GetApplicationPrograms] 1,2, 2254

exec [GetApplicationPrograms] 338,2, 2234
exec [GetApplicationPrograms] 339,2

*/