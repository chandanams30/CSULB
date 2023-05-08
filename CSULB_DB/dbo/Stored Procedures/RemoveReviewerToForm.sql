-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2023-Feb-08
-- Description:	Add Reviewer to Form
-- =============================================
CREATE PROCEDURE [dbo].[RemoveReviewerToForm] 
	@UserID BIGINT
	,@FormID BIGINT
	,@ProgramID BIGINT
	,@TermCode VARCHAR(10)
	,@ReviewerID BIGINT
AS
BEGIN
	--SELECT * FROM [Application].[Reviewer];
	UPDATE [Application].[Reviewer] SET [isAssigned]=0 WHERE [FormID] = @FormID AND [ReviewerID]=@ReviewerID AND [isAssigned]=1

END