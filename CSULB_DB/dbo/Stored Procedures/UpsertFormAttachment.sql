-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--EXEC 
--EXEC [dbo].[[UpsertFormAttachment]] 339,6,12,1,'2234','FileName','pdf','Saved File Name'
CREATE PROCEDURE [dbo].[UpsertFormAttachment]
@UserID BIGINT
,@FormID BIGINT
,@ProgramID BIGINT
,@DocumentID BIGINT
,@TermCode varchar(10)
,@FileName varchar(200)
,@FileExtn varchar(20)
,@SavedFileName nvarchar(100) 
AS
BEGIN
		DECLARE @ProgramDocumentID AS bigint
		SELECT @ProgramDocumentID = PD.[ID] FROM [Master].[ProgramDocuments] PD WHERE PD.[ProgramID]=@ProgramID AND PD.[DocumentID]=@DocumentID
		--SELECT @ProgramDocumentID

		DECLARE @folderName VARCHAR(100);
		--SET @FolderName = CAST(@UserId AS VARCHAR(25)) + '~' + @SavedFileName;
		SELECT @FolderName = CAST(UserID AS VARCHAR(25)) + '~' + @SavedFileName FROM [Application].[Forms] WHERE [ID] = @FormID;

		--CHECK IF [FormID] AND [ProgramDocumentID] EXISTS IN [Application].[FormAttachments]

		IF EXISTS (SELECT FA.[ID] FROM [Application].[FormAttachments] FA WHERE FA.[FormID] = @FormID AND FA.[ProgramDocumentID]=@ProgramDocumentID)
			BEGIN
				PRINT '[FormID] AND [ProgramDocumentID] EXISTS IN [Application].[FormAttachments]'
				-- UPDATE THE Document Attachment Details
				UPDATE [Application].[FormAttachments]
				   SET [FileName] = @FileName
					  ,[FileExtn] = @FileExtn
					  ,[FolderName] = @FolderName
				 WHERE [FormID] = @FormID
					  AND [ProgramDocumentID] = @ProgramDocumentID
			END
		ELSE
		BEGIN 
			PRINT 'NOT EXISTS IN [Application].[FormAttachments]'
			INSERT INTO [Application].[FormAttachments]
					   ([FormID]
					   ,[ProgramDocumentID]
					   ,[FileName]
					   ,[FileExtn]
					   ,[FolderName]
					   ,[CreatedBy]
					   ,[CreatedDate])
				 VALUES
					   (@FormID
					   ,@ProgramDocumentID
					   ,@FileName
					   ,@FileExtn
					   ,@FolderName
					   ,@UserID
					   ,GETDATE())
		END
		SELECT * FROM [Application].[FormAttachments] WHERE [FormID] = @FormID AND [ProgramDocumentID]=@ProgramDocumentID
END