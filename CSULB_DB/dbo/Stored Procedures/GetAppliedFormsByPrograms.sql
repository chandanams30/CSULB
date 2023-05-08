-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Sep-13
-- Description:	Returns Applied Forms List by Programs 
-- =============================================
--exec [GetAppliedFormsByPrograms] 1, 8, 2234,0
--exec [dbo].[GetAppliedFormsByPrograms] @UserId=1,@ProgramID=6,@TermCode=N'2234',@FormStateID=0
CREATE PROCEDURE [dbo].[GetAppliedFormsByPrograms]
@Userid bigint,
@ProgramID bigint,
@TermCode varchar(10),
@FormStateID int
AS
BEGIN

--DECLARE @RoleID AS BIGINT

--SELECT [RoleID] INTO #TempRoles FROM [User].[UserRoles] UR WHERE UR.[UserID]=@Userid AND UR.[RoleID] <>2
------SELECT @RoleID

SET @FormStateID = CASE WHEN @FormStateID=0 THEN NULL ELSE @FormStateID END;

------------------------------------------------------------------------------------------------------------------------------

--Administrator
SELECT F.[ID] AS [FormID] INTO #tempFormIDs FROM [Application].[Forms] F 
WHERE [dbo].[fnIsRoleValid] (@Userid,'1') = 1 --1	Administrator
AND F.[ProgramID]=@ProgramID 
AND F.[TermCode]=@TermCode 
AND F.[FormStateID] = ISNULL(@FormStateID, F.[FormStateID])
UNION
--3	Student
SELECT F.[ID] FROM [Application].[Forms] F 
WHERE [dbo].[fnIsRoleValid] (@Userid,'3') = 1 --3	Student
AND F.[ProgramID]=@ProgramID 
AND F.[TermCode]=@TermCode 
AND F.[FormStateID] = ISNULL(@FormStateID, F.[FormStateID])
AND F.[UserID] = @Userid
UNION
--Program Admin
SELECT F.[ID] FROM [Application].[Forms] F 
JOIN [Master].[ProgramUsers] PU ON PU.ProgramID = F.ProgramID
WHERE [dbo].[fnIsRoleValid] (@Userid,'4') = 1 --4 ProgramAdmin
AND F.[ProgramID]=@ProgramID 
AND F.[TermCode]=@TermCode 
AND F.[FormStateID] = ISNULL(@FormStateID, F.[FormStateID])
AND PU.UserID = @Userid
AND PU.[RoleID] = 4
UNION
--6	Reviewer
SELECT F.[ID] FROM [Application].[Forms] F 
JOIN [Application].[Reviewer] R ON R.[FormID] = F.[ID]
WHERE [dbo].[fnIsRoleValid] (@Userid,'6') = 1 --6	Reviewer
AND F.[ProgramID]=@ProgramID 
AND F.[TermCode]=@TermCode 
AND F.[FormStateID] in (5) --'5,4' --Reviewer
AND R.[ReviewerID] = @Userid
AND r.[isAssigned]=1
UNION
--7	Instructor 
SELECT F.[ID] FROM [Application].[Forms] F 
JOIN [Application].[Instructor] INS ON INS.[FormID] = F.[ID]
WHERE [dbo].[fnIsRoleValid] (@Userid,'7') = 1 --7	Instructor
AND F.[ProgramID]=@ProgramID 
AND F.[TermCode]=@TermCode 
AND F.[FormStateID] = ISNULL(@FormStateID, F.[FormStateID])
--AND F.[FormStateID] in (1,2) --'1,2' --Instructor
AND INS.[InstructorUserID] = @Userid
AND INS.[isAssigned]=1
UNION
--8	Interviewer  
SELECT F.[ID] FROM [Application].[Forms] F 
JOIN [Application].[Interviewer] INVR ON INVR.[FormID] = F.[ID]
WHERE [dbo].[fnIsRoleValid] (@Userid,'8') = 1 --8	Interviewer 
AND F.[ProgramID]=@ProgramID 
AND F.[TermCode]=@TermCode 
AND F.[FormStateID] = ISNULL(@FormStateID, F.[FormStateID])
--AND F.[FormStateID] = 7 --7	Schedule Interview
AND INVR.[InterviewerUserID] = @Userid
AND INVR.[isAssigned]=1
UNION
--11	Program Coordinator
SELECT F.[ID] AS [FormID] FROM [Application].[Forms] F 
WHERE [dbo].[fnIsRoleValid] (@Userid,'11') = 1 --11	Program Coordinator
AND F.[ProgramID]=@ProgramID 
AND F.[TermCode]=@TermCode 
AND F.[FormStateID] = ISNULL(@FormStateID, F.[FormStateID]);

-------------------------------------------------------------------------------------------------------------------------------
--Reviewers List as comma seperated 
SELECT R.[FormID]
	,STRING_AGG(RU.[FirstName] + ' ' + RU.[LastName], ', ') WITHIN
GROUP (
		ORDER BY RU.[FirstName]
		) AS [ReviewersName]
INTO #tempReviewerName
FROM [Application].[Forms] F 
JOIN [Application].[Reviewer] R ON R.[FormID] = F.[ID]
JOIN [User].[Users] RU ON R.[ReviewerID] = RU.[ID]
JOIN [User].[UserRoles] UR ON UR.[UserID] = RU.[ID] AND UR.[RoleID]=6  -- 6 Reviewer
JOIN [Master].[ProgramUsers] PU ON PU.[UserID] = RU.[ID] AND PU.[RoleID]=6  AND PU.[ProgramID] = F.[ProgramID] -- 6 Reviewer
WHERE R.[FormID] IN (SELECT [FormID] FROM #tempFormIDs)
AND R.[isAssigned] = 1
GROUP BY R.[FormID];

--WITH Reviewer_Row_Number AS (
--  SELECT
--    *,
--    ROW_NUMBER() OVER(PARTITION BY R.[FormID] ORDER BY [ReviewerID]  DESC) AS row_number
--  FROM [Application].[Reviewer] R WHERE R.[FormID] IN (SELECT [FormID] FROM #tempFormIDs) AND [isAssigned] = 1)
--SELECT
--  RRN.[FormID], RU.[FirstName] + ' ' + RU.[LastName] AS [ReviewersName]
--  INTO #tempReviewerName
--FROM Reviewer_Row_Number RRN
--JOIN [User].[Users] RU ON RRN.[ReviewerID] = RU.[ID]
--WHERE row_number = 1;
-------------------------------------------------------------------------------------------------------------------------------


SELECT 
F.[ID]
,U.FirstName + ' ' + U.LastName AS [StudentName]
,U.[FirstName] AS [StudentFirstName]
,U.[LastName] AS [StudentLastName]
,U.[CSULBID] 
,F.[FormStateID]
,FS.[Name] AS [FormState]
,F.[CreatedDateTime] AS [AppliedDate]
--,F.[UserID]
--,P.[ID]
,F.[ProgramID]
,P.[Name] AS [ProgramName]
,T.[Name] AS [Semester]
,F.[TermCode]
,RN.[ReviewersName]
FROM [Application].[Forms] F
JOIN [User].[Users] U ON U.ID = F.UserID
LEFT JOIN [Master].[Programs] P ON P.[ID] = F.[ProgramID]
LEFT JOIN [Master].[Term] T ON T.[TermCode] = F.[TermCode]
JOIN [Master].[FormState] FS ON FS.[ID] = F.[FormStateID]
JOIN [Master].[ProgramApplicationDates] PAD ON PAD.[ProgramID] = F.[ProgramID] AND PAD.[TermCode] = F.[TermCode]
LEFT JOIN #tempReviewerName RN ON RN.[FormID] = F.[ID]
WHERE F.[ID] in (SELECT [FormID] FROM #tempFormIDs)
AND PAD.[ApplicationCloseDate] > (CASE WHEN F.[FormStateID] IN (1,2) THEN GETDATE() -1 ELSE PAD.[ApplicationCloseDate] -1  END)

--SELECT 
--F.[ID]
--,U.FirstName + ' ' + U.LastName AS [StudentName]
--,U.[CSULBID] 
--,F.[FormStateID]
--,FS.[Name] AS [FormState]
--,F.[CreatedDateTime] AS [AppliedDate]
----,F.[UserID]
----,P.[ID]
--,F.[ProgramID]
--,P.[Name] AS [ProgramName]
--,T.[Name] AS [Semester]
--,F.[TermCode]
--FROM [Application].[Forms] F
--JOIN [User].[Users] U ON U.ID = F.UserID
--LEFT JOIN [Master].[Programs] P ON P.[ID] = F.[ProgramID]
--LEFT JOIN [Master].[Term] T ON T.[TermCode] = F.[TermCode]
--JOIN [Master].[FormState] FS ON FS.[ID] = F.[FormStateID]
--WHERE F.[ProgramID]=@ProgramID AND F.[TermCode]=@TermCode
--AND F.[FormStateID] = @FormStateID





-------------------------------------------------------------------------------------------------------------------------------
DECLARE @showAssignApplicationToReviewers AS BIT
--SELECT @showAssignApplicationToReviewers = CASE WHEN [RoleID] in (1,4) THEN 1 ELSE 0 END FROM [User].[UserRoles] UR WHERE UR.[UserID]=@Userid AND UR.[RoleID] <>2
--SELECT @showAssignApplicationToReviewers = CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END FROM [User].[UserRoles] UR WHERE UR.[UserID]=@Userid AND [RoleID] IN (1,4);
SELECT @showAssignApplicationToReviewers =[dbo].[fnIsRoleValid] (@Userid,'1,4');

--select @showAssignApplicationToReviewers;

SELECT T.[Name] AS [Semester], T.[TermCode], P.[ID] AS [ProgramID], p.[Name] AS [ProgramName], @showAssignApplicationToReviewers AS [showAssignApplicationToReviewers]
	from [Master].[Term] T CROSS JOIN
	[Master].[Programs] p 
	WHERE 
		P.[ID] = @ProgramID
		AND T.TermCode = @TermCode
	
END

/*
SELECT * FROM [Application].[Forms]
SELECT * FROM [Master].[ProgramApplicationType]
SELECT * FROM [Master].[Programs]
SELECT * FROM [Master].[Term]
SELECT * FROM [Master].[FormState]



*/