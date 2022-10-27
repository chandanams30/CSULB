--exec [dbo].[AuthenticateUsers] '000077023', '1234'
CREATE PROCEDURE [dbo].[AuthenticateUsers]
    @Username nvarchar(100),
    @Password nvarchar(100)
AS
BEGIN

--Check if user is present in the [User].[UserRegistration]
IF NOT EXISTS(SELECT [ID] FROM [User].[UserCred] WHERE [Username] = @Username AND [Password] = @Password)
	BEGIN
		IF EXISTS(SELECT [ID] FROM [User].[UserRegistration] WHERE [CSULBID] = @Username AND [Password] = @Password AND [isUserCreated] = 0)
			BEGIN

				PRINT 'REGISTER USER'

				DECLARE @UserRegistrationID BIGINT, @newUserID BIGINT = NULL

				SELECT @UserRegistrationID = [ID] FROM [User].[UserRegistration] WHERE [CSULBID] = @Username AND [Password] = @Password AND [isUserCreated] = 0

				INSERT INTO [User].[Users]
						   ([FirstName]
						   ,[LastName]
						   ,[Email]
						   ,[AuthenticationTypeId]
						   ,[Status]
						   ,[CSULBID]
						   ,[FirstNamePref]
						   ,[LastNamePref]
						   ,[DisplayName]
						   ,[CreatedDateTime]
						   ,[CreatedByUserID])
						   (SELECT [FirstName]
								  ,[LastName]
								  ,[Email]
								  ,[AuthenticationTypeId]
								  ,1
								  ,[CSULBID]
								  ,[FirstName]
								  ,[LastName]
								  ,[FirstName] +' '+[LastName]
								  ,GETDATE()
								  ,[CreatedByUserID]
							  FROM [User].[UserRegistration]
							  WHERE [ID] = @UserRegistrationID)

							IF (@@ERROR = 0)
							BEGIN
								SELECT @newUserID = @@IDENTITY; 
							END

							INSERT INTO [User].[UserCred]
								   ([UserId]
								   ,[UserName]
								   ,[Password])
								   (SELECT @newUserID
								  ,[CSULBID]
								  ,[Password]
							  FROM [User].[UserRegistration]
							  WHERE [ID] = @UserRegistrationID)

								INSERT INTO [User].[UserRoles]
										   ([UserID]
										   ,[RoleID]
										   ,[CreatedDateTime]
										   ,[CreatedByUserID]
										   ,[ApplicationTypeID])
									 VALUES
										   (@newUserID
										   ,3	--Student
										   ,GETDATE()
										   ,1
										   ,2);

						UPDATE [User].[UserRegistration] SET [isUserCreated] = 1 WHERE [ID] = @UserRegistrationID;
			END
	END
---
    Declare @UserId bigint = 0;

    SELECT @UserId = [UserId] FROM [User].[UserCred] WHERE [Username] = @Username AND [Password] = @Password;

    IF @UserId > 0
    BEGIN

		SELECT
			U.[ID] AS [UserId],
			U.[FirstName],
			U.[LastName],
			U.[Email],
			U.[CSULBID]
		FROM [User].[Users] U
		WHERE U.[ID] = @UserId;

        SELECT R.[ID] as [RoleId],R.[Name] as RoleName FROM [User].[UserRoles] U 
        JOIN [Master].[Role] R ON R.ID = U.RoleID
        WHERE U.[UserID] = @UserId AND R.[ID]<>2
		ORDER BY  R.[ID];

        --BEGIN
        --INSERT INTO [User].[UserActivityLog]
  --         ([UserId]
  --         ,[LoginDateTime])
        --VALUES
  --         (@UserId
  --         ,getdate())
        --END
    END
END