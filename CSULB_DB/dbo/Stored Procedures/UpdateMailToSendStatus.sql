-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2023-Jan-13
-- Description:	Update mail Outbox sent status
-- =============================================
CREATE PROCEDURE UpdateMailToSendStatus
	-- Add the parameters for the stored procedure here
	@mailID BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- UPDATE statements for procedure here
	UPDATE [dbo].[mailOutbox]
	   SET [isMailSent] = 1
		  ,[malSentON] = GETDATE()
	 WHERE [mailID] = @mailID

END