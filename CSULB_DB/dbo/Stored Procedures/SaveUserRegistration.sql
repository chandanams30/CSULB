--EXEC 
--EXEC 
--EXEC [SaveUserRegistration] 'FirstName', 'LastName', 'TEST2@TEST.COM', '000567890', 'PASSWORD'
CREATE PROCEDURE [dbo].[SaveUserRegistration] 
(
	@FirstName nvarchar(255)
	,@LastName nvarchar(255)
	,@Email nvarchar(255)
	,@CSULBID varchar(50)
	,@Password varchar(50)
           
) AS
BEGIN

DECLARE @Staus AS INT, @Message nvarchar(255)

IF ((ISNULL(@Email,'')='') OR (ISNULL(@CSULBID,'')='') OR (ISNULL(@Password,'')=''))
	BEGIN
		SET @Staus = 0 -- Error
		SET @Message = 'Invalid data for user registration.'
	END
ELSE IF EXISTS(SELECT [ID] FROM [User].[Users] WHERE [CSULBID] = @CSULBID OR [Email]=@Email)
	BEGIN
		SET @Staus = 0 -- Error
		SET @Message = 'User already registered.'
	END
ELSE IF EXISTS(SELECT [ID] FROM [User].[UserRegistration] WHERE [CSULBID] = @CSULBID OR [Email]=@Email)
BEGIN
		SET @Staus = 0 -- Error
		SET @Message = 'User already registered.'
	END
ELSE IF NOT EXISTS(SELECT [ID] FROM [User].[UserRegistration] WHERE [CSULBID] = @CSULBID OR [Email]=@Email)
	BEGIN
		INSERT INTO [User].[UserRegistration]
			   ([FirstName]
			   ,[LastName]
			   ,[Email]
			   ,[AuthenticationTypeId]
			   ,[CSULBID]
			   ,[Password]
			   ,[isUserCreated]
			   ,[CreatedDateTime]
			   ,[CreatedByUserID])
		 VALUES
			   (@FirstName
			   ,@LastName
			   ,@Email
			   ,2 --Basic Authenticated
			   ,@CSULBID
			   ,@Password
			   ,0
			   ,GETDATE()
			   ,1)
		SET @Staus = 1
		SET @Message = 'User Registered Sucessfully'
	END

	SELECT @Staus [Status], @Message [Message]

END

/*



SELECT * FROM [User].[UserRegistration]
*/