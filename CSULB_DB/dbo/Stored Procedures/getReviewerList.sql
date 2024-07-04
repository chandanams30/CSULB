
-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2023-Feb-08
-- Description:	Returns list of Reviewer for the program
-- =============================================
CREATE PROCEDURE [dbo].[getReviewerList]
@UserID BIGINT
,@FormID BIGINT
,@ProgramID BIGINT
,@TermCode varchar(10)

AS
BEGIN

SELECT U.[ID] AS [ReviewerID]
	, U.[FirstName] + ' ' + U.[LastName] AS [ReviewerName]
	,CAST(ISNULL(R.[isAssigned], 0) AS BIT) AS [isAssigned]
FROM 
[Master].[ProgramUsers] PU
LEFT JOIN [Application].[Forms] F ON PU.[ProgramID] = F.[ProgramID]
LEFT JOIN [Application].[Reviewer] R ON F.[ID] = R.[FormID] AND PU.[UserID] = R.[ReviewerID]
JOIN [User].[Users] U ON U.[ID] = PU.[UserID]
WHERE F.[ID] = @FormID
AND PU.[RoleID] = 6 --6	Reviewer


END