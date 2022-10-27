CREATE PROCEDURE [dbo].[UpdateFormState]
	@UserID BIGINT
	,@FormID BIGINT
	,@ProgramID BIGINT
	,@TermCode varchar(10) 
	,@FormStateID INT
AS
BEGIN
--select * from [Application].[Forms]

UPDATE [Application].[Forms]
   SET [FormStateID] = CASE WHEN [FormStateID] = 4 THEN 5 ELSE @FormStateID END,
   [ModifiedDateTime]=GETDATE()
   ,[ModifiedBy]=@UserID
WHERE [ID]=@FormID 
	AND [ProgramID] = @ProgramID
    AND [TermCode] = @TermCode
END