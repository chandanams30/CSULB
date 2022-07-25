--exec [GetFieldWork] 614,1
CREATE PROCEDURE [dbo].[GetFieldWork] @UserId BIGINT
	,@FieldWorkId BIGINT
AS
BEGIN
	SELECT F.ID
		,U.FirstName + ' ' + U.LastName AS [StudentName]
		,C.CourseTitle
		,c.[Subject]+'_'+c.CourseNumber Course
		,c.ClassSection Section
		,C.College
		,S.[Name] AS Term
		,FWPS.[FieldWorkPrerequisiteStatus]
	FROM [FieldWork].[FieldWork] F
	JOIN [User].[Users] U ON U.ID = F.UserID
	JOIN [Master].[FieldWorkCourses] C ON C.ID = F.CourseID
	JOIN [Master].[Semester] S ON S.TermCode = C.TermCode
	JOIN [CSULB_DB].[dbo].[View_FieldWorkPrerequisiteStatus] FWPS on FWPS.FieldWorkID=F.ID
	WHERE F.ID = @FieldWorkId

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
	JOIN [FieldWork].[FieldWork] F ON F.ID = @fieldWorkID AND f.UserID = A.UserID
	JOIN [Master].[FieldWorkDocuments] FWD ON FWD.CourseID = F.CourseID AND A.DocumentID = FWD.DocumentID
	JOIN [Master].[Documents] D ON D.ID = FWD.DocumentID
	LEFT JOIN [User].[Users] U ON u.ID = A.ApprovedBy
END