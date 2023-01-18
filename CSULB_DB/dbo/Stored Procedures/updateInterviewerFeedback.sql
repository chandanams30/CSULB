-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Dec-14
-- Description:	update Interviewer Feedback
-- =============================================
CREATE PROCEDURE [dbo].[updateInterviewerFeedback]
@InterviewID bigint
,@InterviewerUserID bigint
,@FormID bigint
,@InterviewAttachmentID bigint
,@FileName varchar(200)
,@FileExtn varchar(20)
,@SavedFileName nvarchar(100) 

AS
BEGIN
	--SELECT * FROM [Application].[Interviewer]
	--SELECT * FROM [Application].[InterviewerAttachments]

	DECLARE @UserID BIGINT

	SELECT @UserID = F.[UserID] FROM [Application].[Forms] F WHERE F.[ID] = @FormID;

	DECLARE @folderName VARCHAR(100);
	SET @FolderName = CAST(@UserId AS VARCHAR(25)) + '~' + @SavedFileName;

	UPDATE [Application].[InterviewerAttachments]
	   SET 
		  [FileName] = @FileName
		  ,[FileExtn] = @FileExtn
		  ,[FolderName] = @FolderName
		  ,[UploadedDate] = GETDATE()
	 WHERE [InterviewID] = @InterviewID
	 AND [ID] = @InterviewAttachmentID;

	 SELECT * FROM [Application].[InterviewerAttachments] WHERE [ID]=@InterviewID
END