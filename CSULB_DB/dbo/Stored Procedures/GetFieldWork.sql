-- exec [dbo].[GetFieldWork] 338, 37
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
		--,'{"ShowSummaryTab" : false,"ShowActivityLogTab" : false,"ShowPrerequisitesTab" : true,"ShowFacultySupervisorTab" : false,"ShowCommunityPartnerUSerTab" : false}' AS [UIHandler]
	FROM [FieldWork].[FieldWork] FW
	JOIN [User].[Users] U ON U.ID = FW.UserID
	LEFT JOIN [Master].[FieldWorkCourses] FWC ON FWC.ID =  FW.FieldWorkCourseID 
	JOIN [Master].[CourseTerm] CT ON FWC.CourseTermID = CT.ID
	JOIN [Master].[Courses] C ON CT.CourseID = C.[ID]
	JOIN [Master].[Term] T ON T.TermCode = CT.TermCode
	JOIN [dbo].[View_FieldWorkPrerequisiteStatus] FWPS on FWPS.FieldWorkID=FW.ID
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

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

DECLARE @instruction7 VARCHAR(500) = '<P><b>Instruction</b><br/>' + 'For TB skin/blood/risk assessment test, expiration date is four (4) years from test date. For x-ray, expiration date is eight (8) years from test date.' + '</P>';
DECLARE @instruction8 VARCHAR(500) = '<P><b>Instruction</b><br/>' + 'Website screenshot of valid CTC fingerprint clearance, showing issuance/expiration dates and document number.' + '</P>';

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------



	SELECT A.[ID]
		,A.[UserID]
		,A.[DocumentID]
		,D.[Name] DocumentName
		--,A.[FileName]
		,CASE WHEN A.[ValidTill] < GETDATE() THEN NULL ELSE A.[FileName] END AS [FileName]
		--,A.[FileExtn]
		,CASE WHEN A.[ValidTill] < GETDATE() THEN NULL ELSE A.[FileExtn] END AS [FileExtn]
		,A.[FolderName]
		--,A.[IsApproved]
		,CASE WHEN A.[ValidTill] < GETDATE() THEN 0 ELSE A.[IsApproved] END AS [IsApproved]
		,(U.FirstName + ' ' + U.LastName) ApprovedBy
		,A.[ValidatedDate]
		,A.[CreatedBy]
		,A.[CreatedDate]
		,ISNULL(A.[ValidTill], GETDATE() + 10) AS [ValidTill]
		,A.[RejectedReason]
		--,A.[Comments]
		,CASE WHEN A.[ValidTill] < GETDATE() THEN NULL ELSE A.[Comments] END AS [Comments]
		,CASE 
			WHEN A.[FileName] IS NULL THEN 'Not Submitted'
			WHEN A.[FileName] IS NOT NULL and A.[IsApproved] is null THEN 'Submitted'
			WHEN A.[FileName] IS NOT NULL AND A.[IsApproved] =0 THEN 'Not Approved'
			WHEN A.[FileName] IS NOT NULL AND A.[ValidTill] > GETDATE()  and A.[IsApproved] =1 THEN 'Approved'
			WHEN A.[FileName] IS NOT NULL and A.[ValidTill] < GETDATE() THEN 'Expired'
			ELSE 'NOT HANDLED' 
		END AS [DocumentStatus]
		, 
		--'<P>' + 
		--<P><b>Reason</b><br/>' + A.[RejectedReason] + '</P>'
		CASE 
			WHEN A.[FileName] IS NULL THEN  IIF (A.[DocumentID] =7,  @instruction7, IIF (A.[DocumentID] =8,  @instruction8, '')) 
			WHEN A.[FileName] IS NOT NULL and A.[IsApproved] is null THEN IIF (A.[DocumentID] =7,  @instruction7, IIF (A.[DocumentID] =8,  @instruction8, '')) 
			WHEN A.[FileName] IS NOT NULL AND A.[IsApproved] =0 THEN '<P><b>Reason</b><br/>' + ISNULL(A.[RejectedReason], '') + '</P>' + '<P><b>Comments</b><br/>' + ISNULL(A.[Comments], '') + '</P>' + '<P>More info requested on ' + ISNULL(CONVERT(VARCHAR(20), A.[ValidatedDate], 101), '')  + '</P>' + '<P>Reviewed on ' + ISNULL(CONVERT(VARCHAR(20), A.[ValidatedDate], 101), '')  + CASE WHEN EXISTS (SELECT * FROM [User].[Users] U JOIN [User].[UserRoles] UR ON UR.UserID = U.ID AND UR.RoleID IN(1,4) AND U.[ID] = @UserId) THEN   '<br/>Reviewed by ' + (U.FirstName + ' ' + U.LastName)   ELSE '' END + + '</P>' + IIF (A.[DocumentID] =7,  @instruction7, IIF (A.[DocumentID] =8,  @instruction8, ''))
			WHEN A.[FileName] IS NOT NULL AND A.[ValidTill] > GETDATE()  and A.[IsApproved] =1 THEN '<P>Expiration Date ' + ISNULL(CONVERT(VARCHAR(20), A.[ValidTill], 101), '')  + '</P>' + '<P>Reviewed on ' + ISNULL(CONVERT(VARCHAR(20), A.[ValidatedDate], 101), '')  + CASE WHEN EXISTS (SELECT * FROM [User].[Users] U JOIN [User].[UserRoles] UR ON UR.UserID = U.ID AND UR.RoleID IN(1,4) AND U.[ID] = @UserId) THEN   '<br/>Reviewed by ' + (U.FirstName + ' ' + U.LastName)    ELSE '' END + '</P>' + IIF (A.[DocumentID] =7,  @instruction7, IIF (A.[DocumentID] =8,  @instruction8, ''))
			WHEN A.[FileName] IS NOT NULL and A.[ValidTill] < GETDATE() THEN '<P>Expired on ' + ISNULL(CONVERT(VARCHAR(20), A.[ValidTill], 101), '')  + '</P>'
			ELSE 'NOT HANDLED'  
		END 
		-- + '</P>' 
		AS [DocumentInfo]
		, CASE WHEN EXISTS (SELECT * FROM [User].[Users] U JOIN [User].[UserRoles] UR ON UR.UserID = U.ID AND UR.RoleID IN(3) AND U.[ID] = @UserId) THEN   1 ELSE 0 END AS [CanUpload]
		, CASE WHEN EXISTS (SELECT * FROM [User].[Users] U JOIN [User].[UserRoles] UR ON UR.UserID = U.ID AND UR.RoleID IN(1,4) AND U.[ID] = @UserId) THEN   1 ELSE 0 END AS [CanValidate]
	FROM [FieldWork].[Attachments] A
	JOIN [FieldWork].[FieldWork] FW ON FW.ID = @fieldWorkID AND FW.UserID = A.UserID
	LEFT JOIN [Master].[FieldWorkCourses] FWC ON FWC.ID =  FW.FieldWorkCourseID 
	JOIN [Master].[CourseTerm] CT ON FWC.CourseTermID = CT.ID
	--JOIN [Master].[Courses] C ON CT.CourseID = C.[ID]
	JOIN [Master].[FieldWorkDocuments] FWD ON FWD.CourseID = CT.CourseID AND A.DocumentID = FWD.DocumentID
	JOIN [Master].[Documents] D ON D.ID = FWD.DocumentID
	LEFT JOIN [User].[Users] U ON u.ID = A.ApprovedBy
	order by A.[DocumentID]
	--WHERE FWD.[IsRestricted] =  CASE WHEN EXISTS (SELECT * FROM [User].[Users] U JOIN [User].[UserRoles] UR ON UR.UserID = U.ID AND UR.RoleID IN(1,3,4) AND U.[ID] = @UserId) THEN   FWD.[IsRestricted] ELSE 0 END
END