-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
-- exeC [dbo].[getFormRecommenderDetailsByFormID] 1,1,1,1

CREATE PROCEDURE [dbo].[getFormRecommenderDetailsByFormID] 
	@UserID BIGINT
	,@FormID BIGINT
	,@ProgramID BIGINT
	,@TermCode VARCHAR(10)
AS
BEGIN
	SELECT R.[ID] INTO #TEMPRecommender
	FROM [Application].[Recommendations] R
	WHERE R.[FormID] = @FormID AND R.[isMailSent]=0;

	UPDATE [Application].[Recommendations] SET [isMailSent]=1 WHERE [ID] IN (SELECT [ID] FROM #TEMPRecommender);

	SELECT R.[ID]
		,[FormID]
		,[RecommenderName]
		,[RecommenderEmail]
		,[RecommenderURL] + CONVERT(NVARCHAR(50), [RecommenderIdentifier]) AS [RecommenderURL]
		,[RecommenderURLValidTill]
		,u.[FirstName] + ' ' + u.[LastName] AS [ApplicantName]
		,PAD.[ApplicationDeadline]
		,PC.[RecommenderMailBody] AS [MailBody]
	FROM [Application].[Recommendations] R
	JOIN [Application].[Forms] F ON F.[ID] = R.[FormID]
	JOIN [User].[Users] U ON U.ID = F.[UserID]
	JOIN [Master].[ProgramApplicationDates] PAD ON PAD.[ProgramID] = F.[ProgramID]
		AND PAD.[TermCode] = F.[TermCode]
	JOIN [Master].[ProgramConfigurations] PC ON PC.[ProgramID] = F.[ProgramID]
	WHERE R.[ID] IN (SELECT [ID] FROM #TEMPRecommender) 
END