-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2023-Feb-08
-- Description:	Add Reviewer to Form
-- =============================================
CREATE PROCEDURE [dbo].[AddReviewerToForm] 
	@UserID BIGINT
	,@FormID BIGINT
	,@ProgramID BIGINT
	,@TermCode VARCHAR(10)
	,@ReviewerID BIGINT
AS
BEGIN
	--SELECT * FROM [Application].[Reviewer];
	
	IF NOT EXISTS (SELECT [ID] FROM [Application].[Reviewer] WHERE [FormID] = @FormID AND [ReviewerID]=@ReviewerID)
	BEGIN
		INSERT INTO [Application].[Reviewer] (
			[ReviewerID]
			,[FormID]
			,[CreatedBy]
			,[CreatedDate]
			)
		VALUES (
			@ReviewerID
			,@FormID
			,@UserID
			,GETDATE()
			)
	END
	ELSE IF EXISTS (SELECT [ID] FROM [Application].[Reviewer] WHERE [FormID] = @FormID AND [ReviewerID]=@ReviewerID AND [isAssigned]=0)
	BEGIN
		UPDATE [Application].[Reviewer] SET [isAssigned]=1 WHERE [FormID] = @FormID AND [ReviewerID]=@ReviewerID AND [isAssigned]=0
	END
END