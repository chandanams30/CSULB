-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Dec-14
-- Description:	Returns Interview Attachment
-- =============================================
--EXEC [dbo].[GetInterviewAttachments] 1,1
CREATE PROCEDURE [dbo].[GetInterviewAttachments]
@UserID bigint,
@InterviewAttachmentID bigint
AS
BEGIN

SELECT [ID]
      ,[InterviewID]
      ,[DocumentID]
      ,[FileName]
      ,[FileExtn]
      ,[FolderName]
      ,[UploadedDate]
  FROM [Application].[InterviewerAttachments]
  WHERE [ID]=@InterviewAttachmentID

END