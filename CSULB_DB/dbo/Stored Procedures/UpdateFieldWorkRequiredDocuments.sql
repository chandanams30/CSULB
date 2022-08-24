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

	DECLARE @folderName VARCHAR(100);
	SET @FolderName = CAST(@UserId AS VARCHAR(25)) + '~' + @SavedFileName;

	IF (@SavedFileName IS NULL OR @SavedFileName = '')
	BEGIN
		UPDATE [FieldWork].[Attachments]
		SET [ValidTill] = @ValidTill
			,[Comments] = @Comments
		WHERE ID = @FieldWorkAttachmentID;
	END
	ELSE
	BEGIN
		UPDATE [FieldWork].[Attachments]
		SET [FileName] = @FileName
			,FileExtn = @FileExtn
			,FolderName = @folderName
			,[IsApproved] = NULL
			,[ApprovedBy] = NULL
			,[ValidatedDate] = NULL
			,[RejectedReason] = NULL
			,[ValidTill] = @ValidTill
			,[Comments] = NULL
		WHERE ID = @FieldWorkAttachmentID;
	END

	SELECT *  FROM [FieldWork].[Attachments] WHERE ID = @FieldWorkAttachmentID;
END