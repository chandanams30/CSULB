
--EXEC [dbo].[GetFormStates] 3

CREATE PROCEDURE [dbo].[GetFormStates]
@UserID bigint
AS
BEGIN

DECLARE @RoleID AS BIGINT

---SELECT @RoleID = [RoleID] FROM [User].[UserRoles] UR WHERE UR.[UserID]=@Userid AND UR.[RoleID] <>2
SELECT @RoleID = [RoleID] FROM [User].[UserRoles] UR WHERE UR.[UserID]=@Userid AND UR.[RoleID] not in (2,5,9,10) order by [RoleID] desc
--SELECT @RoleID
--SELECT * FROM [Master].[Role]
--SELECT * FROM [Master].[FormState]


/*
1	Administrator
2	Staff
3	Student
4	ProgramAdmin
5	Faculty/Supervisor
6	Reviewer
7	Instructor
8	Interviewer
9	CommunityPartnerUser
10	CommunitySupervisor
*/


SELECT [ID] AS [StateID]
      ,[Name] AS [StateName]
      --,[Description]
      --,[CreatedDateTime]
      --,[CreatedByUserID]
  FROM [Master].[FormState]
  WHERE [ID] IN (
		(SELECT * FROM [dbo].[SplitString] (
		(CASE WHEN @RoleID = 1 THEN '1,2,3,4,5,6,7,8,9,10,11,12,13' --Administrator 
				WHEN @RoleID = 3 THEN '1,2,3,4,5,6,7,8,9,10,11,12,13' --Student
				WHEN @RoleID = 4 THEN '1,2,3,4,5,6,7,8,9,10,11,12,13' --ProgramAdmin
				WHEN @RoleID = 5 THEN '0' --Faculty/Supervisor
				WHEN @RoleID = 6 THEN '5,4' --Reviewer
				WHEN @RoleID = 7 THEN '1,2' --Instructor
				WHEN @RoleID = 8 THEN '7' --Interviewer
				WHEN @RoleID = 9 THEN '0' --CommunityPartnerUser
				WHEN @RoleID = 10 THEN '0' --CommunitySupervisor
				ELSE '0' END),','))
  )

END