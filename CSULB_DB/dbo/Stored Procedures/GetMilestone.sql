-- =============================================
-- Author:		ThoughtFocus
-- Create date: 24-Apr-2023
-- Description:	get Milestone Forms by ID
-- =============================================
CREATE PROCEDURE [dbo].[GetMilestone]
	@MilestoneID BIGINT
AS
BEGIN
--declare @isPublished bit = null

	SELECT MS.[ID] AS [MilestoneID]
				,[MilestoneName]
			   ,[MilestoneDescription]
			   ,[MilestoneForm]
			   ,[isPublished]
			   ,[isMandatory]
			   ,[CreatedBy]
			   ,[CreatedDate]
			   , U.[FirstName] + ' ' + U.[LastName] AS [CreatedByName]
			   FROM [Master].[Milestone] MS
			   LEFT JOIN [User].[Users] U ON U.[ID] = MS.[CreatedBy]
			   WHERE MS.[ID] = @MilestoneID
			  -- FOR JSON AUTO

	SELECT [ApproverUserID]
      ,[Sequence]
	  ,U.[FirstName] + ' ' + U.[LastName] AS  [ApproverUserName]
  FROM [Master].[MileStoneApprovers] MA
  JOIN [User].[Users] U ON U.[ID] = MA.[ApproverUserID]
  WHERE MA.[MilestoneID] = @MilestoneID

---------------------------------------------------------------------------
--exec [dbo].[GetMilestone] @MilestoneID=6
----------------------------------------------------------------------

END