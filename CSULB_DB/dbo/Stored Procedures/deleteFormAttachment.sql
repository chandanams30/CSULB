-- =============================================
-- Author:		ThoughtFocus
-- Create date: 10-Jan-2023
-- Description:	updates attachemnt detials to NULL
-- =============================================
CREATE PROCEDURE [dbo].[deleteFormAttachment]
@UserID BIGINT
,@FormID BIGINT
,@ProgramID BIGINT
,@DocumentID BIGINT
,@TermCode varchar(10)
,@FormAttachmentID BIGINT
--,@FileName varchar(200)
--,@FileExtn varchar(20)
--,@SavedFileName nvarchar(100) 

AS
BEGIN

DECLARE @ProgramDocumentID AS bigint
SELECT @ProgramDocumentID = PD.[ID] FROM [Master].[ProgramDocuments] PD WHERE PD.[ProgramID]=@ProgramID AND PD.[DocumentID]=@DocumentID

-- UPDATE THE Document Attachment Details
UPDATE [Application].[FormAttachments]
	SET [FileName] = NULL
		,[FileExtn] = NULL
		,[FolderName] = NULL
	WHERE [FormID] = @FormID
		AND [ProgramDocumentID] = @ProgramDocumentID
		AND [ID] = @FormAttachmentID
			
SELECT * FROM [Application].[FormAttachments] WHERE [FormID] = @FormID AND [ProgramDocumentID]=@ProgramDocumentID AND [ID] = @FormAttachmentID;

END