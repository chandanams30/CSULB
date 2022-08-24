--exec GetFieldWorkAttachedFileName 942
CREATE PROCEDURE GetFieldWorkAttachedFileName 
@FieldWorkAttachmentID bigint
AS
BEGIN
	select FolderName,fileextn from FieldWork.Attachments where ID=@FieldWorkAttachmentID
END