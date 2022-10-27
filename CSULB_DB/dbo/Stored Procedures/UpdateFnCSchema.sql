--exec [dbo].[UpdateFnCSchema] 2,1,'test ActivityLogFnCSchema',1
CREATE PROCEDURE [dbo].[UpdateFnCSchema] 
@schemaType int,
@fieldWorkID bigint,
@schema nvarchar(max),
@fieldWorkActivityLogID bigint
AS
BEGIN

    --public enum FnCSchema
    --{
    --    SummaryFnCSchema = 1,
    --    ActivityLogFnCSchema=2,
    --    SAFnCSchema=3,
    --    CPAFnCSchema=4
    --}

	IF (@schemaType=1) --SummaryFnCSchema
		BEGIN
			PRINT ' SummaryFnCSchema = 1'
			UPDATE [FieldWork].[FieldWork] SET [SummaryFnCSchema] = @schema WHERE [ID]=@fieldWorkID
		END
	ELSE IF (@schemaType=2) --ActivityLogFnCSchema
		BEGIN
			PRINT 'ActivityLogFnCSchema=2'
			UPDATE [FieldWork].[FieldWorkActivityLog] SET [ActivityLogFnCSchema]=@schema WHERE [ID]=@fieldWorkActivityLogID AND [FieldWorkID]=@fieldWorkID
		END
	ELSE IF (@schemaType=3) --SAFnCSchema
		BEGIN
			PRINT ' SAFnCSchema = 3'
			UPDATE [FieldWork].[FieldWork] SET [SAFnCSchema]=@schema WHERE [ID]=@fieldWorkID
		END
	ELSE IF (@schemaType=4) --CPAFnCSchema
		BEGIN
			PRINT 'CPAFnCSchema = 4'
			UPDATE [FieldWork].[FieldWork] SET [CPAFnCSchema]=@schema WHERE [ID]=@fieldWorkID
		END

END