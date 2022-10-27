
CREATE PROCEDURE [dbo].[UpdatePersonalInfoSchema_old]
@UserID bigint,
@ProgramID bigint,
@TermCode varchar(10),
@FormSchema nvarchar(max)
AS
BEGIN
	UPDATE [Application].[Forms] set [Form]=@FormSchema, [FormStateID]=2 WHERE [ProgramID]=@ProgramID AND UserID=@UserID AND TermCode=@TermCode
END