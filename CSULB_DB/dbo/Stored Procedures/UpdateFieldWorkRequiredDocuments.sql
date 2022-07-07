
-- exec UpdateFieldWorkRequiredDocuments 25,0,'',''
CREATE PROCEDURE UpdateFieldWorkRequiredDocuments
@UserId bigint,
@FieldWorkAttachmentID bigint,
@FileName varchar(250),
@FileExtn varchar(20)
AS
BEGIN
declare @folderName varchar(100);
set @folderName= (select 
		(case when CSULBID is null then CAST(ID as varchar(25))+FirstName+LastName else CAST(ID as varchar(25))+CSULBID+FirstName+LastName end) foldername 
		from [User].Users where ID=@UserId)

	update [FieldWork].[Attachments]  set [FileName]=@FileName,FileExtn=@FileExtn,FolderName=@folderName
	where ID=@FieldWorkAttachmentID;

	select * from [FieldWork].[Attachments] where ID=@FieldWorkAttachmentID;
END