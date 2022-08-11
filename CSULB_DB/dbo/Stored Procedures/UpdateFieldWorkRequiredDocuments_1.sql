CREATE PROCEDURE [dbo].[UpdateFieldWorkRequiredDocuments]
@UserId bigint,
@FieldWorkAttachmentID bigint,
@FileName varchar(250),
@FileExtn varchar(20),
@SavedFileName nvarchar(100),
@ValidTill datetime,
@Comments nvarchar(Max)
AS
BEGIN
		declare @folderName varchar(100);
		set @FolderName=CAST(@UserId as varchar(25))+'~'+@SavedFileName;
	 --(select @folderName=CAST(ID as varchar(25))+trim(isnull(CSULBID,''))+TRIM(REPLACE(FirstName, ' ', '')) + TRIM(REPLACE(LastName, ' ', ''))
		
		--from [User].Users where ID=@UserId)

	update [FieldWork].[Attachments]  set [FileName]=@FileName,FileExtn=@FileExtn,FolderName=@folderName, 
	[IsApproved]=NULL,[ApprovedBy]=NULL, [ValidatedDate]=Null, [RejectedReason]=Null,
	[ValidTill]=@ValidTill,[Comments]=@Comments
	where ID=@FieldWorkAttachmentID;

	select * from [FieldWork].[Attachments] where ID=@FieldWorkAttachmentID;
END