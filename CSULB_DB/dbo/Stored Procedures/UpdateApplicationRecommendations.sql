-- =============================================
-- Author:		ThoughtFocus
-- Create date: 11/02/2022
-- Description:	To update LetterOfRecommendationJSON for Recommendations
-- =============================================
CREATE PROCEDURE [dbo].[UpdateApplicationRecommendations]
	@FormID BIGINT
	,@RecommenderIdentifier uniqueidentifier 
	,@LetterOfRecommendationJSON varchar(MAX)
AS
BEGIN
	UPDATE [Application].[Recommendations]
	   SET [LetterOfRecommendationJSON] = @LetterOfRecommendationJSON
	 WHERE [FormID] = @formid
		AND [RecommenderIdentifier] = @RecommenderIdentifier
END