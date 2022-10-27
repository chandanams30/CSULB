CREATE PROCEDURE [dbo].[GetFormRecommendations]
@UserID bigint,
@RecommendationttachmentID bigint
AS
BEGIN
	select * from [Application].[RecommendationAttachments] where [ID]=@RecommendationttachmentID
END