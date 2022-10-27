CREATE PROCEDURE [dbo].[UpdateFormMessageBoardSchema]
@UserID bigint,
@FormID bigint,
@ProgramID bigint,
@TermCode varchar(10),
@MessageBoardSchema nvarchar(max)
AS
BEGIN
	update [Application].[Forms] set [MessageBoard]=@MessageBoardSchema,[ModifiedDateTime]=GETDATE()
   ,[ModifiedBy]=@UserID WHERE [ID]=@FormID AND [ProgramID]=@ProgramID  AND TermCode=@TermCode --AND UserID=@UserID
END