CREATE PROCEDURE [dbo].[GetFieldWorkData]
@UserId bigint
AS
BEGIN

	SELECT FW.ID
		,U.FirstName + ' ' + U.LastName AS [StudentName]
		,U.[FirstName]
		,U.[LastName]
		,U.[CSULBID]
		,C.[Name] AS CourseTitle
		,c.[Subject]+'_'+c.CourseNumber Course
		,c.ClassSection Section
		,C.College
		,S.[Name] AS Term
		,FWPS.FieldWorkPrerequisiteStatus
	FROM [FieldWork].[FieldWork] FW
		JOIN [User].[Users] U ON U.ID = FW.UserID
		LEFT JOIN [Master].[FieldWorkCourses] FWC ON FWC.ID =  FW.FieldWorkCourseID 
		JOIN [Master].[CourseTerm] CT ON FWC.CourseTermID = CT.ID
		JOIN [Master].[Courses] C ON CT.CourseID = C.[ID]
		JOIN [Master].[Term] S ON S.TermCode = CT.TermCode
		JOIN [dbo].[View_FieldWorkPrerequisiteStatus] FWPS ON FWPS.FieldWorkID = FW.ID
	WHERE FW.[Status]=1 and FW.ID IN (
				--Student, Administrator wise selecting [FieldWork]
				SELECT FW.ID
				FROM [FieldWork].[FieldWork] FW
				JOIN [User].[Users] U ON U.ID = FW.UserID
				WHERE --F.UserID = 338 OR 
				FW.UserID = 
				CASE WHEN EXISTS (SELECT * FROM [User].[Users] U JOIN [User].[UserRoles] UR ON UR.UserID = U.ID AND UR.RoleID = 1 AND U.[ID] = @UserId) THEN  FW.UserID ELSE @UserId END
			UNION
				--Supervisor wise selecting [FieldWork]
				SELECT FW.ID
				FROM [FieldWork].[FieldWork] FW
				JOIN [FieldWork].[FieldWorkUsers] FWU ON FWU.FieldWorkID = FW.ID
				WHERE FWU.UserID = @UserId
			UNION
				--ProgramAdmin wise selecting [FieldWork]
				SELECT FW.ID
					FROM [FieldWork].[FieldWork] FW 
					JOIN [Master].[FieldWorkCourses] FWC ON FWC.[ID] = FW.[FieldWorkCourseID]
					JOIN [Master].[CourseTerm] CT ON  CT.ID = FWC.[CourseTermID]
					JOIN [Master].[ProgramCourse] PC ON PC.CourseID = CT.CourseID
					JOIN [Master].[ProgramUsers] PU ON PU.ProgramID = PC.ProgramID AND PU.UserID = @UserId AND PU.[RoleID] = 4
			UNION
				--Community Site Users wise selecting [FieldWork]
				SELECT FW.ID
					FROM [FieldWork].[FieldWork] FW
					JOIN [FieldWork].[FieldWorkActivityLog] FWAL ON  FWAL.[FieldWorkID] = FW.[ID] AND FWAL.CommunitySiteUsersID = @UserId 
					JOIN [FieldWork].[CommunitySiteUsers] CSU ON CSU.UserID = FWAL.CommunitySiteUsersID
			)
			ORDER BY CASE WHEN FWPS.FieldWorkPrerequisiteStatus = 0 THEN 20 ELSE FWPS.FieldWorkPrerequisiteStatus END , U.FirstName, U.LastName

END