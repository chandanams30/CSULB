-- =============================================
-- Author:		ThoughtFocus
-- Create date: 2022-Nov-17
-- Description:	Return JSOn Form String based on Application Type
-- =============================================
CREATE FUNCTION [dbo].[fnIsFormValid] 
(
	-- Add the parameters for the function here
	@FormID bigint
)
RETURNS BIT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result BIT = 0

DECLARE @ProgramID BIGINT

SELECT @ProgramID = [ProgramID] FROM [Application].[Forms] F  WHERE [ID] = @FormID

------------------------------------------------------------------------------------------------------
--check if form state in --Open or draft
------------------------------------------------------------------------------------------------------
--IF EXISTS (SELECT * FROM [Application].[Forms] F  WHERE [ID] = @FormID  AND F.[FormStateID] in (1,2)) --Open, --draft
--	BEGIN
--		PRINT 'GOTO StartValidation'
--		GOTO StartValidation;
--	END
--ELSE
--	BEGIN
--		PRINT 'GOTO FormValid'
--		GOTO FormValid;
--	END
------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------
--check for Application Type
------------------------------------------------------------------------------------------------------
StartValidation:
--PRINT 'StartValidation:';
DECLARE @ApplicationTypeID as bigint

SELECT @ApplicationTypeID = [ApplicationTypeID]  FROM [Master].[ProgramApplicationType] WHERE [ProgramID] = @ProgramID
IF(@ApplicationTypeID = 1) --1	Initial Teacher Credential Programs
	BEGIN
		DECLARE @ProgramFormIdentifier as VARCHAR(10)
		SELECT @ProgramFormIdentifier = ProgramFormIdentifier FROM [Master].[Programs] WHERE [ID] = @ProgramID

		IF(@ProgramFormIdentifier = 'ESCP') --Graduate / Advanced Credential Programs
			BEGIN
				GOTO ESCPValidation;	
			END
		ELSE IF(@ProgramFormIdentifier = 'MSCP') --Graduate / Advanced Credential Programs
			BEGIN
				GOTO MSCPValidation;	
			END
		ELSE IF(@ProgramFormIdentifier = 'SSCP') --Graduate / Advanced Credential Programs
			BEGIN
				GOTO SSCPValidation;	
			END	
		ELSE IF(@ProgramFormIdentifier = 'UDCP') --Graduate / Advanced Credential Programs
			BEGIN
				GOTO UDCPValidation;	
			END	
	END
ELSE IF(@ApplicationTypeID = 2) --Graduate / Advanced Credential Programs
	BEGIN
		GOTO GACPValidation;	
	END
ELSE
	BEGIN
		--PRINT 'GOTO NotValid';
		GOTO NotValid;
	END

------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------
--check for Graduate / Advanced Credential Programs required field validation
------------------------------------------------------------------------------------------------------
GACPValidation:
--PRINT 'GACPValidation';
IF NOT EXISTS(SELECT * FROM [Application].[Forms] F
			 CROSS APPLY OPENJSON(F.[FORM])
			   WITH (
					firstName nvarchar(500)
					,lastName nvarchar(500)
					,preferredName nvarchar(500)
					,otherName nvarchar(500)
					,cusulbEmail nvarchar(500)
					,altEmail nvarchar(500)
					,phoneNumber nvarchar(500)
					,csulbCampusId nvarchar(500)
					,Semester nvarchar(500)
					,Program nvarchar(500)
				)T
				WHERE [ID] = @FormID 
					AND (ISNULL(T.[altEmail],'') = ''
						OR ISNULL(T.[phoneNumber],'') = ''
						OR ISNULL(T.[csulbCampusId],'') = ''
						OR ISNULL(T.[firstName],'') = ''
						OR ISNULL(T.[lastName],'') = '')
						)
	BEGIN
		GOTO AttachmentValidation;
	END
ELSE
	BEGIN
		--PRINT 'GOTO NotValid';
		GOTO NotValid;
	END
------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------
--check for Initial Teacher Credential Programs/CED - ESCP Clinical Program required field validation
------------------------------------------------------------------------------------------------------
ESCPValidation:
--PRINT 'ESCPValidation';
IF NOT EXISTS (SELECT * FROM [dbo].[tvfMSCPKeyValues](@FormID) WHERE [value] IS NULL AND [key] not in ('middleName', 'casId','otherName','preferredName','state'))
	BEGIN
		--PRINT 'GOTO Attachment Validation';
		GOTO AttachmentValidation;
	END
ELSE
	BEGIN
		--PRINT 'GOTO NotValid';
		GOTO NotValid;
	END
------------------------------------------------------------------------------------------------------
--check for Initial Teacher Credential Programs/CED - MSCP Clinical Program required field validation
------------------------------------------------------------------------------------------------------
MSCPValidation:
--PRINT 'MSCPValidation';
IF NOT EXISTS (SELECT * FROM [dbo].[tvfMSCPKeyValues](@FormID) WHERE [value] IS NULL AND [key] not in ('middleName', 'casId','otherName','preferredName','state'))
--(SELECT J.* FROM [Application].[Forms] F
--			CROSS APPLY OPENJSON(F.[FORM]) AS J 
--			WHERE [ID] = @FormID AND (J.[VALUE] IS NULL OR J.[VALUE]='')
--			AND J.[key] not in ('middleName'))
	BEGIN
		GOTO AttachmentValidation;
	END
ELSE
	BEGIN
		--PRINT 'GOTO NotValid';
		GOTO NotValid;
	END
------------------------------------------------------------------------------------------------------
--check for Initial Teacher Credential Programs/CED - SSCP Clinical Program required field validation
------------------------------------------------------------------------------------------------------
SSCPValidation:
--PRINT 'SSCPValidation';
IF NOT EXISTS (SELECT * FROM [dbo].[tvfSSCPKeyValues](@FormID) WHERE [value] IS NULL AND [key] not in ('middleName', 'casId','otherName','preferredName','state','altEmail'))
	BEGIN
		GOTO AttachmentValidation;
	END
ELSE
	BEGIN
		--PRINT 'GOTO NotValid';
		GOTO NotValid;
	END
------------------------------------------------------------------------------------------------------
--check for Initial Teacher Credential Programs/CED - UDCP Clinical Program required field validation
------------------------------------------------------------------------------------------------------
UDCPValidation:
--PRINT 'UDCPValidation';
IF NOT EXISTS (SELECT * FROM [dbo].[tvfUDCPKeyValues](@FormID) WHERE [value] IS NULL AND [key] not in ('middleName', 'casId','otherName','preferredName','state','subjectMatterCompetenceVia','altEmail','apartment'))
	BEGIN
		GOTO AttachmentValidation;
	END
ELSE
	BEGIN
		--PRINT 'GOTO NotValid';
		GOTO NotValid;
	END
------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------
--Validate Attachment
------------------------------------------------------------------------------------------------------
AttachmentValidation:
--PRINT 'AttachmentValidation';
--IF NOT EXISTS (SELECT * FROM [Master].[ProgramDocuments] PD JOIN [Application].[Forms] F ON F.[ProgramID] = PD.[ProgramID] LEFT JOIN [Application].[FormAttachments] FA ON FA.[FormID] = F.[ID] WHERE F.[ID]=@FormID AND PD.[IsOptional]=0 AND (FA.[ID] IS NULL OR FA.[FileName] is NULL))
IF NOT EXISTS (SELECT * FROM [Master].[ProgramDocuments] PD JOIN [Application].[Forms] F ON F.[ProgramID] = PD.[ProgramID] LEFT JOIN [Application].[FormAttachments] FA ON FA.[FormID] = F.[ID] AND PD.[ID] = FA.[ProgramDocumentID] WHERE F.[ID]=@FormID AND PD.[IsOptional]=0 AND (FA.[ID] IS NULL OR FA.[FileName] is NULL))
	BEGIN
		GOTO RecommendationValidation;
	END
ELSE
	BEGIN
		--PRINT 'GOTO NotValid';
		GOTO NotValid;
	END
------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------
--Validate for # of Recommender is more than 2
------------------------------------------------------------------------------------------------------
RecommendationValidation:
--PRINT 'RecommendationValidation';
IF EXISTS (SELECT F.[ID], COUNT(R.[ID]) FROM [Application].[Recommendations] R JOIN [Application].[Forms] F ON F.[ID] = R.[FormID] WHERE F.[ID]=@FormID GROUP BY F.[ID] HAVING COUNT(R.[ID])>1)
	BEGIN
		--PRINT 'GOTO FormValid';
		GOTO FormValid;
	END
ELSE
	BEGIN
		--PRINT 'GOTO NotValid';
		GOTO NotValid;
	END
------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------
--Set form to Open State
------------------------------------------------------------------------------------------------------
NotValid:
--PRINT 'Form Valid';
RETURN @Result;
------------------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------------------
--Set form to Open State
------------------------------------------------------------------------------------------------------
FormValid:
--PRINT 'FormValid: Form inValid';
SET @Result=1;
RETURN @Result;
------------------------------------------------------------------------------------------------------


END