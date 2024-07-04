-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2023-Mar-30
-- Description:	returns true if Instructor Attachments Present
-- =============================================
-- select [dbo].[fnIsInstructorAttachmentsPresent] (10255, 171)
-- =============================================
CREATE FUNCTION [dbo].[fnIsInstructorAttachmentsPresent] 
(
	-- Add the parameters for the function here
	@FormID bigint,
	@InstructorUserID bigint
)
RETURNS bit
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result bit

	IF EXISTS (SELECT * FROM [Application].[Instructor] I JOIN [Application].[InstructorAttachments] IA ON IA.[InstructorID] = I.[ID] WHERE I.[FormID] = @FormID
	AND I.[InstructorUserID] = @InstructorUserID AND IA.[FileExtn]  IS NOT NULL AND IA.[FileName] IS NOT NULL AND IA.[FolderName] IS NOT NULL) 
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