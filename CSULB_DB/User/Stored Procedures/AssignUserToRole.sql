-- =============================================
-- Author:		ThoughtFocus
-- Create date: 02/14/2023
-- Description:	Assign User to Role
-- =============================================
CREATE PROCEDURE [User].[AssignUserToRole]
@UserID AS BIGINT
,@RoleID AS BIGINT
 ,@createdByUserID bigint
AS
BEGIN
	--SELECT * FROM [User].[UserRoles];

	IF NOT EXISTS (SELECT * FROM [User].[UserRoles] WHERE [UserID] = @UserID AND [RoleID]=@RoleID)
	BEGIN
		INSERT INTO [User].[UserRoles]
				   ([UserID]
				   ,[RoleID]
				   ,[CreatedDateTime]
				   ,[CreatedByUserID]
				   ,[ApplicationTypeID])
			 VALUES
				   (@UserID
				   ,@RoleID
				   ,GETDATE()
				   ,@createdByUserID
				   ,4)
	END
END