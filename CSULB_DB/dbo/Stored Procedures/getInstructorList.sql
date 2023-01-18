

-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Dec-14
-- Description:	Returns list of Instructor
-- =============================================
CREATE PROCEDURE [dbo].[getInstructorList]
@UserID BIGINT
,@FormID BIGINT
,@ProgramID BIGINT
,@TermCode varchar(10)

AS
BEGIN
	--SELECT * FROM [Master].[Role]
	SELECT UR.[UserID] AS [InstructorUserID]
	, U.[FirstName] + ' ' + U.[LastName] AS [InstructorName]
	FROM [User].[Users] U
		JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID]
	WHERE UR.[RoleID] = 7
	--7	Instructor
END