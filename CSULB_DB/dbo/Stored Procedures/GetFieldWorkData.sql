-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetFieldWorkData]
@UserId bigint
AS
BEGIN
	select F.ID,U.FirstName+' '+U.LastName as [StudentName] , C.CourseTitle , C.CSULBCourseId , C.College , S.[Name] as Term ,
				case when tb.ValidTill>GETDATE() then isnull(TB.IsApproved,'') else 0 end as TB,
				case when CTC.ValidTill>GETDATE() then isnull(CTC.IsApproved,'') else 0 end as CTC
				from [FieldWork].[FieldWork] F
				Join [User].[Users] U on U.ID=F.UserID 
				Join [FieldWork].[Courses] C on C.ID=F.CourseID
				Join [Master].[Semester] S on S.TermCode=C.TermCode
				Join [FieldWork].[Attachments]TB on TB.UserID=F.UserID and TB.DocumentID =7
				Join [FieldWork].[Attachments] CTC  on CTC.UserID=F.UserID and CTC.DocumentID =8
				where F.ID in (

				select F.ID from [FieldWork].[FieldWork] F 
				Join [User].[Users] U on U.ID=F.UserID 
				where F.UserID=@UserId

				union

				select F.ID from [FieldWork].[FieldWork] F 
				Join [FieldWork].[FieldWorkUsers] FWU on FWU.FieldWorkID=F.ID
				where FWU.UserID=@UserId
				)
END