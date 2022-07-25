
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec UpdateFieldWorkActivityLog 614,1,'My JSON String'
CREATE PROCEDURE [dbo].[UpdateFieldWorkActivityLog]
	@UserID bigint,
	@FieldWorkID bigint,
	@ResponseSchema nvarchar(max)
AS
BEGIN
	update [FieldWork].[FieldWork] set ResponseSchema=@ResponseSchema where ID=@FieldWorkID

	select fw.ID as FieldWorkID,fc.BaseSchema,fw.ResponseSchema from [FieldWork].[FieldWork] fw
	join [Master].[FieldWorkCourses] fc on fc.ID=fw.CourseID
	where fw.ID=@FieldWorkID and fw.[Status]=1
END