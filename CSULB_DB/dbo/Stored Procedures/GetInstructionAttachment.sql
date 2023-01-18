
-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Dec-14
-- Description:	Returns Instructor Attachment
-- =============================================
--EXEC [dbo].[GetInstructorAttachment] 1,1
CREATE PROCEDURE [dbo].[GetInstructionAttachment]
@UserID bigint,
@InstructionAttachmentID bigint
AS
BEGIN
	SELECT [ID]
      ,[InstructorID]
      ,[DocumentID]
      ,[FileName]
      ,[FileExtn]
      ,[FolderName]
      ,[UploadedDate]
  FROM [Application].[InstructorAttachments] 
  WHERE [ID]=@InstructionAttachmentID

END