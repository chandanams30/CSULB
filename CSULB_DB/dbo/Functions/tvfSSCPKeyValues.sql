
-- =============================================
-- Author:		ThougthFocus
-- Create date: 06-Dec-2022
-- Description:	JSON to table by form ID
-- =============================================
CREATE FUNCTION [dbo].[tvfSSCPKeyValues]
(	
	-- Add the parameters for the function here
	@FormID bigint
)
RETURNS @Output TABLE (
    [key] NVARCHAR(MAX),
	[value] NVARCHAR(MAX),
	[type] NVARCHAR(MAX),
	[Section] NVARCHAR(100)
)
AS
BEGIN
	-- Add the SELECT statement with parameter references here
	DECLARE @PersonalInfoForm nvarchar(MAX);
	SELECT @PersonalInfoForm = f.[Form] FROM [Application].[Forms] F WHERE [ID] = @FormID 

	INSERT INTO @Output([key], [value],[type], [Section])
	(
		SELECT T.[key], CASE WHEN LEN(T.[value])=0 OR TRIM(TRANSLATE(CONVERT(NVARCHAR(500), T.[value]), '[{}]', '    '))='' THEN NULL ELSE T.[value] END AS [value],T.[type], 'Root' AS [Section] FROM OPENJSON(@PersonalInfoForm) AS T
		UNION
		SELECT AD.[key],CASE WHEN LEN(AD.[value])=0 OR TRIM(TRANSLATE(CONVERT(NVARCHAR(500), AD.[value]), '[{}]', '    '))='' THEN NULL ELSE AD.[value] END AS [value],AD.[type], 'Address' AS [Section] FROM OPENJSON(@PersonalInfoForm, '$.address') AS [AD]
		UNION
		SELECT D.[key],CASE WHEN LEN(D.[value])=0 OR TRIM(TRANSLATE(CONVERT(NVARCHAR(500), D.[value]), '[{}]', '    '))='' THEN NULL ELSE D.[value] END AS [value],D.[type], 'Degrees' AS [Section] FROM OPENJSON(@PersonalInfoForm, '$.degrees') AS C CROSS APPLY OPENJSON(c.[value],'$')  AS D
	)

	RETURN 
END