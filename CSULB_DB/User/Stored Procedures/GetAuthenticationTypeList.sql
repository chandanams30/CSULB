-- =============================================
-- Author:		ThoughtFocus
-- Create date: 02/14/2023
-- Description:	get Authentication Type Lsit
-- =============================================
CREATE PROCEDURE [User].[GetAuthenticationTypeList]
AS
BEGIN
	SELECT [AuthenticationTypeID], [AuthenticationTypeName] FROM [Master].[AuthenticationType];
END