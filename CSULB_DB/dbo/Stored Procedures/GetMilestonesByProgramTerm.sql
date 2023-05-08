-- =============================================
-- Author:		ThoughtFocus
-- Create date: 04/05/2023
-- Description:	get GetMilestones by Program & Term
-- =============================================
CREATE PROCEDURE [dbo].[GetMilestonesByProgramTerm]
@ProgramID BIGINT,
@TermCode VarChar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
DECLARE @JSONResult AS NVARCHAR(MAX)
SELECT @JSONResult =
(
SELECT DISTINCT PT.[ProgramID]
	,PT.[ProgramName]
	,PT.[Semester]
	,PT.[TermCode]
	,[Milestones].[ID] AS [MilestoneID]
	,[Milestones].MilestoneName
FROM (
SELECT P.[ID] AS [ProgramID]
		,P.[Name] AS [ProgramName]
		,T.[Name] AS [Semester]
		,T.[TermCode] AS [TermCode]
		,pms.[MilestoneID]
	FROM [Master].[Programs] P 
	CROSS  JOIN [Master].[Term] T 
	LEFT JOIN [Master].[ProgramMilestones] PMS ON PMS.[ProgramID] = @ProgramID AND  PMS.[TermCode] = @TermCode
	WHERE P.[ID] = @ProgramID AND T.[TermCode] = @TermCode) PT
LEFT JOIN [Master].[Milestone] AS [Milestones] ON [Milestones].[ID] = PT.[MilestoneID]
FOR JSON AUTO ,WITHOUT_ARRAY_WRAPPER
)

	SELECT REPLACE(@JSONResult,'[{}]', '[]') AS [MilestonesByProgramTerm];

--execute [dbo].[GetMilestonesByProgramTerm]  @ProgramID = 1,  @TermCode = 2234
--execute [dbo].[GetMilestonesByProgramTerm]  @ProgramID = 2,  @TermCode = 2234
END