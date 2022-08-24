CREATE PROCEDURE [dbo].[UpdateFieldWorkValidation]
@FieldWorkID bigint,
@IsApproved bit,
@ApprovedBy bigint,
@ValidatedDate datetime,
@ValidTill datetime,
@RejectReason nvarchar(255),
@Comments nvarchar(Max)
AS
BEGIN
	--update [FieldWork].[Attachments] set 
	--IsApproved=@IsApproved,
	--ApprovedBy=@ApprovedBy,
	--ValidatedDate=@ValidatedDate,
	--ValidTill=@ValidTill,
	--RejectedReason=@RejectReason,
	--Comments=@Comments
	--where ID=@FieldWorkID


	UPDATE [FieldWork].[Attachments]
	SET IsApproved = @IsApproved
		,ApprovedBy = @ApprovedBy
		,ValidatedDate = @ValidatedDate
		,ValidTill = @ValidTill
		,RejectedReason = @RejectReason
		,Comments = @Comments
	WHERE ID = @FieldWorkID

END