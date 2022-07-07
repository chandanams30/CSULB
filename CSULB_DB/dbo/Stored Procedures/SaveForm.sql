-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SaveForm]
@FormId int,
@UserId int ,
@ProgramId int,
@SemesterId int,
@Form nvarchar(max),
@State int,
@ApplicationNumber nvarchar(100)
AS
if (@FormId=0)
begin
INSERT INTO [Application].[Forms]
           ([UserID]
           ,[ProgramID]
           ,[SemesterID]
           ,[Form]
           ,[CreatedDateTime]
           ,[CreatedByUserID]
           ,[ModifiedDateTime]
           ,[ModifiedBy]
           ,[State]
		   ,[ApplicationNumber])
     VALUES
           (@UserId
		   ,@ProgramId
		   ,@SemesterId
		   ,@Form
		   ,GETDATE()
		   ,@UserId
		   ,GETDATE()
		   ,@UserId
		   ,@State
		   ,@ApplicationNumber)

end
else
begin 
		update [Application].[Forms] set [Form]=@Form , [State]=@State , [ModifiedDateTime]=GETDATE(), [ModifiedBy]=@UserId
	where [ID]=@FormId

end