-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--EXEC 
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
	  
SELECT FA.[FormID], FA.[FileName], FA.[FileExtn], FA.[FolderName] FROM [Application].[FormAttachments] FA WHERE FA.[FormID]=@FormID
UNION
SELECT R.[FormID], RA.[FileName], RA.[FileExtn], RA.[FolderName] FROM [Application].[RecommendationAttachments] RA JOIN [Application].[Recommendations] R ON R.[ID] = RA.[RecomendationID] WHERE R.[FormID]=@FormID

SELECT [UserID] as UserFolder FROM [Application].[Forms] where [ID]=@FormID

END