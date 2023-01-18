-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Dec-14
-- Description:	validates user for multiple Role permission
-- =============================================
--SELECT [dbo].[fnIsRoleValid] (1,'2,3,4,5');
--SELECT [dbo].[fnIsRoleValid] (1,'1,2,3,4,5');
-- =============================================
CREATE FUNCTION [dbo].[fnIsRoleValid] 
(
	-- Add the parameters for the function here
	@Userid bigint,
	@RoleIDArry as nvarchar(50)
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result bit

	IF EXISTS (SELECT [RoleID] FROM [User].[UserRoles] UR JOIN [dbo].[SplitString] (@RoleIDArry,',') SS on ss.Item = ur.RoleID WHERE UR.[UserID]=@Userid AND UR.[RoleID] <>2 ) 
		BEGIN
			SELECT @Result = 1; 
		END
	ELSE
		BEGIN
			SELECT @Result = 0;
		END
	
	-- Return the result of the function
	RETURN @Result
END