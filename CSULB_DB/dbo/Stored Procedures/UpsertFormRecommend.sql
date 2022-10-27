-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
/*
DECLARE	@return_value int

EXEC	@return_value = [dbo].[UpsertFormRecommend]
		@UserID = 339,
		@FormID = 8,
		@ProgramID = 14,
		@TermCode = N'2234',
		@RecommenderName = N'Recommender Name',
		@RecommenderEmail = N'RecommenderEmail@test.Name',
		@DocumentID =  NULL,
		@RecommenderIdentifier = NULL,
		@FileName = NULL,
		@FileExtn =  NULL,
		@SavedFileName =  NULL
SELECT	'Return Value' = @return_value
GO

DECLARE	@return_value int
EXEC	@return_value = [dbo].[UpsertFormRecommend]
		@UserID = 339,
		@FormID = 8,
		@ProgramID = 14,
		@TermCode = N'2234',
		@RecommenderName = N'Recommender Name',
		@RecommenderEmail = N'RecommenderEmail@test.Name',
		@DocumentID =  3,
		@RecommenderIdentifier = 'F5B67BC0-AB2A-429D-8A49-DD0363C61DD2',
		@FileName = 'FileName2',
		@FileExtn =  'pdf2',
		@SavedFileName =  '2SavedFileName'

SELECT	'Return Value' = @return_value
GO

SELECT * FROM [Application].[Recommendations]
SELECT * FROM [Application].[RecommendationAttachments]
SELECT * FROM [Application].[Forms]
*/

CREATE PROCEDURE [dbo].[UpsertFormRecommend]
	@UserID BIGINT
	,@FormID BIGINT
	,@ProgramID BIGINT
	,@TermCode varchar(10) 
	,@RecommenderName varchar(200)
	,@RecommenderEmail  varchar(200) 
	,@DocumentID BIGINT
	,@RecommenderIdentifier uniqueidentifier 
	,@FileName varchar(250)
	,@FileExtn varchar(20)
	,@SavedFileName nvarchar(100)
AS
BEGIN
	IF ((@RecommenderIdentifier IS NOT NULL) AND (@FileName IS NOT NULL) AND (@FileExtn IS NOT NULL) AND (@SavedFileName IS NOT NULL) AND (@DocumentID IS NOT NULL))
		BEGIN
			PRINT '[RecommenderIdentifier] IS NOT NULL - enter attachments'
			DECLARE @folderName VARCHAR(100);
			SELECT @FolderName = CAST(F.[UserID] AS VARCHAR(25)) + '~' + @SavedFileName FROM [Application].[Forms] F
						JOIN [Application].[Recommendations] R ON R.[FormID] = F.[ID]
					WHERE R.[RecommenderIdentifier]=@RecommenderIdentifier

					SELECT @FolderName
			--SET @FolderName = CAST(@UserId AS VARCHAR(25)) + '~' + @SavedFileName;

			IF EXISTS (SELECT * FROM [Application].[RecommendationAttachments] RA JOIN [Application].[Recommendations] R ON R.[ID] = RA.[RecomendationID] WHERE RA.[DocumentID] = @DocumentID AND R.[RecommenderIdentifier]=@RecommenderIdentifier)
				BEGIN
				--WHERE RA.[DocumentID] = 3 AND R.[RecommenderIdentifier]='8427B305-E32A-450B-B135-945676CF8A2A' AND R.[RecommenderEmail] ='RecommenderEmail@test.Name'
				-- UPDATE THE Document Attachment Details
				PRINT '[RecommenderIdentifier] IS NOT NULL [Application].[RecommendationAttachments] UPDATES SAVED FILE DETAILS'
				UPDATE RA 
					SET 
						RA.[FileName] = @FileName
						,RA.[FileExtn] = @FileExtn
						,RA.[FolderName] = @FolderName
						,RA.[UploadedDate] = GETDATE()
					FROM [Application].[RecommendationAttachments]  RA
						JOIN [Application].[Recommendations] R ON R.[ID] = RA.[RecomendationID]
					WHERE RA.[DocumentID] = @DocumentID AND R.[RecommenderIdentifier]=@RecommenderIdentifier --AND R.[RecommenderEmail] = @RecommenderEmail
				END
			ELSE
				BEGIN
					PRINT 'Insert attachments'
					INSERT INTO [Application].[RecommendationAttachments]
					   ([RecomendationID]
					   ,[DocumentID]
					   ,[FileName]
					   ,[FileExtn]
					   ,[FolderName]
					   ,[UploadedDate])
					   (SELECT R.[ID], @DocumentID, @FileName, @FileExtn, @FolderName, GETDATE() FROM [Application].[Recommendations] R
							WHERE R.[RecommenderIdentifier]=@RecommenderIdentifier) --AND R.[RecommenderEmail] = @RecommenderEmail)
			   END
		END

IF (ISNULL(@FormID,0)<>0) AND (NOT EXISTS (SELECT R.[ID] FROM [Application].[Recommendations] R WHERE R.[RecommenderEmail] = @RecommenderEmail AND R.[FormID]=ISNULL(@FormID,0)))
	BEGIN 
		PRINT 'Recommendor NOT EXISTS IN [Application].[Recommendations] - insert as new Recommendor'

		INSERT INTO [Application].[Recommendations]
			([FormID]
			,[RecommenderName]
			,[RecommenderEmail]
			,[CreatedBy]
			,[CreatedDate]
			,[RecommenderURL]
			,[RecommenderURLValidTill]
			,[AllowUpload]
			,[RecommenderIdentifier])
		VALUES
			(@FormID
			,@RecommenderName
			,@RecommenderEmail
			,@UserID
			,GETDATE()
			,'http://180.151.61.78:7852/csulb/#/recommendation/'
			,GETDATE() + 30
			,1
			,NEWID())

		DECLARE @newRecomendationID bigint = NULL
		IF (@@ERROR = 0)
			BEGIN
				SELECT @newRecomendationID = @@IDENTITY; 
			END
			
		--INSERT INTO [Application].[RecommendationAttachments]
		--	([RecomendationID]
		--	,[DocumentID])
		--VALUES
		--(@newRecomendationID,3), (@newRecomendationID,18)

		SELECT R.[ID]
			  ,[FormID]
			  ,[RecommenderName]
			  ,[RecommenderEmail]
			  --,[CreatedBy]
			  --,[CreatedDate]
			  ,[RecommenderURL] +  CONVERT(NVARCHAR(50),[RecommenderIdentifier]) AS [RecommenderURL] 
			  ,[RecommenderURLValidTill]
			  ,u.[FirstName] + ' ' + u.[LastName] AS [ApplicantName]
			  ,PAD.[ApplicationDeadline]
			  ,PC.[RecommenderMailBody] AS [MailBody]
			  --,[AllowUpload]
			  --,[RecommenderIdentifier]
		  FROM [Application].[Recommendations] R
		  JOIN [Application].[Forms] F ON F.[ID] = R.[FormID]
		  JOIN [User].[Users] U ON U.ID = F.[UserID] 
		  JOIN [Master].[ProgramApplicationDates] PAD ON PAD.[ProgramID] = F.[ProgramID] AND PAD.[TermCode] = F.[TermCode]
		  JOIN [Master].[ProgramConfigurations] PC ON PC.[ProgramID]=F.[ProgramID]
		  WHERE R.[ID]=@newRecomendationID

	END	



   
END