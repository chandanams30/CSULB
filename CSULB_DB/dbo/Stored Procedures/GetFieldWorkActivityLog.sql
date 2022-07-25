
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--exec dbo.GetFieldWorkActivityLog 614,1
CREATE PROCEDURE [dbo].[GetFieldWorkActivityLog]
@UserID bigint,
@FieldWorkID bigint
AS
BEGIN
			select fw.ID as FieldWorkID,fc.BaseSchema,fw.ResponseSchema from [FieldWork].[FieldWork] fw
			join [Master].[FieldWorkCourses] fc on fc.ID=fw.CourseID
			where fw.ID=@FieldWorkID and fw.[Status]=1
END