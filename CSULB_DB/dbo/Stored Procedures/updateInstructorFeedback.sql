-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Dec-14
-- Description:	update Instructor Feedback
-- =============================================
CREATE PROCEDURE [dbo].[updateInstructorFeedback]
@InstructionID bigint
,@InstructorUserID bigint
,@FormID bigint
,@InstructionAttachmentID bigint
,@FileName varchar(200)
,@FileExtn varchar(20)
,@SavedFileName nvarchar(100) 

AS
BEGIN
	--SELECT * FROM [Application].[Instructor]
	--SELECT * FROM [Application].[InstructorAttachments]0

	DECLARE @UserID BIGINT

	SELECT @UserID = F.[UserID] FROM [Application].[Forms] F WHERE F.[ID] = @FormID;

	DECLARE @folderName VARCHAR(100);
	SET @FolderName = CAST(@UserId AS VARCHAR(25)) + '~' + @SavedFileName;

	UPDATE [Application].[InstructorAttachments]
	   SET 
		  [FileName] = @FileName
		  ,[FileExtn] = @FileExtn
		  ,[FolderName] = @FolderName
		  ,[UploadedDate] = GETDATE()
	 WHERE [InstructorID] = @InstructionID
	 AND [ID] = @InstructionAttachmentID;

	  SELECT * FROM [Application].[InstructorAttachments] WHERE [ID]=@InstructionAttachmentID

END