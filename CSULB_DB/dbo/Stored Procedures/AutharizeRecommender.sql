-- =============================================
-- Author:		ThoughtFocus
-- Create date: 09/19/2022
-- Description:	Autharize Recommender by RecommenderIdentifier
-- =============================================

CREATE PROCEDURE [dbo].[AutharizeRecommender]
	@RecommenderIdentifier uniqueidentifier
AS
BEGIN
   
  SELECT(
				SELECT R.[ID] AS [RecommendationID]
					,U.FirstName + ' ' + U.LastName AS [StudentName]
					,U.FirstName AS [StudentFirstName]
					,U.LastName AS [StudentLastName]
					,U.[CSULBID]
					,U.[Email] AS [StudentEmail]
					,R.[RecommenderName]
					,R.[AllowUpload]
					,P.[ProgramFormIdentifier]
					,R.[LetterOfRecommendationJSON]
					,P.[Name] AS [ProgramName]
					,T.[Name] AS [TermName]
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
				JOIN [Master].[Programs] P ON P.[ID] = F.[ProgramID]
				JOIN [Master].[Term] T ON T.[TermCode] = F.[TermCode]
				JOIN [User].[Users] U ON U.[ID] = F.[UserID] 
				WHERE R.[RecommenderIdentifier] = @RecommenderIdentifier
				--and 1 = case when @RecommenderIdentifier = '4C8FABD2-6D9B-47FB-8341-A198C87B2308' then 0 else 1 end
				FOR JSON PATH,INCLUDE_NULL_VALUES) AS [Recommendations];


END

/* 
SELECT * from [Application].[Recommendations] R




exec [dbo].[AutharizeRecommender] '2C643C51-34B6-4470-836B-20773C8C2DC7'
*/