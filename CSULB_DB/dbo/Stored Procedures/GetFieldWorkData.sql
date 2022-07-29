CREATE PROCEDURE [dbo].[GetFieldWorkData]
@UserId bigint
AS
BEGIN
	SELECT FW.ID
		,U.FirstName + ' ' + U.LastName AS [StudentName]
		,C.[Name] AS CourseTitle
		,c.[Subject]+'_'+c.CourseNumber Course
		,c.ClassSection Section
		,C.College
		,S.[Name] AS Term
		,FWPS.FieldWorkPrerequisiteStatus
	FROM [FieldWork].[FieldWork] FW
	JOIN [User].[Users] U ON U.ID = FW.UserID
	--JOIN [Master].[FieldWorkCourses] C ON C.ID = F.CourseID
	--JOIN [Master].[Semester] S ON S.TermCode = C.TermCode
	LEFT JOIN [Master].[FieldWorkCourses] FWC ON FWC.ID =  FW.FieldWorkCourseID 
	JOIN [Master].[CourseTerm] CT ON FWC.CourseTermID = CT.ID
	JOIN [Master].[Courses] C ON CT.CourseID = C.[ID]
	JOIN [Master].[Term] S ON S.TermCode = CT.TermCode
	JOIN [CSULB_DB].[dbo].[View_FieldWorkPrerequisiteStatus] FWPS ON FWPS.FieldWorkID = FW.ID
	WHERE FW.[Status]=1 and FW.ID IN (
			SELECT F.ID
			FROM [FieldWork].[FieldWork] F
			JOIN [User].[Users] U ON U.ID = F.UserID
			WHERE F.UserID = @UserId
		
			UNION
		
			SELECT F.ID
			FROM [FieldWork].[FieldWork] F
			JOIN [FieldWork].[FieldWorkUsers] FWU ON FWU.FieldWorkID = F.ID
			WHERE FWU.UserID = @UserId
			)

END