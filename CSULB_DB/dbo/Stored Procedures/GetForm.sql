-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Sep-19
-- Description:	Returns the form details 
-- =============================================
--EXEC [dbo].[GetForm] 340,0,7,2234
--EXEC [dbo].[GetForm] @UserID = 401, @FormID =2, @ProgramID=8,@TermCode=2234 
--EXEC [dbo].[GetForm] @UserID = 401, @FormID =4, @ProgramID=13,@TermCode=2234 

CREATE PROCEDURE [dbo].[GetForm]
@UserID bigint
,@FormID bigint
,@ProgramID bigint
,@TermCode varchar(10) 
AS
BEGIN
				-- if @FormID is null or 0 then create new entry and set @FormID value
		IF (ISNULL(@FormID,0) = 0)
		BEGIN

		IF EXISTS(SELECT * FROM [Application].[Forms] F WHERE F.[ProgramID]=@ProgramID AND F.UserID=@UserID AND F.TermCode=@TermCode)
		BEGIN
			PRINT 'FORM ALREADY PRESENT TO THE USER, PROGRAM AND TERM'
			SELECT @FormID = [ID] FROM [Application].[Forms] F WHERE F.[ProgramID]=@ProgramID AND F.UserID=@UserID AND F.TermCode=@TermCode; 
		END
		ELSE
			BEGIN
				DECLARE @RoleID AS BIGINT
				SELECT @RoleID = [RoleID] FROM [User].[UserRoles] UR WHERE UR.[UserID]=@Userid AND UR.[RoleID] <>2

				IF (@RoleID =3)
					BEGIN

					DECLARE @newSemester nvarchar(50), @newProgram nvarchar(250),@newForm nvarchar(max)

					SELECT @newSemester = [Name] FROM [Master].[Term] WHERE [TermCode] = @TermCode
					SELECT @newProgram = [Name] FROM [Master].[Programs] WHERE [ID] = @ProgramID
					
						--SELECT @newForm='{"firstName": "'+ U.[FirstName] +'","lastName": "'+ U.[LastName] +'",
						--	"preferredName": "","otherName": "","altEmail": "","phoneNumber": "","csulbCampusId": "'+ U.[CSULBID] +'", 
						--	"cusulbEmail": "'+ U.[Email] +'","casId": "","languages": [""],"Semester": "'+ @newSemester +'","Program": "'+ @newProgram +'"}' FROM  [User].[Users] U WHERE U.[ID] = @UserID
							--'+ U.[CSULBID] +'

							SELECT @newForm=[dbo].[fnGetNewFormJson] (@UserID,@ProgramID,@TermCode);

						INSERT INTO [Application].[Forms]
							   ([UserID]
							   ,[ProgramID]
							   ,[TermCode]
							   ,[Form]
							   ,[FormStateID]
							   ,[ApplicationNumber]
							   ,[CreatedDateTime]
							   ,[CreatedByUserID]
							   ,[ModifiedDateTime]
							   ,[ModifiedBy]
							   ,[MessageBoard])
						 VALUES
							   (@UserID
							   ,@ProgramID
							   ,@TermCode
							   ,@newForm
							   ,1
							   ,NULL
							   ,GETDATE()
							   ,@UserID
							   ,GETDATE()
							   ,@UserID
							   ,NULL)

							   IF (@@ERROR = 0)
								BEGIN
									SELECT @FormID = @@IDENTITY; 
								END


						PRINT 'NEW FORM GENERATED'

						--ADD Instructor
						IF (@ProgramID = 4) --4	CED - SSCP Clinical Program
							BEGIN 
								PRINT 'Insert Instructor for 4	CED - SSCP Clinical Program'

								INSERT INTO [Application].[Instructor]
								   ([FormID]
								   ,[InstructorUserID])
								(
									SELECT @FormID, FWU.[UserID]
										FROM 
											[FieldWork].[FieldWorkUsers] FWU
											JOIN [FieldWork].[FieldWork] F ON F.[ID] = FWU.[FieldWorkID]
											JOIN [Master].[FieldWorkCourses] FWC ON FWC.[ID] = F.FieldWorkCourseID
											JOIN [Master].[CourseTerm] CT ON CT.[ID] = FWC.[CourseTermID]
											JOIN [Master].[Courses] C ON C.[ID] = CT.[CourseID]
										WHERE 
										FWU.[RoleID]=5
										AND C.[Subject] = 'EDSS'
										AND C.[CourseNumber] LIKE '300%'
										AND F.[UserID] = @UserID
								)
		 
								 DECLARE @InstructionID AS BIGINT
									 IF (@@ERROR = 0)
										BEGIN
											SELECT @InstructionID = @@IDENTITY; 
										
											INSERT INTO [Application].[InstructorAttachments]
											   ([InstructorID]
											   ,[DocumentID])
											 VALUES
											   (@InstructionID,26) --26	EDSS 300 Instructor Assessment
										END
							END 
							--ADD Instructor

					END
			END
		END

		-----------------
		--DATA SELECTION
		DECLARE @SubmittedDate as datetime;
		--SELECT @SubmittedDate = [ModifiedDateTime] FROM [Application].[FormsHistory] WHERE [FormsID] = @FormID AND [FormStateID] = 3 ORDER BY [ModifiedDateTime] asc
		SELECT TOP 1 @SubmittedDate = [ModifiedDateTime]  FROM 
			(
				SELECT [ModifiedDateTime] FROM [Application].[Forms] WHERE [FormStateID]=3 AND [ID]=@FormID
				UNION
				SELECT [ModifiedDateTime] FROM [Application].[FormsHistory] WHERE [FormsID] = @FormID AND [FormStateID] = 3 
			) T ORDER BY T.[ModifiedDateTime] ASC

		SELECT 
		F.[ID] AS [FormID]
			  ,F.[UserID] AS [StudentID]
			  ,F.[ProgramID] 
			  ,P.[Name] AS [ProgramName]
			  ,F.[TermCode]
			  ,T.[Name] AS [Semester]
			  ,F.[Form] 
			  ,F.[FormStateID]
			  ,FS.[Name] AS [FormState]
			  ,F.[ApplicationNumber]
			  ,F.[CreatedDateTime]
			  ,@SubmittedDate AS [SubmittedDateTime]
			  --,[CreatedByUserID]
			  ,F.[ModifiedDateTime]
			  ,F.[ModifiedBy]
			  ,F.[MessageBoard]
			  ,PC.[CompletingYourApplication]
			  ,pc.[ApplicationTypeID]
			  ,P.[ProgramFormIdentifier]
		FROM [Application].[Forms] F 
		JOIN [Master].[FormState] FS ON FS.[ID] = F.[FormStateID]
		JOIN [Master].[Programs] P ON P.[ID] = F.[ProgramID] 
		JOIN [Master].[Term] T ON T.[TermCode] = F.[TermCode]
		JOIN [Master].[ProgramConfigurations] PC ON PC.ProgramID = F.[ProgramID]
		WHERE F.[ID]=@FormID AND F.[ProgramID]=@ProgramID AND F.TermCode=@TermCode


		--STATE HANDLER
		DECLARE @ApplicationTypeID as bigint;
		SELECT @ApplicationTypeID = [ApplicationTypeID] FROM [Master].[ProgramApplicationType] WHERE [ProgramID] = @ProgramID;

		Declare @showAddInterviewer as bit = 0;
		IF EXISTS(SELECT [ID] FROM [Master].[ProgramUsers] WHERE [UserID] = @UserID AND [ProgramID]=@ProgramID AND [RoleID] in (11,1,4))
		BEGIN
			SET @showAddInterviewer = 1
		END

		IF ([dbo].[fnIsRoleValid] (@Userid,'1') = 1)
		BEGIN
			SET @showAddInterviewer = 1
		END
		

		SELECT (
		SELECT TOP 1 FSH.[RoleID]
			  ,FSH.[FormStateID]
			  ,FSH.[showCancel]
			  ,FSH.[showSave]
			  ,FSH.[showEdit]
			  ,CASE WHEN UR.RoleID =3 AND F.[FormStateID] IN (1,2,4) THEN CAST(1 AS BIT) else  CAST(0 AS BIT) end AS [showSubmit]
			  ,FSH.[showSubmit] AS [enableSubmit]
			  --,FSH.[showSubmit]
			  ,FSH.[showDecline]
			  ,FSH.[showAccept]
			  ,FSH.[showRequestformoreInfo]
			  ,FSH.[showDisqualify]
			  ,FSH.[showWaitlist]
			  ,FSH.[showOffer]
			  ,FSH.[showNotOffer]
			  ,FSH.[showMergeandDownloadAttachments]
			  ,FSH.[showCompletingYourApplication]
			  ,FSH.[showReviewerSection]
			  ,FSH.[editReviewerSection]
			  --,FSH.[showAddInterviewer]
			  --,FSH.[showAddInstructor]
			  ,CASE WHEN @ApplicationTypeID=1 AND @showAddInterviewer=1  THEN FSH.[showAddInterviewer] ELSE  CAST(0 AS BIT) END AS [showAddInterviewer]
			  ,CASE WHEN @ApplicationTypeID=2 THEN CAST(0 AS BIT) else  FSH.[editReviewerSection] end AS [editReviewerSection]
			  ,FSH.[showInReview]
			  ,CASE WHEN UR.RoleID = 3 THEN CONVERT(BIT, 0) ELSE  CONVERT(BIT, 1) END AS [showMessageBoard]
			  --,CONVERT(BIT, 1) AS [showMessageBoard]
			  ,CASE WHEN UR.RoleID = 3 THEN CONVERT(BIT, 0) ELSE  CONVERT(BIT, 1) END AS [showPreviousNext]
		  FROM [Master].[FormStateHandler] FSH
		  JOIN [User].[UserRoles] UR ON UR.UserID=@UserID AND UR.RoleID <> 2 AND UR.RoleID=FSH.RoleID
		  JOIN [Application].[Forms] F ON F.[ID] = @FormID AND F.[FormStateID]=FSH.[FormStateID]
		  ORDER BY FSH.[RoleID]
		  FOR JSON AUTO) AS [StateHandler];
		--
					

		-- Attachments
		SELECT 
		FA.[ID] AS [FormAttachmentID]
		,PD.[DocumentID] AS [DocumentID]
		,PD.[ProgramID] AS [ProgramID]
		,@FormID  AS [FormID]
		,D.[Name] AS [AttachmentTitle]
		,FA.[FileName]
		,FA.[FileExtn]
		,PD.[IsOptional]
		FROM 
		[Master].[ProgramDocuments] PD 
		JOIN [Master].[Documents] D ON D.[ID] = PD.[DocumentID]
		LEFT JOIN [Application].[FormAttachments] FA ON FA.[ProgramDocumentID] = PD.[ID] AND FA.[FormID]=@FormID
		WHERE PD.[ProgramID]=@ProgramID and PD.[DocumentID] not in (3,6,18) --6	Unofficial Transcripts, 3	Letters of Recommendation, 18	Recommendation Letter
		ORDER BY PD.[SortingOrder]

		--Recomendation
		DECLARE @rCOUNT AS INT
		SELECT @rCOUNT = COUNT(*) FROM [Application].[Recommendations] R WHERE R.FormID = @FormID
		--SELECT @rCOUNT

		DECLARE @tempRecommendation TABLE (
		[RecommendationID] bigint
		,[FormID] bigint
		,[RecommenderName] VARCHAR(200)
		,[RecommenderEmail] VARCHAR(200)
		,[CreatedBy] bigint
		,[CreatedDate] datetime
		,[RecommenderURL] VARCHAR(200)
		,[RecommenderURLValidTill] VARCHAR(200)
		,[AllowUpload] bit
		,[RecommenderIdentifier] VARCHAR(200)
		)
		--select * from @tempRecommendation

		INSERT INTO @tempRecommendation
				   ([RecommendationID]
				   ,[FormID]
				   ,[RecommenderName]
				   ,[RecommenderEmail]
				   ,[CreatedBy]
				   ,[CreatedDate]
				   ,[RecommenderURL]
				   ,[RecommenderURLValidTill]
				   ,[AllowUpload]
				   ,[RecommenderIdentifier])
				   (SELECT R.[ID] AS [RecommendationID]
						,R.[FormID]
						,R.[RecommenderName]
						,R.[RecommenderEmail]
						,R.[CreatedBy]
						,R.[CreatedDate]
						,R.[RecommenderURL]
						,R.[RecommenderURLValidTill]
						,R.[AllowUpload]
						,R.[RecommenderIdentifier]
					FROM [Application].[Recommendations] R
					WHERE R.FormID = @FormID)

		IF(@rCOUNT=0)
		BEGIN
			PRINT 'Recommendations count is zero 0'

		INSERT INTO @tempRecommendation
				   ([RecommendationID]
				   ,[FormID]
				   ,[RecommenderName]
				   ,[RecommenderEmail]
				   ,[CreatedBy]
				   ,[CreatedDate]
				   ,[RecommenderURL]
				   ,[RecommenderURLValidTill]
				   ,[AllowUpload]
				   ,[RecommenderIdentifier])
			 VALUES
				   ('',@FormID,'','',1,'','','',1,''),('',@FormID,'','',1,'','','',1,'')
		END
		ELSE IF(@rCOUNT=1)
		BEGIN
			PRINT 'Recommendations count is one 1'
			INSERT INTO @tempRecommendation
				   ([RecommendationID]
				   ,[FormID]
				   ,[RecommenderName]
				   ,[RecommenderEmail]
				   ,[CreatedBy]
				   ,[CreatedDate]
				   ,[RecommenderURL]
				   ,[RecommenderURLValidTill]
				   ,[AllowUpload]
				   ,[RecommenderIdentifier])
			 VALUES
				   ('',@FormID,'','',1,'','','',1,'')
		END

			DECLARE @RRoleID AS BIGINT
				SELECT @RRoleID = [RoleID] FROM [User].[UserRoles] UR WHERE UR.[UserID]=@Userid AND UR.[RoleID] <>2

				--select * from [Master].[Role]


		SELECT(
				SELECT R.[RecommendationID]
					,R.[FormID]
					,R.[RecommenderName]
					,R.[RecommenderEmail]
					,R.[CreatedBy]
					,R.[CreatedDate]
					,R.[RecommenderURL]
					,R.[RecommenderURLValidTill]
					,R.[AllowUpload]
					--,R.[RecommenderIdentifier]
					,JSON_QUERY((
							SELECT 
								RA.[ID] AS [RecomendationAttachmentID]
								,D.[Name] AS [DocumentName]
								,D.[ID] AS [DocumentID]
								,REPLACE(RA.[FileName],'_', ' ') AS [FileName]
								,RA.[FileExtn]
								,RA.[FolderName]
								,RA.[UploadedDate]
								,CASE WHEN @RRoleID = 3 THEN 'false' WHEN @RRoleID IN (1,4,6) THEN 'true' ELSE 'false' END AS [CanView]
							FROM [Master].[Documents] D
							LEFT JOIN [Application].[RecommendationAttachments] RA ON D.[ID] = RA.[DocumentID] AND RA.[RecomendationID] = R.[RecommendationID]
							WHERE D.[ID] IN (3,18)
							FOR JSON PATH,INCLUDE_NULL_VALUES
							)) AS [Attachments]
				FROM @tempRecommendation R
				--WHERE R.FormID = @FormID
				FOR JSON PATH) AS [Recommendations];


		--SELECT(
		--SELECT R.[ID] AS [RecommendationID]
		--	,R.[FormID]
		--	,R.[RecommenderName]
		--	,R.[RecommenderEmail]
		--	,R.[CreatedBy]
		--	,R.[CreatedDate]
		--	,R.[RecommenderURL]
		--	,R.[RecommenderURLValidTill]
		--	,R.[AllowUpload]
		--	,R.[RecommenderIdentifier]
		--	,JSON_QUERY((
		--			SELECT 
		--				RA.[ID] AS [RecomendationAttachmentID]
		--				,D.[Name] AS [DocumentName]
		--				,D.[ID] AS [DocumentID]
		--				,RA.[FileName]
		--				,RA.[FileExtn]
		--				,RA.[FolderName]
		--				,RA.[UploadedDate]
		--			FROM [Master].[Documents] D
		--			LEFT JOIN [Application].[RecommendationAttachments] RA ON D.[ID] = RA.[DocumentID] AND RA.[RecomendationID] = R.[ID]
		--			WHERE D.[ID] IN (3,18)
		--			FOR JSON PATH,INCLUDE_NULL_VALUES
		--			)) AS [Attachments]
		--FROM [Application].[Recommendations] R
		--WHERE R.FormID = @FormID
		--FOR JSON PATH) AS [Recommendations];
		

		--Reviewer
		DECLARE @ReviewerRoleID AS BIGINT

		
		SELECT @ReviewerRoleID = UR.RoleID FROM 
			[User].[Users] U 
			JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID] 
			JOIN [Master].[Role] R ON R.[ID] = UR.RoleID AND UR.[ID]<>2
			WHERE U.[ID]=@UserID;

		--SELECT(
		--SELECT REV.[FormID] 
		--		,REV.[ID] AS [Review.ReviewID]
		--		,REV.[ReviewerID] AS [Review.ReviewerID]
		--		,REV.[ReviewerRecommendation] AS [Review.ReviewerRecommendation]
		--		,REV.[ReviewerComments] AS [Review.ReviewerComments]
		--		,REV.[CreatedDate] AS [Review.Date]
		--		,U.[FirstName] + ' ' + U.[LastName] AS [Review.ReviewerName]
		--		,R.[Name] AS [Review.RoleName]
		--  FROM [Application].[Reviewer] REV
		--	JOIN [User].[Users] U ON U.[ID] = REV.[ReviewerID]
		--	JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID] 
		--	JOIN [Master].[Role] R ON R.[ID] = UR.RoleID AND UR.[ID]<>2
		--  WHERE REV.[FormID]=@FormID 
		--  AND REV.[ReviewerID] = (CASE WHEN @ReviewerRoleID=1 THEN REV.[ReviewerID] WHEN @ReviewerRoleID=6 THEN @UserID ELSE 0 END)
		--  FOR JSON PATH, INCLUDE_NULL_VALUES) AS [Reviewer]

		SELECT(
		SELECT REV.[FormID] 
				,REV.[ID] AS [ReviewID]
				,REV.[ReviewerID] AS [ReviewerID]
				,REV.[ReviewerRecommendation] AS [ReviewerRecommendation]
				,REV.[ReviewerComments] AS [ReviewerComments]
				,REV.[CreatedDate] AS [Date]
				,U.[FirstName] + ' ' + U.[LastName] AS [ReviewerName]
				,R.[Name] AS [RoleName]
				,CAST(CASE WHEN REV.[ReviewerID] =@UserID THEN 1 ELSE 0 END AS bit) AS [editReviewerSection]
		  FROM [Application].[Reviewer] REV
			JOIN [User].[Users] U ON U.[ID] = REV.[ReviewerID]
			JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID]  AND UR.RoleID=6
			JOIN [Master].[Role] R ON R.[ID] = UR.RoleID AND UR.[ID]<>2
		  WHERE REV.[FormID]=@FormID 
		  AND REV.[ReviewerID] = (CASE WHEN  @ReviewerRoleID in (1,4) THEN REV.[ReviewerID] WHEN @ReviewerRoleID=6 THEN @UserID ELSE 0 END)
		  FOR JSON PATH, INCLUDE_NULL_VALUES) AS [Reviewer]

		--Form Control Handler
		SELECT(SELECT [ControlName], [showControl] 
			FROM [Master].[FormControlHandler] FCH
			JOIN [Master].[Programs] P ON P.[ProgramFormIdentifier] = FCH.[ProgramFormIdentifier] 
			WHERE P.[ID]=@ProgramID FOR JSON AUTO)  AS [FormControlHandler]	


	-------------------------------------------------------------------------------------------------------------------
	--INSTRUCTOR
	SELECT
		(
				SELECT INS.[FormID] 
						,INS.[ID] AS [InstructionID]
						,INS.[InstructorUserID] AS [InstructorUserID]
						,U.[FirstName] + ' ' + U.[LastName] AS [InstructorName]
						,R.[Name] AS [RoleName]
						,CAST(CASE WHEN INS.[InstructorUserID] =@UserID THEN 1 ELSE 0 END AS bit) AS [editInstructorSection]
						 ,JSON_QUERY((
									SELECT 
										IA.[ID] AS [InstructionAttachmentID]
										,D.[Name] AS [DocumentName]
										,IA.[DocumentID]
										,REPLACE(IA.[FileName],'_', ' ') AS [FileName]
										,IA.[FileExtn]
										,IA.[FolderName]
										,IA.[UploadedDate]
										,CASE WHEN [dbo].[fnIsRoleValid] (@Userid,'1,4') = 1 THEN 'true' ELSE 'false' END AS [CanView]
									FROM [Application].[InstructorAttachments] IA 
									JOIN [Master].[Documents] D ON D.[ID] = IA.[DocumentID]
									WHERE IA.[ID] = INS.[ID]
									FOR JSON PATH,INCLUDE_NULL_VALUES
									)) AS [Attachments]
					FROM [Application].[Instructor] INS
					JOIN [User].[Users] U ON U.[ID] = INS.[InstructorUserID]
					JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID]  AND UR.RoleID=7
					JOIN [Master].[Role] R ON R.[ID] = UR.RoleID AND UR.[ID]<>2
				  WHERE INS.[FormID]=@FormID 
				  AND INS.[InstructorUserID] = (CASE WHEN [dbo].[fnIsRoleValid] (@Userid,'1,4,3') = 1 THEN INS.[InstructorUserID] WHEN  [dbo].[fnIsRoleValid] (@Userid,'7') = 1 THEN @UserID ELSE 0 END)
				  FOR JSON PATH, INCLUDE_NULL_VALUES) AS [Instructor]
	-------------------------------------------------------------------------------------------------------------------
	--Interviewer
	SELECT
		(
			SELECT INS.[FormID] 
			,INS.[ID] AS [InterviewID]
			,INS.[InterviewerUserID] AS [InterviewerUserID]
			,U.[FirstName] + ' ' + U.[LastName] AS [InterviewerName]
			,R.[Name] AS [RoleName]
			,CAST(CASE WHEN INS.[InterviewerUserID] =@UserID THEN 1 ELSE 0 END AS bit) AS [editInterviewSection]
			,JSON_QUERY((
				SELECT 
				IA.[ID] AS [InterviewAttachmentID]
				,D.[Name] AS [DocumentName]
				,IA.[DocumentID]
				,REPLACE(IA.[FileName],'_', ' ') AS [FileName]
				,IA.[FileExtn]
				,IA.[FolderName]
				,IA.[UploadedDate]
				,CASE WHEN [dbo].[fnIsRoleValid] (@Userid,'1,4') = 1 THEN 'true' ELSE 'false' END AS [CanView]
				FROM [Application].[InterviewerAttachments] IA 
					JOIN [Master].[Documents] D ON D.[ID] = IA.[DocumentID]
				WHERE IA.[ID] = INS.[ID]
					FOR JSON PATH,INCLUDE_NULL_VALUES
				)) AS [Attachments]
			FROM [Application].[Interviewer] INS
				JOIN [User].[Users] U ON U.[ID] = INS.[InterviewerUserID]
				JOIN [User].[UserRoles] UR ON UR.[UserID] = U.[ID]  AND UR.RoleID=8
				JOIN [Master].[Role] R ON R.[ID] = UR.RoleID AND UR.[ID]<>2
			WHERE INS.[FormID]=@FormID 
				AND INS.[InterviewerUserID] = (CASE WHEN [dbo].[fnIsRoleValid] (@Userid,'1,4') = 1 THEN INS.[InterviewerUserID] WHEN  [dbo].[fnIsRoleValid] (@Userid,'8') = 1 THEN @UserID ELSE 0 END)
		FOR JSON PATH, INCLUDE_NULL_VALUES) AS [Interviewer]

	--------------------------------------------------------------------------------------------------------------------
END