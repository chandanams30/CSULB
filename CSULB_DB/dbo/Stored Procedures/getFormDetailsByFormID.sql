-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
-- exeC [dbo].[getFormDetailsByFormID]1,1,1,1

CREATE PROCEDURE [dbo].[getFormDetailsByFormID]
	@UserID bigint
,@FormID bigint
,@ProgramID bigint
,@TermCode varchar(10) 
AS
BEGIN
	
	SELECT T.[firstName] + ' ' + T.[lastName] AS [ApplicantName]
		--,T.[cusulbEmail]
		--,T.[altEmail]
		,T.[Program] as [programName]
		,'venkatesan.shanmugam@thoughtfocus.com' as [cusulbEmail]
		,'asif.khan@thoughtfocus.com' as [altEmail]
			FROM [Application].[Forms] F
			 CROSS APPLY OPENJSON(F.[FORM])
			   WITH (
				firstName nvarchar(500)
			  ,lastName nvarchar(500)
			  ,preferredName nvarchar(500)
			  ,otherName nvarchar(500)
			  ,cusulbEmail nvarchar(500)
			  ,altEmail nvarchar(500)
			  ,phoneNumber nvarchar(500)
			  ,csulbCampusId nvarchar(500)
			   ,Semester nvarchar(500)
			   ,Program nvarchar(500)
			  )T 
			  WHERE F.[ID] = @FormID;

			 --SELECT R.[ID]
			 -- ,[FormID]
			 -- ,[RecommenderName]
			 -- ,[RecommenderEmail]
			 -- ,[RecommenderURL] +  CONVERT(NVARCHAR(50),[RecommenderIdentifier]) AS [RecommenderURL] 
			 -- ,[RecommenderURLValidTill]
			 -- ,u.[FirstName] + ' ' + u.[LastName] AS [ApplicantName]
			 -- ,PAD.[ApplicationDeadline]
			 -- ,PC.[RecommenderMailBody] AS [MailBody]
		  --FROM [Application].[Recommendations] R
		  --JOIN [Application].[Forms] F ON F.[ID] = R.[FormID]
		  --JOIN [User].[Users] U ON U.ID = F.[UserID] 
		  --JOIN [Master].[ProgramApplicationDates] PAD ON PAD.[ProgramID] = F.[ProgramID] AND PAD.[TermCode] = F.[TermCode]
		  --JOIN [Master].[ProgramConfigurations] PC ON PC.[ProgramID]=F.[ProgramID]
		  --WHERE F.[ID]=@FormID
   
END