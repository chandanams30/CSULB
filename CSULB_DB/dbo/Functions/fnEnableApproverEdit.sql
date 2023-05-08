-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2023-Mar-04
-- Description:	Return BIT value if enable the approver user to edit
-- =============================================
CREATE FUNCTION [dbo].[fnEnableApproverEdit] 
(
	-- Add the parameters for the function here
	@UserID BIGINT
	,@MilestoneFormID BIGINT
	,@FormID BIGINT
)
RETURNS BIT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result BIT = 0

DECLARE @TempMilestoneFormID BIGINT
	,@TempSequence INT
	,@isPreviousApproverApproved BIT
	,@isThisApproverApproved BIT
	,@enableApproverEdit BIT
------------------------------------------------------------------------------------------------------
--
------------------------------------------------------------------------------------------------------
SELECT @TempMilestoneFormID=MF.[ID], @TempSequence=MFA.[Sequence] 
FROM [Application].[MilestoneForms] MF JOIN [Application].[MileStoneFormApprovers] MFA ON MFA.[MilestoneFormID] = MF.[ID] 
WHERE MF.[FormID]=@FormID AND MF.[ID] = @MilestoneFormID 
AND MF.[Status] =4
AND MFA.[ApproverUserID]=@UserID;

SELECT @isThisApproverApproved = MFA.[isApproved] 
FROM [Application].[MilestoneForms] MF JOIN [Application].[MileStoneFormApprovers] MFA ON MFA.[MilestoneFormID] = MF.[ID] 
WHERE MF.[ID]=@TempMilestoneFormID AND MFA.[ApproverUserID]=@UserID;

SELECT --MFA.[Sequence] AS [PreviousSequence], MFA.[isApproved] AS [isPreviousApproverApproved]  
@isPreviousApproverApproved = MFA.[isApproved]
FROM [Application].[MilestoneForms] MF JOIN [Application].[MileStoneFormApprovers] MFA ON MFA.[MilestoneFormID] = MF.[ID] 
WHERE MF.[ID]=@TempMilestoneFormID AND MFA.[Sequence] = @TempSequence-1;

--SELECT *,  MFA.[Sequence]-1 FROM [Application].[MilestoneForms] MF JOIN [Application].[MileStoneFormApprovers] MFA ON MFA.[MilestoneFormID] = MF.[ID] WHERE MF.[FormID]=@FormID AND MF.[ID] = @MilestoneFormID AND MF.[Status] =4
--AND MFA.[Sequence] = 1;

--select @isPreviousApproverApproved as [isPreviousApproverApproved], @isThisApproverApproved as [isThisApproverApproved]

SELECT @Result= case when @isThisApproverApproved=1 Then 0 WHEN @isThisApproverApproved= 0 AND ISNULL(@isPreviousApproverApproved,1) =1 THEN 1 ELSE 0 END 

--SELECT @enableApproverEdit AS [enableApproverEdit]

RETURN @Result;
------------------------------------------------------------------------------------------------------
-- =============================================
-- Example to execute the FUNCTION
-- =============================================
--SELECT [dbo].[fnEnableApproverEdit] (@UserID, @MilestoneFormID, @FormID)
--SELECT [dbo].[fnEnableApproverEdit] (3, 3, 10266)
--SELECT [dbo].[fnEnableApproverEdit] (4, 3, 10266)

END