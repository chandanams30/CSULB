-- =============================================
-- Author:		ThoughtFocus
-- Create date: 14-Apr-2023
-- Description:	Upsert Milestone Filled Form
-- =============================================
CREATE PROCEDURE [dbo].[UpsertMilestoneFilledForm]
	@MilestoneFormID BIGINT,
	@MilestoneID BIGINT, 
	@FormID BIGINT,
	@MilestoneFilledForm nvarchar(max)= null,
	@createdByUserID BIGINT,
	@ApproverUserID BIGINT = null,
	@ApproverComments nvarchar(max)= null,
	@ActivityDefinitionID BIGINT,
	@ActivityDefinitionState nvarchar(max),
	@ActivityControlLabel nvarchar(20)
AS
BEGIN

/*
[CSULBWorkFlow].[ActivityDefinition]
ID	State
1	Open
2	Drafted
3	Submitted
4	InReview
5	RequestedMoreInfo
6	Approved
*/

DECLARE @wth_WorkFlowDefinationID AS bigint = 1 --MileStone
	,@wth_FormID AS bigint = @MilestoneFormID
	,@wth_FromActivityDefinitionID bigint
	,@wth_ToActivityDefinitionID bigint = @ActivityDefinitionID

SELECT @wth_FromActivityDefinitionID = [Status] FROM [Application].[MilestoneForms]  WHERE [ID] = @MilestoneFormID;

---------------------------------------------------------------
--Save filled form if @ActivityDefinitionID is 2 Drafted
IF (@ActivityDefinitionID IN (2))
BEGIN
	--OpenToDraft
	UPDATE [Application].[MilestoneForms]
	   SET [MilestoneFilledForm] = @MilestoneFilledForm
		  ,[Status] = @ActivityDefinitionID
		  ,[CreatedBy] = @createdByUserID
		  ,[CreatedDate] = GETDATE()
	 WHERE [ID] = @MilestoneFormID
	 -- InsertWorkflowProcessTransitionHistory
	 EXEC [dbo].[InsertWorkflowProcessTransitionHistory] @WorkFlowDefinationID=@wth_WorkFlowDefinationID,@FormID = @wth_FormID,@FromActivityDefinitionID=@wth_FromActivityDefinitionID,@ToActivityDefinitionID = @wth_ToActivityDefinitionID
END
ELSE IF (@ActivityDefinitionID IN (3,4,5,6))
BEGIN
	--OpenToDraft
	UPDATE [Application].[MilestoneForms]
	   SET [Status] = @ActivityDefinitionID
		  ,[CreatedBy] = @createdByUserID
		  ,[CreatedDate] = GETDATE()
	 WHERE [ID] = @MilestoneFormID

		UPDATE [Application].[MileStoneFormApprovers]
		   SET     
			  [isApproved] = 1
			  ,[ApproverComments] = @ApproverComments
			  ,[ApproveredOn] = GETDATE()
		 WHERE [MilestoneFormID] =@MilestoneFormID
		 AND [ApproverUserID] = @ApproverUserID
		 AND [isApproved] = 0 AND @ActivityDefinitionID=6

	 -- InsertWorkflowProcessTransitionHistory
	 EXEC [dbo].[InsertWorkflowProcessTransitionHistory] @WorkFlowDefinationID=@wth_WorkFlowDefinationID,@FormID = @wth_FormID,@FromActivityDefinitionID=@wth_FromActivityDefinitionID,@ToActivityDefinitionID = @wth_ToActivityDefinitionID
END

----------------------------------------------------------
--Return
----------------------------------------------------------
SELECT 1 AS [Status], '' AS [Message];

END