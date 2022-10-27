--exec [dbo].[GetFieldWorkActivityLogStandards] 3
CREATE PROCEDURE [dbo].[GetFieldWorkActivityLogStandards]
	@FieldWorkActivityLogID bigint
AS
BEGIN
	SET NOCOUNT ON

	SELECT 
		LS.[FieldWorkActivityLogID]
		,FAL.[ActivityStartDate]
		, FAL.[ActivityEndDate]
		,LS.[Hours]
		,CS.[Name] 'CommunitySite'
		,CSU.[UserID]
		,U.[DisplayName] 'UserName' 
		

	FROM 
		[Fieldwork].[FieldWorkActivityLogStandards] LS
	JOIN [FieldWork].[FieldWorkActivityLog] FAL ON FAL.ID = LS.FieldWorkActivityLogID
	JOIN [Fieldwork].[CommunitySiteUsers] CSU ON FAL.CommunitySiteUsersID = CSU.UserID
	JOIN [Fieldwork].[CommunitySites] CS ON CS.ID = CSU.CommunitySiteID
	JOIN [User].[Users] U ON U.ID = CSU.UserID
	--JOIN [Master].[FieldWorkCoursesCategory] CC ON LS.[FieldWorkCoursesCategoryStandardID] = CC.ID
	WHERE 
		LS.FieldWorkActivityLogID = @FieldWorkActivityLogID
	ORDER BY
		FAL.ActivityStartDate, LS.FieldWorkActivityLogID
END