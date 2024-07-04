-- =============================================
-- Author:		ThoughtFocus
-- Create date: 03/06/2023
-- Description:	Assign User to Program
-- =============================================
CREATE PROCEDURE [User].[AssignUserToProgram]
	@ProgramID [bigint]
	,@RoleID [bigint]
	,@UserPrograms [User].[UDT_UserPrograms] READONLY
	,@createdByUserID bigint
AS
BEGIN
	SET NOCOUNT ON;
--SELECT * FROM [Master].[ProgramUsers]

--DELETE [Master].[ProgramUsers]
DELETE
FROM [Master].[ProgramUsers]
WHERE [ID] IN (SELECT PU.[ID] FROM [Master].[ProgramUsers] PU 
		LEFT JOIN @UserPrograms UP ON UP.[UserID]=PU.[UserID]
		WHERE PU.[ProgramID] = @ProgramID
		AND PU.[RoleID] = @RoleID
		AND UP.[UserID] IS NULL);

--INSERT [Master].[ProgramUsers]

INSERT INTO [Master].[ProgramUsers]
           ([UserID]
           ,[ProgramID]
           ,[RoleID])
    (SELECT UP.[UserID], @ProgramID,@RoleID FROM  @UserPrograms UP
		LEFT JOIN [Master].[ProgramUsers] PU  ON UP.[UserID]=PU.[UserID] AND PU.[ProgramID] = @ProgramID
		AND PU.[RoleID] = @RoleID
		WHERE PU.[UserID] IS NULL)

   	SELECT 1 AS [Status], '' AS [Message];

END