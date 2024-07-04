-- =============================================
-- Author:		ThoughtFocus
-- Create date: 24-Apr-2023
-- Description:	get Milestone Approver User List
-- =============================================
CREATE PROCEDURE [dbo].[GetMilestoneApproverUserList]
	
AS
BEGIN

	SELECT DISTINCT U.[ID] AS [ApproverUserID]
	  ,U.[FirstName] + ' ' + U.[LastName] AS  [ApproverUserName]
  FROM [User].[Users] U 
  JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID]
  WHERE UR.[RoleID] IN (6) -- ASSUMING Reviewer as "Milestone Approver User"

  --select * from [Master].[Role]

---------------------------------------------------------------------------
--exec [dbo].[GetMilestoneApproverUserList]
----------------------------------------------------------------------

END