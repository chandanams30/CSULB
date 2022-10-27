CREATE PROCEDURE [UpdateMessageBoardSchema_old]
@UserID bigint,
@ProgramID bigint,
@TermCode varchar(10),
@FormSchema nvarchar(max)
AS
BEGIN
	update [Application].[Forms] set [MessageBoard]=@FormSchema WHERE [ProgramID]=@ProgramID AND UserID=@UserID AND TermCode=@TermCode
END