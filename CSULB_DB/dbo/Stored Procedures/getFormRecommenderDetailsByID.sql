-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Nov-28
-- Description:	Gets Recommender Details by ID
-- =============================================
-- EXEC [dbo].[getFormRecommenderDetailsByID] @RecommendationID=1

CREATE PROCEDURE [dbo].[getFormRecommenderDetailsByID] 
	@RecommendationID BIGINT
AS
BEGIN
	--SELECT R.[ID] INTO #TEMPRecommender
	--FROM [Application].[Recommendations] R
	--WHERE R.[ID]=@RecommendationID;

	SELECT R.[ID]
		,[FormID]
		,[RecommenderName]
		,[RecommenderEmail]
		,[RecommenderURL] + CONVERT(NVARCHAR(50), [RecommenderIdentifier]) AS [RecommenderURL]
		,[RecommenderURLValidTill]
		,u.[FirstName] + ' ' + u.[LastName] AS [ApplicantName]
		,PAD.[ApplicationDeadline]
		,PC.[RecommenderMailBody] AS [MailBody]
		,CASE WHEN F.[ProgramID] IN (1,2,4,6) THEN 0 else 1 END AS [RecommenderMailTemplateAttachement]
	FROM [Application].[Recommendations] R
	JOIN [Application].[Forms] F ON F.[ID] = R.[FormID]
	JOIN [User].[Users] U ON U.ID = F.[UserID]
	JOIN [Master].[ProgramApplicationDates] PAD ON PAD.[ProgramID] = F.[ProgramID]
		AND PAD.[TermCode] = F.[TermCode]
	JOIN [Master].[ProgramConfigurations] PC ON PC.[ProgramID] = F.[ProgramID]
	WHERE R.[ID] =@RecommendationID;
END