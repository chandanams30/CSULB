-- =============================================
-- Author:		ThoughtFocus
-- Create date: 20-Mar-2023
-- Description:	Add/update Milestone Forms
-- =============================================
CREATE PROCEDURE [dbo].[AssignMilestoneProgram]
	@ProgramID bigint,
	@MilestoneID bigint,
	@TermCode varchar(10)

AS
BEGIN

IF NOT EXISTS (SELECT * FROM [Master].[ProgramMilestones] WHERE [ProgramID] =@ProgramID AND @MilestoneID=[MilestoneID] AND @TermCode = [TermCode])
	BEGIN
		INSERT INTO  [Master].[ProgramMilestones](
		[ProgramID],[MilestoneID],[TermCode])
		VALUES
		(@ProgramID, @MilestoneID,@TermCode)
	END
END