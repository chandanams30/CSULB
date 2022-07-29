
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec UpdateFieldWorkActivityLog 338,1,'My JSON String'
CREATE PROCEDURE [dbo].[UpdateFieldWorkActivityLog]
	@UserID bigint,
	@FieldWorkID bigint,
	@ResponseSchema nvarchar(max)
AS
BEGIN
	update [FieldWork].[FieldWork] set ResponseSchema=@ResponseSchema where ID=@FieldWorkID

	select fw.ID as FieldWorkID,FWC.BaseSchema,fw.ResponseSchema from [FieldWork].[FieldWork] fw
	--join [Master].[FieldWorkCourses] fc on fc.ID=fw.CourseID
	LEFT JOIN [Master].[FieldWorkCourses] FWC ON FWC.ID =  FW.FieldWorkCourseID 
	JOIN [Master].[CourseTerm] CT ON FWC.CourseTermID = CT.ID
	JOIN [Master].[Courses] C ON CT.CourseID = C.[ID]
	where fw.ID=@FieldWorkID and fw.[Status]=1
END