-- =============================================
-- Author:		ThoughtFocus
-- Create date: 14-Apr-2023
-- Description:	Insert Workflow Process Transition History
-- =============================================
CREATE PROCEDURE [dbo].[InsertWorkflowProcessTransitionHistory]
	@WorkFlowDefinationID bigint
	,@FormID bigint
	,@FromActivityDefinitionID bigint
	,@ToActivityDefinitionID bigint

AS
BEGIN

INSERT INTO [CSULBWorkFlow].[WorkflowProcessTransitionHistory]
           ([WorkFlowDefinationID]
           ,[FormID]
           ,[FromActivityDefinitionID]
           ,[ToActivityDefinitionID]
           ,[TransitionDateTime])
     VALUES
           (@WorkFlowDefinationID
           ,@FormID
           ,@FromActivityDefinitionID
           ,@ToActivityDefinitionID
           ,GETDATE())
END