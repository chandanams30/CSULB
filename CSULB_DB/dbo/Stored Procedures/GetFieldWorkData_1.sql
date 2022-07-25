CREATE PROCEDURE [dbo].[GetFieldWorkData]
@UserId bigint
AS
BEGIN
	SELECT F.ID
		,U.FirstName + ' ' + U.LastName AS [StudentName]
		,C.CourseTitle
		,C.CSULBCourseId
		,C.College
		,S.[Name] AS Term
		,FWPS.FieldWorkPrerequisiteStatus
	FROM [FieldWork].[FieldWork] F
	JOIN [User].[Users] U ON U.ID = F.UserID
	JOIN [Master].[FieldWorkCourses] C ON C.ID = F.CourseID
	JOIN [Master].[Semester] S ON S.TermCode = C.TermCode
	JOIN [CSULB_DB].[dbo].[View_FieldWorkPrerequisiteStatus] FWPS ON FWPS.FieldWorkID = F.ID
	WHERE F.[Status]=1 and F.ID IN (
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