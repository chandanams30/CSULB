-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE DownloadFieldWorkRequiredDocument
@FieldWorkAttachmentId bigint
AS
BEGIN

select * from [FieldWork].[Attachments] where ID=@FieldWorkAttachmentId

END