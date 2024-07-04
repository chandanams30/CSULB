
-- =============================================
-- Author:		ThoughtFocus
-- Create date: 16-Feb-2023
-- Description:	get users list
-- =============================================

CREATE PROCEDURE [User].[GetUserDataOptionList]
AS
BEGIN
	SELECT(
		SELECT 
			(SELECT [ID] AS [RoleID], [Name] AS [RoleName], CAST((CASE WHEN [ID] in (1,2,3,9,10) THEN 0 ELSE 1 END) AS BIT) AS [isProgramAssociated], CAST((CASE WHEN [ID] in (9,10) THEN 1 ELSE 0 END) AS BIT) AS [isCommunityDistrictAssociated] FROM [Master].[Role] WHERE [ID] NOT IN (2) FOR JSON PATH) AS [Roles],  
			(SELECT [AuthenticationTypeID], [AuthenticationTypeName] FROM [Master].[AuthenticationType] FOR JSON PATH) AS [AuthenticationTypes],
			(SELECT [ID] AS [ProgramID],[Name] AS [ProgramName] FROM [Master].[Programs] FOR JSON PATH) AS [Programs],
			(SELECT [ID] AS [CommunityDistrictID],[Name] AS [CommunityDistrictName] FROM [FieldWork].[CommunityDistrict] FOR JSON PATH) AS [CommunityDistrict]
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER) AS  [UserDataOptionList]

-- =============================================
-- Example to execute the stored procedure
-- =============================================
--EXECUTE [User].[GetUserDataOptionList] 
--
END