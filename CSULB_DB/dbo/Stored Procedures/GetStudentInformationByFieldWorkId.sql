-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
-- exec GetStudentInformationByFieldWorkId 224,3301
CREATE PROCEDURE GetStudentInformationByFieldWorkId 
@FieldWorkID bigint,
@FieldWorkAttachmentID bigint
AS
BEGIN
	select 

	u.ID UserId,
	u.FirstName,
	u.LastName,
	u.DisplayName,
	--u.Email,
	'asif.khan@thoughtfocus.com' Email,
	u.CSULBID,
	fa.[FileName],
	fa.FileExtn
	
	from [FieldWork].[FieldWork] fw
	join [user].[users] u on u.id=fw.UserID
	join [FieldWork].[Attachments] fa on fa.UserID=u.ID 
	where fw.id=@FieldWorkID and fa.ID=@FieldWorkAttachmentID
END