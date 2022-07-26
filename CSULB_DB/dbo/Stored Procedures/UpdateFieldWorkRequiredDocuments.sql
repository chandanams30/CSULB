
-- exec UpdateFieldWorkRequiredDocuments 614,2999,'TBTEST.pdf','','2222TBTEST07262022103811'
CREATE PROCEDURE [dbo].[UpdateFieldWorkRequiredDocuments]
@UserId bigint,
@FieldWorkAttachmentID bigint,
@FileName varchar(250),
@FileExtn varchar(20),
@SavedFileName nvarchar(100)
AS
BEGIN
declare @folderName varchar(100);
set @FolderName=CAST(@UserId as varchar(25))+'~'+@SavedFileName;
	 --(select @folderName=CAST(ID as varchar(25))+trim(isnull(CSULBID,''))+TRIM(REPLACE(FirstName, ' ', '')) + TRIM(REPLACE(LastName, ' ', ''))
		
	 --from [User].Users where ID=@UserId)

	update [FieldWork].[Attachments]  set [FileName]=@FileName,FileExtn=@FileExtn,FolderName=@folderName, [IsApproved]=NULL, [ValidTill]=NULL,[ApprovedBy]=NULL, [ValidatedDate]=Null, [RejectedReason]=Null
	where ID=@FieldWorkAttachmentID;

	select * from [FieldWork].[Attachments] where ID=@FieldWorkAttachmentID;
END