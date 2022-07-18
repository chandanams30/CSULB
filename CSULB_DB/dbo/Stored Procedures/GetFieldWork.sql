-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
-- exec [dbo].[GetFieldWork] 1545,3553
CREATE PROCEDURE [dbo].[GetFieldWork]
@UserId bigint,
@FieldWorkId bigint
AS
BEGIN
		
					select F.ID,U.FirstName+' '+U.LastName as [StudentName] , C.CourseTitle , C.CSULBCourseId , C.College , S.[Name] as Term 
					 
					--,UR.UserID
					from [FieldWork].[FieldWork] F
					Join [User].[Users] U on U.ID=F.UserID 
					Join [Master].[FieldWorkCourses] C on C.ID=F.CourseID
					Join [Master].[Semester] S on S.TermCode=C.TermCode
					--join [FieldWork].[UserRoles] UR on ur.FieldWorkID=f.ID
					where F.ID =@FieldWorkId


					SELECT FWU.[ID]
						  ,FWU.[UserID]
						  ,FWU.[FieldWorkID]
						  ,FWU.[RoleID]
						  ,U.FirstName+' '+U.LastName as [Name]
						  ,r.[Description]
					  FROM [FieldWork].[FieldWorkUsers] FWU
					  Join [User].[Users] U on U.ID=FWU.UserID 
					  join [Master].[Role] r on r.ID=FWU.RoleID
					  where FWU.FieldWorkID=@FieldWorkId


				SELECT a.[ID]
					  ,a.[UserID]
					  ,a.[DocumentID]
					  ,d.[Description] DocumentType
					  ,a.[FileName]
					  ,a.[FileExtn]
					  ,a.[FolderName]
					  ,a.[IsApproved]
					  --,a.[ApprovedBy]
					  ,(u.FirstName+' '+u.LastName) ApprovedBy
					  ,a.[ValidatedDate]
					  ,a.[CreatedBy]
					  ,a.[CreatedDate]
					  ,a.[ValidTill]
					  ,a.[RejectedReason]
				  FROM [FieldWork].[Attachments] a
				  join [FieldWork].[FieldWork] f on f.ID=@fieldWorkID and f.UserID=a.UserID
				  join [Master].[Documents] d on d.ID=a.DocumentID
				  left join [User].[Users] u on u.ID=a.ApprovedBy


END