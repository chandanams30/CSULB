
--exec AuthenticateSSOUsers '002520269'
CREATE PROCEDURE AuthenticateSSOUsers
@CSULBID nvarchar(50)
AS
BEGIN
	Declare @UserId bigint = 0;

    SELECT @UserId = [ID] FROM [User].[Users] WHERE [CSULBID] = @CSULBID;

    if @UserId > 0
    BEGIN

		SELECT
		U.[ID] AS [UserId],
		U.[FirstName],
		U.[LastName],
		U.[Email],
		U.[CSULBID]
		FROM [User].[Users] U
		WHERE U.[CSULBID] = @CSULBID;

        SELECT R.[ID] as [RoleId],R.[Name] as RoleName FROM [User].[UserRoles] U 
        JOIN [Master].[Role] R ON R.ID = U.RoleID
        WHERE U.[UserID] = @UserId;

    END
END