CREATE PROCEDURE [dbo].[SchoolTypesList] 
	@UserID bigint,
	@FieldWorkID bigint
AS
BEGIN
	SET NOCOUNT ON

	SELECT FWCCST.[ID]
		,FWCCST.[SchoolType] 
	FROM [Master].[FieldWorkCoursesCategorySchoolTypes] FWCCST
		--JOIN [Master].[FieldWorkCoursesConfiguration] FWCC ON FWCC.[CategoryID] = FWCCS.[CategoryID]
		--JOIN [Master].[Courses] C ON C.[Subject]=FWCC.[Subject] AND C.[CourseNumber] = FWCC.[CourseNumber]
		--JOIN [Master].[CourseTerm] CT ON CT.[CourseID]=C.[ID]
		--JOIN [Master].[FieldWorkCourses] FWC ON FWC.[CourseTermID]=CT.[ID]
		--JOIN [FieldWork].[FieldWork] FW ON FW.[FieldWorkCourseID] = FWC.[ID] AND FW.[ID] = @FieldWorkID --AND FW.[UserID]=@UserID 
END