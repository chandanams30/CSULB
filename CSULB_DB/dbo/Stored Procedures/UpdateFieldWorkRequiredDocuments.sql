
-- exec UpdateFieldWorkRequiredDocuments 1545,2586,'',''
CREATE PROCEDURE UpdateFieldWorkRequiredDocuments
@UserId bigint,
@FieldWorkAttachmentID bigint,
@FileName varchar(250),
@FileExtn varchar(20)
AS
BEGIN
declare @folderName varchar(100);

	 (select @folderName=CAST(ID as varchar(25))+trim(isnull(CSULBID,''))+TRIM(REPLACE(FirstName, ' ', '')) + TRIM(REPLACE(LastName, ' ', ''))
		
		from [User].Users where ID=@UserId)

	update [FieldWork].[Attachments]  set [FileName]=@FileName,FileExtn=@FileExtn,FolderName=@folderName
	where ID=@FieldWorkAttachmentID;

	select * from [FieldWork].[Attachments] where ID=@FieldWorkAttachmentID;
END