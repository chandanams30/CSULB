
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetPrerequisitesApprovalEmailConfirmation]
@FieldWorkId bigint
AS
BEGIN
			--	SELECT U.FirstName + ' ' + U.LastName as [ApplicantName], U.CSULBID, T.[Name],
			----U.Email
			--'asif.khan@thoughtfocus.com' AS [Email]
			--FROM
			--[FieldWork].[FieldWork] FW
			--JOIN [dbo].[View_FieldWorkPrerequisiteStatus] FWPRS ON FWPRS.FieldWorkID = FW.ID
			--AND [FieldWorkPrerequisiteStatus]=2
			--JOIN [User].[Users] U ON u.ID = fw.UserID
			--JOIN [Master].[FieldWorkCourses] FWC on fwc.ID = fw.FieldWorkCourseID
			--JOIN [Master].[CourseTerm] CT on CT.[ID] = FWC.[CourseTermID]
			--JOIN [Master].[Term] T on T.[TermCode] = CT.[TermCode]
			--WHERE FW.[ID]=@FieldWorkId

				SELECT U.FirstName + ' ' + U.LastName as [ApplicantName], U.CSULBID, T.[Name],
			--U.Email
			'asif.khan@thoughtfocus.com' AS [Email]
			FROM
			[FieldWork].[FieldWork] FW
			JOIN [dbo].[View_FieldWorkPrerequisiteStatus] FWPRS ON FWPRS.FieldWorkID = FW.ID
			AND [FieldWorkPrerequisiteStatus]=2
			JOIN [User].[Users] U ON u.ID = fw.UserID
			JOIN [Master].[FieldWorkCourses] FWC on fwc.ID = fw.FieldWorkCourseID
			JOIN [Master].[CourseTerm] CT on CT.[ID] = FWC.[CourseTermID]
			JOIN [Master].[Term] T on T.[TermCode] = CT.[TermCode]
			WHERE FW.[ID]=
			 (SELECT MAX(FW1.[ID]) FROM [FieldWork].[FieldWork] FW1 
JOIN [FieldWork].[FieldWork] FW2 ON FW1.[UserID] = FW2.[UserID]
WHERE FW2.[ID] = @FieldWorkId)


END