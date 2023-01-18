--exec [GetApplicationPrograms] 600,2
CREATE PROCEDURE [dbo].[GetApplicationPrograms]
@Userid bigint,
@ApplicationTypeID bigint,
@TermCode VarChar(10) = NULL

AS
BEGIN

SELECT P.[ID]
		,P.[Name]
		,T.[Name] AS [Semester]
		,T.[TermCode] AS [TermCode]
		,PAD.[ApplicationOpens]
		,PAD.[ApplicationCloseDate]
		,PAD.[ApplicationDeadline] AS [ApplicationDeadline]
		,ISNULL(TC.[TotalCount],0) AS [TotalCount]
		,ISNULL(TC.[AcceptedCount], 0) AS [AcceptedCount]
		into #tempApplicationPrograms
FROM [Master].[ProgramApplicationDates] PAD 
JOIN [Master].[Term] T ON T.[TermCode] = PAD.[TermCode] 
LEFT JOIN [Master].[Programs] P ON P.[ID] = PAD.[ProgramID]
LEFT JOIN (SELECT F.[ProgramID], F.[TermCode], COUNT(*) AS [TotalCount], SUM(CASE WHEN F.FormStateID=12 THEN 1 ELSE 0 END) AS [AcceptedCount] FROM [Application].[Forms] F GROUP BY F.[ProgramID], F.[TermCode]) TC ON TC.[ProgramID] = PAD.[ProgramID] AND TC.[TermCode] = PAD.[TermCode]
WHERE PAD.[ApplicationTypeID]=@ApplicationTypeID
AND PAD.[ApplicationOpens] < GETDATE() 
AND PAD.[TermCode] = isnull(@TermCode, PAD.[TermCode])
ORDER BY T.[TermCode], P.[ID];
---------------------------------------------------------------------------------
---------------------------------------------------------------------------------
--1 Administrator
SELECT T.[ID] AS [ProgramID],T.[TermCode]
INTO #tempProgramID
FROM #tempApplicationPrograms T
WHERE [dbo].[fnIsRoleValid](@Userid, '1') = 1 --1	Administrator

UNION

--3	Student FOR 1 Initial Teacher Credential Programs
SELECT T.[ID] AS [ProgramID],T.[TermCode]
FROM #tempApplicationPrograms T
LEFT JOIN [Application].[Forms] F ON F.ProgramID = T.[ID] AND F.TermCode = T.TermCode AND F.[UserID] = @Userid
WHERE [dbo].[fnIsRoleValid](@Userid, '3') = 1 --3	Student
	AND f.[ID] IS NULL

UNION

--4 Program Admin
SELECT T.[ID] AS [ProgramID],T.[TermCode]
FROM #tempApplicationPrograms T
JOIN [Master].[ProgramUsers] PU ON T.[ID] = PU.[ProgramID]
WHERE [dbo].[fnIsRoleValid](@Userid, '4') = 1 --4 ProgramAdmin
	AND PU.UserID = @Userid

UNION

--6	Reviewer 
SELECT T.[ID] AS [ProgramID],T.[TermCode]
FROM #tempApplicationPrograms T
JOIN [Application].[Forms] F ON F.[ProgramID] = T.[ID]
	AND F.[TermCode] = T.[TermCode]
JOIN [Application].[Reviewer] R ON R.[FormID] = F.[ID]
WHERE [dbo].[fnIsRoleValid](@Userid, '6') = 1 --6	Reviewer
	AND R.[ReviewerID] = @Userid

UNION

--7	Instructor 
SELECT T.[ID] AS [ProgramID],T.[TermCode]
FROM #tempApplicationPrograms T
JOIN [Application].[Forms] F ON F.[ProgramID] = T.[ID]
	AND F.[TermCode] = T.[TermCode]
JOIN [Application].[Instructor] INS ON INS.[FormID] = F.[ID]
WHERE [dbo].[fnIsRoleValid](@Userid, '7') = 1 --7	Instructor
	AND INS.[InstructorUserID] = @Userid

UNION

--8	Interviewer 
SELECT T.[ID] AS [ProgramID],T.[TermCode]
FROM #tempApplicationPrograms T
JOIN [Application].[Forms] F ON F.[ProgramID] = T.[ID]
	AND F.[TermCode] = T.[TermCode]
JOIN [Application].[Interviewer] INVR ON INVR.[FormID] = F.[ID]
WHERE [dbo].[fnIsRoleValid](@Userid, '8') = 1 --8	Interviewer
	AND INVR.[InterviewerUserID] = @Userid

UNION

--11	Program Coordinator
SELECT T.[ID] AS [ProgramID],T.[TermCode]
FROM #tempApplicationPrograms T
WHERE [dbo].[fnIsRoleValid](@Userid, '11') = 1 --11	Program Coordinator
---------------------------------------------------------------------------------
---------------------------------------------------------------------------------
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
			--,T.[showApply]
			--,T.[showView]
			, CASE WHEN [dbo].[fnIsRoleValid] (@Userid,'3') =1 THEN 1 ELSE 0 END AS [showApply]
			, CASE WHEN [dbo].[fnIsRoleValid] (@Userid,'3') = 1 THEN 0 ELSE 1 END AS [showView] 
		FROM #tempApplicationPrograms T 
		JOIN #tempProgramID P ON P.[ProgramID] = T.[ID] AND T.[TermCode]=P.[TermCode]
		WHERE 
		T.[ApplicationCloseDate] > (CASE WHEN [dbo].[fnIsRoleValid] (@Userid,'3') = 1 THEN GETDATE() -1 ELSE T.[ApplicationCloseDate] -1  END)
---------------------------------------------------------------------------------
---------------------------------------------------------------------------------
SELECT 
 DISTINCT T.[Semester], T.[TermCode]
	, CASE WHEN [dbo].[fnIsRoleValid] (@Userid,'3') =1 THEN 1 ELSE 0 END AS [showApply]
		, CASE WHEN [dbo].[fnIsRoleValid] (@Userid,'3') = 1 THEN 0 ELSE 1 END AS [showView]
		, CASE WHEN [dbo].[fnIsRoleValid] (@Userid,'1,4') = 1 THEN 1 ELSE 0 END AS [showAssignApplicationToReviewers]
FROM #tempApplicationPrograms T
---------------------------------------------------------------------------------
---------------------------------------------------------------------------------
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