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

AS
BEGIN
	--SELECT * FROM [Application].[Instructor]
	--SELECT * FROM [Application].[InstructorAttachments]

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