
CREATE PROCEDURE [dbo].[UpdateReviewerReview]
	@FormID BIGINT
	,@ReviewID BIGINT
	,@ReviewerID BIGINT
	,@ReviewerRecommendation nvarchar(200) 
	,@ReviewerComments nvarchar(2000) 
AS
BEGIN


UPDATE [Application].[Reviewer]
   SET 
      [ReviewerRecommendation] = @ReviewerRecommendation
      ,[ReviewerComments] = @ReviewerComments
      ,[ReviewedOn] = GETDATE()
 WHERE [FormID] = @FormID
      and [ReviewerID] = @ReviewerID and [ID]=@ReviewID

END