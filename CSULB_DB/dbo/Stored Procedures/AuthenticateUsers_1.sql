CREATE PROCEDURE [dbo].[AuthenticateUsers]
    @Username nvarchar(100),
    @Password nvarchar(100)
AS
BEGIN
    Declare @UserId bigint = 0;

    SELECT @UserId = [UserId] FROM [User].[UserCred] WHERE [Username] = @Username AND [Password] = @Password;

    if @UserId > 0
    BEGIN

        SELECT [ID] as [UserId],[FirstName],[LastName],[Email] FROM [User].[Users] WHERE [ID] = @UserId;

        SELECT R.[ID] as [RoleId],R.[Name] as RoleName FROM [User].[UserRoles] U 
        JOIN [Master].[Role] R ON R.ID = U.RoleID
        WHERE U.[UserID] = @UserId;

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