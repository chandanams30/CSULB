CREATE PROCEDURE [dbo].[GetApplicationNumber]
@ProgramId int,
@SemesterId int,
@Message nvarchar(100) output
AS
BEGIN

BEGIN
if not exists(select * from [Application].[ApplicationNumberFactory] where ProgramID=@ProgramId and SemesterID=@SemesterId) 
begin


INSERT INTO [Application].[ApplicationNumberFactory] VALUES (@programid,@SemesterId,1)

end
else
begin
	update [Application].[ApplicationNumberFactory] set [ApplicationNumber]=[ApplicationNumber]+1 where ProgramID=@ProgramId and SemesterID=@SemesterId
end
	-- Declare the return variable here
	--DECLARE @ApplicationNumber nvarchar(100);
	DECLARE @ProgramAlias nvarchar(10);
	DECLARE @MaxApplicationNumber int;
	DECLARE @SemesterNumber nvarchar(5);
	DECLARE @CurrentYear nvarchar(4);

	select @ProgramAlias=ShortName from [Master].[Programs] where ID=@ProgramId;
	select @SemesterNumber=RIGHT('00'+cast(@SemesterId as varchar(3)),5);
	select @MaxApplicationNumber=[ApplicationNumber] from [Application].[ApplicationNumberFactory] where ProgramID=@ProgramId and SemesterID=@SemesterId 
	select @CurrentYear=[Year] from [Master].[Semester] where ID=@SemesterId;

	select  @Message=CONCAT(@ProgramAlias,RIGHT('000000000'+cast(@MaxApplicationNumber as varchar(3)),12),@SemesterNumber,@CurrentYear);


END

END