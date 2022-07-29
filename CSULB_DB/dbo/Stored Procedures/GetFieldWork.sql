CREATE PROCEDURE [dbo].[GetFieldWork] @UserId BIGINT
	,@FieldWorkId BIGINT
AS
BEGIN
	SELECT FW.ID
		,U.FirstName + ' ' + U.LastName AS [StudentName]
		,C.[Name] as [CourseTitle]
		,c.[Subject]+'_'+c.CourseNumber Course
		,c.ClassSection Section
		,C.College
		,T.[Name] AS Term
		,FWPS.[FieldWorkPrerequisiteStatus]
	FROM [FieldWork].[FieldWork] FW
	JOIN [User].[Users] U ON U.ID = FW.UserID
	LEFT JOIN [Master].[FieldWorkCourses] FWC ON FWC.ID =  FW.FieldWorkCourseID 
	JOIN [Master].[CourseTerm] CT ON FWC.CourseTermID = CT.ID
	JOIN [Master].[Courses] C ON CT.CourseID = C.[ID]
	JOIN [Master].[Term] T ON T.TermCode = CT.TermCode
	JOIN [CSULB_DB].[dbo].[View_FieldWorkPrerequisiteStatus] FWPS on FWPS.FieldWorkID=FW.ID
	WHERE FW.ID = @FieldWorkId

	SELECT FWU.[ID]
		,FWU.[UserID]
		,FWU.[FieldWorkID]
		,FWU.[RoleID]
		,U.FirstName + ' ' + U.LastName AS [Name]
		,R.[Description]
	FROM [FieldWork].[FieldWorkUsers] FWU
	JOIN [User].[Users] U ON U.ID = FWU.UserID
	JOIN [Master].[Role] r ON R.ID = FWU.RoleID
	WHERE FWU.FieldWorkID = @FieldWorkId

	SELECT A.[ID]
		,A.[UserID]
		,A.[DocumentID]
		,D.[Name] DocumentName
		,A.[FileName]
		,A.[FileExtn]
		,A.[FolderName]
		,A.[IsApproved]
		,(U.FirstName + ' ' + U.LastName) ApprovedBy
		,A.[ValidatedDate]
		,A.[CreatedBy]
		,A.[CreatedDate]
		,A.[ValidTill]
		,A.[RejectedReason]
	FROM [FieldWork].[Attachments] A
	JOIN [FieldWork].[FieldWork] FW ON FW.ID = @fieldWorkID AND FW.UserID = A.UserID
	LEFT JOIN [Master].[FieldWorkCourses] FWC ON FWC.ID =  FW.FieldWorkCourseID 
	JOIN [Master].[CourseTerm] CT ON FWC.CourseTermID = CT.ID
	--JOIN [Master].[Courses] C ON CT.CourseID = C.[ID]
	JOIN [Master].[FieldWorkDocuments] FWD ON FWD.CourseID = CT.CourseID AND A.DocumentID = FWD.DocumentID
	JOIN [Master].[Documents] D ON D.ID = FWD.DocumentID
	LEFT JOIN [User].[Users] U ON u.ID = A.ApprovedBy
END