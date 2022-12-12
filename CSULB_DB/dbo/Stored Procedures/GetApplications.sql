-- =============================================
-- Author:		ThoughtFocus
-- Create date: 19-May-2022
-- Description:	Applications list for the user
-- =============================================
/*
EXEC [dbo].[GetApplications]
EXEC [dbo].[GetApplications] 1
EXEC [dbo].[GetApplications] 15

*/

CREATE PROCEDURE [dbo].[GetApplications] 
@UserID bigint
AS
BEGIN

	SET NOCOUNT ON;
	--SELECT * FROM [Master].[ApplicationTypes] WHERE [ID] IN (2,4)
--	DECLARE @RoleID AS BIGINT
--	SELECT @RoleID = [RoleID] FROM [User].[UserRoles] UR WHERE UR.[UserID]=@Userid AND UR.[RoleID] <> 2 order by [RoleID] desc
--	--SELECT [ID], [Name] FROM [Master].[Role]

--	/*

--	1	Administrator
--2	Staff
--3	Student
--4	ProgramAdmin
--5	Faculty/Supervisor
--6	Reviewer
--7	Instructor
--8	Interviewer
--9	CommunityPartnerUser
--10	CommunitySupervisor
--*/


--SELECT [ID]
--      ,[Name]
--      ,[Description]
--      ,[CreatedDateTime]
--      ,[CreatedByUserID]
--  FROM [Master].[ApplicationTypes]
--    WHERE [ID] IN (SELECT * FROM [dbo].[SplitString] (
--	(CASE WHEN @RoleID = 1 THEN '2,4' --Administrator 
--		WHEN @RoleID = 3 THEN '2,4' --Student
--		WHEN @RoleID = 4 THEN '2,4' --ProgramAdmin
--		WHEN @RoleID = 5 THEN '4' --Faculty/Supervisor
--		WHEN @RoleID = 6 THEN '2' --Reviewer
--		WHEN @RoleID = 7 THEN '2' --Instructor
--		WHEN @RoleID = 8 THEN '2' --Interviewer
--		WHEN @RoleID = 9 THEN '4' --CommunityPartnerUser
--		WHEN @RoleID = 10 THEN '4' --CommunitySupervisor
--		ELSE '0' END),','))



DECLARE @RoleIDs varchar(50)
SELECT @RoleIDs = STRING_AGG(
	CASE WHEN [RoleID] = 1 THEN '1,2,4' --Administrator 
		WHEN [RoleID] = 3 THEN '1,2,4' --Student
		WHEN [RoleID] = 4 THEN '1,2,4' --ProgramAdmin
		WHEN [RoleID] = 5 THEN '1,4' --Faculty/Supervisor
		WHEN [RoleID] = 6 THEN '1,2' --Reviewer
		WHEN [RoleID] = 7 THEN '1,2' --Instructor
		WHEN [RoleID] = 8 THEN '1,2' --Interviewer
		WHEN [RoleID] = 9 THEN '4' --CommunityPartnerUser
		WHEN [RoleID] = 10 THEN '4' --CommunitySupervisor
		ELSE '0' END, ', ') 
	FROM [User].[UserRoles] UR WHERE UR.[UserID]=@UserID AND UR.[RoleID] <> 2

	SELECT [ID]
      ,[Name]
      ,[Description]
      ,[CreatedDateTime]
      ,[CreatedByUserID]
  FROM [Master].[ApplicationTypes]
    WHERE [ID] IN (SELECT * FROM [dbo].[SplitString](@RoleIDs,','))

END