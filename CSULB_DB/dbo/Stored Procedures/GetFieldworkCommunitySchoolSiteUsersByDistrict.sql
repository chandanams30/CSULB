-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec [dbo].[GetFieldworkCommunitySchoolSiteUsersByDistrict] 1
CREATE PROCEDURE [dbo].[GetFieldworkCommunitySchoolSiteUsersByDistrict]
 @CommunityDistrictID bigint
AS
BEGIN

--CommunitySchool

SELECT [ID] AS [CommunitySchoolID]
      ,[Name] AS [CommunitySchoolName]
  FROM [FieldWork].[CommunitySchool]
  WHERE [CommunityDistrictID] = @CommunityDistrictID

  --CommunitySiteUsers
  SELECT U.ID
		,U.FirstName+' '+ U.LastName as CommunitySiteUser 
  FROM FieldWork.CommunitySiteUsers  CSU
		JOIN [User].[Users] U on u.ID=CSU.UserID
		where CSU.[CommunityDistrictID] = @CommunityDistrictID
END