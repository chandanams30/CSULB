CREATE PROCEDURE [dbo].[AutharizeRecommender]
	@RecommenderIdentifier uniqueidentifier
AS
BEGIN
   
  SELECT(
				SELECT R.[ID] AS [RecommendationID]
					--,R.[FormID]
					,U.FirstName + ' ' + U.LastName AS [StudentName]
					,R.[RecommenderName]
					--,R.[RecommenderEmail]
					--,R.[CreatedBy]
					--,R.[CreatedDate]
					--,R.[RecommenderURL]
					--,R.[RecommenderURLValidTill]
					,R.[AllowUpload]
					--,R.[RecommenderIdentifier]
					,JSON_QUERY((
							SELECT 
								--RA.[ID] AS [RecomendationAttachmentID],
								CASE WHEN D.[ID]=3 THEN 'Completed Recommendation Form (Attached in Email)' ELSE D.[Name] END AS [DocumentName]
								,D.[ID] AS [DocumentID]
								,RA.[FileName]
								,RA.[FileExtn]
								--,RA.[FolderName]
								,RA.[UploadedDate]
							FROM [Master].[Documents] D
							LEFT JOIN [Application].[RecommendationAttachments] RA ON D.[ID] = RA.[DocumentID] AND RA.[RecomendationID] = R.[ID]
							WHERE D.[ID] IN (3,18)
							FOR JSON PATH,INCLUDE_NULL_VALUES
							)) AS [Attachments]
				FROM [Application].[Recommendations] R
				JOIN [Application].[Forms] F ON F.[ID] = R.[FormID]
				JOIN [User].[Users] U ON U.[ID] = F.[UserID] 
				WHERE R.[RecommenderIdentifier] = @RecommenderIdentifier
				FOR JSON PATH) AS [Recommendations];


END

/* 
SELECT * from [Application].[Recommendations] R


exec [dbo].[AutharizeRecommender] 'E67B96FE-ADFA-4A69-A256-6470CB0D1B5'
*/