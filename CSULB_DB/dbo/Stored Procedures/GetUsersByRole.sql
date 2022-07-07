-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE GetUsersByRole
@roleid int,
@userId int
AS
BEGIN
if(@roleid>0)
begin
	select us.id as UserId,us.FirstName+' '+us.lastname as [Name], us.Email as Email, mr.ID as RoleId, mr.Description from  [User].[Users] us
	join [User].[UserRoles] ur on us.ID=ur.UserID
	join [Master].[Role] mr on ur.RoleID=mr.ID
	where ur.RoleID=@roleid
end 
else
begin 
	select us.id as UserId,us.FirstName+' '+us.lastname as [Name], us.Email as Email, mr.ID as RoleId, mr.Description from  [User].[Users] us
	join [User].[UserRoles] ur on us.ID=ur.UserID
	join [Master].[Role] mr on ur.RoleID=mr.ID
	where ur.UserID=@userId
end
END