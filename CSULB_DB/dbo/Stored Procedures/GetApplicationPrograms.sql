--exec [GetApplicationPrograms] 600,2
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
		,T.[TermCode] AS [TermCode]
		,PAD.[ApplicationOpens]
		,PAD.[ApplicationCloseDate]
		,PAD.[ApplicationDeadline] AS [ApplicationDeadline]
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
		AND PAD.[ApplicationOpens] < GETDATE() --CASE WHEN @TermCode IS NOT NULL THEN GETDATE() + 30 ELSE '3000-01-01' END
		--AND PAD.[ApplicationDeadline] > CASE WHEN @TermCode IS NOT NULL THEN GETDATE() -5 ELSE '1900-01-01' END
		AND PAD.[ApplicationTypeID] = @ApplicationTypeID
		AND PAD.[Status]=1
		--AND P.[ID] =8

		--SELECT DISTINCT
		--	T.[ID]
		--	,T.[Name]
		--	,T.[Semester]
		--	,T.[ApplicationOpens]
		--	,T.[ApplicationCloseDate]
		--	,T.[TotalCount]
		--	,T.[AcceptedCount]
		--	,T.[showApply]
		--	,T.[showView]
		--FROM #tempApplicationPrograms T 
		--	JOIN [Master].[ProgramUsers] PU ON PU.ProgramID = T.[ID]
		--WHERE PU.UserID = (CASE WHEN @RoleID IN (1,3) THEN PU.UserID ELSE @Userid END)

		SELECT DISTINCT
			T.[ID]
			,T.[Name]
			,T.[Semester]
			,T.[TermCode]
			,T.[ApplicationOpens]
			,T.[ApplicationCloseDate]
			,T.[ApplicationDeadline]
			,T.[TotalCount]
			,T.[AcceptedCount]
			,T.[showApply]
			,T.[showView]
		FROM #tempApplicationPrograms T, [Master].[ProgramUsers] PU
			--JOIN [Master].[ProgramUsers] PU ON PU.ProgramID = T.[ID]
		WHERE PU.UserID = (CASE WHEN @RoleID IN (1,3) THEN PU.UserID ELSE @Userid END)
		AND PU.ProgramID = (CASE WHEN @RoleID IN (1,3) THEN PU.ProgramID ELSE T.[ID] END)
		AND T.[ApplicationCloseDate] > (CASE WHEN @RoleID IN (3) THEN GETDATE() -1 ELSE T.[ApplicationCloseDate] -1  END)


		drop table #tempApplicationPrograms
-------------------------------------------------------------------------------------------------------------------------------
SELECT DISTINCT T.[Name] AS [Semester], T.[TermCode]
		, CASE WHEN @RoleID=3 THEN 1 ELSE 0 END AS [showApply]
		, CASE WHEN @RoleID<>3 THEN 1 ELSE 0 END AS [showView]
		, CASE WHEN @RoleID in (1,4) THEN 1 ELSE 0 END AS [showAssignApplicationToReviewers]
	FROM [Master].[ProgramApplicationDates] PAD
		LEFT JOIN [Master].[Term] T ON T.[TermCode] = PAD.[TermCode]
	WHERE 
		PAD.[TermCode] = isnull(@TermCode, PAD.[TermCode])
		AND PAD.[ApplicationOpens] <  GETDATE() --CASE WHEN @TermCode IS NULL THEN GETDATE() + 30 ELSE '3000-01-01' END
		--AND PAD.[ApplicationCloseDate] > CASE WHEN @TermCode IS NULL THEN GETDATE() - 5 ELSE '1900-01-01' END
		AND PAD.[ApplicationTypeID] = @ApplicationTypeID

		
END

/*
exec [GetApplicationPrograms] 1,1
exec [GetApplicationPrograms] 1,1, 2234
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

select * from [Master].[ProgramApplicationDates] 
select * from [Master].[Programs]

1,2,4,6

*/