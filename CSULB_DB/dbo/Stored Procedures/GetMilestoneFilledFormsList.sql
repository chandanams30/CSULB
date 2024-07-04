
-- =============================================
-- Author:		ThoughtFocus
-- Create date: 06-Apr-2023
-- Description:	Get Milestone Application Forms List
-- =============================================
CREATE PROCEDURE [dbo].[GetMilestoneFilledFormsList]
	@UserID bigint
,@FormID bigint
,@ProgramID bigint
,@TermCode varchar(10) 

AS
BEGIN
--------------------------------------------------------
--update MileStone forms and assign
--------------------------------------------------------
--SELECT * FROM  [Master].[ProgramMilestones]
--SELECT * FROM  [Application].[MilestoneForms]

INSERT INTO [Application].[MilestoneForms] (
	[MilestoneID]
	,[FormID]
	,[Status]
	,[CreatedBy]
	,[CreatedDate]
	) (
	SELECT PM.[MilestoneID]
	,@FormID
	,1
	,1
	,GETDATE() FROM  [Master].[ProgramMilestones] PM
		LEFT JOIN [Application].[MilestoneForms] MF ON MF.[MilestoneID] = PM.[MilestoneID] AND MF.[FormID]=@FormID
		WHERE PM.[ProgramID]=@ProgramID AND 
			PM.[TermCode]=@TermCode AND 
			MF.[ID] IS NULL
	)
----------------
INSERT INTO [Application].[MileStoneFormApprovers]
           ([MilestoneFormID]
           ,[ApproverUserID]
           ,[Sequence]
           )
		   (
		   SELECT MF.[ID] AS [MilestoneFormID]
				,MSA.[ApproverUserID]
				,MSA.[Sequence]
			FROM [Master].[MileStoneApprovers] MSA
			JOIN [Application].[MilestoneForms] MF ON MF.[MilestoneID] = MSA.[MilestoneID] AND MF.[FormID]=@FormID
			LEFT JOIN [Application].[MileStoneFormApprovers] MFA ON MF.[ID] = MFA.[MilestoneFormID]
			WHERE MFA.[ApproverUserID] IS NULL
		   );
--------------------------------------------------------
--------------------------------------------------------
--SELECT MileStone forms and assign
--------------------------------------------------------
--SELECT * FROM [Master].[Milestone]
--SELECT * FROM [Application].[MilestoneForms]

SELECT MF.[ID] AS [MilestoneFormsID]
      ,MF.[MilestoneID]
      ,MF.[FormID]
      --,MF.[MilestoneFilledForm]
      ,MF.[Status]
      --,M.[MilestoneForm]
	  ,M.MilestoneName
  FROM [Application].[MilestoneForms] MF
  JOIN [Master].[Milestone] M ON M.[ID] = MF.[MilestoneID]
  WHERE MF.[FormID]=@FormID
--------------------------------------------------------


-- =============================================
-- Example to execute the stored procedure
-- =============================================
--SELECT * FROM [Application].[Forms] WHERE [Programid]=1 ORDER BY ID
--EXEC [dbo].[GetMilestoneFilledFormsList] @UserID=339, @FormID=10266,@ProgramID =6,@TermCode =2234
--EXEC [dbo].[GetMilestoneFilledFormsList] @UserID=338, @FormID=10276,@ProgramID =6,@TermCode =2234
END