-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE GetProgramList
@ApplicationTypeId int,
@SemesterId int,
@StateId int
AS
BEGIN

	--SET NOCOUNT ON;
	select f.ProgramID,p.Name,count(f.ID) [ApplicationCount],count(case when f.State=@StateId then 1 end) [OfferedCount] from [Application].[Forms] as f 
	join [Master].[Programs] p on p.ID=f.ProgramID
	where p.ApplicationTypesID=@ApplicationTypeId and f.SemesterID=@SemesterId
	group by f.ProgramID,p.Name
END