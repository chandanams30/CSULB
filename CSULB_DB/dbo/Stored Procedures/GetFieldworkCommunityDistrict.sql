-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [dbo].[GetFieldworkCommunityDistrict]
CREATE PROCEDURE [dbo].[GetFieldworkCommunityDistrict]

AS
BEGIN

SELECT [ID] AS [CommunityDistrictID]
      ,[Name] AS [CommunityDistrictName]
      --,[Description]
      --,[CreatedBy]
      --,[CreatedDate]
  FROM [FieldWork].[CommunityDistrict]
   
END