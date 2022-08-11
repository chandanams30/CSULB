
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec UploadFieldWorkActivityLogAttachments 1,'myfile','pdf','1myfile07222022124233.pdf'
CREATE PROCEDURE UploadFieldWorkActivityLogAttachments
@UserId bigint,
@FileName nvarchar(100),
@FileExtn nvarchar(20),
@SavedFileName nvarchar(100)
AS
BEGIN
	declare @FolderName nvarchar(100);
	declare @FieldWorkAttachmentId bigint;
	set @FolderName=CAST(@UserId as varchar(25))+'~'+@SavedFileName;

	INSERT INTO [FieldWork].[Attachments]
           ([UserID]
           ,[DocumentID]
           ,[FileName]
           ,[FileExtn]
           ,[FolderName]
           ,[CreatedBy]
           ,[CreatedDate])
     VALUES
           (@UserId
		   ,9
		   ,@FileName
		   ,@FileExtn
		   ,@FolderName
		   ,@UserId
		   ,GETDATE())
		   
			set @FieldWorkAttachmentId=@@IDENTITY;

			select * from [FieldWork].[Attachments] where ID=@FieldWorkAttachmentId;
END