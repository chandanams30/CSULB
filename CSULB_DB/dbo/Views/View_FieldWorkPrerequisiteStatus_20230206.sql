

CREATE VIEW [dbo].[View_FieldWorkPrerequisiteStatus_20230206]
AS
SELECT [FieldWorkID]
	,MIN([STATUS]) AS [FieldWorkPrerequisiteStatus]
FROM (
	SELECT FW.ID AS FieldWorkID
		,CASE 
			WHEN A.[FileName] IS NULL
				AND A.[ValidTill] IS NULL
				AND A.[IsApproved] IS NULL
				THEN 0
			WHEN A.[FileName] IS NOT NULL
				--AND A.[ValidTill] < GETDATE()
				AND( A.[IsApproved] IS  NULL or A.[IsApproved]=0 or A.[ValidTill] < GETDATE())
				THEN 1
			WHEN A.[FileName] IS NOT NULL
				AND A.[ValidTill] > GETDATE()
				AND A.[IsApproved] = 1
				THEN 2
			--ELSE 1
			END AS [STATUS]
	FROM [FieldWork].[FieldWork] FW
	LEFT JOIN [Master].[FieldWorkCourses] FWC ON FWC.ID =  FW.FieldWorkCourseID 
	JOIN [Master].[CourseTerm] CT ON FWC.CourseTermID = CT.ID
	INNER JOIN [Master].FieldWorkDocuments FWD ON FWD.CourseID = CT.CourseID
	INNER JOIN [FieldWork].Attachments A ON A.DocumentID = FWD.DocumentID
		AND A.UserID = FW.UserID
	) AS T
GROUP BY [FieldWorkID]