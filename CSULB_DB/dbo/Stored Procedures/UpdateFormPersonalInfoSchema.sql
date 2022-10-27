CREATE PROCEDURE [dbo].[UpdateFormPersonalInfoSchema]
@UserID bigint,
@FormID bigint,
@ProgramID bigint,
@TermCode varchar(10),
@FormSchema nvarchar(max)
AS
BEGIN
	UPDATE [Application].[Forms] SET 
	[Form]=@FormSchema
	,[ModifiedDateTime]=GETDATE()
   ,[ModifiedBy]=@UserID 
   --,[FormStateID] = 2
   WHERE [ID]=@FormID AND [ProgramID]=@ProgramID AND TermCode=@TermCode
END