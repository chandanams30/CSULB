-- =============================================
-- Author:		ThoughtFocus
-- Create date: 22-Sep-2022
-- Description:	Assign Forms to Reviewers
-- =============================================
CREATE PROCEDURE [dbo].[AssignFormToReviewers]
	@ProgramId bigint
AS
BEGIN

DECLARE @i INT =1, @formCount int, @TopCount int=0;

SELECT ROW_NUMBER() OVER (ORDER BY F.[ID]) AS row_num,F.[ID]  AS [FormID], F.[ProgramID], Rev.[FormID] AS [RevFormID] INTO #tempUnAssignedForms
FROM [Application].[Forms] F LEFT JOIN [Application].[Reviewer] Rev ON Rev.[FormID]=F.ID
WHERE F.[FormStateID] in (3,5)  AND Rev.[FormID] IS NULL AND F.[ProgramID]=@ProgramId --AND F.[ID]=86
--WHERE F.[FormStateID]=3 AND Rev.[FormID] IS NULL AND F.[ProgramID]=@ProgramId --AND F.[ID]=86

--select * from #tempUnAssignedForms
--DROP TABLE #tempUnAssignedForms

SELECT @formCount = COUNT(*)  FROM #tempUnAssignedForms
 
WHILE @i <= @formCount
	BEGIN
		
		DECLARE @FormID as Bigint, @FormProgramID AS BIGINT
		SELECT @FormID=FormID, @FormProgramID = ProgramID FROM #tempUnAssignedForms WHERE row_num=@i;
		
		SELECT @TopCount=[AssignFormToReviewersCount] FROM [Master].[ProgramConfigurations] WHERE [ProgramID] = @FormProgramID;

		--REVIEWERS LIST FOR THE FROM PROGRAM
		select PU.UserID AS [ReviewerID], PU.[ProgramID], pu.[RoleID]  INTO #tempReviewers FROM [Master].[ProgramUsers] PU WHERE PU.[ProgramID]=@FormProgramID AND PU.[RoleID]=6

		--SELECT * FROM #tempReviewers

		--select rev.[ReviewerID], f.[ProgramID], COUNT(*) as [count] 
		--	from [Application].[Reviewer] Rev join [Application].[Forms] F on rev.FormID = F.[ID]
		--	WHERE F.[ProgramID] = @@FormProgramID
		--	group by  rev.[ReviewerID], f.[ProgramID]

		SELECT TOP (@TopCount) TR.[ReviewerID] as [ReviewerID], @FormID AS [FormID] INTO #ReviewersForms FROM #tempReviewers TR left join 
		(select rev.[ReviewerID], f.[ProgramID], COUNT(*) as [count] 
			from [Application].[Reviewer] Rev join [Application].[Forms] F on rev.FormID = F.[ID]
			WHERE F.[ProgramID] = @FormProgramID
			group by  rev.[ReviewerID], f.[ProgramID]
		)T on T.[ReviewerID] =TR.[ReviewerID] order by isnull(t.[count],0)

		--SELECT * FROM #ReviewersForms

		INSERT INTO [Application].[Reviewer]
           ([ReviewerID]
           ,[FormID]
           ,[CreatedBy]
           ,[CreatedDate])
		   (SELECT RF.[ReviewerID], RF.[FormID], 1,GETDATE() FROM #ReviewersForms RF)

		UPDATE [Application].[Forms] SET [FormStateID] =5  WHERE [ID] = @FormID;

		--SELECT * FROM [Application].[Reviewer] WHERE FormID = @FormID

		DROP TABLE #ReviewersForms;
		DROP TABLE #tempReviewers;
		
		PRINT (@i);
		PRINT ('Row Count: ' + Convert(VARCHAR(50), @i) + ' form ID: ' + Convert(VARCHAR(50), @FormID));
		SET @i = @i + 1;
				
	END;
	
drop table #tempUnAssignedForms

 
END