-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

create PROCEDURE [dbo].[UpdateFieldworkCommunityUsersforCreation]
 @UserID bigint,
 @UserName  nvarchar(50),
 @Password nvarchar(50)
AS
BEGIN
/*
SELECT * FROM [User].[Users] U;
SELECT * FROM [User].[UserRoles] UR;
SELECT * FROM [User].[UserCred] UC;
SELECT * FROM [Master].[Role] R;
*/

IF NOT EXISTS (SELECT * FROM [User].[UserCred] UC WHERE UC.[UserId] = @UserID)
BEGIN

	INSERT INTO [User].[UserCred]
			   ([UserId]
			   ,[UserName]
			   ,[Password])
		 VALUES
			   (@UserId
			   --,@UserName
			   ,'partner' + CONVERT(VARCHAR(50), @UserID)
			   ,@Password)
END
--DELETE FROM [User].[UserCred] WHERE UserId IN (2215,2253,2254,2255,2256,2257)

END