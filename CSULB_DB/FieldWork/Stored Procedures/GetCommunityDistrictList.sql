-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2023-Mar-17
-- Description:	Community District List
-- =============================================
create PROCEDURE [FieldWork].[GetCommunityDistrictList]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT (
		SELECT [ID] AS [CommunityDistrictID]
			,[Name] AS [CommunityDistrictName]
		FROM [FieldWork].[CommunityDistrict] ORDER BY [Name]
		FOR JSON AUTO
		) [CommnunityDistrictList]

END