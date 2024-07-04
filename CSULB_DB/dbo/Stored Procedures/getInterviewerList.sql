
-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Dec-14
-- Description:	Returns list of Interviewer
-- =============================================
--exec [dbo].[getInterviewerList] @UserID=1,@FormID=10255,@ProgramID=4,@TermCode=N'2234'
--exec [dbo].[getInterviewerList] @UserID=1,@FormID=10255,@ProgramID=6,@TermCode=N'2234'
--exec [dbo].[getInterviewerList] @UserID=1,@FormID=10255,@ProgramID=null,@TermCode=N'2234'
--exec [dbo].[getInterviewerList] @UserID=1,@FormID=10255,@ProgramID=4,@TermCode=N'2234'
CREATE PROCEDURE [dbo].[getInterviewerList]
@UserID BIGINT
,@FormID BIGINT
,@ProgramID BIGINT
,@TermCode varchar(10)

AS
BEGIN
	--SELECT * FROM [Master].[Role]
	--SELECT UR.[UserID] AS [InterviewerUserID]
	--, U.[FirstName] + ' ' + U.[LastName] AS [InterviewerName]
	--FROM [User].[Users] U
	--	JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID]
	--WHERE UR.[RoleID] = 8
	--8	Interviewer

SELECT U.[ID] AS [InterviewerUserID]
	,U.[FirstName] + ' ' + U.[LastName] AS [InterviewerName]
	,CAST(ISNULL(I.[isAssigned], 0) AS BIT) AS [isAssigned]
FROM [User].[Users] U
JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID] AND UR.[RoleID] = 8
LEFT JOIN [Application].[Interviewer] I ON I.[InterviewerUserID] = U.[ID] AND I.[FormID] = @FormID
JOIN [Master].[ProgramUsers] PU ON PU.[ProgramID] = ISNULL(@ProgramID, PU.[ProgramID]) AND pu.[UserID] = U.[ID] AND PU.[RoleID] = 8

--select * from [Master].[ProgramUsers]  where [RoleID]=8;

END