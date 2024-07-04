

-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Dec-14
-- Description:	Returns list of Instructor
-- =============================================
--exec [dbo].[getInstructorList] @UserID=1,@FormID=10255,@ProgramID=6,@TermCode=N'2234'
CREATE PROCEDURE [dbo].[getInstructorList]
@UserID BIGINT
,@FormID BIGINT
,@ProgramID BIGINT
,@TermCode varchar(10)

AS
BEGIN
	--SELECT * FROM [Master].[Role]
	--SELECT UR.[UserID] AS [InstructorUserID]
	--, U.[FirstName] + ' ' + U.[LastName] AS [InstructorName]
	--FROM [User].[Users] U
	--	JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID]
	--WHERE UR.[RoleID] = 7
	--7	Instructor
	SELECT U.[ID] AS [InstructorUserID]
	,U.[FirstName] + ' ' + U.[LastName] AS [InstructorName]
	,CAST(ISNULL(I.[isAssigned], 0) AS BIT) AS [isAssigned]
FROM [User].[Users] U
JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID] AND UR.[RoleID] = 7
LEFT JOIN [Application].[Instructor] I ON I.[InstructorUserID] = U.[ID] AND I.[FormID] = @FormID
JOIN [Master].[ProgramUsers] PU ON PU.[ProgramID] = ISNULL(@ProgramID, PU.[ProgramID]) AND pu.[UserID] = u.[ID] AND PU.RoleID=7

--select * from [Master].[ProgramUsers] where RoleID=7;

END