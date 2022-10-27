-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[getApplicantByRecommenderIdentifier]
	@RecommenderIdentifier uniqueidentifier 
AS
BEGIN
	
	SELECT T.[firstName] + ' ' + T.[lastName] AS [ApplicantName]
		--,T.[cusulbEmail]
		--,T.[altEmail]
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
			  )T JOIN [Application].[Recommendations] R ON R.[FormID] = F.[ID]
			  WHERE R.[RecommenderIdentifier] = @RecommenderIdentifier
   
END