--exec GetFieldWorkFnCSchema '1','1','5'
CREATE PROCEDURE [dbo].[GetFieldWorkFnCSchema]
@fieldWorkID bigint,
@fieldWorkActivityLogID bigint,
@schemaType int
AS
BEGIN
	--declare @SQL nvarchar(2000)
	--set @SQL='select '+ @schemaType+' as [schema]  from [FieldWork].[FieldWork] where ID='+@fieldWorkID
	----print @SQL
	--exec(@SQL)

	IF (@schemaType=1) --SummaryFnCSchema
		BEGIN
			PRINT ' SummaryFnCSchema = 1'
			SELECT [SummaryFnCSchema] AS [schema] FROM [FieldWork].[FieldWork] WHERE [ID]=@fieldWorkID
		END
	ELSE IF (@schemaType=2) --ActivityLogFnCSchema
		BEGIN
			PRINT 'ActivityLogFnCSchema=2'
			SELECT [ActivityLogFnCSchema] AS [schema] FROM [FieldWork].[FieldWorkActivityLog] WHERE [ID]=@fieldWorkActivityLogID AND [FieldWorkID]=@fieldWorkID
		END
	ELSE IF (@schemaType=3) --SAFnCSchema
		BEGIN
			PRINT ' SAFnCSchema = 3'
			SELECT [SAFnCSchema] AS [schema] FROM [FieldWork].[FieldWork] WHERE [ID]=@fieldWorkID
		END
	ELSE IF (@schemaType=4) --CPAFnCSchema
		BEGIN
			PRINT 'CPAFnCSchema = 4'
			SELECT [CPAFnCSchema] AS [schema] FROM [FieldWork].[FieldWork] WHERE [ID]=@fieldWorkID
		END
END