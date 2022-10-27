CREATE PROCEDURE [dbo].[UpdateFieldWorkActivityLogStatus]
	@UserID BIGINT
	,@FieldWorkID BIGINT
	,@FieldWorkActivityLogID BIGINT
	,@Status varchar(50)  
AS
BEGIN
--@Status Values - Saved,Submitted,Approved,Not-Approved
UPDATE [FieldWork].[FieldWorkActivityLog]
   SET [Status] = @Status,
   [ModifiedDateTime]=GETDATE()
   ,[ModifiedBy]=@UserID
WHERE [ID]=@FieldWorkActivityLogID 
	AND [FieldWorkID] = @FieldWorkID

END