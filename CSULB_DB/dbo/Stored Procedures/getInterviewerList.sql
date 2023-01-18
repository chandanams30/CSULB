

-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Dec-14
-- Description:	Returns list of Interviewer
-- =============================================
CREATE PROCEDURE [dbo].[getInterviewerList]
@UserID BIGINT
,@FormID BIGINT
,@ProgramID BIGINT
,@TermCode varchar(10)

AS
BEGIN
	--SELECT * FROM [Master].[Role]
	SELECT UR.[UserID] AS [InterviewerUserID]
	, U.[FirstName] + ' ' + U.[LastName] AS [InterviewerName]
	FROM [User].[Users] U
		JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID]
	WHERE UR.[RoleID] = 8
	--8	Interviewer
END