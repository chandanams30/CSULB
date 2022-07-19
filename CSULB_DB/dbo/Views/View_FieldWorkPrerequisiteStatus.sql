CREATE VIEW [dbo].[View_FieldWorkPrerequisiteStatus]
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
				AND A.[ValidTill] > GETDATE()
				AND A.[IsApproved] = 1
				THEN 2
			ELSE 1
			END AS [STATUS]
	FROM FieldWork.FieldWork AS FW
	INNER JOIN Master.FieldWorkDocuments AS FWD ON FWD.CourseID = FW.CourseID
	INNER JOIN FieldWork.Attachments AS A ON A.DocumentID = FWD.DocumentID
		AND A.UserID = FW.UserID
	) AS T
GROUP BY [FieldWorkID]