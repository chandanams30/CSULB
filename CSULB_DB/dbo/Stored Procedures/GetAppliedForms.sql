
CREATE PROCEDURE [dbo].[GetAppliedForms]
@Userid bigint,
@ApplicationTypeID bigint
AS
BEGIN

--DECLARE @RoleID AS BIGINT

--SELECT @RoleID = [RoleID] FROM [User].[UserRoles] UR WHERE UR.[UserID]=@Userid AND UR.[RoleID] <>2
----SELECT @RoleID


SELECT 
F.[ID]
,F.[FormStateID]
,F.[CreatedDateTime] AS [AppliedDate]
--,F.[UserID]
--,P.[ID]
,P.[Name]
,T.[Name] AS [Semester]
,FS.[Name] AS [Status]
,F.[TermCode]
,F.[ProgramID]
FROM [Application].[Forms] F
LEFT JOIN [Master].[ProgramApplicationType] PAT ON PAT.[ProgramID] = F.[ProgramID] 
LEFT JOIN [Master].[Programs] P ON P.[ID] = F.[ProgramID]
LEFT JOIN [Master].[Term] T ON T.[TermCode] = F.[TermCode]
JOIN [Master].[FormState] FS ON FS.[ID] = F.[FormStateID]
WHERE F.UserID = @Userid
AND PAT.[ApplicationTypeID]=@ApplicationTypeID
	
		
END

/*
SELECT * FROM [Application].[Forms]
SELECT * FROM [Master].[ProgramApplicationType]
SELECT * FROM [Master].[Programs]
SELECT * FROM [Master].[Term]
SELECT * FROM [Master].[FormState]

exec [GetAppliedForms] 1,2
exec [GetAppliedForms] 338,2
exec [GetAppliedForms] 339,2

*/