-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetFormsList]
@ProgramID int,
@SemesterID int,
@State int
AS
BEGIN
				SELECT * FROM [Application].[Forms]
				--select forms.ID,
				--forms.UserID,
				--users.FirstName+' '+users.LastName ApplicantName,
				--forms.ProgramID,
				--programs.Name ProgramName,
				--forms.SemesterID,
				--semesters.Name SemesterName,
				--forms.ApplicationNumber

				--from [Application].[Forms] forms
				--join [Master].[Programs] programs on forms.ProgramID=programs.ID
				--join [Master].[Semester] semesters on forms.SemesterID=semesters.ID
				--join [User].[Users] users on forms.UserID=users.ID

				--where
				--forms.SemesterID=@SemesterID 
				--and forms.ProgramID= ISNULL(@ProgramID,forms.ProgramID)
				--and forms.State=@State
END