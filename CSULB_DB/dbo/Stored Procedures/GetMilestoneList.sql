-- =============================================
-- Author:		ThoughtFocus
-- Create date: 20-Mar-2023
-- Description:	Add/update Milestone Forms
-- =============================================
CREATE PROCEDURE [dbo].[GetMilestoneList]
	@isPublished [bit] = NULL

AS
BEGIN
--declare @isPublished bit = null
SELECT (
	SELECT MS.[ID] AS [MilestoneID]
				,[MilestoneName]
			   ,[MilestoneDescription]
			   --,[MilestoneForm]
			   ,[isPublished]
			   --,[CreatedBy]
			   --,[CreatedDate]
			   , U.[FirstName] + ' ' + U.[LastName] AS [CreatedByName]
			   FROM [Master].[Milestone] MS
			   LEFT JOIN [User].[Users] U ON U.[ID] = MS.[CreatedBy]
			   WHERE [isPublished] = ISNULL(@isPublished, [isPublished])
			   FOR JSON AUTO) AS MilestoneList
END