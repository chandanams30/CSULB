-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2023-Jan-13
-- Description:	mail Outbox
-- =============================================
CREATE PROCEDURE [dbo].[getMailToSend] 
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--DECLARE @mailID bigint

--SELECT TOP 5 [mailID] FROM [dbo].[mailOutbox] WHERE [isMailSent] =0  ORDER BY [createdON] ASC;

SELECT TOP 5 MOB.[mailID]
      , MOB.[emailUserName]
      , MOB.[emailPassword]
      , MOB.[fromAddress]
      , MOB.[smtpSever]
      , MOB.[portNumber]
      , MOB.[enableSSL]
      , MOB.[toAddress]
      , MOB.[ccAddress]
      , MOB.[bccAddress]
      , MOB.[mailSubject]
      , MOB.[mailBody]
	  , MOBA.[attachmentHTML]
      --,[createdON]
      --,[isMailSent]
      --,[malSentON]
  FROM [dbo].[mailOutbox] MOB
  JOIN [dbo].[mailOutboxAttachments] MOBA ON MOBA.[mailID] = MOB.[mailID]
   WHERE [isMailSent] =0  ORDER BY [createdON] ASC
  --WHERE  MOB.[mailID] = @mailID

--SELECT [mailAttachmentID]
--      ,[attachmentHTML]
--  FROM [dbo].[mailOutboxAttachments]
--   WHERE [mailID] = @mailID


END