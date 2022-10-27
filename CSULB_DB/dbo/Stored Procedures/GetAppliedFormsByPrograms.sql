
--exec [GetAppliedFormsByPrograms] 1, 9, 2234
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

--select * from #TempRoles

SELECT 
F.[ID]
,U.FirstName + ' ' + U.LastName AS [StudentName]
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
FROM [Application].[Forms] F
JOIN [User].[Users] U ON U.ID = F.UserID
LEFT JOIN [Master].[Programs] P ON P.[ID] = F.[ProgramID]
LEFT JOIN [Master].[Term] T ON T.[TermCode] = F.[TermCode]
JOIN [Master].[FormState] FS ON FS.[ID] = F.[FormStateID]
WHERE F.[ProgramID]=@ProgramID AND F.[TermCode]=@TermCode
AND F.[FormStateID] = @FormStateID
	

SELECT T.[Name] AS [Semester], T.[TermCode], P.[ID] AS [ProgramID], p.[Name] AS [ProgramName]
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