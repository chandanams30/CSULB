-- exec GetFormAttachment 338,15
CREATE PROCEDURE GetFormAttachment
@UserID bigint,
@FormAttachmentID bigint
AS
BEGIN
	select * from [Application].[FormAttachments] where [ID]=@FormAttachmentID
END