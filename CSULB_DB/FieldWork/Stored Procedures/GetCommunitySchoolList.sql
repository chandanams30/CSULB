-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2023-Mar-17
-- Description:	Community School List
-- =============================================
CREATE PROCEDURE [FieldWork].[GetCommunitySchoolList]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

WITH CommunitySchoolList
AS (
	SELECT CS.[ID] AS [CommunitySchoolID]
		,CS.[Name] AS [CommunitySchoolName]
		,CD.[ID] AS [CommunityDistrictID]
		,CD.[Name] AS [CommunityDistrictName]
	FROM [FieldWork].[CommunitySchool] CS
	JOIN [FieldWork].[CommunityDistrict] CD ON CD.ID=CS.CommunityDistrictID
	)
SELECT (
		SELECT [CommunitySchoolID]
			,[CommunitySchoolName]
			,[CommunityDistrictID]
			,[CommunityDistrictName]
		FROM CommunitySchoolList
		ORDER BY [CommunityDistrictName], [CommunitySchoolName]
		FOR JSON AUTO
		) [CommunitySchoolList]

END