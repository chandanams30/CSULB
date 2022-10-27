
--exec AuthenticateSSOUsers '002520269'
CREATE PROCEDURE [dbo].[AuthenticateSSOUsers]
	@CSULBID nvarchar(50)
	,@displayName nvarchar(255) 
	,@mail nvarchar(255)
	,@LastName nvarchar(255)
	,@FirstName nvarchar(255)
AS
BEGIN
	DECLARE @UserId bigint = 0;
    SELECT @UserId = [ID] FROM [User].[Users] WHERE [CSULBID] = @CSULBID;

	--If SSO authenicated user is a student and not present in MyCED then register user as student
	IF (@UserId = 0) AND (@mail like '%student.csulb.edu')
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
			VALUES (@CSULBID,@FirstName,@LastName,@mail,GETDATE(),1,1,1,@FirstName,@LastName,@displayName)

			IF (@@ERROR = 0)
				BEGIN
					SELECT @newUserID = @@IDENTITY; 
				END
			
			print @newUserID

			--Insert New user as student role
			DECLARE @StudentRoleID BIGINT;
			SELECT @StudentRoleID = ID from  [MyCED].[Master].[Role] where [Name]='Student';

			INSERT INTO  [User].[UserRoles]
			(
				[UserID]
			   ,[RoleID]
			   ,[CreatedDateTime]
			   ,[CreatedByUserID]
			   ,[ApplicationTypeID]
			)
			VALUES (@newUserID, @StudentRoleID, GETDATE(),1,2)
			
			SELECT @UserId = [ID] FROM [User].[Users] WHERE [CSULBID] = @CSULBID;
		END 

	--SELECTION FOR RETURNING USER DATA
    IF @UserId > 0
    BEGIN

		--SELECT
		--U.[ID] AS [UserId],
		--U.[FirstName],
		--U.[LastName],
		--U.[Email],
		--U.[CSULBID]
		--FROM [User].[Users] U
		--WHERE U.[CSULBID] = @CSULBID;

  --      SELECT R.[ID] as [RoleId],R.[Name] as RoleName FROM [User].[UserRoles] U 
  --      JOIN [Master].[Role] R ON R.ID = U.RoleID
  --      WHERE U.[UserID] = @UserId;

		SELECT
			U.[ID] AS [UserId],
			U.[FirstName],
			U.[LastName],
			U.[Email],
			U.[CSULBID]
		FROM [User].[Users] U
		WHERE U.[ID] = @UserId AND U.[CSULBID] = @CSULBID;

        SELECT R.[ID] as [RoleId],R.[Name] as RoleName FROM [User].[UserRoles] U 
        JOIN [Master].[Role] R ON R.ID = U.RoleID
        WHERE U.[UserID] = @UserId AND R.[ID]<>2
		ORDER BY  R.[ID];
	  END
	  
END