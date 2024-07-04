-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Dec-14
-- Description:	Add Instructor to Form
-- =============================================
CREATE PROCEDURE [dbo].[AddInstructorToForm]
@UserID BIGINT
,@FormID BIGINT
,@ProgramID BIGINT
,@TermCode varchar(10)
,@InstructorUserID bigint
,@isAssigned bit = 1

AS
BEGIN
	--SELECT * FROM [Application].[Instructor]
	--SELECT * FROM [Application].[InstructorAttachments]
IF NOT EXISTS (SELECT * FROM [Application].[Instructor] WHERE [FormID]=@FormID AND [InstructorUserID]=@InstructorUserID)
	BEGIN
		INSERT INTO [Application].[Instructor]
			   ([FormID]
			   ,[InstructorUserID])
		 VALUES
			   (@FormID
			   ,@InstructorUserID)

	 DECLARE @InstructionID AS BIGINT
		IF (@@ERROR = 0)
		BEGIN
			SELECT @InstructionID = @@IDENTITY; 
										
			INSERT INTO [Application].[InstructorAttachments]
				([InstructorID]
				,[DocumentID])
				VALUES
				(@InstructionID,26) --26	EDSS 300 Instructor Assessment
		END
	END
ELSE 
	BEGIN
		UPDATE [Application].[Instructor] SET [isAssigned]=@isAssigned WHERE [FormID] = @FormID AND [InstructorUserID]=@InstructorUserID
	END

END