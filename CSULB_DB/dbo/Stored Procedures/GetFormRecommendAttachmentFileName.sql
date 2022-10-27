CREATE PROCEDURE [dbo].[GetFormRecommendAttachmentFileName]
@RecommenderIdentifier uniqueidentifier 
,@DocumentID BIGINT
AS
BEGIN

DECLARE @timeStamp as varchar(200)
DECLARE @userFolder as bigint, 
@FormUserID as bigint,
@FormID AS bigint = null

SELECT @userFolder=[UserID], @FormID = F.[ID]
FROM [Application].[Forms] F
	JOIN [Application].[Recommendations] Rec ON Rec.[FormID] = F.[ID]
WHERE Rec.[RecommenderIdentifier] = @RecommenderIdentifier

SELECT @timeStamp =  TRIM(REPLACE(REPLACE([NAME], ' ',''), '/','')) + '_' + replace(convert(varchar, getdate(),101),'/','') + replace(convert(varchar, getdate(),108),':','')
FROM [Master].[Documents] D WHERE [ID] = @DocumentID


SELECT convert(varchar,@FormID) + '_' + @timeStamp AS 'SavedFileName'
, @timeStamp AS 'FileName'
, @userFolder AS 'UserFolder'
FROM [Master].[Documents] D WHERE [ID] = @DocumentID AND @FormID is not null
				
END