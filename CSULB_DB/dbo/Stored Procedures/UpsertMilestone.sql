-- =============================================
-- Author:		ThoughtFocus
-- Create date: 20-Mar-2023
-- Description:	Add/update Milestone Forms
-- =============================================
CREATE PROCEDURE [dbo].[UpsertMilestone]
	@MilestoneID BIGINT,
	@MilestoneName nvarchar(50)= null, 
	@MilestoneDescription nvarchar(500)= null,
	@MilestoneForm nvarchar(max)= null,
	@isPublished [bit],
	@isMandatory [bit],
	@createdByUserID bigint,
	@MileStoneApprovers [dbo].[UDT_MileStoneApprovers] READONLY
AS
BEGIN
	IF ((@MilestoneID IS NULL) OR (@MilestoneID=0))
	BEGIN
		INSERT INTO  [Master].[Milestone]
			   ([MilestoneName]
			   ,[MilestoneDescription]
			   ,[MilestoneForm]
			   ,[isPublished]
			   ,[isMandatory]
			   ,[CreatedBy]
			   ,[CreatedDate])
		 VALUES
			   (@MilestoneName
			   ,@MilestoneDescription
			   ,@MilestoneForm
			   ,@isPublished
			   ,@isMandatory
			   ,@createdByUserID
			   ,GETDATE())
		
		
		IF (@@ERROR = 0)
		BEGIN
			DECLARE @NewMilestoneID AS BIGINT
			SELECT @NewMilestoneID = @@IDENTITY; 

			INSERT INTO [Master].[MileStoneApprovers]
					   ([MilestoneID]
					   ,[ApproverUserID]
					   ,[Sequence])
						(SELECT @NewMilestoneID, [ApproverUserID], [Sequence] FROM @MileStoneApprovers)
		END

	END
	ELSE
		BEGIN
				UPDATE [Master].[Milestone]
				SET
				   [MilestoneName]=@MilestoneName
				  ,[MilestoneForm]=@MilestoneForm
				  ,[MilestoneDescription]=@MilestoneDescription
				  ,[isPublished] = @isPublished
				  ,[isMandatory] = @isMandatory
				  WHERE [ID]=@MilestoneID

				  DELETE FROM [Master].[MileStoneApprovers] WHERE [MilestoneID] =@MilestoneID;

				  INSERT INTO [Master].[MileStoneApprovers]
					   ([MilestoneID]
					   ,[ApproverUserID]
					   ,[Sequence])
						(SELECT @MilestoneID, [ApproverUserID], [Sequence] FROM @MileStoneApprovers)
		END
END