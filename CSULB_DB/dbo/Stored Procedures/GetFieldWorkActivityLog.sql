CREATE PROCEDURE [dbo].[GetFieldWorkActivityLog]
@UserID bigint,
@FieldWorkID bigint
AS
BEGIN
			select fw.ID as FieldWorkID,fc.BaseSchema,fw.ResponseSchema 
			from [FieldWork].[FieldWork] fw
			join [Master].[FieldWorkCourses] fc on fc.ID=fw.FieldWorkCourseID
			where fw.ID=@FieldWorkID and fw.[Status]=1
END