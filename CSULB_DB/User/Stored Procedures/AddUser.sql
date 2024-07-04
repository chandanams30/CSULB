-- =============================================
-- Author:		ThoughtFocus
-- Create date: 02/14/2023
-- Description:	Add User
-- =============================================
CREATE PROCEDURE [User].[AddUser]
	@CSULBID nvarchar(50)
	,@displayName nvarchar(255) 
	,@mail nvarchar(255)
	,@LastName nvarchar(255)
	,@FirstName nvarchar(255)
	,@RoleID bigint --'Student','Staff'
	,@AuthenticationTypeId int --1	SSO Authenticated, 2 Basic Authenticated
	,@createdByUserID bigint

AS
BEGIN
	DECLARE @UserId bigint = 0;
    SELECT @UserId = [ID] FROM [User].[Users] WHERE [CSULBID] = @CSULBID OR [Email] = @mail;

	--If SSO authenicated user is a student and not present in MyCED then register user as student
	IF (@UserId = 0)
		BEGIN
			--REGISTER USER AS NEW STUDENT
			PRINT @mail
			DECLARE @newUserID bigint = NULL
			
			--Register student as new user
			INSERT INTO [User].[Users]
			(
				 [CSULBID]
				,[FirstName]
				,[LastName]
				,[Email]
				,[CreatedDateTime]
				,[CreatedByUserID]
				,[AuthenticationTypeId]
				,[Status]        
				,[FirstNamePref]
				,[LastNamePref]
				,[DisplayName]
			)
			VALUES (@CSULBID,@FirstName,@LastName,@mail,GETDATE(),@createdByUserID,1,1,@FirstName,@LastName,@displayName)

			IF (@@ERROR = 0)
				BEGIN
					SELECT @newUserID = @@IDENTITY; 
				END
			
			print @newUserID

			--Insert New user as role
			--DECLARE @RoleID BIGINT;
			--SELECT @RoleID = ID from  [Master].[Role] where [Name]=@UserRoleType;
			--SELECT * from [Master].[Role]
		IF @RoleID NOT IN (3,9,10) --3	Student, 9	Community Site Supervisor/Demonstration Teacher, 10	Community Supervisor
		BEGIN
			INSERT INTO  [User].[UserRoles]
			(
				[UserID]
			   ,[RoleID]
			   ,[CreatedDateTime]
			   ,[CreatedByUserID]
			   ,[ApplicationTypeID]
			)
			VALUES (@newUserID, 2, GETDATE(),1,2) --Default to Staff Account
		END

			INSERT INTO  [User].[UserRoles]
			(
				[UserID]
			   ,[RoleID]
			   ,[CreatedDateTime]
			   ,[CreatedByUserID]
			   ,[ApplicationTypeID]
			)
			VALUES (@newUserID, @RoleID, GETDATE(),1,2)
			
			SELECT 1 AS [Status], '' AS [Message];
		END 
		ELSE 
		BEGIN
			SELECT 0 AS [Status], 'Either CSULBID/EMAIL already exists.' AS [Message];
		END
  
  --SELECT @UserId = [ID] FROM [User].[Users] WHERE [CSULBID] = @CSULBID;

END