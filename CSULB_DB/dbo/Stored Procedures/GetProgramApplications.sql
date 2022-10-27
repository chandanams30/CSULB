--exec GetProgramApplications 2,10,1
CREATE PROCEDURE [dbo].[GetProgramApplications]
@ProgramID bigint,
@StateID bigint,
@SemesterID bigint
AS
BEGIN
		SELECT * FROM [Application].[Forms]
	--select f.ID FormId,p.Description ProgramName,s.[Description] as SemesterName,f.ApplicationNumber,u.FirstName+''+u.LastName as ApplicantName 
	--	,isnull(u.CSULBID,'XXXXXXXXXXXXX') CSULBID 
	--	,us.FirstName+' '+us.LastName as ReviewerName 
	--	from [Application].[Forms] f
	--	join [User].[Users] u on u.ID=f.UserID
	--	join [Master].[Programs] p on p.ID=f.ProgramID
	--	join [Master].[Semester] s on s.ID=f.SemesterID
	--	left join [Application].[FormUsers] fu on fu.FormId=f.ID and fu.RoleId=6
	--	left join [User].[Users] us on us.ID=fu.UserId 
	--	where f.ProgramID=@ProgramID and f.State=@StateId
END