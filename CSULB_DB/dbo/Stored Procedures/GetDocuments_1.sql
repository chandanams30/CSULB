-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec GetDocuments 25,16,0  -- for file merge 
--exec GetDocuments 25,0,1   -- for single file download
CREATE PROCEDURE GetDocuments
@UserID bigint,
@FormID bigint,
@FormAttachmentId bigint
AS
BEGIN
			select
		(case when CSULBID is null then CAST(ID as varchar(25))+FirstName+LastName else CAST(ID as varchar(25))+CSULBID+FirstName+LastName end) foldername 
		from [User].Users where ID=@UserID

if(@FormID>0)
Begin
		select * from [Application].[FormAttachments] where FormID=@FormID
end
else
begin 
		select * from [Application].[FormAttachments] where ID=@FormAttachmentId
end
END