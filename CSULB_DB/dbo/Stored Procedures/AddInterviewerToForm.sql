-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Dec-14
-- Description:	Add  Interviewer to Form
-- =============================================
CREATE PROCEDURE [dbo].[AddInterviewerToForm]
@UserID BIGINT
,@FormID BIGINT
,@ProgramID BIGINT
,@TermCode varchar(10)
,@InterviewerUserID bigint
,@isAssigned bit = 1
AS
BEGIN
	--SELECT * FROM [Application].[Interviewer]
	--SELECT * FROM [Application].[InterviewerAttachments]
IF NOT EXISTS (SELECT * FROM [Application].[Interviewer] WHERE [FormID]=@FormID AND [InterviewerUserID]=@InterviewerUserID)
	BEGIN
		INSERT INTO [Application].[Interviewer] ([FormID],[InterviewerUserID])
		VALUES (@FormID,@InterviewerUserID)

		DECLARE @InterviewID AS BIGINT

		IF (@@ERROR = 0)
			BEGIN
				SELECT @InterviewID = @@IDENTITY;

				INSERT INTO [Application].[InterviewerAttachments]([InterviewID],[DocumentID])
				VALUES(@InterviewID,27) --27	Interview Rating Sheet
			END
	END
ELSE 
	BEGIN
		UPDATE [Application].[Interviewer] SET [isAssigned]=@isAssigned WHERE [FormID] = @FormID AND [InterviewerUserID]=@InterviewerUserID
	END

END