
-- =============================================
-- Author:		ThoughtFocus
-- Create date: 20-Mar-2023
-- Description:	Add/update Milestone Forms
-- =============================================
CREATE PROCEDURE [dbo].[GetMilestoneFilledForm]
	@UserID bigint
	,@MilestoneFormID bigint
	,@FormID bigint

AS
BEGIN
--------------------------------------------------------
--SELECT MileStone forms and assign
--------------------------------------------------------
--SELECT * FROM [Master].[Milestone]
--SELECT * FROM [Application].[MilestoneForms]

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
--------------------------------------------------------
--SELECT * FROM [Application].[MilestoneForms] MF JOIN [Application].[MileStoneFormApprovers] MFA ON MFA.[MilestoneFormID] = MF.[ID] WHERE MF.[FormID]=@FormID AND MF.[ID] = @MilestoneFormID AND MF.[Status] =3
--AND MFA.[ApproverUserID]=@UserID;

UPDATE MF SET MF.[Status]=4
 FROM [Application].[MilestoneForms] MF JOIN [Application].[MileStoneFormApprovers] MFA ON MFA.[MilestoneFormID] = MF.[ID] WHERE MF.[FormID]=@FormID AND MF.[ID] = @MilestoneFormID AND MF.[Status] =3
AND MFA.[ApproverUserID]=@UserID;
--------------------------------------------------------
SELECT MF.[ID] AS [MilestoneFormID]
      ,MF.[MilestoneID]
      ,MF.[FormID]
      ,MF.[MilestoneFilledForm]
      ,MF.[Status]
	  ,AD.[Name] AS [StatusName]
      ,M.[MilestoneForm]
	  ,M.MilestoneName
	  ,CASE WHEN [dbo].[fnIsRoleValid] (@Userid,'3') = 1 AND MF.[Status] IN (1,2) THEN 1 ELSE 0 END AS [isEditable]
  FROM [Application].[MilestoneForms] MF
  JOIN [Master].[Milestone] M ON M.[ID] = MF.[MilestoneID]
  JOIN [CSULBWorkFlow].[ActivityDefinition] AD ON AD.[ID] = MF.[Status]
  WHERE MF.[FormID]=@FormID AND MF.[ID] = @MilestoneFormID;
--------------------------------------------------------
--SELECT 
--AD.*
--FROM [Application].[MilestoneForms] MF JOIN [CSULBWorkFlow].[TransitionDefinition] TD ON MF.[Status] = TD.[FromID]
--JOIN [CSULBWorkFlow].[ActivityDefinition] AD ON AD.[ID] = TD.[ToID]
--WHERE MF.[FormID]=@FormID AND MF.[ID] = @MilestoneFormID;
SELECT (SELECT DISTINCT
AD.[ID]
,AD.[State]
,AD.[ActivityControlLabel]
FROM [Application].[MilestoneForms] MF 
JOIN [Application].[MileStoneFormApprovers] MFA ON MFA.[MilestoneFormID] = MF.[ID]
JOIN [CSULBWorkFlow].[TransitionDefinition] TD ON MF.[Status] = TD.[FromID]
JOIN [CSULBWorkFlow].[ActivityDefinition] AD ON AD.[ID] = TD.[ToID]
WHERE MF.[FormID]=@FormID AND MF.[ID] = @MilestoneFormID AND AD.ActivityControlLabel IS NOT NULL
--AND [dbo].[fnIsRoleValid] (@Userid,'3') = 1 
AND (CASE WHEN [dbo].[fnIsRoleValid] (@Userid,'3') = 1 AND MF.[Status] in (1,2) THEN 1 
		WHEN MFA.[ApproverUserID] = @UserID AND MF.[Status] in (3,4,5,6) THEN 1 ELSE 0  END ) =1
FOR JSON AUTO) AS [MilestoneFormActivityHandler]
--------------------------------------------------------------------------------------------------------------------
--Approvers
--SELECT * FROM 
--------------------------------------------------------------------------------------------------------------------
SELECT( 
SELECT  MFA.[ID] AS [MileStoneApproveID]
	,MFA.[MilestoneFormID]
      ,MFA.[ApproverUserID]
	   ,U.[FirstName] + ' ' + U.[LastName] AS  [ApproverUserName]
      ,MFA.[Sequence]
      ,MFA.[ApproverComments]
      ,MFA.[ApproveredOn]
	  ,CASE WHEN MFA.[ApproverUserID]= @UserID AND [dbo].[fnEnableApproverEdit] (@UserID, @MilestoneFormID, @FormID) = 1 THEN 1 ELSE 0 END AS [isEditable] -- NEED TO WORK MORE FOR NEXT LEVEL APPORVER HANDLING
  FROM [Application].[MilestoneForms] MF
  JOIN [Application].[MileStoneFormApprovers] MFA ON MFA.[MilestoneFormID] = MF.[ID]  AND MF.[FormID]=@FormID 
    JOIN [User].[Users] U ON U.[ID] = MFA.[ApproverUserID]
  WHERE 
  --(CASE WHEN MFA.[ApproverUserID]= @UserID THEN 1 ELSE [dbo].[fnIsRoleValid] (@Userid,'1,4') END )= 1 
  --AND MFA.[ApproverUserID] =(CASE WHEN MFA.[ApproverUserID]= @UserID THEN @UserID ELSE MFA.[ApproverUserID] END )
   MFA.[ApproverUserID] =(CASE WHEN MFA.[ApproverUserID] = @UserID  AND MF.[Status] in (3,4,5,6) THEN @UserID WHEN [dbo].[fnIsRoleValid] (@Userid,'1') =1 THEN MFA.[ApproverUserID] ELSE 0  END )
  AND MFA.[MilestoneFormID] = @MilestoneFormID 
  ORDER BY MFA.[Sequence] FOR JSON AUTO) AS [MileStoneFilledFormApprovers]
--------------------------------------------------------------------------------------------------------------------
-- =============================================
-- Example to execute the stored procedure
-- =============================================
--SELECT * FROM [Application].[Forms] WHERE [Programid]=1 ORDER BY ID
--EXEC [dbo].[GetMilestoneFilledForm]  @UserID=338, @FormID=1,@MilestoneFormID =1
--EXEC [dbo].[GetMilestoneFilledForm] @UserID=338, @FormID=10266,@MilestoneFormID =1
--Approver
--EXEC [dbo].[GetMilestoneFilledForm] @UserID=3, @FormID=10266,@MilestoneFormID =1
END