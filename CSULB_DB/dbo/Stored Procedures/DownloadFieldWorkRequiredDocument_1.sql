-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DownloadFieldWorkRequiredDocument]
@UserID bigint,
@FieldWorkAttachmentId bigint

AS
BEGIN

select * from [FieldWork].[Attachments] where ID=@FieldWorkAttachmentId and UserID=@UserID

END