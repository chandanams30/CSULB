
-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2023-Mar-30
-- Description:	returns true if Interviewer Attachments Present
-- =============================================
-- select [dbo].[fnIsInterviewerAttachmentsPresent] (10255, 171)
-- =============================================
CREATE FUNCTION [dbo].[fnIsInterviewerAttachmentsPresent] 
(
	-- Add the parameters for the function here
	@FormID bigint,
	@InterviewerUserID bigint
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result bit

	IF EXISTS (SELECT * FROM [Application].[Interviewer] I JOIN [Application].[InterviewerAttachments] IA ON IA.[InterviewID] = I.[ID] WHERE I.[FormID] = @FormID
	AND I.[InterviewerUserID] = @InterviewerUserID AND IA.[FileExtn]  IS NOT NULL AND IA.[FileName] IS NOT NULL AND IA.[FolderName] IS NOT NULL) 
		BEGIN
			SELECT @Result = 1; 
		END
	ELSE
		BEGIN
			SELECT @Result = 0;
		END
	
	-- Return the result of the function
	RETURN @Result
END