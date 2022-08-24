-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec GetFieldworkCommunitySitesAndCommunityUsers 0 
CREATE PROCEDURE GetFieldworkCommunitySitesAndCommunityUsers
@CommunitySiteId bigint
AS
BEGIN
	if(@CommunitySiteId=0)
	begin
		select ID,[Name] as CommunitySite from FieldWork.communitysites
	end
else
	begin
		select u.ID,u.FirstName+' '+u.LastName as CommunitySiteUser from FieldWork.CommunitySiteUsers  csu
		join [User].[Users] u on u.ID=csu.UserID
		where CommunitySiteID=@CommunitySiteId
	end
END