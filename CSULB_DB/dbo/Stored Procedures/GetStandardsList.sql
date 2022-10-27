CREATE PROCEDURE [dbo].[GetStandardsList] 
	@UserID bigint,
	@FieldWorkID bigint
AS
BEGIN
	SET NOCOUNT ON

	SELECT FWCCS.[ID]
		,FWCCS.[Standard] 
	FROM [Master].[FieldWorkCoursesCategoryStandards] FWCCS
		JOIN [Master].[FieldWorkCoursesConfiguration] FWCC ON FWCC.[CategoryID] = FWCCS.[CategoryID]
		JOIN [Master].[Courses] C ON C.[Subject]=FWCC.[Subject] AND C.[CourseNumber] = FWCC.[CourseNumber]
		JOIN [Master].[CourseTerm] CT ON CT.[CourseID]=C.[ID]
		JOIN [Master].[FieldWorkCourses] FWC ON FWC.[CourseTermID]=CT.[ID]
		JOIN [FieldWork].[FieldWork] FW ON FW.[FieldWorkCourseID] = FWC.[ID] AND FW.[ID] = @FieldWorkID --AND FW.[UserID]=@UserID 
END