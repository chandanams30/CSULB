-- =============================================
-- Author:		ThoughtFocus
-- Create date: 02/14/2023
-- Description:	get Role Lsit
-- =============================================
CREATE PROCEDURE [User].[GetRolesList]
AS
BEGIN
	SELECT [ID] AS [RoleID], [Name] AS [RoleName] FROM [Master].[Role] WHERE [ID] NOT IN (2);
END