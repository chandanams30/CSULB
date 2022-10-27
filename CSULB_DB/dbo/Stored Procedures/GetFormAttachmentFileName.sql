-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
--EXEC 
--EXEC [dbo].[GetFormAttachmentFileName] 1, 18
CREATE PROCEDURE [dbo].[GetFormAttachmentFileName]
@FormID BIGINT
,@DocumentID BIGINT

AS
BEGIN


DECLARE @timeStamp as varchar(200)
DECLARE @userFolder as bigint

select @userFolder=[UserID] from  [Application].[Forms] where [ID]=@FormID

SELECT @timeStamp =  TRIM(REPLACE(REPLACE([NAME], ' ',''), '/','')) + '_' + replace(convert(varchar, getdate(),101),'/','') + replace(convert(varchar, getdate(),108),':','')
FROM [Master].[Documents] D WHERE [ID] = @DocumentID



SELECT convert(varchar,@FormID) + '_' + @timeStamp AS 'SavedFileName'
, @timeStamp AS 'FileName'
, @userFolder AS 'UserFolder'
FROM [Master].[Documents] D WHERE [ID] = @DocumentID
				
END