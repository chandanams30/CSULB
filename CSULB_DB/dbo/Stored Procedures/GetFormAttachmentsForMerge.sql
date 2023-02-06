-- =============================================
-- Author:		ThoughtFocus
-- Create date: <Create Date,,>
-- Description:	Returns attachments by sorting for merge
-- =============================================

--EXEC [dbo].[GetFormAttachmentsForMerge] 2
CREATE PROCEDURE [dbo].[GetFormAttachmentsForMerge]
@FormID BIGINT

AS
BEGIN

DECLARE @JSON    VARCHAR(MAX) 
SELECT @JSON=[form] FROM [Application].[Forms] where [ID]=@FormID

SELECT *
FROM OPENJSON(@json)
  WITH (
    firstName nvarchar(500)
  ,lastName nvarchar(500)
  ,preferredName nvarchar(500)
  ,otherName nvarchar(500)
  ,cusulbEmail nvarchar(500)
  ,altEmail nvarchar(500)
  ,phoneNumber nvarchar(500)
  ,csulbCampusId nvarchar(500)
  --,casId nvarchar(500)
   ,Semester nvarchar(500)
   ,Program nvarchar(500)
  )T CROSS JOIN   (SELECT STRING_AGG([value], ',') as languages
      FROM OPENJSON(@json, '$.languages') where [value] <> '') T2;

----------------------------------------------

--SELECT T.[FormID], T.[FileName], T.[FileExtn], T.[FolderName] from (
--SELECT FA.[FormID], FA.[FileName], FA.[FileExtn], FA.[FolderName], PD.[DocumentID] FROM [Application].[FormAttachments] FA 
--JOIN [Master].[ProgramDocuments] PD ON PD.[ID] = FA.[ProgramDocumentID]
--WHERE FA.[FormID]=@FormID
--UNION
--SELECT R.[FormID], RA.[FileName], RA.[FileExtn], RA.[FolderName], RA.[DocumentID]
--FROM [Application].[RecommendationAttachments] RA JOIN [Application].[Recommendations] R ON R.[ID] = RA.[RecomendationID] WHERE R.[FormID]=@FormID
--)T
--JOIN [Application].[Forms] F ON F.[ID] = T.[FormID]
--JOIN [Master].[ProgramDocuments] PD ON PD.[ProgramID] = F.[ProgramID] AND PD.[DocumentID] = T.[DocumentID]
--WHERE PD.[isRequiredForMerge]=1
--ORDER BY PD.[SortingOrder]

SELECT T.[FormID], T.[FileName], T.[FileExtn], T.[FolderName]
--,T.[DocumentID], T.RecomendationID, PD.[SortingOrder] 
FROM (
SELECT FA.[FormID], FA.[FileName], FA.[FileExtn], FA.[FolderName], PD.[DocumentID], 0 AS [RecomendationID] FROM [Application].[FormAttachments] FA 
JOIN [Master].[ProgramDocuments] PD ON PD.[ID] = FA.[ProgramDocumentID]
WHERE FA.[FormID]=@FormID
UNION
SELECT R.[FormID], RA.[FileName], RA.[FileExtn], RA.[FolderName], RA.[DocumentID], RA.[RecomendationID]
FROM [Application].[RecommendationAttachments] RA JOIN [Application].[Recommendations] R ON R.[ID] = RA.[RecomendationID] WHERE R.[FormID]=@FormID
)T
JOIN [Application].[Forms] F ON F.[ID] = T.[FormID]
JOIN [Master].[ProgramDocuments] PD ON PD.[ProgramID] = F.[ProgramID] AND PD.[DocumentID] = T.[DocumentID]
WHERE PD.[isRequiredForMerge]=1 AND T.[FileName] IS NOT NULL
ORDER BY PD.[SortingOrder],T.RecomendationID,t.[DocumentID]
----------------------------------------------

----------------------------------------------
SELECT [UserID] as UserFolder FROM [Application].[Forms] where [ID]=@FormID
----------------------------------------------

END